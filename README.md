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
    
## Data Types
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

## Variables

Basice supports variables and there are two types of variables. String variables and number variables. String variables must end with a dollar sign `$` :

    10 PI=3.14
    20 PIE$ = "Pie!"
    30 PRINT "I love " + PIE$
    40 PRINT PI

This program will print `I love Pie!` to the console and then print the value `3.14` to the console. If you use a string variable without initializing it, it defaults to an empty string `""`. If you use a number variable without initializing it, it defaults to a zero `0`. 

## Arrays

Basice supports one and two dimensional arrays. You can create a one dimensional string array or number array like this:

    10 DIM A$(10)
    20 A$(2) = "Test"
    30 PRINT A$(2)
    
This will print `TEST` to the console. Before you can use arrays, you must use the `DIM` statement to allocate the number of elements your array is going to use. In the example above, `10` elements are allocated and can be access with an index selector of 1-10. For compatibility purposes, you can also use an index selector of `0`. Here is an example of a two dimensional array:

    10 DIM A(3,4)
    20 A(2,1) = 15
    30 PRINT A(2,1)
    
This will print `15` to the console. 

## Commands
#### CLS & LOCATE
To clear the console, you can use the `CLS` statement. The `CLS` statement will clear the console and set the cursor position to `0`, `0`. To move the cursor position, you can use the `LOCATE` statement. The `LOCATE` statement will move the cursor to the given row and column. The console has 24 rows and 80 columns, so `Y` must be a number between 1 and 24 and `X` must be a number between 1 and 80. Here is an example:

    10 CLS
    20 ' Move the cursor to the 7th row down and 30 columns over.
    30 LOCATE 7, 30
    40 PRINT "HELLO WORLD!"
    
#### COLOR
The `COLOR` statement allows you to change the foreground (text) color and/or the background color. You can use this in conjunction with the `RGB()` function to specify a new foreground or background color. Here is an example:

    10 CLS
    20 COLOR RGB(255, 0, 0)
    30 PRINT "THIS IS RED TEXT"
    
The program above will print `THIS IS RED TEXT` in red letters. The `RGB()` function takes three numbers, between `0` and `255`, for the red value, green value and blue value, respectively. The `RGB()` function then returns a number that can be passed into the `COLOR` statement. Here is an example of changing the background color:

    10 CLS
    20 COLOR ,RGB(80, 0, 80)
    30 PRINT "THIS SHOULD HAVE A PURPLE BACKGROUND"
    
The program above will print `THIS SHOULD HAVE A PURPLE BACKGROUND`, but the background of the letters will be purple. You can also change both the foreground and the background colors at the same time:

    10 CLS
    20 COLOR RGB(255,255,0), RGB(80,0,80)
    30 PRINT "THIS SHOULD BE YELLOW LETTERS ON A PURPLE BACKGROUND"
    
The program above will print `THIS SHOULD BE YELLOW LETTERS ON A PURPLE BACKGROUND` in yellow letters with a purple background.

#### READ / DATA / RESTORE
The `READ` command will read data from `DATA` commands. The data in the `DATA` command(s) can be numbers or strings. Here is an example:

    10 CLS
    20 DIM STATE$(13)
    30 FOR X = 1 TO 13
    40 READ STATE$(X)
    50 NEXT X
    60 PRINT "THE ORIGINAL 13 COLONIES ARE:"
    70 FOR X = 1 TO 13
    80 PRINT STATE$(X)
    90 NEXT X
    100 DATA "VIRGINIA", "MASSACHUSETTS", "RHODE ISLAND", "CONNECTICUT"
    110 DATA "NEW HAMPSHIRE", "NEW YORK", "NEW JERSEY", "PENNSYLVANIA"
    120 DATA "DELAWARE", "MARYLAND", "NORTH CAROLINA", "SOUTH CAROLINA"
    130 DATA "GEORGIA"
    
The program above will print out the original 13 colonies to the screen. First, an array is created named `STATE$`. Next, the data is `READ` in from the `DATA` statements beginning at line `100`. Finally, another loop begins and that loop prints out each colony. Data does not need to be read into array. You can read data into a single variable, either a string variable or a number variable.

The `RESTORE` statement will restore the `READ` pointer in case you need to read in the same data again. If you were to add the following line to the above program, the program would get an error, expecting to read more data:

    95 GOTO 30
To be able to re-read the data again, you must use `RESTORE`:
    
    95 RESTORE: GOTO 30

## Graphics
#### SCREEN
To be able to use graphics, you need to switch to a graphics screen instead of the default console text screen. To do this, you can use the `SCREEN` statement:

    10 SCREEN 2
    
The above program will switch to the graphics screen and you can then issue graphics statements. The `SCREEN` statement will accept either `1` or `2` as a parameter. `SCREEN 1` is the default console text screen. `SCREEN 2` is the graphics screen.

#### POINT
The `POINT` statement will draw a single point on the screen at the given coordinates, in the given color. If no color is specified, then it will draw the point using the foreground color of the `COLOR` statement. Here is an example:

    10 SCREEN 2
    20 POINT 35, 50, RGB(255, 0, 0)
    
The program above will draw a red dot, `35` pixels right and `50` pixels down. The following program does the same exact thing:

    10 SCREEN 2
    20 COLOR RGB(255, 0, 0)
    30 POINT 35, 50
#### LINE
The `LINE` statement will draw a line starting at the first coordinates specified and ending at the last coordinates specified. You can also add an optional color. Just like with the `POINT` statement, if you do not specify a color, it uses the color set from the `COLOR` statement. Here is an example:

    10 SCREEN 2
    20 LINE 35, 50, 128, 72, RGB(0, 255, 0)

The program above will draw a green line starting `35` pixels right and `50` pixels down and ending at `128` pixels right and `72` pixels down. The following program does the same exact thing:

    10 SCREEN 2
    20 COLOR RGB(0, 255, 0)
    30 LINE 35, 50, 128, 72

#### CURSOR OFF / ON
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

#### FOR / NEXT / STEP
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
    
#### IF / THEN / ELSE
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
    
#### END
Another useful statement is the `END` statement. This command simply ends the program. Here is an example:

    10 CLS
    20 FOR X=1 TO 10
    30 IF X=8 THEN END
    40 PRINT X
    50 NEXT X
    
This will print the numbers `1-7`, but the program ends after the 8th iteration. 

#### INPUT
Basice also supports the `INPUT` statement. This statement will prompt the user to enter something. The program then waits for a user to enter something before continuing execution. Whatever the user types is stored in a string variable. Here is an example:

    10 CLS
    20 PRINT "WHAT IS YOUR NAME? ";
    30 INPUT NAME$
    40 PRINT "WELL HELLO, " + NAME$
    
#### GOTO
Basice supports the `GOTO` statement. This statement causes program execution to jump to another line in the program. Here is an example:

    10 CLS
    20 PRINT "HELLO ";
    30 GOTO 50
    40 PRINT "THIS LINE WILL NEVER EXECUTE!"
    50 PRINT "WORLD!!!"

The program will print `HELLO WORLD!!!`. This is because line 30 jumps to line 50 and line 40 is never executed.

#### GOSUB / RETURN
Basice also has the `GOSUB` statement. This statement is like the `GOTO` statement, except, you can use `RETURN` to jump back to the `GOSUB` command and execution resumes where it left off. Here is an example:

    10 CLS
    20 PRINT "HELLO ";
    30 GOSUB 60
    40 PRINT "THIS LINE WILL EXECUTE AFTER GOSUB RETURNS!"
    50 END
    60 PRINT "WORLD!!!"
    70 RETURN
    
This program will print `HELLO `, then jump to line 60 and print `WORLD!!!`, then it will return and execution will start back at line 40 and print `THIS LINE WILL EXECUTE AFTER GOSUB RETURNS!`.



## Functions
#### INKEY$()
In addition to `INPUT`,  Basice also has an `INKEY$()` function. This function checks to see if a key has been pressed. If it has, it returns the pressed key. If no key was pressed, then it returns a value of zero. Here is an example:

    10 CLS
    20 PRINT "PRESS A KEY"
    30 KEY$=INKEY$()
    40 IF KEY$=CHR$(0) THEN GOTO 30
    50 PRINT "YOU PRESSED " + KEY$
    
The program above uses `INKEY$` function to check if a key was pressed. If there hasn't, then it just keeps checking. When a key finally gets pressed, it prints it to the screen. 

Here is a summary of all functions in Basice:

| Function   | Description                                                                                                                                                                                                                                          | Example                                                                              |
|------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------|
| `ABS()`    | Returns the absolute value of a number                                                                                                                                                                                                               | `PRINT ABS(-3)` **' Prints 3**                                                       |
| `ASC()`    | Returns the ASCII code of the first character in a string                                                                                                                                                                                            | `PRINT ASC("A")` **' Prints 65**                                                     |
| `CHR$()`   | Returns the character represented by the ASCII code                                                                                                                                                                                                  | `PRINT CHR$(65)` **' Prints A**                                                      |
| `COS()`    | Returns the Cosine of a number                                                                                                                                                                                                                       | `PRINT COS(2/5)` **' Prints 0.921060994002885**                                      |
| `DAY()`    | Returns the day of the current month (1-31)                                                                                                                                                                                                          | `PRINT DAY()` **' If today's date is July 31st, then it prints 31**                  |
| `HEX$()`   | Returns the hexadecimal equivalent of the number                                                                                                                                                                                                     | `PRINT HEX$(15)` **' Prints F**                                                      |
| `HOUR()`   | Returns the current hour of the current time                                                                                                                                                                                                         | `PRINT HOUR()` **' Prints 14 if the current time was 2:23pm**                        |
| `INKEY$()` | Returns the character pressed on the keyboard, or `\0` if no key was pressed                                                                                                                                                                         | `PRINT INKEY$()` **' Prints A if the A key was pressed**                             |
| `INSTR()`  | Returns the location of a string inside another string. The first parameter is the starting position, the second parameters is the string to be search and the third parameters is the string to search for. Returns `0` if the string is not found. | `PRINT INSTR(1, "HELLO WORLD!", "WORLD")` **' Prints 7**                             |
| `INT()`      | Returns the whole number part of a fraction.                                                                                                                                                                                                         | `PRINT INT(3.2518)` **' Prints 3**                                                   |
| `LEFT$()`  | Returns the left most part of a string. The second parameter determines how many characters to return.                                                                                                                                               | `PRINT LEFT$("HELLO",2)` **' Prints HE**                                             |
| `LEN()`    | Returns the length of a string                                                                                                                                                                                                                       | `PRINT LEN("HELLO")` **' Prints 5**                                                  |
| `LOG()`    | Returns the natural logarithm of a number                                                                                                                                                                                                            | `PRINT LOG(10)` **' Prints 2.30258509**                                              |
| `MID$()`   | Returns part of a string. The second parameter determines where to start in the string and the third parameter determines how many characters to return                                                                                              | `PRINT MID$("HELLO", 2, 3)` **' Prints ELL**                                         |
| `MINUTE()` | Returns the current minute of the current time                                                                                                                                                                                                       | `PRINT MINUTE()` **' Prints 23 if the current time was 2:23pm**                      |
| `MONTH()`  | Returns the month of the current date (1-12)                                                                                                                                                                                                         | `PRINT MONTH()` **' If today's date is July 31st, then it prints 7**                 |
| `RGB()`    | Converts a red, green and blue value to a number used with the `COLOR` statement.                                                                                                                                                                    | `COLOR RGB(255, 195, 195)` **' Changes the current foreground color to a light red** |
| `RIGHT$()` | Returns the right most part of a string. The second parameter determines how many characters to return.                                                                                                                                              | `PRINT RIGHT$("HELLO", 2)` **' Prints LO**                                           |
| `RND()`    | Returns a random number between 1 and the value passed into the function.                                                                                                                                                                            | `PRINT RND(50)` **' Prints a random number between 1 and 50**                        |
| `SECOND()` | Returns the current second of the current time                                                                                                                                                                                                       | `PRINT SECOND()` **' Prints 45 if the current time was 2:23:45pm**                   |
| `SIN()`    | Returns the sine of an angle using radians.                                                                                                                                                                                                          | `PRINT SIN(-0.1234)` **' Prints -0.123087058211376**                                 |
| `SQR()`    | Returns the square root of a number                                                                                                                                                                                                                  | `PRINT SQR(9)` **' Prints 3**                                                        |
| `STR$()`   | Converts a number to a string.                                                                                                                                                                                                                       | `PRINT STR$(15.2)` **' Prints 15.2**                                                 |
| `VAL()`    | Converts a string to a number. Will return `0` if it can't convert the string to a valid number.                                                                                                                                                     | `PRINT VAL("15.2")` **' Prints 15.2**                                                |
| `YEAR()`   | Returns the year of the current date.                                                                                                                                                                                                                | `PRINT YEAR()` **' Prints 2022 if the current year is 2022**                         |


## Author

[Alan Bryan](https://www.icemanind.com)

