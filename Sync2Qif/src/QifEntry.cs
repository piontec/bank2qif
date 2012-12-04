using System;

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
        /// The remote account number, if present and applicable
        /// </summary>
        public string RemoteAccountNumber { get; set; }
               

		public override string ToString ()
		{
			return string.Format ("[QifEntry: Date={0}, Amount={1}, RemoteNumber={2}," +
                "Description={3}, AccountName={4}]", Date, Amount, RemoteAccountNumber, Description, AccountName);
		}


        public string ToQifString()
        {
            throw new NotImplementedException();
        }
	}
}

