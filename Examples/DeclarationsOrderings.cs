using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CPP14Parser;

namespace Examples
{
    public enum AccessModifier
    {
        Public,
        Protected,
        Private
    }

    public enum DeclarationType
    {
        Typedef,
        Enum,
        Constant,
        Constructor,
        CreationMethod, // static create(...) method
        Destructor,
        StaticMethod,
        NonStaticMethod,
        DataMember
    }

    public class DeclarationsOrderings
    {
        public static MemberdeclarationContext FindFirstIncorrectLocation(
            List<(MemberdeclarationContext, AccessModifier, DeclarationType)> allDeclarations)
        {
            var theDecl = allDeclarations.Last();
            foreach (var decl in allDeclarations)
            {
                if (decl == theDecl)
                    break; // no preceding item is at wrong location

                if (decl.Item2 > theDecl.Item2)
                    return decl.Item1;

                if (decl.Item2 == theDecl.Item2)
                    if (decl.Item3 > theDecl.Item3)
                        return decl.Item1;
            }

            return null;
        }

        public static List<(MemberdeclarationContext, AccessModifier, DeclarationType)> FixOrder(
            CPP14Parser.ClassSpecifierContext classSpecifierContext)
        {
            var allDeclarations = GetAllDeclarations(classSpecifierContext);

            // Order by access modifier and declaration type
            var orderedDeclarations = allDeclarations
                .OrderBy(d => d.Item2) // Item2 is AccessModifier
                .ThenBy(d => d.Item3)  // Item3 is DeclarationType
                .ToList();

            return orderedDeclarations;
        }

        public static List<(MemberdeclarationContext, AccessModifier, DeclarationType)> 
            GetAllDeclarations(CPP14Parser.ClassSpecifierContext classSpecifierContext)
        {
            var result = new List<(MemberdeclarationContext, AccessModifier, DeclarationType)>();

            var accessModifier = (classSpecifierContext.classHead().classKey() == null) 
                ? AccessModifier.Public 
                : AccessModifier.Private;

            var memberSpecification = classSpecifierContext.memberSpecification();
            if (memberSpecification == null) 
                return result;

            foreach (var memberSpecChild in memberSpecification.children)
            {
                if (memberSpecChild is AccessSpecifierContext accessSpecifierContext)
                {
                    var isPublic = accessSpecifierContext.Public() != null;
                    var isPrivate = accessSpecifierContext.Private() != null;

                    if (isPrivate)
                        accessModifier = AccessModifier.Private;
                    else if (isPublic)
                        accessModifier = AccessModifier.Public;
                    else
                        accessModifier = AccessModifier.Protected;
                }
                else if (memberSpecChild is MemberdeclarationContext memberDeclContext)
                {
                    var isEnum = memberDeclContext.DescendentsWithout
                        <EnumSpecifierContext, FunctionDefinitionContext>().Count;
                    System.Diagnostics.Debug.Assert(isEnum < 2);
                    if (isEnum == 1)
                    {
                        result.Add((memberDeclContext, accessModifier, DeclarationType.Enum));
                        continue;
                    }

                    var isTypedef = memberDeclContext.DescendentsWithout
                        <TypedefNameContext, FunctionDefinitionContext>().Count;
                    System.Diagnostics.Debug.Assert(isTypedef < 2);
                    if (isTypedef == 1)
                    {
                        result.Add((memberDeclContext, accessModifier, DeclarationType.Typedef));
                        continue;
                    }

                    var parametersAndQualifiers = memberDeclContext.DescendentsWithout
                        <ParametersAndQualifiersContext, FunctionBodyContext>();
                    var hasParametersAndQualifiers = parametersAndQualifiers.Count;
                    System.Diagnostics.Debug.Assert(hasParametersAndQualifiers < 2);

                    var isFunction = hasParametersAndQualifiers == 1;

                    if (!isFunction)
                    {
                        var isConst = memberDeclContext.declSpecifierSeq().Start.Text == "const";
                        if (isConst)
                            result.Add((memberDeclContext, accessModifier, DeclarationType.Constant));
                        else
                            result.Add((memberDeclContext, accessModifier, DeclarationType.DataMember));
                        continue;
                    }
                    else
                    {
                        var firstParametersAndQualifiers = parametersAndQualifiers.First();
                        var parent = firstParametersAndQualifiers.Parent as ParserRuleContext;
                        System.Diagnostics.Debug.Assert(parent != null);

                        var preceding = parent.children.TakeWhile(p => p != firstParametersAndQualifiers);
                        var oneBeforeParametersAndQualifier = preceding.Last();

                        var isConstructor = oneBeforeParametersAndQualifier.GetText() ==
                            classSpecifierContext.classHead().classHeadName().GetText();
                        if (isConstructor)
                        {
                            result.Add((memberDeclContext, accessModifier, DeclarationType.Constructor));
                            continue;
                        }

                        var isCreateMethod = oneBeforeParametersAndQualifier.GetText() == "create";
                        if (isCreateMethod)
                        {
                            result.Add((memberDeclContext, accessModifier, DeclarationType.CreationMethod));
                            continue;
                        }


                        var isDestructor = oneBeforeParametersAndQualifier.GetText() ==
                            "~" + classSpecifierContext.classHead().classHeadName().GetText();
                        if (isDestructor)
                        {
                            result.Add((memberDeclContext, accessModifier, DeclarationType.Destructor));
                            continue;
                        }


                        var storageSpecifiers = memberDeclContext.DescendentsWithout
                            <StorageClassSpecifierContext, FunctionBodyContext>();
                        if (storageSpecifiers.Any(s => (s as StorageClassSpecifierContext).Static() != null))
                        {
                            result.Add((memberDeclContext, accessModifier, DeclarationType.StaticMethod));
                            continue;
                        }

                        result.Add((memberDeclContext, accessModifier, DeclarationType.NonStaticMethod));
                        continue;
                    }
                }
            }

            return result;
        }
    }
}
