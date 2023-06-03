using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using static CPP14Parser;
using Antlr4.Runtime.Tree;
using System.Linq;
using System;
using System.Collections.Generic;

namespace CPPFilesTests
{
    [TestClass]
    public class TreeWalkTests
    {
        private TranslationUnitContext GetTranslationUnitContext(string path)
        {
            var text = File.ReadAllText(path);

            var inputStream = new AntlrInputStream(text.ToString());
            var lexer = new CPP14Lexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new CPP14Parser(commonTokenStream);

            return parser.translationUnit();
        }

        [TestMethod]
        public void SimpleDeclarationIsFound()
        {
            var path = @"basicTest.cpp";

            var context = GetTranslationUnitContext(path);

            var simpleDeclarations = context.Descendents<SimpleDeclarationContext>();
            Assert.AreEqual(4, simpleDeclarations.Count, "simpleDeclarations");
        }

        [TestMethod]
        public void AutoAreFound()
        {
            var path = @"autoVarsTest.cpp";

            var context = GetTranslationUnitContext(path);

            var terminalNodes = context.Descendents<ITerminalNode>();
            var autoTerminalNodes = terminalNodes.Where(n => n.Symbol.Type == CPP14Lexer.Auto);
            Assert.AreEqual(1, autoTerminalNodes.Count(), "autoTerminalNodes.Count()");

            var autoTerminalNode = autoTerminalNodes.First();
            var simpleDeclarationContext= autoTerminalNode.Parent<SimpleDeclarationContext>();
            Assert.IsNotNull(simpleDeclarationContext, "simpleDeclarationContext1");
            
            var initList = simpleDeclarationContext.initDeclaratorList().initDeclarator().Select(d => d.declarator());
            var unqualifiedIdContexts = initList.SelectMany(l => l.Descendents<UnqualifiedIdContext>());
            Assert.AreEqual(4, unqualifiedIdContexts.Count(), "unqualifiedIdContexts1.Count()");

            var variablesToCheck = unqualifiedIdContexts.Select(c => c.Identifier().Symbol.Text).ToList();
            Assert.AreEqual("i1", variablesToCheck[0], "variablesToCheck[0]");
            Assert.AreEqual("i2", variablesToCheck[1], "variablesToCheck[1]");
            Assert.AreEqual("i3", variablesToCheck[2], "variablesToCheck[2]");
            Assert.AreEqual("i4", variablesToCheck[3], "variablesToCheck[3]");

            // let's identify the left/right statement locations of the variables
            var statementContext = simpleDeclarationContext.Parent<StatementContext>();
            var statementSeqContext = statementContext.Parent<StatementSeqContext>();

            var leftStatements = statementSeqContext.statement().SkipWhile(s => s != statementContext);
            var declaredVariablesInLeftStatements = leftStatements.Skip(1).SelectMany(s =>
            {
                var declarations = s.Descendents<InitDeclaratorContext>().Select(c => c.declarator());
                return declarations.SelectMany(d => d.Descendents<UnqualifiedIdContext>());
            });
            var declaredVariablesNamesInLeftStatements = declaredVariablesInLeftStatements.Select(c => c.Identifier().Symbol.Text);

            // if declaredVariablesNamesInLeftStatements is true - then the variable is definitely not const
            Assert.AreEqual(false, declaredVariablesNamesInLeftStatements.Contains("i1"), "declaredVariablesNamesInLeftStatements.Contains(i1)");
            Assert.AreEqual(true, declaredVariablesNamesInLeftStatements.Contains("i2"), "declaredVariablesNamesInLeftStatements.Contains(i2)");
            Assert.AreEqual(false, declaredVariablesNamesInLeftStatements.Contains("i3"), "declaredVariablesNamesInLeftStatements.Contains(i3)");
            Assert.AreEqual(false, declaredVariablesNamesInLeftStatements.Contains("i4"), "declaredVariablesNamesInLeftStatements.Contains(i4)");

            var declaredVariablesInRightStatements = leftStatements.SelectMany(s =>
            {
                var inits = s.Descendents<InitDeclaratorContext>().Select(c => c.initializer());
                return inits.SelectMany(d => d.Descendents<UnqualifiedIdContext>());
            });
            var declaredVariablesNamesInRightStatements = declaredVariablesInRightStatements.Select(c => c.Identifier().Symbol.Text);

            // if declaredVariablesNamesInRightStatements is false - it is unreferenced local variable
            Assert.AreEqual(true, declaredVariablesNamesInRightStatements.Contains("i1"), "declaredVariablesNamesInRightStatements.Contains(i1)");
            Assert.AreEqual(true, declaredVariablesNamesInRightStatements.Contains("i2"), "declaredVariablesNamesInRightStatements.Contains(i2)");
            Assert.AreEqual(false, declaredVariablesNamesInRightStatements.Contains("i3"), "declaredVariablesNamesInRightStatements.Contains(i3)");
            Assert.AreEqual(true, declaredVariablesNamesInRightStatements.Contains("i4"), "declaredVariablesNamesInRightStatements.Contains(i4)");
        }
    }
}
