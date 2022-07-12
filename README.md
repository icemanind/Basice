# Basice

Basice (pronounced like *basis*) is a simple open source interpreter for a vintage BASIC language.

## Description

Basice comes with a class project that includes a lexical analyzer, a parser and an interpreter. The lexical analyzer (or lexer) converts input into a stream of tokens. These tokens are then fed into the parser. The parser parses the tokens and create a syntax tree from the tokens. Finally, this syntax tree is fed into the interpreter and the interpreter interprets the tree and produces output.

Basice also comes with a simple WinForms application that allows you to create and edit a BASIC program and a simulated console window. Once you type in your BASIC program, you can run the program and you will see the output on the simulate console.  

## Getting Started

### Dependencies

* Basice was created with the .NET Framework 4.7.2. It should work on any computer or OS that has .NET Framework 4.7.2, like Windows or Mono. 
* Basice was created using Visual Studio 2022, although you should be able to use Visual Studio 2019 to compile it.

## Syntax
All Basice lines of code should begin with a line number, followed by an instruction. Here is an example:

    10 PRINT "HELLO WORLD!"

You can put multiple instructions on one line by separating the instructions with a colon `:`. Here is an example:

    10 PRINT "HELLO ":PRINT "WORLD!"
    
Spaces are ignored, so the above example is equivalent to the following example:

    10 PRINT"HELLO"    :     PRINT       "WORLD!"

Line numbers must be whole integer numbers and they must increase in order. The following example is invalid and will produce a parse error:

    10 PRINT "HELLO"
    20 PRINT " WORLD!!"
    15 PRINT "This is invalid since the next line after line 20 must be greater than 20."
    
Comments are allowed and begin with an apostrophe `'`. Comments must also be on a line number. Here is an example:

    10 ' This program will print HELLO WORLD!!!
    20 PRINT "HELLO WORLD!!!"
    
Basice only has two data types, strings and numbers. A number can be a whole integer number or a floating point number. Full expressions are permitted:

    10 ' This program will print 356.9
    20 PRINT 12 * 38.4 + 7 * 2.3 - 8 * (9+6)
    30 ' Spaces are ignored so the next line prints the same thing
    40 PRINT 12*38.4+7*2.3-8*(9+6)
    
Basice evaluates expressions using the standard math rules. Anything in parenthesis's are evaluated first, followed my multiplication and division and finally addition and subtraction. 

Basice can concatenate strings using the plus `+` operator:

    10 PRINT "HELLO " + "WORLD" + "!!!"

This prints `HELLO WORLD!!!` to the console.

Basice supports variables and there are two types of variables. String variables and number variables. String variables must end with a dollar sign `$` :

    10 PI=3.14
    20 PIE$ = "Pie!"
    30 PRINT "I love " + PIE$
    40 PRINT PI

This program will print `I love Pie!` to the console and then print the value `3.14` to the console. If you use a string variable without initializing it, it defaults to an empty string `""`. If you use a number variable without initializing it, it defaults to a zero `0`. 

## Author

[Alan Bryan](https://www.icemanind.com)

