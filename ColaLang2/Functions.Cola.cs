using SplitAndMerge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SplitAndMerge
{
    class DIE : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            MessageBox.Show("Cola did the ded", "Cola Ded");
            Environment.Exit(0);

            return Variable.EmptyInstance;
        }
    }
}
