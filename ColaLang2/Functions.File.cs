using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SplitAndMerge
{
    class FileCreate : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var path = Utils.GetSafeString(args, 0);

            File.Create(path);

            return Variable.EmptyInstance;
        }
    }
    class FileWrite : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            string path = Utils.GetSafeString(args, 0);
            string text = Utils.GetSafeString(args, 1);

            text = text.Replace("\\n", "\n");

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(text);
                sw.Close();
            }

            return Variable.EmptyInstance;
        }
    }
    class FileReadToEnd : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var path = Utils.GetSafeString(args, 0);

            string readout = "";

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.UTF8)) // Using Unicode by default :P
            {
                readout = sr.ReadToEnd();
            }

            return new Variable(readout);
        }
    }
    class FileDelete : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var path = Utils.GetSafeString(args, 0);

            File.Delete(path);

            return Variable.EmptyInstance;
        }
    }
    class FileCopy : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var path0 = Utils.GetSafeString(args, 0);
            var path1 = Utils.GetSafeString(args, 1);

            File.Copy(path0, path1);

            return Variable.EmptyInstance;
        }
    }
    class FileExists : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var path = Utils.GetSafeString(args, 0);

            return new Variable(File.Exists(path));
        }
    }
    class FileWriteAllBytes : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var path = Utils.GetSafeString(args, 0);
            var bytes = Utils.GetSafeString(args, 1);

            bytes = bytes.Replace("[", "").Replace("]", "").Replace(" ", "");

            string[] bstrings = bytes.Split(',');
            byte[] barray = new byte[bstrings.Length];

            for (int i = 0; i < bstrings.Length; i++)
            {
                barray[i] = byte.Parse(bstrings[i]);
            }

            File.WriteAllBytes(path, barray);

            return Variable.EmptyInstance;
        }
    }
    class FileReadAllBytes : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var path = Utils.GetSafeString(args, 0);

            byte[] barray;
            List<Variable> bytes = new List<Variable>();

            barray = File.ReadAllBytes(path);

            foreach (byte b in barray)
            {
                bytes.Add(new Variable(b));
            }

            return new Variable(bytes);
        }
    }

    // Directories

    class DirCreate : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var path = Utils.GetSafeString(args, 0);

            Directory.CreateDirectory(path);

            return Variable.EmptyInstance;
        }
    }
    class DirDelete : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var path = Utils.GetSafeString(args, 0);

            Directory.Delete(path);

            return Variable.EmptyInstance;
        }
    }
    class DirExists : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var path = Utils.GetSafeString(args, 0);

            return new Variable(Directory.Exists(path));
        }
    }
    class DirGetDirs : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var path = Utils.GetSafeString(args, 0);

            string[] dirs = Directory.GetDirectories(path);
            List<Variable> vars = new List<Variable>();

            foreach (string s in dirs)
            {
                vars.Add(new Variable(s));
            }

            return new Variable(vars);
        }
    }
    class DirGetFiles : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var path = Utils.GetSafeString(args, 0);

            string[] files = Directory.GetFiles(path);
            List<Variable> vars = new List<Variable>();

            foreach (string s in files)
            {
                vars.Add(new Variable(s));
            }

            return new Variable(vars);
        }
    }
    class DirGetParent : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var path = Utils.GetSafeString(args, 0);

            return new Variable(Directory.GetParent(path).FullName);
        }
    }
}