using System;
using System.Collections.Generic;
using System.Text;

namespace ColaCompiler
{
    public class ColaCompiler
    {

    }

    public class Parse
    {
        public Parse()
        {

        }
        public string ParsePrint(string code)
        {
            string tmp = null;
            if (code.Contains("print"))
            {
               tmp =  code.Replace("print", "Console.WriteLine");
            }


            return tmp;
        }
    }

}
