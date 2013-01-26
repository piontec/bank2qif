using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Parsers;
using Sprache;

namespace Bank2Qif.Converters.Inteligo
{
    //"Id","Data księgowania","Data zlecona","Typ transakcji","Kwota","Waluta","Saldo po transakcji","Rachunek nadawcy/odbiorcy","Nazwa nadawcy/odbiorcy","Opis transakcji"
    //"3065","2012-01-15","2012-01-15","Przelew z rachunku","-198.00","PLN","493.61","50102055581111174342100049","Joanna Piątkowska  ul. Cisowa 6, Pasieka 77-200 Miastko","Joanna Piątkowska  ul. Cisowa 6, Pasieka 77-200 Miastko","Nr banku: 10205558","Nr rach.: 50102055581111174342100049","Tytuł: za bilet na coldplay","Data przygotowania: 2012-01-15","Data waluty: 2012-01-15"
    //"3067","2012-01-15","2012-01-15","Opłata","-0.99","PLN","492.62","","","Opłata miesięczna","Karta płatnicza: 0,99","Data waluty: 2012-01-15"

    public static class InteligoCsvParsers
    {
    }
}
