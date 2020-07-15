using Roslyn.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Threading.Tasks;
using Roslyn.Scripting.CSharp;
using Roslyn.Compilers.CSharp;

namespace ColaLang
{
    class CompileCSharp : ParserFunction
    {

        private static ScriptEngine rosylnEngine = new ScriptEngine();
        private static Session session = rosylnEngine.CreateSession();
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var code = Utils.GetSafeString(args, 0);
            session.ImportNamespace("System");
            session.ImportNamespace("System.Math");
            
            //Console.WriteLine(session.Execute(code));
           // session.Execute(code);
           //a = Cola.Eval();
            return new Variable(session.Execute(code));
            
        }

    }
}
