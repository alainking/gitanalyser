# Git Ananalyser
a .Net 6 utility to see the frequency of file changes on a git repo.


This tool will scan a local git repo and output a list of files changed, with the number of commits, lines added and deleted within the time range specified.  

## Requirements  

[.Net 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

This has been tested on Windows and MacOS but should work on all platforms supported by .Net 6  

To build the app run the following command from a terminal: `dotnet build`

To then run the app, you will need to the following command from the `\bin\Debug\net6.0` folder: `dotnet GitAnalyser.dll` and follow the prompts.

The output file will be in the repo folder you entered in the first step named `git-analysis.csv`

