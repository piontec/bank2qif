using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Parsers;
using Sprache;


namespace Bank2Qif.Converters.IdeaBank
{
	//Zestawienie transakcji;20190809;
	//Data waluty;Data zlecenia;Numer rachunku nadawcy;Numer konta VAT;Nadawca;Nadawca;Nadawca;Nadawca;Numer banku nadawcy;Kwota w walucie rachunku;Waluta;Kurs;Kwota w walucie zlecenia;Kwota VAT;Numer rachunku odbiorcy;NIP odbiorcy;Odbiorca;Odbiorca;Odbiorca;Odbiorca;Numer banku odbiorcy;Tytuł;Numer faktury;Rodzaj operacji;Numer transakcji w systemie centralnym
	//20190731;20190801;012301230123;;IDEA Bank SA;ul. Przyokopowa 33;01-208 Warszawa;;19500001;1,11;PLN;1,00;1,11;;00195000011111;;IDEA Bank SA;ul. Przyokopowa 33;01-208 Warszawa;;19500001;Naliczanie odsetek;;obciążenie;3111111111

	[Converter("ideabank", "csv")]
	public class IdeaBankCsvToQif : BaseConverter
	{
		public override IList<QifEntry> ConvertLinesToQif (string lines)
		{
			var entries = from csvline in CsvParser.CsvSemicolon.Parse (lines).Skip (2)
				let csv = csvline.ToArray ()
				let bookingDate = GenericParsers.DateYyyyMmDd.Parse (csv [1])
				let opDate = GenericParsers.DateYyyyMmDd.Parse (csv [0])
				let type = csv [23]
				let amount = type == "uznanie" ? csv [9] : "-"+csv [9]
				let account = type == "uznanie" ? csv [2] : csv [14]
				let payee = type == "uznanie" ? string.Format("{0},{1},{2}", csv [4], csv [5], csv [6]) : string.Format("{0},{1},{2}", csv [16], csv [17], csv [18])
				let desc = csv [21]
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