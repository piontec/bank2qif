using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nini.Config;

namespace Bank2Qif.Transformers
{
    public abstract class BaseTransformer
    {
        protected const string INI_ENABLED = "Enabled";
        protected bool m_enabled;

        public void Initialize(IConfig config)
        {
            m_enabled = config.GetBoolean(INI_ENABLED, true);
        }
    }
}
