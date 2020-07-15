using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.FreeGlut;
using OpenGL;
using OpenGL.Platform;
namespace SplitAndMerge
{
    class COLA_UI
    {
        public static void Init()
        {
            ParserFunction.RegisterFunction("MsgBox", new MsgBox());
        }

    }

    class MsgBox : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var title = Utils.GetSafeString(args, 0);
            var msg = Utils.GetSafeString(args, 1);

            MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            return Variable.EmptyInstance;
        }
    }
}
