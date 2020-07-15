using ColaLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColaLang
{
    class DIE : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            MessageBox.Show("Cola did the ded", "Cola Ded");
            Environment.Exit(0);

            return Variable.EmptyInstance;
        }
    }
}
