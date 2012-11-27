using System;
using System.IO;
using System.Xml.Linq;
using System.Data.Linq;
using Nini.Config;
using Sync2Qif.Converters;

namespace Sync2Qif
{
	public class Runner
	{
        internal enum ExitCodes : int
        {
            Success = 0,
            SyntaxError = 2,
        }

        private string fileName, bankType;


		public static void Main (string[] args)
		{
            Runner runner = new Runner();
            runner.Run(args);            
		}


        private void Run(string[] args)
        {
            LoadCmdLineArgs(args);
            LoadConverters();

            var converter = GetConverter(bankType);
            VerifyArgs(converter, fileName);

            var entries = converter.ConvertFileToQif(fileName);
            ProcessEntries();
        }

        private void ProcessEntries()
        {
            throw new NotImplementedException();
        }

        private IConverter GetConverter(string bankType)
        {
            throw new NotImplementedException();
        }

        private void VerifyArgs(IConverter converter, string fileName)
        {
            throw new NotImplementedException();
            // file extension and converter type
        }


        private void LoadConverters()
        {
            throw new NotImplementedException();
        }


        private void LoadCmdLineArgs(string[] args)
        {
            ArgvConfigSource source = new ArgvConfigSource(args);

            source.AddSwitch("Main", "file-name", "f");
            source.AddSwitch("Main", "bank-type", "t");
                    
            if (args.Length != 5)
            {
                Console.WriteLine ("Wrong number of parameters");
                DisplayHelpAndExit(args, ExitCodes.SyntaxError);
            }
            
            fileName = source.Configs["Main"].Get("file-name");
            bankType = source.Configs["Main"].Get("bank-type");
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
