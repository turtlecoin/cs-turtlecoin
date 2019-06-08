//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace Canti
{
    // TODO - finish summary labeling on anything public
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

        /// <summary>
        /// Whether or not this database has been started
        /// </summary>
        public bool Started { get; private set; }

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

            // Set as started
            Started = true;
        }

        /// <summary>
        /// Stops this database
        /// </summary>
        public void Stop()
        {
            // Set as stopped
            Started = false;

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

        // Creates a table if it does not exist
        // TODO - add table versions to update existing tables
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
            StringBuilder Data = new StringBuilder($"CREATE TABLE IF NOT EXISTS {TableName} (");
            for (int i = 0; i < Values.Count; i++)
            {
                if (i > 0) Data.Append(", ");

                var Entry = Values[i];
                Data.Append($"{Entry.Name}");
                if (!Entry.Primary)
                {
                    Data.Append($" {Entry.Type}");
                    if (Entry.Size > 0) Data.Append($"({Entry.Size})");
                    if (Entry.Unique) Data.Append(" UNIQUE");
                    if (Entry.Value != null) Data.Append($" DEFAULT '{Entry.Value}'");
                }
                else Data.Append(" INTEGER PRIMARY KEY");
            }
            Data.Append(")");

            // Create SQL command
            var Command = new SqliteCommand(Data.ToString(), Connection);

            // Add data to our write queue
            WriteQueue.Enqueue(Command);
            ReadyEvent.Set();
        }

        // Non-queries the database manually
        public void NonQuery(string Data)
        {
            // Create SQL command
            var Command = new SqliteCommand(Data, Connection);

            // Add data to our write queue
            WriteQueue.Enqueue(Command);
            ReadyEvent.Set();
        }

        // Queries the database manually
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

        // Adds a row to the database
        public void Add(string TableName, ValueList Values)
        {
            // Check for bad values
            if (string.IsNullOrEmpty(TableName)) return;
            if (Values == null || Values.Count == 0) return;

            // Begin constructing an SQL command
            StringBuilder Data = new StringBuilder($"INSERT INTO {TableName} (");

            // Loop through the given values
            StringBuilder ValueList = new StringBuilder();
            for (int i = 0; i < Values.Count; i++)
            {
                // Add name to command string
                if (i > 0)
                {
                    Data.Append(", ");
                    ValueList.Append(", ");
                }
                Data.Append($"{Values[i].Name}");
                ValueList.Append($"@{Values[i].Name}_1");
            }
            Data.Append($") VALUES ({ValueList})");

            // Create SQL command
            var Command = new SqliteCommand(Data.ToString(), Connection);

            // Add values from the value list
            foreach (var Value in Values)
            {
                Command.Parameters.AddWithValue($"{Value.Name}_1", Value.Value);
            }

            // Add data to our write queue
            WriteQueue.Enqueue(Command);
            ReadyEvent.Set();
        }

        // Updates a row in the database
        public void Update(string TableName, ValueList Values, ValueList Conditions)
        {
            // Check for bad values
            if (string.IsNullOrEmpty(TableName)) return;
            if (Values == null || Values.Count == 0) return;
            if (Conditions == null || Conditions.Count == 0) return;

            // Begin constructing an SQL command
            StringBuilder Data = new StringBuilder($"UPDATE {TableName} SET ");

            // Loop through the given values
            for (int i = 0; i < Values.Count; i++)
            {
                // Add name to command string
                if (i > 0) Data.Append(", ");
                Data.Append($"{Values[i].Name} = @{Values[i].Name}_1");
            }

            // Add conditions
            Data.Append(" WHERE ");
            for (int i = 0; i < Conditions.Count; i++)
            {
                // Add name to command string
                if (i > 0) Data.Append(" AND ");
                Data.Append($"{Conditions[i].Name} = @{Conditions[i].Name}_2");
            }

            // Create SQL command
            var Command = new SqliteCommand(Data.ToString(), Connection);

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

        // Selects rows based on a set of conditions
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
                StringBuilder Data = new StringBuilder($"SELECT * FROM {TableName}");

                // Add conditions
                if (Conditions.Count > 0)
                {
                    Data.Append(" WHERE ");
                    for (int i = 0; i < Conditions.Count; i++)
                    {
                        // Add name to command string
                        if (i > 0) Data.Append(" AND ");
                        Data.Append($"{Conditions[i].Name} = @{Conditions[i].Name}_1");
                    }
                }

                // Create SQL command
                SqliteCommand Command = new SqliteCommand(Data.ToString(), Connection);

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
                StringBuilder Data = new StringBuilder($"SELECT COUNT(*) FROM {TableName}");

                // Add conditions
                if (Conditions.Count > 0)
                {
                    Data.Append(" WHERE ");
                    for (int i = 0; i < Conditions.Count; i++)
                    {
                        // Add name to command string
                        if (i > 0) Data.Append(" AND ");
                        Data.Append($"{Conditions[i].Name} = @{Conditions[i].Name}_1");
                    }
                }

                // populate command
                SqliteCommand Command = new SqliteCommand(Data.ToString(), Connection);

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
