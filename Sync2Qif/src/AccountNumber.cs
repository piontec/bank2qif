using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bank2Qif
{
    public class AccountNumber
    {
        public AccountNumber(string num)
        {
            if (num == string.Empty) 
            {
                Number = num;
                return;
            }

            var nums = num.Split(new char[] { ' ' });
            if (nums.Length != 7)
                throw new ArgumentException("Wrong format of account number");
            if (nums[0].Length != 2)
                throw new ArgumentException("Wrong number of digits in first section (two expected)");
            for (int i = 1; i < 7; i++)
                if (nums[i].Length != 4)
                    throw new ArgumentException(
                        string.Format ("Wrong number of digits in section no {0} (four expected)", i));
            Number = num;
        }

        public string Number { get; private set; }


        public override string ToString()
        {
            return Number;
        }
    }
}
