using Nini.Config;


namespace Bank2Qif.Transformers
{
    public abstract class BaseTransformer
    {
        protected const string INI_ENABLED = "Enabled";
        protected bool m_enabled;
        protected IConfig m_config;


        protected void Initialize (IConfig config)
        {
            m_config = config;
            m_enabled = m_config.GetBoolean (INI_ENABLED, true);
        }
    }
}