//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System.Collections.Generic;
using System.Linq;

namespace Canti
{
    /// <summary>
    /// Stores information about a database column or value
    /// </summary>
    public sealed class DatabaseValue
    {
        /// <summary>
        /// This column's name in the database
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The SQL type of this column
        /// </summary>
        public SqlType Type { get; set; }

        /// <summary>
        /// Only required for types CHAR and BINARY, sets a fixed size for these types
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Whether or not this column must be unique in the database
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        /// Whether this column will be the row's primary key
        /// </summary>
        public bool Primary { get; set; }

        /// <summary>
        /// The value or default value of this column
        /// </summary>
        public dynamic Value { get; set; }
    }

    /// <summary>
    /// Stores a list of database values
    /// </summary>
    public sealed class ValueList : List<DatabaseValue>
    {
        /// <summary>
        /// Gets or sets a value
        /// </summary>
        /// <param name="Key">The name of the associated value</param>
        /// <returns>A stored value with the associated name, or null if not found</returns>
        public dynamic this[string Key]
        {
            get
            {
                lock (this)
                {
                    if (this.Any(x => x.Name == Key))
                    {
                        return this.First(x => x.Name == Key).Value;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            set
            {
                lock (this)
                {
                    if (this.Any(x => x.Name == Key))
                    {
                        int Index = FindIndex(x => x.Name == Key);
                        this[Index].Value = value;
                    }
                    else
                    {
                        Add(new DatabaseValue() { Name = Key, Value = value });
                    }
                }
            }
        }

        /// <summary>
        /// Adds a value column to the list, used only when creating tables, otherwise use other add method
        /// </summary>
        /// <param name="Name">The name of this value in the database</param>
        /// <param name="Type">This column's SQL type</param>
        /// <param name="Size">Only required for types CHAR and BINARY, set to 0 to be ignored</param>
        /// <param name="Unique">Whether or not this column must be unique</param>
        /// <param name="Primary">Whether this column will be the row's primary key</param>
        /// <param name="Default">The default value for this column</param>
        public void Add(string Name, SqlType Type, int Size = 0, bool Unique = false, bool Primary = false,
            dynamic Default = null)
        {
            var SqlValue = new DatabaseValue
            {
                Name = Name,
                Type = Type,
                Size = Size,
                Unique = Unique,
                Primary = Primary,
                Value = Default
            };
            Add(SqlValue);
        }

        /// <summary>
        /// Adds a value to the list
        /// </summary>
        /// <param name="Name">The name of this value in the database</param>
        /// <param name="Value">The value to store or search for</param>
        public void Add(string Name, dynamic Value)
        {
            var SqlValue = new DatabaseValue
            {
                Name = Name,
                Value = Value
            };
            Add(SqlValue);
        }

        /*/// <summary>
        /// Adds a value to the list
        /// </summary>
        /// <param name="Name">The name of this value in the database</param>
        public void Add(string Name)
        {
            var SqlValue = new DatabaseValue
            {
                Name = Name
            };
            Add(SqlValue);
        }*/

        /// <summary>
        /// Initializes a new value list
        /// </summary>
        public ValueList() : base() { }
    }
}
