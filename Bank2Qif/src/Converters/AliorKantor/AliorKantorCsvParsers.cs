using System.Globalization;
using System.Linq;
using Bank2Qif.Parsers;
using Sprache;


namespace Bank2Qif.Converters.AliorKantor
{
    public static class AliorKantorCsvParsers
    {
        public static readonly Parser<decimal> Amount =
            from minus in Parse.String("-").Text().Or(Parse.Return(string.Empty))
            from triples in GenericParsers.OptionalSpaceThenNumber.Token().Many()
            from separator in Parse.String(",").Text().Or(Parse.Return(string.Empty))
            from pointPart in Parse.Number.Or(Parse.Return("00"))
            let strDecimal = string.Format("{0}{1}.{2}", minus, triples.Aggregate((s1, s2) => s1 + s2),
                                            pointPart)
            select decimal.Parse(strDecimal, CultureInfo.InvariantCulture);
    }
}