using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Examples;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CPP14Parser;

namespace CPPFilesTests
{
    [TestClass]
    public class DeclarationsOrderingsTest
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
        public void BasicTest()
        {
            var path = @"basicOrderingsTest.h";

            var context = GetTranslationUnitContext(path);

            var classSpecifierContexts = context.Descendents<CPP14Parser.ClassSpecifierContext>();
            Assert.AreEqual(2, classSpecifierContexts.Count, "classSpecifierContexts");

            foreach(var classSpecifierContext in classSpecifierContexts)
            {
                var newOrder = DeclarationsOrderings.FixOrder(classSpecifierContext);

                Assert.AreEqual(null,
                    DeclarationsOrderings.FindFirstIncorrectLocation(newOrder),
                    "order is not fixed");
            }
        }

    }
}
