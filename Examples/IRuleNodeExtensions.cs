using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CPP14Parser;

namespace Examples
{
    public static class IRuleNodeExtensions
    {
        public static List<T> Descendents<T>(this IParseTree ruleNode)
        {
            var result = new List<T>();
            if(ruleNode != null)
                for(int i = 0; i < ruleNode.ChildCount; i++)
                {
                    var childRule = ruleNode.GetChild(i);
                    if (childRule is T childT)
                        result.Add(childT);
                    result.AddRange(childRule.Descendents<T>());
                }
            return result;
        }

        public static List<T> DescendentsWithout<T, T2>(this IParseTree ruleNode)
        {
            var result = new List<T>();
            if (ruleNode != null)
                for (int i = 0; i < ruleNode.ChildCount; i++)
                {
                    var childRule = ruleNode.GetChild(i);
                    if (childRule is T2)
                        continue;
                    if (childRule is T childT)
                        result.Add(childT);
                    result.AddRange(
                        childRule.DescendentsWithout<T, T2>());
                }
            return result;
        }

        public static T Parent<T>(this IParseTree ruleNode)
        {
            if (ruleNode?.Parent is null)
                return default(T);
            if (ruleNode.Parent is T parentT)
                return parentT;
            return Parent<T>(ruleNode.Parent);
        }
    }
}
