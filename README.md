[![Build status](https://travis-ci.com/piontec/bank2qif.svg?branch=master)](https://travis-ci.com/piontec/bank2qif.svg?branch=master)
# bank2qif
A companion project for https://www.gnucash.org/. Helps import bank statements to GnuCash.

## Intro
This project is aimed to help you easily import your bank statements into Gnucash. Currently, it supports only statements from a few polish banks. Bank2qif does two things for you: it gets your statement in a bank provided format (usually some form of CSV file) and converts it to QIF file, which can be easily imported to Gnucash, Second, during the process, it runs each of your transactions in the statement through a set of user provided rules, which can automatically match your transaction with a specific Gnucach accont. That way, for your regular transactions which have rules configured, there's nothing left to do after importing the statement to Gnucasch.

## How to use
Download the binary file from the [releases page](https://github.com/piontec/bank2qif/releases). Bank2qif requires an installed mono runtime on Linux. On Ubuntu and similar systems you can install it by running `apt-get install mono-runtime`. On Windows, all requirements should be already met.

### Supported banks
Currently, the following banks are supported:
* Kantor Internetowy Alior
* Inteligo (deprecated, I don't have account there anymore, maybe still works)
* mBank
* Alior Sync (deprecated, might be still used by Alior group)
* BZWBK (deprecated, I don't have account there anymore, maybe still works)
* BPH (deprecated, bank has merged with Alior and switched their system)
* Idea Bank
* Bank Smart (now branded as Nest Bank)
* BPH after merge with Alior (deprecated, I don't have account there anymore, maybe still works)

### Getting the statement
Just login to your bank and download a history of your account in a supported text file. The format of supported file can be checked by running `bank2qif.exe` without any command line options.

### Configure your matching rules
Edit the file `etc/rules.txt`. Comments start with "#". Every not commented non empty line forms a single rule. Each rule has the form:
```
[match_field] [operator] [text_to_match] => [gnucach_account]
```
where:
* `match_field` can have one of the values:
    * `any` - matches to any field in transaction's description
    * `Payee` - matches payee of the transaction
    * `Description` - matches description part of the transaction info
* `operator` can have one of the values
    * `=` - must be exact full text match
    * `%` - matches any substring
* `text_to_match` - the text in transaction to search in
* `gnucash_account` - name of the gnucash account to assign the transaction to

#### Examples
* if "SHELL" is found in a transaction description, assign it to account "Expenses:Car:Gas" in Gnucash
    ```
    Description % SHELL => Expenses:Car:Gas
    ```
* if the Payee field contains "John Smith", assign in Gnucash to "Expenses:Education:English"
    ```
    Payee % John Smith => Expenses:Education:English
    ```
* if the text "LADYBUG" is found anywhere, assign to "Expenses:Groceries"
    ```
    any % LADYBUG => Expenses:Groceries
    ```

### Running bank2qif
To check the names of available converters, just run:
```bash
./bank2qif.exe
```
When you have your bank statements and your rules ready, you can run bank2qif like this:
```bash
./bank2qif.exe -t [converter_name] [statement_file_path]
```
The resulting QIF file will be written in the same location as the statement file, but with `.qif` extension. 

### Importing to gnucash
In general, the import is done by selecting "File -> Import -> Import QIF" in the menu. This opens a pretty intuitive creator that guides you through the process. Still, pay attention to the following details:
* gnucash internally maps account names from QIF files into its own account names using the following rules:
    * if there already is an account with exactly the same name as in the QIF file, it is used and in the account mapping window the column "New" doesn't have a check mark
    * if there's no account with exactly the same name as in the QIF file, it will be created by gnucash and it shows a checked field in the "New" column; if you want, you can map the non existing account into a one that already exists in gnucash - just select the target account for each row; gnucash saves this mapping for later, so for a given gnucash file you only need to do this once
* double check the currency that will be used for your import - QIF files have no currency information, so you need to provide it
* after the creator closes, all your transactions will be already visible in gnucash accounts: the ones that weren't matched by bank2qif to any specific account are by default assigned to "QifImport" account; you can edit and move them to valid accounts from there.

## Developing bank2qif
### Support for bank statements formats - Converters
To develop a converter for a new bank and its statement format, have a look at the simplest converters like [src/IdeaBank/IdeaBankCsvToQif.cs](src/IdeaBank/IdeaBankCsvToQif.cs). A Converter musts extend the `BaseConverter` class, be tagged with `Converter` attribute and implement at least the `ConvertLinesToQif` method. All tagged converters are auto-registered on startup, so there's nothing else you have to do to make it available. Parsers used for Converters are implemented using [Sprache](https://github.com/sprache/Sprache) library. An example of Converter class declaration looks like this:
```csharp
[Converter("ideabank", "csv")]
public class IdeaBankCsvToQif : BaseConverter
{
	public override IList<QifEntry> ConvertLinesToQif (string lines)
    {
        ...
    }
}
```

### Statement data modification - Transformers
If you want to transform your data in a bank statement before it is saved into QIF file, have a look at [src/Transformers/ITransformer.cs](src/Transformers/ITransformer.cs) interface and the most frequently used implementation - the `SimpleMatch` transformer in [src/Transformers/SimpleMatch/SimpleMatchTransformer.cs](src/Transformers/SimpleMatch/SimpleMatchTransformer.cs). This is the transformer that matches transactions to account based on the `rules.txt` file. I have started a few others, but they're not really supported right now. A Transformer is also auto-registered at startup based on the `[Transformer` attribute. An example Transformer declaration looks like this:
```csharp
[Transformer (priority: 100)]
public class SimpleMatchTransformer : BaseTransformer, ITransformer
{
    public IEnumerable<QifEntry> Transform (IEnumerable<QifEntry> entries)
    {
        ...
    }
}
```

### Independent text format parsers - Parsers
If you want to reuse a rule that knows how to parse a specific, but recurring piece of text, like location specific date or currency format, please add them in `src/Parsers` directory.