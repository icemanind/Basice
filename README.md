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
    
Basice only has two data types: strings and numbers. A string is a sequence of characters between two double quotes `""`. A number can be a whole integer number or a floating point number. Full expressions are permitted:

    10 ' This program will print 356.9
    20 PRINT 12 * 38.4 + 7 * 2.3 - 8 * (9+6)
    30 ' Spaces are ignored so the next line prints the same thing
    40 PRINT 12*38.4+7*2.3-8*(9+6)
    
Basice evaluates expressions using the standard math rules. Anything in parenthesis's are evaluated first, followed my multiplication and division and finally addition and subtraction. 

Basice can concatenate strings using the plus `+` operator:

    10 PRINT "HELLO " + "WORLD" + "!!!"

This prints `HELLO WORLD!!!` to the console.

By default, after a `PRINT` command is issued, the cursor moves down to the next line. You can suppress this by adding a semicolon after the `PRINT` statement, like this:

    10 PRINT "HELLO ";
    20 PRINT "WORLD!!!";

This will print `HELLO WORLD!!!` all on one line.

Basice supports variables and there are two types of variables. String variables and number variables. String variables must end with a dollar sign `$` :

    10 PI=3.14
    20 PIE$ = "Pie!"
    30 PRINT "I love " + PIE$
    40 PRINT PI

This program will print `I love Pie!` to the console and then print the value `3.14` to the console. If you use a string variable without initializing it, it defaults to an empty string `""`. If you use a number variable without initializing it, it defaults to a zero `0`. 

Basice supports one and two dimensional arrays. You can create a one dimensional string array or number array like this:

    10 DIM A$(10)
    20 A$(2) = "Test"
    30 PRINT A$(2)
    
This will print `TEST` to the console. Before you can use arrays, you must use the `DIM` statement to allocate the number of elements your array is going to use. In the example above, `10` elements are allocated and can be access with an index selector of 1-10. For compatibility purposes, you can also use an index selector of `0`. Here is an example of a two dimensional array:

    10 DIM A(3,4)
    20 A(2,1) = 15
    30 PRINT A(2,1)
    
This will print `15` to the console. 

To clear the console, you can use the `CLS` statement. The `CLS` statement will clear the console and set the cursor position to `0`, `0`. To move the cursor position, you can use the `LOCATE` statement. The `LOCATE` statement will move the cursor to the given row and column. The console has 24 rows and 80 columns, so `Y` must be a number between 1 and 24 and `X` must be a number between 1 and 80. Here is an example:

    10 CLS
    20 ' Move the cursor to the 7th row down and 30 columns over.
    30 LOCATE 7, 30
    40 PRINT "HELLO WORLD!"
    
The cursor, by default, is a destructive cursor that will overwrite the character at the current position. Sometimes you may want to turn the cursor off so that it will not overwrite the character at the current position. You can do this by using the `CURSOR OFF` and `CURSOR ON` statements:

    10 CLS
    20 LOCATE 3,3
    30 PRINT "HELLO"
    40 LOCATE 3,3
    
Running the above program will print `HELLO` at cursor position `3, 3`. It will then reposition the cursor to `3, 3`, thus overwriting the `H` in `HELLO` (thus, printing `ELLO`). You can fix this by using the `CURSOR OFF` statement:

    10 CLS
    20 CURSOR OFF
    30 LOCATE 3,3
    40 PRINT "HELLO"
    50 LOCATE 3,3

Running the above program will print `HELLO` and still reposition the cursor to `3, 3`, but the `H` will not be overwritten. 

Basice also support the `FOR/NEXT/STEP` loop. Here is an example:

    10 CLS
    20 FOR X=1 TO 10
    30 PRINT X
    40 NEXT X
    
This will print the numbers `1-10` to the console. Anything between the `FOR` and the `NEXT` statements will be repeated for `X` number of iterations. By default, index iterators will increment by `1`. If you wanted to increment by a different amount, you can use the `STEP` keyword. Here is a program to print only even numbers to the console.

    10 CLS
    20 FOR X=2 to 10 STEP 2
    30 PRINT X
    40 NEXT X
    
This starts the count at `2` and for each iteration, increments by 2. You can also count backwards. To print even numbers between 1 and 10 in reverse order:

    10 CLS
    20 FOR X=10 TO 2 STEP -2
    30 PRINT X
    40 NEXT X
    
Basice also supports `IF/THEN/ELSE`. You can use the `IF` statement to test if a condition is true, then do something based on that condition being true. `ELSE` can be used to do something if the condition is false. All `IF` statements must be on one line since there is no statement to end an `IF` statement. Here is an example:

    10 CLS
    20 FOR X=1 TO 10
    30 IF X=3 THEN PRINT "--> ";
    40 PRINT X
    50 NEXT X
    
This program prints this output:

    1
    2
    --> 3
    4
    5
    6
    7
    8
    9
    10
    
Notice when `X=3` then it prints the arrow before the number. If you wanted to print left arrows for all numbers except `3` and a right arrow for `3`, you can use `ELSE`:

    10 CLS
    20 FOR X=1 TO 10
    30 IF X=3 THEN PRINT "--> "; ELSE PRINT "<-- ";
    40 PRINT X
    50 NEXT X
    
Another useful statement is the `END` statement. This command simply ends the program. Here is an example:

    10 CLS
    20 FOR X=1 TO 10
    30 IF X=8 THEN END
    40 PRINT X
    50 NEXT X
    
This will print the numbers `1-7`, but the program ends after the 8th iteration. 

## Author

[Alan Bryan](https://www.icemanind.com)

