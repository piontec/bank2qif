using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bank2Qif.src.Transformers
{
    /// <summary>
    /// Attribute to decorate QifEntries transformers
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TransformerAttribute : Attribute
    {        
        readonly int priority;
    
        /// <summary>
        /// The priority of transformers. Transformers are applied in ascending order of the priority.
        /// </summary>
        /// <param name="priority"></param>
        public TransformerAttribute (int priority) 
        { 
            this.priority = priority;
        }
   
        public int Priority
        {
            get { return priority; }
        }   
    }
}
