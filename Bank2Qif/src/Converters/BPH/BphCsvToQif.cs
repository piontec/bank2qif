using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Parsers;
using Sprache;


namespace Bank2Qif.Converters.BPH
{
	//Unikalny identyfikator|Data księgowania|Data efektywnej operacji|Rachunek nadawcy/odbiorcy|Nazwa nadawcy/odbiorcy|Tytułem|Opis operacji|Kwota|Typ|Notatka|Saldo|
	//1111111111111110001|28-12-2015|24-12-2015|11 2222 2222 2222 2222 2222 2222| NADAWCA |OPIS OD NADAWCY|Transakcja kartą                   |-22.33|Obciążenie|Transakcja kartą                   ||1 111.11|

	[Converter("bph", "txt")]
	public class BphCsvToQif : BaseConverter
	{
		public override IList<QifEntry> ConvertLinesToQif (string lines)
		{
			var entries = from csvline in CsvParser.CsvPipe.Parse (lines).Skip (1)
				let csv = csvline.ToArray ()
				let bookingDate = GenericParsers.DateDdMmYyyy.Parse (csv [1])
				let opDate = GenericParsers.DateDdMmYyyy.Parse (csv [2])
				let account = csv [3]
				let payee = csv [4]  
				let desc = csv [5].Split (new []{'-'}, 2)
				let type = csv [6]
				let amount = csv [7]
				select new QifEntry
			{
				Amount = Decimal.Parse(amount.Replace(",", ".").Replace(" ", "")),
				Date = new BankDates {OperationDate = opDate, BookingDate = bookingDate},
				Payee = payee.Trim (),
				Description = string.Format ("{1}: {0}", (desc.Length > 1 ? desc[1] : desc[0]).Trim (new [] {' ',';'}),
					type.Trim()),
				AccountName = account
			};

			return entries.ToList ();
		}


		public override Encoding GetEncoding ()
		{
			return Encoding.GetEncoding ("windows-1250");
		}
	}
}