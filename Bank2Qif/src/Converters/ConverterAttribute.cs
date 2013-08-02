using System;


namespace Bank2Qif.Converters
{
    /// <summary>
    ///     Attribute to decorate Converters from a specific bank format to <see cref="QifEntry" />
    /// </summary>
    [AttributeUsage (AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ConverterAttribute : Attribute
    {
        private readonly string m_bank;
        private readonly string m_ext;


        /// <summary>
        ///     A default constructor
        /// </summary>
        /// <param name="bank">A name of a bank, from which the decorated class is used to convert data.</param>
        /// <param name="ext">An expected file extension for data files of this bank</param>
        public ConverterAttribute (string bank, string ext)
        {
            m_bank = bank;
            m_ext = ext;
        }


        public string Bank
        {
            get { return m_bank; }
        }

        public string Extension
        {
            get { return m_ext; }
        }
    }
}