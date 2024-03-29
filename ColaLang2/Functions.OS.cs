﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace ColaLang
{

    class ArrayExt : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var amount = Utils.GetSafeInt(args, 0);

            List<double> arr = new List<double>();

            for (int i = 0; i < amount; i++)
            {
                arr.Add(i);
            }

            return new Variable(arr);
        }
    }
    #region printing
    // Prints passed list of argumentsand
    class PrintfFunction : ParserFunction
    {
        internal PrintfFunction(bool newLine = true)
        {
            m_newLine = newLine;
        }
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            AddOutput(args, script, m_newLine);

            return Variable.EmptyInstance;
        }
        public override async Task<Variable> EvaluateAsync(ParsingScript script)
        {
            List<Variable> args = await script.GetFunctionArgsAsync();
            AddOutput(args, script, m_newLine);

            return Variable.EmptyInstance;
        }

        public static void AddOutput(List<Variable> args, ParsingScript script = null,
                                     bool addLine = true, bool addSpace = true, string start = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(start);
            foreach (var arg in args)
            {
                sb.Append(arg.AsString() + (addSpace ? " " : ""));
            }

            string output = sb.ToString() + (addLine ? Environment.NewLine : string.Empty);
            output = output.Replace("\\t", "\t").Replace("\\n", "\n");
            Interpreter.Instance.AppendOutput(output);

            Debugger debugger = script != null && script.Debugger != null ?
                                script.Debugger : Debugger.MainInstance;
            if (debugger != null)
            {
                debugger.AddOutput(output, script);
            }
        }

        private bool m_newLine = true;
    }

    //IMPORTANT print function

    class PrintFunction : ParserFunction
    {
        internal PrintFunction(bool newLine = true)
        {
            m_newLine = newLine;
        }
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            AddOutput(args, script, m_newLine);

            return Variable.EmptyInstance;
        }
        public override async Task<Variable> EvaluateAsync(ParsingScript script)
        {
            List<Variable> args = await script.GetFunctionArgsAsync();
            AddOutput(args, script, m_newLine);

            return Variable.EmptyInstance;
        }

        public static void AddOutput(List<Variable> args, ParsingScript script = null,
                                     bool addLine = true, bool addSpace = true, string start = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(start);
            foreach (var arg in args)
            {
                sb.Append(arg.AsString() + (addSpace ? " " : ""));
            }

            string output = sb.ToString() + (addLine ? Environment.NewLine : string.Empty);
            output = output.Replace("\\t", "\t").Replace("\\n", "\n");
            Interpreter.Instance.AppendOutput(output);

            Debugger debugger = script != null && script.Debugger != null ?
                                script.Debugger : Debugger.MainInstance;
            if (debugger != null)
            {
                debugger.AddOutput(output, script);
            }
        }

        private bool m_newLine = true;
    }


    #region colorful printing

    enum FancyMode
    {
        Write,
        WriteL,
        Rgb,
        FromHtml,
        FromOle,
        AsciiArt,
        Gradient,
        GradientL,
    }

    class ColoredTest : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            //Utils.CheckArgs(args.Count, 2, m_name);

            var mode = Utils.GetSafeInt(args, 0);


            if (mode == (int)FancyMode.WriteL)
            {
                var message = Utils.GetSafeString(args, 1);
                var color = Utils.GetSafeString(args, 2);
                Colorful.Console.WriteLine(message, Color.FromName(color));
                //ColorTranslator.
                //PrintFancy.PrintFancyColors(mode, message, (FancyColors)color);
            }
            else if (mode == (int)FancyMode.Write)
            {
                var message = Utils.GetSafeString(args, 1);
                var color = Utils.GetSafeString(args, 2);
                Colorful.Console.Write(message, Color.FromName(color));
            }
            else if (mode == (int)FancyMode.FromHtml)
            {
                var message = Utils.GetSafeString(args, 1);
                var color = Utils.GetSafeString(args, 2);

                Colorful.Console.WriteLine(message, ColorTranslator.FromHtml(color));
            }
            else if (mode == (int)FancyMode.FromOle)
            {
                var message = Utils.GetSafeString(args, 1);
                var color = Utils.GetSafeInt(args, 2);

                Colorful.Console.WriteLine(message, ColorTranslator.FromOle(color));
            }
            else if (mode == (int)FancyMode.Rgb)
            {
                var message = Utils.GetSafeString(args, 1);
                var r = Utils.GetSafeInt(args, 2);
                var g = Utils.GetSafeInt(args, 3);
                var b = Utils.GetSafeInt(args, 4);
                Colorful.Console.WriteLine(message, Color.FromArgb(r, g, b));
            }
            else if (mode == (int)FancyMode.AsciiArt)
            {
                var message = Utils.GetSafeString(args, 1);
                var wFont = Utils.GetSafeBool(args, 2);
                
                //PrintFancy.PrintFancyColors(mode, message, (FancyColors)color);
                if (wFont == true)
                {
                    var fontPath = Utils.GetSafeString(args, 3);
                    var color = Utils.GetSafeString(args, 4);
                    Colorful.FigletFont font = Colorful.FigletFont.Load(fontPath);
                    Colorful.Figlet figlet = new Colorful.Figlet(font);
                    
                    Colorful.Console.WriteLine(figlet.ToAscii(message), Color.FromName(color));
                    
                }
                else
                {
                    var color = Utils.GetSafeString(args, 3);
                    Colorful.Console.WriteAscii(message, Colorful.FigletFont.Default, Color.FromName(color));
                }
            }
            else if (mode == (int)FancyMode.GradientL)
            {
                var message = Utils.GetSafeString(args, 1);
                var startColor = Utils.GetSafeString(args, 2);
                var endColor = Utils.GetSafeString(args, 3);
                var count = Utils.GetSafeInt(args, 4);
                Colorful.Console.WriteLineWithGradient(message, Color.FromName(startColor), Color.FromName(endColor), count);
            }
            else if (mode == (int)FancyMode.Gradient)
            {
                var message = Utils.GetSafeString(args, 1);
                var startColor = Utils.GetSafeString(args, 2);
                var endColor = Utils.GetSafeString(args, 3);
                var count = Utils.GetSafeInt(args, 4);
                Colorful.Console.WriteWithGradient(message.ToCharArray(), Color.FromName(startColor), Color.FromName(endColor), count);
                //Colorful.Console.ReplaceAllColorsWithDefaults();
            }

           

           // if(colortype == (int)FancyColors.MediumPurple)
           // {
           //     Colorful.Console.WriteLine(colored, System.Drawing.Color.MediumPurple);
           // }
            
            // Colorful.Console.WriteLine("stuff", System.Drawing.Color.FromArgb(255, 255, 250));
            // Colorful.Console.WriteAscii("HASSELOFF", System.Drawing.Color.FromArgb(244, 212, 255));


            return Variable.EmptyInstance;
        }
    }

    /*
    class PrintFancy
    {
        public static void PrintFancyColors(int mode, string Message, FancyColors Colors)
        {
            if (mode == (int)FancyMode.FancyColors)
            {
                if (Colors == FancyColors.MediumPurple)
                {
                    Colorful.Console.WriteLine(Message, Color.MediumPurple);
                }
                else if (Colors == FancyColors.MediumSeaGreen)
                {
                    Colorful.Console.WriteLine(Message, Color.MediumSeaGreen);
                }
                else if (Colors == FancyColors.MediumSlateBlue)
                {
                    Colorful.Console.WriteLine(Message, Color.MediumSlateBlue);
                }
                else if (Colors == FancyColors.MediumSpringGreen)
                {
                    Colorful.Console.WriteLine(Message, Color.MediumSpringGreen);
                }
                else if (Colors == FancyColors.MediumTurquoise)
                {
                    Colorful.Console.WriteLine(Message, Color.MediumTurquoise);
                }
                else if (Colors == FancyColors.MediumVioletRed)
                {
                    Colorful.Console.WriteLine(Message, Color.MediumVioletRed);
                }
                else if (Colors == FancyColors.MidnightBlue)
                {
                    Colorful.Console.WriteLine(Message, Color.MidnightBlue);
                }
                else if (Colors == FancyColors.MediumOrchid)
                {
                    Colorful.Console.WriteLine(Message, Color.MediumOrchid);
                }
                else if (Colors == FancyColors.MintCream)
                {
                    Colorful.Console.WriteLine(Message, Color.MintCream);
                }
                else if (Colors == FancyColors.Moccasin)
                {
                    Colorful.Console.WriteLine(Message, Color.Moccasin);
                }
                else if (Colors == FancyColors.NavajoWhite)
                {
                    Colorful.Console.WriteLine(Message, Color.NavajoWhite);
                }
                else if (Colors == FancyColors.Navy)
                {
                    Colorful.Console.WriteLine(Message, Color.Navy);
                }
                else if (Colors == FancyColors.OldLace)
                {
                    Colorful.Console.WriteLine(Message, Color.OldLace);
                }
                else if (Colors == FancyColors.Olive)
                {
                    Colorful.Console.WriteLine(Message, Color.Olive);
                }
                else if (Colors == FancyColors.OliveDrab)
                {
                    Colorful.Console.WriteLine(Message, Color.OliveDrab);
                }
                else if (Colors == FancyColors.Orange)
                {
                    Colorful.Console.WriteLine(Message, Color.Orange);
                }
                else if (Colors == FancyColors.MistyRose)
                {
                    Colorful.Console.WriteLine(Message, Color.MistyRose);
                }
                else if (Colors == FancyColors.OrangeRed)
                {
                    Colorful.Console.WriteLine(Message, Color.OrangeRed);
                }
                else if (Colors == FancyColors.MediumBlue)
                {
                    Colorful.Console.WriteLine(Message, Color.MediumBlue);
                }
                else if (Colors == FancyColors.Maroon)
                {
                    Colorful.Console.WriteLine(Message, Color.Maroon);
                }
                else if (Colors == FancyColors.LightBlue)
                {
                    Colorful.Console.WriteLine(Message, Color.LightBlue);
                }
                else if (Colors == FancyColors.LightCoral)
                {
                    Colorful.Console.WriteLine(Message, Color.LightCoral);
                }
                else if (Colors == FancyColors.LightGoldenrodYellow)
                {
                    Colorful.Console.WriteLine(Message, Color.LightGoldenrodYellow);
                }
                else if (Colors == FancyColors.LightGreen)
                {
                    Colorful.Console.WriteLine(Message, Color.LightGreen);
                }
                else if (Colors == FancyColors.LightGray)
                {
                    Colorful.Console.WriteLine(Message, Color.LightGray);
                }
                else if (Colors == FancyColors.LightPink)
                {
                    Colorful.Console.WriteLine(Message, Color.LightPink);
                }
                else if (Colors == FancyColors.LightSalmon)
                {
                    Colorful.Console.WriteLine(Message, Color.LightSalmon);
                }
                else if (Colors == FancyColors.MediumAquamarine)
                {
                    Colorful.Console.WriteLine(Message, Color.MediumAquamarine);
                }
                else if (Colors == FancyColors.LightSeaGreen)
                {
                    Colorful.Console.WriteLine(Message, Color.LightSeaGreen);
                }
                else if (Colors == FancyColors.LightSlateGray)
                {
                    Colorful.Console.WriteLine(Message, Color.LightSlateGray);
                }
                else if (Colors == FancyColors.LightSteelBlue)
                {
                    Colorful.Console.WriteLine(Message, Color.LightSteelBlue);
                }
                else if (Colors == FancyColors.LightYellow)
                {
                    Colorful.Console.WriteLine(Message, Color.LightYellow);
                }
                else if (Colors == FancyColors.Lime)
                {
                    Colorful.Console.WriteLine(Message, Color.Lime);
                }
                else if (Colors == FancyColors.LimeGreen)
                {
                    Colorful.Console.WriteLine(Message, Color.LimeGreen);
                }
                else if (Colors == FancyColors.Linen)
                {
                    Colorful.Console.WriteLine(Message, Color.Linen);
                }
                else if (Colors == FancyColors.Magenta)
                {
                    Colorful.Console.WriteLine(Message, Color.Magenta);
                }

                /*
            LightSkyBlue,
            LemonChiffon,
            Orchid,
            PaleGreen,
            SlateBlue,
            SlateGray,
            Snow,
            SpringGreen,
            SteelBlue,
            Tan,
            Teal,
            SkyBlue,
            Thistle,
            Turquoise,
            Violet,
            Wheat,
            White,
            WhiteSmoke,
            Yellow,
            YellowGreen,
            Tomato,
            PaleGoldenrod,
            Silver,
            SeaShell,
            PaleTurquoise,
            PaleViolateRed,
            PapayaWhip,
            PeachPuff,
            Peru,
            Pink,
            Plum,
            Sienna,
            PowderBlue,
            Red,
            RosyBrown,
            RoyalBlue,
            SaddleBrown,
            Salmon,
            SandyBrown,
            SeaGreen,
            Purpe,
            LawnGreen,
            LightCyan,
            Lavender,
            DarkKhanki,
            DarkGreen,
            DarkGray,
            DarkGoldenrod,
            DarkCyan,
            DarkBlue,
            Cyan,
            Crimson,
            Cornsilk,
            LavenderBlush,
            Coral,
            Chocolate,
            Chartreuse,
            DarkMagenta,
            CadetBlue,
            Brown,
            BlueViolet,
            Blue,
            BlanchedAlmond,
            Black,
            Bisque,
            Beige,
            Azure,
            Aquamarine,
            Aqua,
            AntiqueWhite,
            AliceBlue,
            Transparent,
            BurlyWood,
            DarkOliveGreen,
            CornFlowerBlue,
            DarkOrchid,
            Khaki,
            Ivory,
            DarkOrange,
            Indigo,
            IndianRed,
            HotPink,
            Honeydew,
            GreenYellow,
            Green,
            Gray,
            Goldenrod,
            GhostWHite,
            Gainsboro,
            Fuchsia,
            Gold,
            FloralWhite,
            DarkRed,
            DarkSalmon,
            DarkSeaGreen,
            ForestGreen,
            DarkSlateGray,
            DarkTurquoise,
            DarkSlateBlue,
            DeepPink,
            DeepSkyBlue,
            DimGray,
            DodgerBlue,
            Firebrick,
            DarkViolet
                
            }

            if (mode == (int)FancyMode.AsciiArt)
            {

            }
        }

    }
    */
    #endregion
    #endregion
    class DataFunction : ParserFunction
    {
        internal enum DataMode { ADD, SUBSCRIBE, SEND };

        DataMode m_mode;

        static string s_method;
        static string s_tracking;
        static bool s_updateImmediate = false;

        static StringBuilder s_data = new StringBuilder();

        internal DataFunction(DataMode mode = DataMode.ADD)
        {
            m_mode = mode;
        }
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            string result = "";

            switch (m_mode)
            {
                case DataMode.ADD:
                    Collect(args);
                    break;
                case DataMode.SUBSCRIBE:
                    Subscribe(args);
                    break;
                case DataMode.SEND:
                    result = SendData(s_data.ToString());
                    s_data.Clear();
                    break;
            }

            return new Variable(result);
        }

        public void Subscribe(List<Variable> args)
        {
            s_data.Clear();

            s_method = Utils.GetSafeString(args, 0);
            s_tracking = Utils.GetSafeString(args, 1);
            s_updateImmediate = Utils.GetSafeDouble(args, 2) > 0;
        }

        public void Collect(List<Variable> args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var arg in args)
            {
                sb.Append(arg.AsString());
            }
            if (s_updateImmediate)
            {
                SendData(sb.ToString());
            }
            else
            {
                s_data.AppendLine(sb.ToString());
            }
        }

        public string SendData(string data)
        {
            if (!string.IsNullOrWhiteSpace(s_method))
            {
                CustomFunction.Run(s_method, new Variable(s_tracking),
                                   new Variable(data));
                return "";
            }
            return data;
        }
    }

    class CurrentPathFunction : ParserFunction, INumericFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            return new Variable(script.PWD);
        }
    }

    // Returns how much processor time has been spent on the current process
    class ProcessorTimeFunction : ParserFunction, INumericFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            Process pr = Process.GetCurrentProcess();
            TimeSpan ts = pr.TotalProcessorTime;

            return new Variable(Math.Round(ts.TotalMilliseconds, 0));
        }
    }

    class TokenizeFunction : ParserFunction, IArrayFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();

            Utils.CheckArgs(args.Count, 1, m_name);
            string data = Utils.GetSafeString(args, 0);

            string sep = Utils.GetSafeString(args, 1, "\t");
            var option = Utils.GetSafeString(args, 2);

            return Tokenize(data, sep, option);
        }

        static public Variable Tokenize(string data, string sep, string option = "")
        {
            if (sep == "\\t")
            {
                sep = "\t";
            }

            string[] tokens;
            var sepArray = sep.ToCharArray();
            if (sepArray.Count() == 1)
            {
                tokens = data.Split(sepArray);
            }
            else
            {
                List<string> tokens_ = new List<string>();
                var rx = new System.Text.RegularExpressions.Regex(sep);
                tokens = rx.Split(data);
                for (int i = 0; i < tokens.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(tokens[i]) || sep.Contains(tokens[i]))
                    {
                        continue;
                    }
                    tokens_.Add(tokens[i]);
                }
                tokens = tokens_.ToArray();
            }

            List<Variable> results = new List<Variable>();
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                if (i > 0 && string.IsNullOrWhiteSpace(token) &&
                    option.StartsWith("prev", StringComparison.OrdinalIgnoreCase))
                {
                    token = tokens[i - 1];
                }
                results.Add(new Variable(token));
            }

            return new Variable(results);
        }
    }

    class StringManipulationFunction : ParserFunction
    {
        public enum Mode
        {
            CONTAINS, STARTS_WITH, ENDS_WITH, INDEX_OF, EQUALS, REPLACE,
            UPPER, LOWER, TRIM, SUBSTRING, BEETWEEN, BEETWEEN_ANY
        };
        Mode m_mode;

        public StringManipulationFunction(Mode mode)
        {
            m_mode = mode;
        }

        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();

            Utils.CheckArgs(args.Count, 1, m_name);
            string source = Utils.GetSafeString(args, 0);
            string argument = Utils.GetSafeString(args, 1);
            string parameter = Utils.GetSafeString(args, 2, "case");
            int startFrom = Utils.GetSafeInt(args, 3, 0);
            int length = Utils.GetSafeInt(args, 4, source.Length);

            StringComparison comp = StringComparison.Ordinal;
            if (parameter.Equals("nocase") || parameter.Equals("no_case"))
            {
                comp = StringComparison.OrdinalIgnoreCase;
            }

            source = source.Replace("\\\"", "\"");
            argument = argument.Replace("\\\"", "\"");

            switch (m_mode)
            {
                case Mode.CONTAINS:
                    return new Variable(source.IndexOf(argument, comp) >= 0);
                case Mode.STARTS_WITH:
                    return new Variable(source.StartsWith(argument, comp));
                case Mode.ENDS_WITH:
                    return new Variable(source.EndsWith(argument, comp));
                case Mode.INDEX_OF:
                    return new Variable(source.IndexOf(argument, startFrom, comp));
                case Mode.EQUALS:
                    return new Variable(source.Equals(argument, comp));
                case Mode.REPLACE:
                    return new Variable(source.Replace(argument, parameter));
                case Mode.UPPER:
                    return new Variable(source.ToUpper());
                case Mode.LOWER:
                    return new Variable(source.ToLower());
                case Mode.TRIM:
                    return new Variable(source.Trim());
                case Mode.SUBSTRING:
                    startFrom = Utils.GetSafeInt(args, 1, 0);
                    length = Utils.GetSafeInt(args, 2, source.Length);
                    length = Math.Min(length, source.Length - startFrom);
                    return new Variable(source.Substring(startFrom, length));
                case Mode.BEETWEEN:
                case Mode.BEETWEEN_ANY:
                    int index1 = source.IndexOf(argument, comp);
                    int index2 = m_mode == Mode.BEETWEEN ? source.IndexOf(parameter, index1 + 1, comp) :
                                          source.IndexOfAny(parameter.ToCharArray(), index1 + 1);
                    startFrom = index1 + argument.Length;

                    if (index1 < 0 || index2 < index1)
                    {
                        throw new ArgumentException("Couldn't extract string between [" + argument +
                                                    "] and [" + parameter + "] + from " + source);
                    }
                    string result = source.Substring(startFrom, index2 - startFrom);
                    return new Variable(result);
            }

            return new Variable(-1);
        }
    }

    // Append a string to another string
    class AppendFunction : ParserFunction, IStringFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(script, Constants.NEXT_ARG_ARRAY);
            Utils.CheckNotEmpty(script, varName, m_name);

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetVariable(varName, script);
            Variable currentValue = func.GetValue(script);

            // 3. Get the value to be added or appended.
            Variable newValue = Utils.GetItem(script);

            // 4. Take either the string part if it is defined,
            // or the numerical part converted to a string otherwise.
            string arg1 = currentValue.AsString();
            string arg2 = newValue.AsString();

            // 5. The variable becomes a string after adding a string to it.
            newValue.Reset();
            newValue.String = arg1 + arg2;

            ParserFunction.AddGlobalOrLocalVariable(varName, new GetVarFunction(newValue), script);

            return newValue;
        }
    }

    class SignalWaitFunction : ParserFunction, INumericFunction
    {
        static AutoResetEvent waitEvent = new AutoResetEvent(false);
        bool m_isSignal;

        public SignalWaitFunction(bool isSignal)
        {
            m_isSignal = isSignal;
        }
        public override Variable Evaluate(ParsingScript script)
        {
            bool result = m_isSignal ? waitEvent.Set() :
                                       waitEvent.WaitOne();
            return new Variable(result);
        }
    }

    class ThreadFunction : ParserFunction, INumericFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string body = script.TryPrev() == Constants.START_GROUP ?
                          Utils.GetBodyBetween(script, Constants.START_GROUP, Constants.END_GROUP) :
                          Utils.GetBodyBetween(script, Constants.START_ARG, Constants.END_ARG);
            ThreadPool.QueueUserWorkItem(ThreadProc, body);
            return Variable.EmptyInstance;
        }

        static void ThreadProc(Object stateInfo)
        {
            string body = (string)stateInfo;
            ParsingScript threadScript = new ParsingScript(body);
            threadScript.ExecuteAll();
        }
    }
    class ThreadIDFunction : ParserFunction, IStringFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            int threadID = Thread.CurrentThread.ManagedThreadId;
            return new Variable(threadID.ToString());
        }
    }
    class SleepFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            Variable sleepms = Utils.GetItem(script);
            Utils.CheckPosInt(sleepms, script);

            Thread.Sleep((int)sleepms.Value);

            return Variable.EmptyInstance;
        }
    }

    class BeepFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var frequency = Utils.GetSafeInt(args, 0);
            var duration = Utils.GetSafeInt(args, 1);

            Console.Beep((int)frequency, (int)duration);

            return Variable.EmptyInstance;

        }
    }
    //IMPORTANT Real plugs
    #region ConsoleFunctions

    class LoopFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var text = Utils.GetSafeString(args, 0);
            var times = Utils.GetSafeInt(args, 1);

            for (int i = 0; i < times; i++)
            {
                Console.WriteLine(text);
            }

            return Variable.EmptyInstance;
        }
    }

    class ColaKeyCodeFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var keyCode = Utils.GetSafeString(args, 1);

            if (keyCode == "a")
            {

            }


            return Variable.EmptyInstance;
        }
    }
    class ColaConsoleWidthFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var width = Utils.GetSafeInt(args, 0);

            //Console.WindowHeight = height;
            Console.WindowWidth = width;


            return Variable.EmptyInstance;
        }
    }
    class ColaConsoleFGFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var color = Utils.GetSafeString(args, 0);

            switch (color)
            {
                case "White":
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "Black":
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case "DarkBlue":
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case "DarkGreen":
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case "DarkCyan":
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case "DarkRed":
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case "DarkMagenta":
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case "DarkYellow":
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case "Gray":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case "DarkGray":
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case "Blue":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case "Green":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "Cyan":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case "Red":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "Yellow":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "Magenta":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case "Reset":
                    Console.ResetColor();
                    break;
                case "Colors":
                    Console.WriteLine("White");
                    Console.WriteLine("Black");
                    Console.WriteLine("DarkBlue");
                    Console.WriteLine("DarkGreen");
                    Console.WriteLine("DarkCyan");
                    Console.WriteLine("DarkRed");
                    Console.WriteLine("DarkMagenta");
                    Console.WriteLine("DarkYellow");
                    Console.WriteLine("Grey");
                    Console.WriteLine("DarkGrey");
                    Console.WriteLine("Blue");
                    Console.WriteLine("Green");
                    Console.WriteLine("Magenta");
                    Console.WriteLine("Yellow");
                    Console.WriteLine("Cyan");
                    Console.WriteLine("Red");
                    Console.WriteLine("Reset");
                    break;
                default:
                    Console.WriteLine("Console color not found!");
                    break;
            }

            //Console.ForegroundColor = ConsoleColor.

            return Variable.EmptyInstance;
        }
    }
    class ColaConsoleBGFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var color = Utils.GetSafeString(args, 0);

            switch (color)
            {
                case "Black":
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case "White":
                    Console.BackgroundColor = ConsoleColor.White;
                    break;
                case "DarkBlue":
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    break;
                case "DarkGreen":
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    break;
                case "DarkCyan":
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    break;
                case "DarkRed":
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                case "DarkMagenta":
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    break;
                case "DarkYellow":
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    break;
                case "Gray":
                    Console.BackgroundColor = ConsoleColor.Gray;
                    break;
                case "DarkGray":
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    break;
                case "Blue":
                    Console.BackgroundColor = ConsoleColor.Blue;
                    break;
                case "Green":
                    Console.BackgroundColor = ConsoleColor.Green;
                    break;
                case "Cyan":
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    break;
                case "Red":
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case "Yellow":
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    break;
                case "Magenta":
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    break;
                case "Reset":
                    Console.ResetColor();
                    break;
                case "Colors":
                    Console.WriteLine("Black");
                    Console.WriteLine("White");
                    Console.WriteLine("DarkBlue");
                    Console.WriteLine("DarkGreen");
                    Console.WriteLine("DarkCyan");
                    Console.WriteLine("DarkRed");
                    Console.WriteLine("DarkMagenta");
                    Console.WriteLine("DarkYellow");
                    Console.WriteLine("Grey");
                    Console.WriteLine("DarkGrey");
                    Console.WriteLine("Blue");
                    Console.WriteLine("Green");
                    Console.WriteLine("Magenta");
                    Console.WriteLine("Yellow");
                    Console.WriteLine("Cyan");
                    Console.WriteLine("Red");
                    Console.WriteLine("Reset");
                    break;
                default:
                    Console.WriteLine("Console color not found!");
                    break;
            }

            //Console.ForegroundColor = ConsoleColor.

            return Variable.EmptyInstance;
        }
    }
    class ColaConsoleHeightFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var height = Utils.GetSafeInt(args, 0);

            Console.WindowHeight = height;
            //Console.WindowWidth = width;


            return Variable.EmptyInstance;
        }
    }

    class ColaCursorVisible : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var visible = Utils.GetSafeBool(args, 0);

            Console.CursorVisible = visible;

            return Variable.EmptyInstance;
        }
    }

    class ColaConsoleTitleFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var title = Utils.GetSafeString(args, 0);

            Console.Title = title;

            return Variable.EmptyInstance;
        }
    }

    class ColaSetPositionFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var x = Utils.GetSafeInt(args, 0);
            var y = Utils.GetSafeInt(args, 1);

            Console.SetCursorPosition(x, y);
            return Variable.EmptyInstance;
        }
    }

    class ColaGetKeyFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            //List<Variable> args = script.GetFunctionArgs();
            //Utils.CheckArgs(args.Count, 1, m_name);

            // declare a new ConsoleKeyInfo object 
            ConsoleKeyInfo c = new ConsoleKeyInfo();
            c = Console.ReadKey(true);
            //if (c.Key == ConsoleKey.A)
            //{
            //    Console.WriteLine("A was pressed!");
            //  }
            //ConsoleKey.

            return new Variable(c.Key);
        }
    }

    class ColaRandomNext : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var ran = Utils.GetSafeInt(args, 0);
            Random r = new Random();
            var rnd = r.Next(ran);
            return new Variable(rnd);
        }
    }

    class SignalLooper : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);
            //RedLoop("0", 1);
            var times = Utils.GetSafeInt(args, 0);
            var text = Utils.GetSafeString(args, 1);

            for (int i = 0; i < times; i++)
            {
                Console.WriteLine(text);
            }

            return Variable.EmptyInstance;
        }
    }

    class ColaReadKeyFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            //List<Variable> args = script.GetFunctionArgs();
            //Utils.CheckArgs(args.Count, 1, m_name);


            //var input = Console.ReadKey();
            //Console.
            //Console.ReadKey();
            return new Variable(Console.ReadKey().Key.ToString());
        }
    }


    class ColaClearFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            //List<Variable> args = script.GetFunctionArgs();
            //Utils.CheckArgs(args.Count, 1, m_name);

            //var path = Utils.GetSafeString(args, 0);

            //System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
            //player.Play();
            Console.Clear();

            return Variable.EmptyInstance;
        }
    }
    #endregion
    class LockFunction : ParserFunction
    {
        static Object lockObject = new Object();

        public override Variable Evaluate(ParsingScript script)
        {
            string body = Utils.GetBodyBetween(script, Constants.START_ARG,
                                                       Constants.END_ARG);
            ParsingScript threadScript = new ParsingScript(body);

            // BUGBUG: Alfred - what is this actually locking?
            // Vassili - it's a global (static) lock. used when called from different threads
            lock (lockObject)
            {
                threadScript.ExecuteAll();
            }
            return Variable.EmptyInstance;
        }
    }

    class DateTimeFunction : ParserFunction, IStringFunction
    {
        bool m_stringVersion;

        public DateTimeFunction(bool stringVersion = true)
        {
            m_stringVersion = stringVersion;
        }

        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            string strFormat = m_stringVersion ? Utils.GetSafeString(args, 0, "HH:mm:ss.fff") :
                                          Utils.GetSafeString(args, 1, "yyyy/MM/dd HH:mm:ss");
            Utils.CheckNotEmpty(strFormat, m_name);


            if (m_stringVersion)
            {
                return new Variable(DateTime.Now.ToString(strFormat));
            }

            var date = DateTime.Now;
            string when = Utils.GetSafeString(args, 0);

            if (!string.IsNullOrWhiteSpace(when) && !DateTime.TryParseExact(when, strFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out date))
            {
                throw new ArgumentException("Couldn't parse [" + when + "] using format [" +
                                            strFormat + "].");
            }

            return new Variable(date);
        }

        public static DateTime Add(DateTime current, string delta)
        {
            int sign = 1;
            string part = "";
            int partInt;
            for (int i = 0; i < delta.Length; i++)
            {
                switch (delta[i])
                {
                    case '-':
                        sign *= -1;
                        continue;
                    case 'y':
                        partInt = string.IsNullOrWhiteSpace(part) ? 1 : !int.TryParse(part, out partInt) ? 0 : partInt;
                        current = current.AddYears(partInt * sign);
                        break;
                    case 'M':
                        partInt = string.IsNullOrWhiteSpace(part) ? 1 : !int.TryParse(part, out partInt) ? 0 : partInt;
                        current = current.AddMonths(partInt * sign);
                        break;
                    case 'd':
                        partInt = string.IsNullOrWhiteSpace(part) ? 1 : !int.TryParse(part, out partInt) ? 0 : partInt;
                        current = current.AddDays(partInt * sign);
                        break;
                    case 'H':
                    case 'h':
                        partInt = string.IsNullOrWhiteSpace(part) ? 1 : !int.TryParse(part, out partInt) ? 0 : partInt;
                        current = current.AddHours(partInt * sign);
                        break;
                    case 'm':
                        partInt = string.IsNullOrWhiteSpace(part) ? 1 : !int.TryParse(part, out partInt) ? 0 : partInt;
                        current = current.AddMinutes(partInt * sign);
                        break;
                    case 's':
                        partInt = string.IsNullOrWhiteSpace(part) ? 1 : !int.TryParse(part, out partInt) ? 0 : partInt;
                        current = current.AddSeconds(partInt * sign);
                        break;
                    case 'f':
                        partInt = string.IsNullOrWhiteSpace(part) ? 1 : !int.TryParse(part, out partInt) ? 0 : partInt;
                        current = current.AddTicks(partInt * sign);
                        break;
                    default:
                        part += delta[i];
                        continue;
                }
                part = "";
            }
            return current;
        }
    }

    class DebuggerFunction : ParserFunction
    {
        bool m_start = true;
        public DebuggerFunction(bool start = true)
        {
            m_start = start;
        }
        public override Variable Evaluate(ParsingScript script)
        {
            string res = "OK";
            List<Variable> args = script.GetFunctionArgs();
            if (m_start)
            {
                int port = Utils.GetSafeInt(args, 0, 13337);
                bool allowRemote = Utils.GetSafeInt(args, 1, 0) == 1;
                DebuggerServer.AllowedClients = Utils.GetSafeString(args, 2);

                res = DebuggerServer.StartServer(port, allowRemote);
            }
            else
            {
                DebuggerServer.StopServer();
            }

            return new Variable(res);
        }
    }
    // Returns an environment variable
    class GetEnvFunction : ParserFunction, IStringFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string varName = Utils.GetToken(script, Constants.END_ARG_ARRAY);
            string res = Environment.GetEnvironmentVariable(varName);

            return new Variable(res);
        }
    }

    // Sets an environment variable
    class SetEnvFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string varName = Utils.GetToken(script, Constants.NEXT_ARG_ARRAY);
            Utils.CheckNotEmpty(script, varName, m_name);

            Variable varValue = Utils.GetItem(script);
            string strValue = varValue.AsString();
            Environment.SetEnvironmentVariable(varName, strValue);

            return new Variable(varName);
        }
    }

    class GetFileFromDebugger : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();

            Utils.CheckArgs(args.Count, 2, m_name);
            string filename = Utils.GetSafeString(args, 0);
            string destination = Utils.GetSafeString(args, 1);

            Variable result = new Variable(Variable.VarType.ARRAY);
            result.Tuple.Add(new Variable(Constants.GET_FILE_FROM_DEBUGGER));
            result.Tuple.Add(new Variable(filename));
            result.Tuple.Add(new Variable(destination));

            result.ParsingToken = m_name;

            return result;
        }
    }

    class RegexFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();

            Utils.CheckArgs(args.Count, 2, m_name);
            string pattern = Utils.GetSafeString(args, 0);
            string text = Utils.GetSafeString(args, 1);

            Variable result = new Variable(Variable.VarType.ARRAY);

            Regex rx = new Regex(pattern,
                        RegexOptions.Compiled | RegexOptions.IgnoreCase);

            MatchCollection matches = rx.Matches(text);

            foreach (Match match in matches)
            {
                result.AddVariableToHash("matches", new Variable(match.Value));

                var groups = match.Groups;
                foreach (var group in groups)
                {
                    result.AddVariableToHash("groups", new Variable(group.ToString()));
                }
            }

            return result;
        }
    }

    class EditCompiledEntry : ParserFunction
    {
        internal enum EditMode { ADD_DEFINITION, ADD_NAMESPACE, CLEAR_DEFINITIONS, CLEAR_NAMESPACES };
        EditMode m_mode;

        internal EditCompiledEntry(EditMode mode)
        {
            m_mode = mode;
        }
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            string item = Utils.GetSafeString(args, 0);

#if __ANDROID__ == false && __IOS__ == false

            switch (m_mode)
            {
                case EditMode.ADD_DEFINITION:
                    Precompiler.AddDefinition(item);
                    break;
                case EditMode.ADD_NAMESPACE:
                    Precompiler.AddNamespace(item);
                    break;
                case EditMode.CLEAR_DEFINITIONS:
                    Precompiler.ClearDefinitions();
                    break;
                case EditMode.CLEAR_NAMESPACES:
                    Precompiler.ClearNamespaces();
                    break;
            }
#endif

            return Variable.EmptyInstance;
        }
    }

    class CompiledFunctionCreator : ParserFunction
    {
        bool m_scriptInCSharp = false;

        public CompiledFunctionCreator(bool scriptInCSharp)
        {
#if UNITY_EDITOR == false && UNITY_STANDALONE == false && _ANDROID__ == false && __IOS__ == false
            m_scriptInCSharp = scriptInCSharp;
#endif
        }

        public override Variable Evaluate(ParsingScript script)
        {
            string funcReturn, funcName;
            Utils.GetCompiledArgs(script, out funcReturn, out funcName);

#if __ANDROID__ == false && __IOS__ == false
            Precompiler.RegisterReturnType(funcName, funcReturn);

            Dictionary<string, Variable> argsMap;
            string[] args = Utils.GetCompiledFunctionSignature(script, out argsMap);

            script.MoveForwardIf(Constants.START_GROUP, Constants.SPACE);
            int parentOffset = script.Pointer;

            string body = Utils.GetBodyBetween(script, Constants.START_GROUP, Constants.END_GROUP);

            Precompiler precompiler = new Precompiler(funcName, args, argsMap, body, script);
            precompiler.Compile(m_scriptInCSharp);

            CustomCompiledFunction customFunc = new CustomCompiledFunction(funcName, body, args, precompiler, argsMap, script);
            customFunc.ParentScript = script;
            customFunc.ParentOffset = parentOffset;

            ParserFunction.RegisterFunction(funcName, customFunc, false /* not native */);
#endif
            return new Variable(funcName);
        }
    }

#if __ANDROID__ == false && __IOS__ == false
    class CustomCompiledFunction : CustomFunction
    {
        internal CustomCompiledFunction(string funcName,
                                        string body, string[] args,
                                        Precompiler precompiler,
                                        Dictionary<string, Variable> argsMap,
                                        ParsingScript script)
          : base(funcName, body, args, script)
        {
            m_precompiler = precompiler;
            m_argsMap = argsMap;
        }

        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            script.MoveBackIf(Constants.START_GROUP);

            if (args.Count != m_args.Length)
            {
                throw new ArgumentException("Function [" + m_name + "] arguments mismatch: " +
                                    m_args.Length + " declared, " + args.Count + " supplied");
            }

            Variable result = Run(args);
            return result;
        }

        public override Task<Variable> EvaluateAsync(ParsingScript script)
        {
            return Task.FromResult(Evaluate(script));
        }

        public Variable Run(List<Variable> args)
        {
            RegisterArguments(args);

            List<string> argsStr = new List<string>();
            List<double> argsNum = new List<double>();
            List<List<string>> argsArrStr = new List<List<string>>();
            List<List<double>> argsArrNum = new List<List<double>>();
            List<Dictionary<string, string>> argsMapStr = new List<Dictionary<string, string>>();
            List<Dictionary<string, double>> argsMapNum = new List<Dictionary<string, double>>();
            List<Variable> argsVar = new List<Variable>();

            for (int i = 0; i < m_args.Length; i++)
            {
                Variable typeVar = m_argsMap[m_args[i]];
                if (typeVar.Type == Variable.VarType.STRING)
                {
                    argsStr.Add(args[i].AsString());
                }
                else if (typeVar.Type == Variable.VarType.NUMBER)
                {
                    argsNum.Add(args[i].AsDouble());
                }
                else if (typeVar.Type == Variable.VarType.ARRAY_STR)
                {
                    List<string> subArrayStr = new List<string>();
                    var tuple = args[i].Tuple;
                    for (int j = 0; j < tuple.Count; j++)
                    {
                        subArrayStr.Add(tuple[j].AsString());
                    }
                    argsArrStr.Add(subArrayStr);
                }
                else if (typeVar.Type == Variable.VarType.ARRAY_NUM)
                {
                    List<double> subArrayNum = new List<double>();
                    var tuple = args[i].Tuple;
                    for (int j = 0; j < tuple.Count; j++)
                    {
                        subArrayNum.Add(tuple[j].AsDouble());
                    }
                    argsArrNum.Add(subArrayNum);
                }
                else if (typeVar.Type == Variable.VarType.MAP_STR)
                {
                    Dictionary<string, string> subMapStr = new Dictionary<string, string>();
                    var tuple = args[i].Tuple;
                    var keys = args[i].GetKeys();
                    for (int j = 0; j < tuple.Count; j++)
                    {
                        subMapStr.Add(keys[j], tuple[j].AsString());
                    }
                    argsMapStr.Add(subMapStr);
                }
                else if (typeVar.Type == Variable.VarType.MAP_NUM)
                {
                    Dictionary<string, double> subMapNum = new Dictionary<string, double>();
                    var tuple = args[i].Tuple;
                    var keys = args[i].GetKeys();
                    for (int j = 0; j < tuple.Count; j++)
                    {
                        subMapNum.Add(keys[j], tuple[j].AsDouble());
                    }
                    argsMapNum.Add(subMapNum);
                }
                else if (typeVar.Type == Variable.VarType.VARIABLE)
                {
                    argsVar.Add(args[i]);
                }
            }

            Variable result = Precompiler.AsyncMode ?
                m_precompiler.RunAsync(argsStr, argsNum, argsArrStr, argsArrNum, argsMapStr, argsMapNum, argsVar, false) :
                m_precompiler.Run(argsStr, argsNum, argsArrStr, argsArrNum, argsMapStr, argsMapNum, argsVar, false);

            ParserFunction.PopLocalVariables(m_stackLevel.Id);

            return result;
        }

        Precompiler m_precompiler;
        Dictionary<string, Variable> m_argsMap;

        public Precompiler Precompiler { get { return m_precompiler; } }
    }
#endif
}
