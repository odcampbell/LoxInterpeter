# Lox Interpeter
## Using C#

My Major Sources: 
https://craftinginterpreters.com/scanning.html


## START HERE
1. ### Set Up: 
    1. **GIT:** Clone the git repository into a directory of your choosing 
        1. open a terminal on your device
        2. navigate to desired directory and run following command
        3. git clone https://github.com/odcampbell/LoxInterpeter.git
            - if it does not allow you to clone then you follow **DOWNLOAD** 
        
    2. **DOWNLOAD:**
        1. You can download the code with the zip file "LoxInterpreter.zip"
        2. Unzip the code in a directory of your choosing, or unzip it and
            - copy/drag the files into the directory via file system or even some coding ide's

    3. Make sure you have C# installed on your computer ( mine is .NET SDK used with VSCode)
        - https://dotnet.microsoft.com/en-us/download/visual-studio-sdks

2. ### BUILD: (For Testing: see Step 4)
    1. move into the directory **"LoxInterpreter"** and then **"LoxApp"**
        
    2. Building from here is included in the run step next

3. ### RUN: (For Syntax and Stuff see Step 5)
    1. **To terminal:**
        1. **From File**
            - run cmd **dotnet run fileName.txt**
            - this will run the cmds in the file, output to screen/terminal and exit when done
            - fileName.txt is just a placeholder for whatever file is holding your code
            - **Try** with this cmd: **dotnet run arg1.txt** to use my demo file
            - Answers are located in **"answerKey.txt"**

        2. **Interactive:**
            - run cmd **dotnet run** in your terminal or open **Lox.cs** and press the play/run button in your IDE
            - you can type cmds in one line continuously e.g. print 2+2; to output 4
            - use cmd ctrl + c to exit the interactive terminal

    2. **To File:** 
        - Depending on your terminal e.g., windows terminal or PowerShell the results may be encoded in the output file
        - For Powershell use the dotnet cmd below, for terminal, you may have to write a few lines in Lox.cs main runFile to send data to a file
        - To send results to a file, just add the extraction operator **">>"** and a file name for your output
        - e.g., run cmd **dotnet run fileName.txt >> output.txt**

4. ### TEST:

    1. run cmd **dotnet run fileName.txt >> output.txt**
    2. In **test.py** , make sure this line **loxOut = "output.txt"** matches your output filename
    3. run cmd **py test.py**
    4. If a test passed, it is labeled PASS and if not, then FAIL
    5. If your output is encoded, try changing the utf-16 to utf-8 in test.py line 41

5. ### Syntax and Stuff
    1. **Not Implemented: (Things you can't do/use in my lox)**
        - Class inheritence, didn't finish chapter 13
        - Some closure code works and some doesn't but this would take too long to track down now
        - Also, my lox has some scoping issues that I haven't been able to resolve so some code doesn't work as it should
    2. Otherwise, my syntax follows that of Crafting Interpreters exactly, with the only difference being some bugs here or there

6. ### TEST RESULTS:
    1. I've included the expected test results in **testResults.txt**
    2. When you run the test outlined in step 4, this output is what should be displayed to the screen/terminal
