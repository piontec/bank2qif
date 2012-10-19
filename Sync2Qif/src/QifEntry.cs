using System;

namespace Sync2Qif
{
	public class QifEntry
	{
		public DateTime Date {get; set;}
		public double Amount {get; set;}
		public string Description {get; set;}
		public string AccountName {get; set;}
	}
}

