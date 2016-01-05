// Taken from Sprache http://code.google.com/p/sprache/
// Distributed under Apache License 2.0 http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using Sprache;

namespace Bank2Qif.Parsers
{
    public static class CsvParser
    {        
		public static Parser<char> CellSeparatorFu (char sep) 
		{
		 return Parse.Char(sep);
		}

        static readonly Parser<char> QuotedCellDelimiter = Parse.Char('"');

        static readonly Parser<char> QuoteEscape = Parse.Char('"');

        static Parser<T> Escaped<T>(Parser<T> following)
        {
            return from escape in QuoteEscape
                   from f in following
                   select f;
        }

        static readonly Parser<char> QuotedCellContent =
            Parse.AnyChar.Except(QuotedCellDelimiter).Or(Escaped(QuotedCellDelimiter));

		static Parser<char> LiteralCellContent (char sep)
		{
        	return Parse.AnyChar.Except(CellSeparatorFu(sep)).Except(Parse.String(Environment.NewLine));
		}



        static readonly Parser<string> QuotedCell =
            from open in QuotedCellDelimiter
            from content in QuotedCellContent.Many().Text()
            from end in QuotedCellDelimiter
            select content;

        static readonly Parser<string> NewLine =
            Parse.String(Environment.NewLine).Text();

        static readonly Parser<string> RecordTerminator =
            Parse.Return("").End().XOr(
            NewLine.End()).Or(
            NewLine);

		static Parser<string> Cell (char sep)
		{
			return QuotedCell.XOr(
				LiteralCellContent(sep).XMany().Text());
		}
        
		public static Parser<IEnumerable<string>> CsvRecord (char sep)
		{
			return from leading in Cell (sep)
				from rest in CellSeparatorFu(sep).Then(_ => Cell(sep)).Many()
            	from terminator in RecordTerminator
            	select Cons(leading, rest);
		}            

		public static Parser<IEnumerable<IEnumerable<string>>> Csv (char sep)
		{
			return CsvRecord (sep).XMany().End();
		}

        static IEnumerable<T> Cons<T>(T head, IEnumerable<T> rest)
        {
            yield return head;
            foreach (var item in rest)
                yield return item;
        }

        public static Parser<IEnumerable<IEnumerable<string>>> CsvComma = Csv (',');

        public static Parser<IEnumerable<IEnumerable<string>>> CsvSemicolon = Csv(';');

		public static Parser<IEnumerable<IEnumerable<string>>> CsvPipe = Csv('|');

        public static Parser<IEnumerable<string>> CsvRecordComma = CsvRecord (',');

        public static Parser<IEnumerable<string>> CsvRecordSemicolon = CsvRecord(';');

		public static Parser<IEnumerable<string>> CsvRecordPipe = CsvRecord('|');
    }
}
