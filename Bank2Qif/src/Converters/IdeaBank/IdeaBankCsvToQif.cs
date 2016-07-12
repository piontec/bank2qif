using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Parsers;
using Sprache;


namespace Bank2Qif.Converters.IdeaBank
{
	//Zestawienie transakcji;20160427;
	//20160330;20160330;11sender0123450001;00000001;1111,00;PLN;1,00;1111,00;11receiver00001;Receiver Name;;receiver address;;11000000;description;type;

	[Converter("ideabank", "csv")]
	public class IdeaBankCsvToQif : BaseConverter
	{
		public override IList<QifEntry> ConvertLinesToQif (string lines)
		{
			var entries = from csvline in CsvParser.CsvSemicolon.Parse (lines).Skip (1)
				let csv = csvline.ToArray ()
				let bookingDate = GenericParsers.DateYyyyMmDd.Parse (csv [0])
				let opDate = GenericParsers.DateYyyyMmDd.Parse (csv [0])
				let type = csv [15]
				let amount = type == "uznanie" ? csv [4] : "-"+csv[4]
				let account = type == "uznanie" ? csv [2] : csv [8]
				let payee = csv [9]  
				let desc = csv [14]
				select new QifEntry
			{
				Amount = Decimal.Parse(amount.Replace(",", ".").Replace(" ", "")),
				Date = new BankDates {OperationDate = opDate, BookingDate = bookingDate},
				Payee = string.Format("{0} [{1}]", payee.Trim (), account),
				Description = string.Format ("{1}: {0}", desc.Trim (new [] {' ',';'}), type.Trim())
			};

			return entries.ToList ();
		}


		public override Encoding GetEncoding ()
		{
			return Encoding.UTF8;
		}
	}
}