using System;
using System.Collections.Generic;
using System.Linq;


namespace Bank2Qif
{
	public class BankDates
	{
		public DateTime BookingDate {get; set;}
		public DateTime OperationDate {get; set;}


		public override string ToString ()
		{
			return string.Format ("[BankDates: BookingDate={0}, OperationDate={1}]", BookingDate, OperationDate);
		}
	}
}

