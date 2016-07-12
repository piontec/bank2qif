using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Parsers;
using Sprache;
using System.Text.RegularExpressions;


namespace Bank2Qif.Converters.BankSmart
{
	//"Data transakcji","Data księgowania","Odbiorca / Nadawca","Tytuł operacji","Kwota","Właściciel rachunku","Nazwa konta","Saldo po operacji"
	//2016-04-19,2016-04-19,"SENDER","Przelew zagraniczny przychodzący","1.11 USD","OWNER","SMART Biznes Konto USD","1.11 USD"
	//2016-04-08,2016-04-12,"FM Bank PBP S.A.","Transakcja bezgotówkowa xxxx xxxx xxxx xxxx","-1.11 USD","OWNER","SMART Biznes Konto USD","1.11 USD"

	[Converter("banksmart", "csv")]
	public class BankSmartCsvToQif : BaseConverter
	{
		public override IList<QifEntry> ConvertLinesToQif (string lines)
		{	
			Regex rgx = new Regex("[A-Z]{3}");
			var entries = from csvline in CsvParser.CsvComma.Parse (lines).Skip (1)
				let csv = csvline.ToArray ()
				let opDate = GenericParsers.DateYyyyMmDd.Parse (csv [0])
				let bookingDate = GenericParsers.DateYyyyMmDd.Parse (csv [1])
				let amount = rgx.Replace (csv [4], string.Empty)
				let payee = csv [2]  
				let desc = csv [3]
				select new QifEntry
			{
				Amount = Decimal.Parse(amount.Replace(",", ".").Replace(" ", "")),
				Date = new BankDates {OperationDate = opDate, BookingDate = bookingDate},
				Payee = payee.Trim (),
				Description = desc.Trim (new [] {' ',';'})
			};

			entries.Reverse ();
			return entries.ToList ();
		}


		public override Encoding GetEncoding ()
		{
			return Encoding.UTF8;
		}
	}
}