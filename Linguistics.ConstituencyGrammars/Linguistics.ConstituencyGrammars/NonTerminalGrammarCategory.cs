using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linguistics.ConstituencyGrammars
{
    public class NonTerminalGrammarCategory : GrammarCategory
    {
        public List<GrammarCategory> Children { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var child in Children)
            {
                stringBuilder.Append(child.ToString() + " ");
            }

            return stringBuilder.ToString().Trim(); ;
        }
    }
}
