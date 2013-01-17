using System;
using System.IO;
using System.Linq;
using Nini.Config;
using Bank2Qif.Converters;
using Bank2Qif.Transformers;
using Castle.Windsor;
using Castle.MicroKernel;
using System.Collections.Generic;

namespace Bank2Qif
{
	public class Runner
	{
        internal enum ExitCodes : int
        {
            Success = 0,
            SyntaxError = 2,
            NoConverter = 4,
            WrongFile = 8,
            FileNotFound = 16,
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
            entries = ProcessEntries(entries);

            QifFile.Save(entries, m_fileName);
        }


        private IEnumerable<QifEntry> ProcessEntries(IEnumerable<QifEntry> entries)
        {
            var transformers = m_container.ResolveAll<ITransformer>().OrderBy (
                t => ((TransformerAttribute)
                    Attribute.GetCustomAttribute (t.GetType (), typeof (TransformerAttribute))).Priority
                );

            foreach (var transformer in transformers)
            {
                entries = transformer.Transform(entries);
            }

            return entries;
        }
        

        private IConverter GetConverter(string bankType)
        {
            IConverter result = null;
            try
            {
                result = m_container.Resolve<IConverter>(bankType);
            }
            catch (ComponentNotFoundException)
            {
                Console.Error.WriteLine (string.Format ("No converter found for bank type {0}", bankType));
                DisplayHelpAndExit(ExitCodes.NoConverter);
            }

            return result;
        }


        private void VerifyArgs(IConverter converter, string fileName)
        {
            string supportedExt = ((ConverterAttribute)Attribute.GetCustomAttribute(
                converter.GetType(), typeof(ConverterAttribute))).Extension;

            if (!fileName.EndsWith(supportedExt, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.Error.WriteLine(string.Format("Wrong file name for that type of" +
                    "converter, {0} file expected", supportedExt));
                DisplayHelpAndExit(ExitCodes.WrongFile);
            }

            if (!File.Exists(fileName))
            {
                Console.Error.WriteLine(string.Format ("File {0} does not exists", fileName));
                DisplayHelpAndExit(ExitCodes.FileNotFound);
            }
        }


        private void LoadContainer()
        {
            m_container = new WindsorContainer();
            m_container.Install(new ConvertersInstaller(), new TransformersInstaller());
            m_container.Kernel.Resolver.AddSubResolver(new ConfigSubresolver());
        }


        private void LoadCmdLineArgs(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Wrong number of parameters");
                DisplayHelpAndExit(ExitCodes.SyntaxError);
            }

            ArgvConfigSource source = new ArgvConfigSource(args);

            source.AddSwitch("Main", "file-name", "f");
            source.AddSwitch("Main", "bank-type", "t");
                        
            m_fileName = source.Configs["Main"].Get("file-name");
            m_bankType = source.Configs["Main"].Get("bank-type");
        }
        

		private void DisplayHelpAndExit (ExitCodes code)
		{
            Console.Error.WriteLine(string.Format("Usage: {0} -t [bank type] -f [file name]", 
                System.AppDomain.CurrentDomain.FriendlyName));
            System.Environment.Exit((int) code);
		}	
	}
}
