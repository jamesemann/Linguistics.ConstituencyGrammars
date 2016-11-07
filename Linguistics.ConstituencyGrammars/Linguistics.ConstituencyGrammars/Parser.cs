using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Linguistics.ConstituencyGrammars
{
    public class Parser
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static GrammarCategory ParseConstituencyBasedExpression(string constituencyBasedExpression)
        {
            // the stack is used internally to maintain the current node in the grammar parse tree
            Stack<GrammarCategory> grammarStack = new Stack<GrammarCategory>();
            GrammarCategory rootGrammarCategory = new NonTerminalGrammarCategory() { Tag = "", Children = new List<GrammarCategory>() };
            grammarStack.Push(rootGrammarCategory);

            for (int i = 0; i < constituencyBasedExpression.Length; i++)
            {
                var charAt = constituencyBasedExpression[i];
                if (charAt == '(')
                {
                    var tagLeftIndex = i + 1;
                    var tagRightIndex = constituencyBasedExpression.IndexOf(' ', i);
                    var tag = constituencyBasedExpression.Substring(tagLeftIndex, tagRightIndex - tagLeftIndex);

                    // node can be non-terminal (interior) or terminal (leaf) - terminal have a word association
                    // a terminal node is:
                    string[] terminalTags = {
                                        "$"        ,
                    "\"\""       ,
                    "''"       ,
                    "("        ,
                    ")"        ,
                    ","        ,
                    "--"       ,
                    "."        ,
                    ":"        ,
                    "CC"       ,
                    "CD"       ,
                    "DT"       ,
                    "EX"       ,
                    "FW"       ,
                    "IN"       ,
                    "JJ"       ,
                    "JJR"      ,
                    "JJS"      ,
                    "LS"       ,
                    "MD"       ,
                    "NN"       ,
                    "NNP"      ,
                    "NNPS"     ,
                    "NNS"      ,
                    "PDT"      ,
                    "POS"      ,
                    "PRP"      ,
                    "PRP$"     ,
                    "RB"       ,
                    "RBR"      ,
                    "RBS"      ,
                    "RP"       ,
                    "SYM"      ,
                    "TO"       ,
                    "UH"       ,
                    "VB"       ,
                    "VBD"      ,
                    "VBG"      ,
                    "VBN"      ,
                    "VBP"      ,
                    "VBZ"      ,
                    "WDT"      ,
                    "WP"       ,
                    "WP$"      ,
                    "WRB"};
                    
                    if (terminalTags.Contains(tag))  // terminal
                    {
                        var wordLeftIndex = tagRightIndex + 1;
                        var wordRightIndex = constituencyBasedExpression.IndexOf(')', tagRightIndex);
                        var word = constituencyBasedExpression.Substring(wordLeftIndex, wordRightIndex - wordLeftIndex);
                        //Console.WriteLine(stack.Peek() + ":" + word);

                        var grammar = new TerminalGrammarCategory();

                        var parentGrammar = grammarStack.Peek();
                        if (parentGrammar != null && parentGrammar is NonTerminalGrammarCategory)
                        {
                            ((NonTerminalGrammarCategory)parentGrammar).Children.Add(grammar);
                        }

                        grammar.Tag = tag;
                        grammar.Word = word;
                        grammarStack.Push(grammar);
                        logger.Info("pushed terminal " + grammar.Tag + " " + grammar.Word);
                    }
                    else
                    {
                        var grammar = new NonTerminalGrammarCategory();

                        if (grammarStack.Count > 0)
                        {
                            var parentGrammar = grammarStack.Peek();
                            if (parentGrammar != null && parentGrammar is NonTerminalGrammarCategory)
                            {
                                ((NonTerminalGrammarCategory)parentGrammar).Children.Add(grammar);
                            }
                        }

                        grammar.Tag = tag;
                        grammar.Children = new List<GrammarCategory>();
                        grammarStack.Push(grammar);
                        logger.Info("pushed non-terminal " + grammar.Tag);

                    }

                }
                else if (charAt == ')')
                {
                    var instructionCompleted = grammarStack.Pop();
                    logger.Info("popped " + instructionCompleted.Tag);
                }
            }
            return rootGrammarCategory;
        }
    }
}
