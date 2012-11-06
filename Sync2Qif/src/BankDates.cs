using System;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;


namespace Sync2Qif
{
	public class BankDates
	{
		public DateTime BookingDate {get; set;}
		public DateTime OperationDate {get; set;}

		public static BankDates TryParse (XElement box)
		{
			if (box.Name != "box")
				return null;

			var lines = box.Elements ("line");
			if (lines.Count () != 2)
				return null;

            DateTime booking, operation;            
			bool success = DateTime.TryParse ((string) lines.ElementAt (0), out booking); 
			success &=	DateTime.TryParse ((string) lines.ElementAt (1), out operation);

			return success ? new BankDates {BookingDate = booking, OperationDate = operation} : null;
		}


		public override string ToString ()
		{
			return string.Format ("[BankDates: BookingDate={0}, OperationDate={1}]", BookingDate, OperationDate);
		}
	}
}

