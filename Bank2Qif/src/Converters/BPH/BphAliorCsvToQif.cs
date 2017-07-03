using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Parsers;
using Sprache;


namespace Bank2Qif.Converters.BPH
{
	//Data ksi�gowania;Data transakcji;Nadawca;Odbiorca;Tytu� p�atno�ci (linia 1);Tytu� p�atno�ci (linia 2);Tytu� p�atno�ci (linia 3);Tytu� p�atno�ci (linia 4);Opis transakcji;Kwota;Waluta;Saldo po operacji
	//20170516;20170516;"sender ";"user ";"z BPH alior";"";"";"";"Przelew krajowy";-111,00;"PLN";"1,11"
	//20170516;20170514;"sender ";" ";"Transakcja kart� debetow�;obci��enie 11.11 PLN z dnia:2017-01-11 Kod MCC 1111  POL";"";"";"";"Transakcja kart� debetow�";-11,11;"PLN";"1111,11"
	//20170516;20170516;"sender ";"user ";"desc";"";"";"";"Przelew krajowy";-111,00;"PLN";"1,11"
	[Converter("bphalior", "csv")]
	public class BphAliorCsvToQif : BaseConverter
	{
		public override IList<QifEntry> ConvertLinesToQif (string lines)
		{
			var lns = lines.Replace ("\r", "\n");
			var entries = from csvline in CsvParser.CsvSemicolon.Parse (lns).Skip (1)
				where csvline.Count() == 12
				let csv = csvline.ToArray ()
				let bookingDate = GenericParsers.DateYyyyMmDd.Parse (csv [0])
				let opDate = GenericParsers.DateYyyyMmDd.Parse (csv [1])
				let account = csv [2]
				let payee = csv [3]
				let desc = string.Format ("{0} {1} {2} {3}: {4}", csv[4], csv[5], csv[6], csv[7], csv[8])
				let amount = csv [9]
				select new QifEntry
			{
				Amount = Decimal.Parse(amount.Replace(",", ".").Replace(" ", "")),
				Date = new BankDates {OperationDate = opDate, BookingDate = bookingDate},
				Payee = payee.Trim (),
				Description = desc,
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