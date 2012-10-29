using System;

namespace Sync2Qif
{
	public class QifEntry
	{
		public BankDates Date {get; set;}
		public double Amount {get; set;}
		public string Description {get; set;}
		public string AccountName {get; set;}

		public override string ToString ()
		{
			return string.Format ("[QifEntry: Date={0}, Amount={1}, Description={2}, AccountName={3}]", Date, Amount, Description, AccountName);
		}
	}
}

