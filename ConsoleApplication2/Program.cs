using Linguistics.ConstituencyGrammars;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            //{(TOP (S (NP (PRP I)) (VP (VBP am) (VP (VBG going) (S (VP (TO to) (VP (VB repeal) (CC and) (VB replace) (NNP ObamaCare)))))) (. .)))}
            //var parseTree = "(TOP (S (NP (PRP I)) (VP (VBP am) (VP (VBG going) (S (VP (TO to) (VP (VB repeal) (CC and) (VB replace) (NNP ObamaCare)))))) (. .)))";


            
            ///linguistics/v1.0/analyze
            HttpClient client = new HttpClient() { BaseAddress = new Uri("https://api.projectoxford.ai") };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "4aaf02184ddd4922906c3a5cab63e16b");
            //var text = System.Web.HttpUtility.JavaScriptStringEncode(
            //var text = "Read between the line graphs. If you're only using analytics, you're not getting the full story.";
            var text = "RT @PrivilegeHealth: #Chatbots allow for information to be transferred via hyperlinks and rich multimedia rather than through a linear voice flow";
            var result = client.PostAsync("linguistics/v1.0/analyze", new StringContent("{ \"language\" : \"en\",\"analyzerIds\" : [\"22a6b758-420f-4745-8a3c-46835a67c0d2\"],\"text\" : \"" + text + "\" }",Encoding.UTF8, "application/json")).Result;

            JArray obj = JArray.Parse(result.Content.ReadAsStringAsync().Result);
            var parseTree = obj[0].SelectToken("result").ToString();
            // TODO - james this is actually a json object in this instance, with two root nodes (i.e. TOPS)
            var rootGrammarCategory = Parser.ParseConstituencyBasedExpression(parseTree);


            // should be able to search for all verb phrases
            var verbPhrases = new List<string>();
            var nouns = new List<string>();
            var adjp = new List<string>();

            SearchForVerbPhrases(rootGrammarCategory, verbPhrases);
            SearchForNouns(rootGrammarCategory, nouns);
            SearchForAdjpPhrases(rootGrammarCategory, adjp);

            var sorted = verbPhrases.OrderBy(n => n.Length);
            var shortest = sorted.FirstOrDefault();
            //var longest = sorted.LastOrDefault();
            Console.Write(shortest);
            Console.ReadLine();
        }

        static void SearchForVerbPhrases(GrammarCategory grammarCategory, List<string> verbPhrases)
        {
            var terminalGrammarCategory = grammarCategory as TerminalGrammarCategory;
            var nonTerminalGrammarCategory = grammarCategory as NonTerminalGrammarCategory;

            if (nonTerminalGrammarCategory != null)
            {
                foreach (var child in ((NonTerminalGrammarCategory)grammarCategory).Children)
                {
                    SearchForVerbPhrases(child, verbPhrases);
                }
            }

            if (grammarCategory.Tag == "VP")
            {
                verbPhrases.Add(grammarCategory.ToString());
            }
        }

        static void SearchForAdjpPhrases(GrammarCategory grammarCategory, List<string> verbPhrases)
        {
            var terminalGrammarCategory = grammarCategory as TerminalGrammarCategory;
            var nonTerminalGrammarCategory = grammarCategory as NonTerminalGrammarCategory;

            if (nonTerminalGrammarCategory != null)
            {
                foreach (var child in ((NonTerminalGrammarCategory)grammarCategory).Children)
                {
                    SearchForAdjpPhrases(child, verbPhrases);
                }
            }

            if (grammarCategory.Tag == "ADJP")
            {
                verbPhrases.Add(grammarCategory.ToString());
            }
        }

        static void SearchForNouns(GrammarCategory grammarCategory, List<string> nouns)
        {
            var terminalGrammarCategory = grammarCategory as TerminalGrammarCategory;
            var nonTerminalGrammarCategory = grammarCategory as NonTerminalGrammarCategory;

            if (nonTerminalGrammarCategory != null)
            {
                foreach (var child in ((NonTerminalGrammarCategory)grammarCategory).Children)
                {
                    SearchForNouns(child, nouns);
                }
            }

            if (terminalGrammarCategory != null && grammarCategory.Tag.StartsWith ("N"))
            {
                nouns.Add(grammarCategory.ToString());
            }
        }


        
    }

    

    

    
}
