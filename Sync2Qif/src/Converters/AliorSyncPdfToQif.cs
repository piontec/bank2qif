using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading;
using System.Globalization;
using System.IO;

namespace Bank2Qif.Converters
{
    [Converter ("sync", "pdf")]
    public class AliorSyncPdfToQif : IConverter
    {
        public IList<QifEntry> ConvertFileToQif(string fileName)
        {
            throw new NotImplementedException();
        }

        //private readonly Func<XElement, XElement, XElement> strAgg = 
        //    (e1, e2) => new XElement("line", (string)e1 + (string)e2);

        //private readonly string[] OpNames = new string[] 
        //{
        //    "PRZELEW W RAMACH BANKU NA RACH OBCY", 
        //    "PRZELEW DO INNEGO BANKU KRAJOWEGO",
        //    "PRZELEW KRAJOWY ELIXIR PRZYCHODZACY Z INNEGO",
        //    "PRZELEW NATYCHMIASTOWY",
        //    "ZAŁOŻENIE LOKATY",
        //};

        //private const int TITLE_PAGE_FIRST_BOX = 10;
        //private const int NEXT_PAGE_FIRST_BOX = 1;

        //private IEnumerable<XElement> ExtractBoxes(XElement page, int firstBoxId)
        //{
        //    int lastBoxId = int.Parse((
        //        from b in page.Elements("textbox")
        //        let firstLn = b.Elements("textline").First()
        //        let firstLnTxt = (string)firstLn.Elements("text").Aggregate(strAgg)
        //        where firstLnTxt.StartsWith("Infolinia Alior Sync") || firstLnTxt.StartsWith("Niniejszy dokument jest")
        //        select b.Attribute("id").Value)
        //                               .First());
        //    var boxes =
        //        from b in page.Elements("textbox")
        //        let bid = (int)b.Attribute("id")
        //        where bid > firstBoxId && bid < lastBoxId
        //        select b;
            
        //    int newid = 0;
        //    var lines =
        //        from b in boxes
        //        select new XElement("box",
        //                               new XAttribute("id", newid++),
        //                               from tl in b.Elements("textline")
        //                               select tl.Elements("text")
        //                               .Aggregate(strAgg)
        //                            );
            
        //    return lines;
        //}


        //private IList<QifEntry> ConvertBoxes(IEnumerable<XElement> boxes)
        //{
        //    var result = new List<QifEntry>();
        //    var boxesList = boxes.ToList();

        //    var blockDates = new List<BankDates>();
        //    IList<QifEntry> blockEntries;
        //    for (int i = 0; i < boxesList.Count; i++)
        //    {
        //        var box = boxesList[i];
        //        BankDates date = BankDates.TryParse(box);
        //        if (date != null)
        //        {
        //            blockDates.Add(date);
        //            continue;
        //        }
        //        int parsedBoxes;
        //        blockEntries = TryParseTransactions(boxesList, i, blockDates.Count, out parsedBoxes);
        //        MergeDatesWithTransactions(blockEntries, blockDates);

        //        i = parsedBoxes;
        //        blockDates.Clear();
        //        result.AddRange(blockEntries);
        //    }

        //    return result;
        //}


        //private void MergeDatesWithTransactions(IList<QifEntry> blockEntries, List<BankDates> blockDates)
        //{
        //    if (blockEntries.Count != blockDates.Count)
        //        throw new ApplicationException("Different number of entries");
        //    for (int i = 0; i < blockEntries.Count; i++)
        //        blockEntries[i].Date = blockDates[i];
        //}


        //private IList<QifEntry> TryParseTransactions(List<XElement> boxesList, int startIndex, int count, out int i)
        //{
        //    var result = new List<QifEntry>();

        //    QifEntry current = null;
        //    i = startIndex;
        //    int found = 0;
        //    while (found < count)
        //    {
        //        var line = (string)boxesList[i].Elements("line").Aggregate(strAgg);
        //        // check if current line is a standard operation name
        //        if (!IsOpLine(line))
        //            throw new ApplicationException(string.Format("Wrong line: {0}", line));

        //        current = new QifEntry { Description = line };
        //        decimal amount = decimal.Parse((string)boxesList[i + 1].Element("line"));
        //        //double balance = double.Parse ((string) boxesList [i + 2].Element ("line"));
        //        var nextLine = (string)boxesList[i + 3].Elements("line").Aggregate(strAgg);
        //        current.Amount = amount;
        //        result.Add(current);
        //        found++;

        //        if (IsOpLine(nextLine))
        //        {
        //            i += 3;
        //            continue;
        //        }
        //        current.Description += " " + nextLine;
        //        i += 4;
        //    }

        //    if (count != result.Count)
        //        throw new ApplicationException("Parsing error, wrong number of entries parsed");

        //    return result;
        //}


        //private bool IsOpLine(string line)
        //{
        //    return OpNames.Any(s => line.StartsWith(s));
        //}
        
        
        //public IList<QifEntry> ConvertFileToQif(string fileName)
        //{
        //    if (!fileName.EndsWith ("*.pdf", true, CultureInfo.InvariantCulture))
        //        throw new ApplicationException(string.Format("File name does not end in *.pdf: {0}", fileName));
        //    if (!File.Exists(fileName))
        //        throw new ApplicationException(string.Format("File does not exists: {0}", fileName));
            
        //    return ConvertXmlToQif (PdfToXmlReader.Read(fileName));
        //}


        //public IList<QifEntry> ConvertXmlToQif (XDocument xml)
        //{
        //    var cult = Thread.CurrentThread.CurrentCulture;
        //    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("pl-PL");
			
        //    IEnumerable<XElement> pages = from page in xml.Descendants("page")
        //        select page;
			
        //    var page1 = pages.Where(p => (int)p.Attribute("id") == 1).Single();
        //    var boxes = ExtractBoxes(page1, TITLE_PAGE_FIRST_BOX);
			
        //    var nextPages = pages.Where (p => (int) p.Attribute ("id") > 1);
        //    foreach (var p in nextPages)
        //        boxes = boxes.Concat(ExtractBoxes(p, NEXT_PAGE_FIRST_BOX));
			
        //    var entries = ConvertBoxes(boxes);
        //    Thread.CurrentThread.CurrentCulture = cult;

        //    return entries;
        //}
       
    }
}
