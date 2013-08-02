using System;


namespace Bank2Qif.Transformers
{
    /// <summary>
    ///     Attribute to decorate QifEntries transformers
    /// </summary>
    [AttributeUsage (AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TransformerAttribute : Attribute
    {
        private readonly int m_priority;


        /// <summary>
        ///     The priority of transformers. Transformers are applied in ascending order of the priority.
        /// </summary>
        /// <param name="priority"></param>
        public TransformerAttribute (int priority)
        {
            this.m_priority = priority;
        }


        public int Priority
        {
            get { return m_priority; }
        }
    }
}