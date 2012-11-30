using System;
using System.IO;
using System.Xml.Linq;
using System.Data.Linq;
using Nini.Config;
using Sync2Qif.Converters;
using Castle.Windsor;

namespace Sync2Qif
{
	public class Runner
	{
        internal enum ExitCodes : int
        {
            Success = 0,
            SyntaxError = 2,
        }

        private string m_fileName, m_bankType;
        private WindsorContainer m_container;


		public static void Main (string[] args)
		{
            Runner runner = new Runner();
            runner.Run(args);            
		}


        private void Run(string[] args)
        {
            LoadCmdLineArgs(args);
            LoadContainer();

            var converter = GetConverter(m_bankType);
            VerifyArgs(converter, m_fileName);

            var entries = converter.ConvertFileToQif(m_fileName);
            ProcessEntries();
        }

        private void ProcessEntries()
        {
            throw new NotImplementedException();
        }

        private IConverter GetConverter(string bankType)
        {
            return m_container.Resolve<IConverter>(bankType);
        }

        private void VerifyArgs(IConverter converter, string fileName)
        {
            //var supportedExt = Attribute.GetCustomAttribute(typeof(converter), ConverterAttribute);
            throw new NotImplementedException();
            // file extension and converter type
        }


        private void LoadContainer()
        {
            m_container = new WindsorContainer();
            m_container.Install(new ConvertersInstaller());
        }


        private void LoadCmdLineArgs(string[] args)
        {
            ArgvConfigSource source = new ArgvConfigSource(args);

            source.AddSwitch("Main", "file-name", "f");
            source.AddSwitch("Main", "bank-type", "t");
                    
            if (args.Length != 4)
            {
                Console.WriteLine ("Wrong number of parameters");
                DisplayHelpAndExit(args, ExitCodes.SyntaxError);
            }
            
            m_fileName = source.Configs["Main"].Get("file-name");
            m_bankType = source.Configs["Main"].Get("bank-type");
        }


		private bool ValidateArgs (string[] args)
		{
			if (args.Length != 2) {
			
				return false;
			}

			string fileName = args [1];
			if (!File.Exists (fileName)) {
				Console.WriteLine ("File does not exists");
				return false;
			}

			if (!File.Exists (fileName)) {
				Console.WriteLine ("File does not exists");
				return false;
			}

			if (!fileName.EndsWith (".pdf", StringComparison.CurrentCultureIgnoreCase)) {
				Console.WriteLine ("Wrong file extension, *.pdf expected");
				return false;
			}

			return true;
		}


		private void DisplayHelpAndExit (string[] args, ExitCodes code)
		{
			Console.WriteLine (string.Format ("Usage: {0} -t [bank type] -f [file name]", args [0]));
            System.Environment.Exit((int) code);
		}	
	}
}
