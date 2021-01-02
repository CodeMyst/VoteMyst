using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database
{
    /// <summary>
    /// Represents a configuration value in the database.
    /// </summary>
    public class ConfigurationValue
    {
        /// <summary>
        /// The primary key of the configuration value.
        /// </summary>
        [Key, Required]
        public string Key { get; set; }

        /// <summary>
        /// The value of the configuration value.
        /// </summary>
        public string Value { get; set; }
    }
}
