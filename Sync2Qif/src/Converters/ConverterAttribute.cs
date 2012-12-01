using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bank2Qif.Converters
{
    /// <summary>
    /// Attribute to decorate Converters from a specific bank format to <see cref="QifEntry"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ConverterAttribute : Attribute
    {
        private string bank;
        private string ext;

        /// <summary>
        /// A default constructor
        /// </summary>
        /// <param name="bank">A name of a bank, from which the decorated class is used to convert data.</param>
        /// <param name="ext">An expected file extension for data files of this bank</param>
        public ConverterAttribute(string bank, string ext)
        {
            this.bank = bank;
            this.ext = ext;
        }

        public string Bank { get { return bank; } }

        public string Extension { get { return ext; } }
    }
}
