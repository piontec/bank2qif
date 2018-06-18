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
TBD

## Developing bank2qif
TBD