//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace Canti
{
    // TODO - finish labeling
    /// <summary>
    /// A SQLite database handler
    /// </summary>
    public sealed class Sqlite : IDatabase
    {
        #region Properties and Fields

        #region Public

        /// <summary>
        /// This database's type
        /// </summary>
        public DatabaseType Type { get; private set; }

        #endregion

        #region Private

        // The file this database will read and write to/from
        private string DatabaseFile { get; set; }

        // The sqlite connection object
        private SqliteConnection Connection { get; set; }

        // An event that is set when stopping the database
        private ManualResetEvent StopEvent { get; set; }

        // An event that is set when data is ready to be written to the database
        private ManualResetEvent ReadyEvent { get; set; }

        // The main thread our database operates on
        private Thread WriteThread { get; set; }

        // A queue of data that we want to write to the database
        private Queue<SqliteCommand> WriteQueue { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Starts this database
        /// </summary>
        public void Start()
        {
            // Create sqlite connection
            Connection = new SqliteConnection($"Data Source={DatabaseFile}");

            // Open sqlite connection
            Connection.Open();

            // Begin the write thread
            WriteThread.Start();
        }

        /// <summary>
        /// Stops this database
        /// </summary>
        public void Stop()
        {
            // Signal a stop event
            StopEvent.Set();
            WriteThread.Join();

            // Dispose of sqlite connection
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
            Connection.Dispose();
        }

        public void CreateTable(string TableName, ValueList Values)
        {
            // Check for bad values
            if (string.IsNullOrEmpty(TableName))
            {
                throw new ArgumentException("Table name cannot be empty");
            }
            if (Values == null || Values.Count == 0)
            {
                throw new ArgumentException("Supplied value list was null or empty");
            }

            // Create values string
            string CommandString = "";
            for (int i = 0; i < Values.Count; i++)
            {
                if (i > 0) CommandString += ", ";

                var Entry = Values[i];
                CommandString += $"{Entry.Name} {Entry.Type}";
                if (Entry.Size > 0) CommandString += $"({Entry.Size})";
                if (Entry.Unique) CommandString += " UNIQUE";
                if (Entry.Value != null) CommandString += $" DEFAULT '{Entry.Value.Value}'";
            }

            // Create SQL command
            var Command = new SqliteCommand($"CREATE TABLE IF NOT EXISTS {TableName} ({CommandString})", Connection);

            // Add data to our write queue
            WriteQueue.Enqueue(Command);
            ReadyEvent.Set();
        }

        public void NonQuery(string Data)
        {
            // Create SQL command
            var Command = new SqliteCommand(Data, Connection);

            // Add data to our write queue
            WriteQueue.Enqueue(Command);
            ReadyEvent.Set();
        }

        public ValueList[] Query(string Data)
        {
            // Lock the write queue so we can read data right now
            lock (WriteQueue)
            {
                // Create SQL command
                SqliteCommand Command = new SqliteCommand(Data, Connection);

                // Create an output list
                List<ValueList> Output = new List<ValueList>();

                // Execute command
                using (SqliteDataReader Reader = Command.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        ValueList Row = new ValueList();
                        for (int i = 0; i < Reader.FieldCount; i++)
                        {
                            Row[Reader.GetName(i)] = Reader.GetValue(i);
                        }
                        Output.Add(Row);
                    }
                }

                // Return output as an array
                return Output.ToArray();
            }
        }

        // TODO - make this string creation more efficient
        public void Insert(string TableName, ValueList Values)
        {
            // Check for bad values
            if (string.IsNullOrEmpty(TableName)) return;
            if (Values == null || Values.Count == 0) return;

            // Begin constructing an SQL command
            string Data = $"INSERT INTO {TableName} (";

            // Loop through the given values
            string ValueList = "";
            for (int i = 0; i < Values.Count; i++)
            {
                // Add name to command string
                if (i > 0)
                {
                    Data += ", ";
                    ValueList += ", ";
                }
                Data += $"{Values[i].Name}";
                ValueList += $"@{Values[i].Name}_1";
            }
            Data += $") VALUES ({ValueList})";

            // Create SQL command
            var Command = new SqliteCommand(Data, Connection);

            // Add values from the value list
            foreach (var Value in Values)
            {
                Command.Parameters.AddWithValue($"{Value.Name}_1", Value.Value);
            }

            // Add data to our write queue
            WriteQueue.Enqueue(Command);
            ReadyEvent.Set();
        }
        public void Update(string TableName, ValueList Values, ValueList Conditions)
        {
            // Check for bad values
            if (string.IsNullOrEmpty(TableName)) return;
            if (Values == null || Values.Count == 0) return;
            if (Conditions == null || Conditions.Count == 0) return;

            // Begin constructing an SQL command
            string Data = $"UPDATE {TableName} SET ";

            // Loop through the given values
            for (int i = 0; i < Values.Count; i++)
            {
                // Add name to command string
                if (i > 0) Data += ", ";
                Data += $"{Values[i].Name} = @{Values[i].Name}_1";
            }

            // Add conditions
            Data += " WHERE ";
            for (int i = 0; i < Conditions.Count; i++)
            {
                // Add name to command string
                if (i > 0) Data += " AND ";
                Data += $"{Conditions[i].Name} = @{Conditions[i].Name}_2";
            }

            // Create SQL command
            var Command = new SqliteCommand(Data, Connection);

            // Add values from the value list
            foreach (var Value in Values)
            {
                Command.Parameters.AddWithValue($"{Value.Name}_1", Value.Value);
            }

            // Add conditions from the conditions list
            foreach (var Condition in Conditions)
            {
                Command.Parameters.AddWithValue($"{Condition.Name}_2", Condition.Value);
            }

            // Add data to our write queue
            WriteQueue.Enqueue(Command);
            ReadyEvent.Set();
        }

        public ValueList[] Select(string TableName, ValueList Conditions)
        {
            // Check for bad values
            if (string.IsNullOrEmpty(TableName))
            {
                throw new ArgumentException("Table name cannot be empty");
            }
            if (Conditions == null)
            {
                throw new ArgumentNullException("Supplied condition list was null");
            }

            // Lock the write queue so we can read data right now
            lock (WriteQueue)
            {
                // Begin constructing an SQL command
                string Data = $"SELECT * FROM {TableName}";

                // Add conditions
                if (Conditions.Count > 0)
                {
                    Data += " WHERE ";
                    for (int i = 0; i < Conditions.Count; i++)
                    {
                        // Add name to command string
                        if (i > 0) Data += " AND ";
                        Data += $"{Conditions[i].Name} = @{Conditions[i].Name}_1";
                    }
                }

                // Create SQL command
                SqliteCommand Command = new SqliteCommand(Data, Connection);

                // Add conditions from the conditions list
                foreach (var Condition in Conditions)
                {
                    Command.Parameters.AddWithValue($"{Condition.Name}_1", Condition.Value);
                }

                // Create an output list
                List<ValueList> Output = new List<ValueList>();

                // Execute command
                using (SqliteDataReader Reader = Command.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        ValueList Row = new ValueList();
                        for (int i = 0; i < Reader.FieldCount; i++)
                        {
                            Row[Reader.GetName(i)] = Reader.GetValue(i);
                        }
                        Output.Add(Row);
                    }
                }

                // Return output as an array
                return Output.ToArray();
            }
        }
        public ValueList[] Select(string TableName)
        {
            return Select(TableName, new ValueList());
        }

        public int Count(string TableName, ValueList Conditions)
        {
            // Check for bad values
            if (string.IsNullOrEmpty(TableName))
            {
                throw new ArgumentException("Table name cannot be empty");
            }
            if (Conditions == null)
            {
                throw new ArgumentNullException("Supplied condition list was null");
            }

            // Lock the write queue so we can read data right now
            lock (WriteQueue)
            {
                // Begin constructing an SQL command
                string Data = $"SELECT COUNT(*) FROM {TableName}";

                // Add conditions
                if (Conditions.Count > 0)
                {
                    Data += " WHERE ";
                    for (int i = 0; i < Conditions.Count; i++)
                    {
                        // Add name to command string
                        if (i > 0) Data += " AND ";
                        Data += $"{Conditions[i].Name} = @{Conditions[i].Name}_1";
                    }
                }

                // populate command
                SqliteCommand Command = new SqliteCommand("SELECT COUNT(height) FROM blocks", Connection);

                // Add conditions from the conditions list
                foreach (var Condition in Conditions)
                {
                    Command.Parameters.AddWithValue($"{Condition.Name}_1", Condition.Value);
                }

                // Execute command
                return Convert.ToInt32(Command.ExecuteScalar());
            }
        }
        public int Count(string TableName)
        {
            return Count(TableName, new ValueList());
        }

        #endregion

        #region Private

        // Handles writing anything in our write queue
        private void ProcessCommand()
        {
            // Create a wait handle array so we can cancel this thread if need be
            WaitHandle[] Wait = new[] { ReadyEvent, StopEvent };
            while (0 == WaitHandle.WaitAny(Wait) && Connection.State == ConnectionState.Open)
            {
                // Lock our data queue to prevent race conditions
                lock (WriteQueue)
                {
                    // Data queue has entries
                    if (WriteQueue.Count > 0)
                    {
                        // Dequeue next piece of Data in line
                        var Data = WriteQueue.Dequeue();

                        // Handle this data
                        Data.ExecuteNonQuery();
                    }

                    // There are no entries in the data queue
                    else
                    {
                        // No data in line, reset ready event
                        ReadyEvent.Reset();
                        continue;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new SQLite database connection
        /// </summary>
        public Sqlite(string DatabaseFile)
        {
            // Set database type
            Type = DatabaseType.SQLITE;

            // Set database file
            this.DatabaseFile = DatabaseFile;

            // Setup variables
            WriteQueue = new Queue<SqliteCommand>();
            StopEvent = new ManualResetEvent(false);
            ReadyEvent = new ManualResetEvent(false);
            WriteThread = new Thread(ProcessCommand);
        }

        #endregion
    }
}
