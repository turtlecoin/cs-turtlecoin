//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti
{
    /// <summary>
    /// An enumerator representing allowed database types
    /// </summary>
    public enum DatabaseType
    {
        SQLITE = 0
    }

    /// <summary>
    /// Interface for a database handler
    /// </summary>
    public interface IDatabase
    {
        #region Properties and Fields

        /// <summary>
        /// This database's type
        /// </summary>
        DatabaseType Type { get; }

        /// <summary>
        /// Whether or not this database has been started
        /// </summary>
        bool Started { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Starts this database
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this database
        /// </summary>
        void Stop();

        /// <summary>
        /// Executes a raw non-query command
        /// </summary>
        /// <param name="Data">A raw command to execute against the database</param>
        void NonQuery(string Data);

        /// <summary>
        /// Executes a raw query
        /// </summary>
        /// <param name="Data">A raw command to execute against the database</param>
        /// <returns>An array of value lists containing all values of each row that was read</returns>
        ValueList[] Query(string Data);

        /// <summary>
        /// Creates a table in the database if it does not exist
        /// </summary>
        /// <param name="TableName">The name of the table to create</param>
        /// <param name="Values">A list of column values associated with this table</param>
        void CreateTable(string TableName, ValueList Values);

        /// <summary>
        /// Adds a set of values to the database
        /// </summary>
        /// <param name="TableName">The name of the table to insert into</param>
        /// <param name="Values">The values to set within the new row</param>
        void Add(string TableName, ValueList Values);

        /// <summary>
        /// Updates a set of values within the database
        /// </summary>
        /// <param name="TableName">The name of the table to update</param>
        /// <param name="Values">The values to set or change</param>
        /// <param name="Conditions">Only rows with columns matching these values will be changed</param>
        void Update(string TableName, ValueList Values, ValueList Conditions);

        /// <summary>
        /// Selects a set of values from the database
        /// </summary>
        /// <param name="TableName">The name of the table to select from</param>
        /// <param name="Conditions">Only rows with columns matching these values will be read</param>
        /// <returns>An array of value lists containing all values of each row that was read</returns>
        ValueList[] Select(string TableName, ValueList Conditions);

        /// <summary>
        /// Selects a set of values from the database
        /// </summary>
        /// <param name="TableName">The name of the table to select from</param>
        /// <returns>An array of value lists containing all values of each row that was read</returns>
        ValueList[] Select(string TableName);

        /// <summary>
        /// Counts all rows that match the specified conditions
        /// </summary>
        /// <param name="TableName">The name of the table to count on</param>
        /// <param name="Conditions">A set of values that a row must match to be counted</param>
        /// <returns>An integer representing the number of rows that matched the specified conditions</returns>
        int Count(string TableName, ValueList Conditions);

        /// <summary>
        /// Counts all rows in a table
        /// </summary>
        /// <param name="TableName">The name of the table to count on</param>
        /// <returns>An integer representing the number of rows in a specified table</returns>
        int Count(string TableName);

        #endregion
    }
}
