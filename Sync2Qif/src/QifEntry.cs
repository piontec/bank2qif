using System;
using System.Globalization;
using System.Text;

namespace Bank2Qif
{
	public class QifEntry
	{
		public BankDates Date {get; set;}
		public decimal Amount {get; set;}
		public string Description {get; set;}
        /// <summary>
        /// Name of the account for this operation in a software, which imports .qif files
        /// </summary>
		public string AccountName {get; set;}
        /// <summary>
        /// The payee of the transaction, if present and applicable
        /// </summary>
        public string Payee { get; set; }
               

		public override string ToString ()
		{
			return string.Format ("[QifEntry: Date={0}, Amount={1}, RemoteNumber={2}," +
                "Description={3}, AccountName={4}]", Date, Amount, Payee, Description, AccountName);
		}


        public string ToQifString()
        {
            string nl = System.Environment.NewLine;
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format ("D{0}{1}", Date.BookingDate.ToString("MM/dd/yyyy"), nl));
            sb.Append(string.Format("P{0}{1}", Payee, nl));
            sb.Append(string.Format("M{0}{1}", Description, nl));
            sb.Append(string.Format("T{0}{1}", Amount.ToString (CultureInfo.InvariantCulture), nl));
            sb.Append(string.Format("L{0}{1}", AccountName, nl));
            sb.Append(string.Format("^{0}", nl));

            return sb.ToString();
        }
	}
}

