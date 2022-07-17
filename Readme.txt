Setting Up
==========

Original project from 2019 doesn't want to run. So started again!

Setup of project followed https://tomassetti.me/getting-started-with-antlr-in-csharp/

# this creates a new Solution
dotnet new sln

# these commands create a console and MS Test projects
dotnet new console -o fsm-generator
dotnet new console -o fsm-generatorTests

# these commands add the newly created projects to the solution
dotnet sln add ./fsm-generator/fsm-generator.csproj 
dotnet sln add ./fsm-generatorTests/fsm-generatorTests.csproj 

# installing the ANTLR4 runtime package in the main console project
dotnet add fsm-generator package Antlr4.Runtime.Standard

# adding a reference to the main project in the test one
dotnet add fsm-generatorTests reference fsm-generator 

Settings
========

Edit generator options in workspace settings.  Set to:

"antlr4.generation": {
        "mode": "external",
        "language": "CSharp",
        "listeners": false,
        "visitors": true,
        "outputDir": "Generated"
    }


Grammar
=======

Create Parser folder in main project and put .g4 files there.

Antlr generates files under the Parser folder:

.antlr    - java files for use by intellisense and diagrams.
Generated - C# files for compilation time

