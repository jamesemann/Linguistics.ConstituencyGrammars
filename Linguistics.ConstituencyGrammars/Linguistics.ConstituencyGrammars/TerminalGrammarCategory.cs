using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linguistics.ConstituencyGrammars
{
    public class TerminalGrammarCategory : GrammarCategory
    {
        public string Word { get; set; }

        public override string ToString()
        {
            return $"{Word}";
        }
    }
}
