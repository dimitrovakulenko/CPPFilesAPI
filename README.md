# cppFilesAPI

The CppFilesAPI library provides an API to the Abstract Syntax Tree (AST) of C++ files.

There is a also an examples library that showcases how to benefit the tool.

## Use cases

This library opens up a variety of possibilities for manipulating and understanding C++ code:

- **Custom Code Rules**: Enforce specific coding standards or guidelines within your project.
- **Refactoring Tools**: Build tools to automatically refactor code, such as reordering function declarations or adjusting indentation.
- **Code Metrics**:  Calculate metrics like cyclomatic complexity or number of function points.
- **Code Documentation**:  Generate documentation based on code comments and structure.
- **Code Analysis**:  Understand intricate code dependencies and relationships, or even analyze code for potential bottlenecks.

## Quick start

Here is a basic example of how to access the root of the AST of some valid CPP14 code:
(taken from https://github.com/dimitrovakulenko/CPPFilesAPI/blob/main/CPPFilesAPITests/TreeWalkTests.cs )

```csharp
var inputStream = new AntlrInputStream(text.ToString());
var lexer = new CPP14Lexer(inputStream);
var commonTokenStream = new CommonTokenStream(lexer);
var parser = new CPP14Parser(commonTokenStream);

return parser.translationUnit();
```

From this root, you can then access the various elements of the AST. 
For example, you can get all class specifier tree items like so:

```csharp
var classSpecifierContexts = context.Descendents<CPP14Parser.ClassSpecifierContext>();
```

## Development approach

I recommend to look at DeclarationsOrderings class as an example on how to use tree.
(https://github.com/dimitrovakulenko/CPPFilesAPI/blob/main/Examples/DeclarationsOrderings.cs)

This class allows to check order of declarations inside a class based on 
- their visibility access (public, private, protected) 
- type of declaration (see enum DeclarationType)

With some modification this class can be used to promote some specific coding guidelines within a company when agreed.


Generated API has no documentation, there are a lot of different tree items types.
This is a challenging task to identify what combination of classes do you need to check.
The best approach is try to simply debug.
.Net debugging techniques are powerful.

In a screenshot below I execute LINQ inside watch section and then inspect what does a tree contain.

There is a class, that contains "MemberSpecificationContext" (sounds like members declarations) with 17 children items.

Two of those children items are some member declarations starting with typedef and static.

![alt text](https://i.ibb.co/5Fxhzfy/inspect-Watch.png)

To develop DeclarationsOrderings I constantly looked at watch and tried to choose what subtree would give me that or another type of declaration.

## Implementation details

Following an article from a brilliant Gabriele Tomassetti https://tomassetti.me/getting-started-with-antlr-in-csharp/ I've generated a lexer, parser and AST elements classes using ANTLR4 tool from available on the web C++ G4 grammar file.
(https://github.com/antlr/grammars-v4/tree/master/cpp) (CPP14 standard only for now).

And that's pretty much it!

## Technology

.NET 6.0

