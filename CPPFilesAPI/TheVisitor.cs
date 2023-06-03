using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Linq;

public class TheVisitor : CPP14ParserBaseVisitor<object>
{
	//public File File { get; set; }

	//private Dictionary<Antlr4.Runtime.Tree.IRuleNode, AbstractSyntaxNode> readTokens;
	//private Stack<AbstractSyntaxNode> currentNodePath;

	public override object Visit(Antlr4.Runtime.Tree.IParseTree tree)
    {
		//readTokens = new Dictionary<Antlr4.Runtime.Tree.IRuleNode, AbstractSyntaxNode>();
		//currentNodePath = new Stack<AbstractSyntaxNode>();

		return base.Visit(tree);
    }

	public CPP14Parser.TranslationUnitContext TranslationUnitContext;

	public override object VisitTranslationUnit([NotNull] CPP14Parser.TranslationUnitContext context) 
	{
		TranslationUnitContext = context;
		//File = new File();
		//readTokens.Add(context, File);

		return VisitChildren(context); 
	}

	public override object VisitSimpleDeclaration([NotNull] CPP14Parser.SimpleDeclarationContext context) 
	{
		
		return VisitChildren(context); 
	}

	public override object VisitChildren(Antlr4.Runtime.Tree.IRuleNode node)
    {
  //      if (!readTokens.ContainsKey(node))
  //      {
		//	readTokens.Add(node, new UnimplementedSyntaxNode());
  //      }

		//var syntaxNode = readTokens[node];
		//if(currentNodePath.Any())
  //      {
		//	currentNodePath.Last().SetupChildRelations(syntaxNode);
		//}

		//currentNodePath.Push(syntaxNode);

		var toReturn = base.VisitChildren(node);

		//currentNodePath.Pop();

		return toReturn;
	}

	public override object VisitNamespaceDefinition([NotNull] CPP14Parser.NamespaceDefinitionContext context) 
	{
		//readTokens.Add(context, new Namespace());

		var toReturn = base.VisitChildren(context);
		return toReturn;
	}

}