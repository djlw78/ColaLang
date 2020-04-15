using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitAndMerge
{
    public partial class Constants
    {
        public const string APPEND = "append";
        public const string APPENDLINE = "appendline";
        public const string APPENDLINES = "appendlines";
        public const string CALL_NATIVE = "Sys.CallExtern";
        public const string CD = "IO.cd";
        public const string CD__ = "IO.cd..";
        public const string COPY = "IO.copy";
        public const string CONNECTSRV = "iliilililililiiiiiiillllll";
        public const string CONSOLE_CLR = "clr";
        public const string DIR = "IO.dir";
        public const string DELETE = "IO.delete";
        public const string EXISTS = "IO.exists";
        public const string FINDFILES = "IO.findfiles";
        public const string FINDSTR = "IO.findstr";
        public const string GET_NATIVE = "Sys.GetExtern";
        public const string KILL = "Task.Kill";
        public const string MKDIR = "IO.mkdir";
        public const string MORE = "IO.more";
        public const string MOVE = "IO.move";
        public const string PRINT_BLACK = "printf.black";
        public const string PRINT_GRAY = "printf.gray";
        public const string PRINT_GREEN = "printf.green";
        public const string PRINT_RED = "printf.red";
        public const string PSINFO = "psinfo";
        public const string PWD = "pwd";
        public const string READ = "readLine";
        public const string READFILE = "readfile";
        public const string READNUMBER = "read";
        public const string RUN = "Task.Run";
        public const string SET_NATIVE = "Sys.SetExtern";
        public const string STARTSRV = "iiiiilllllliiiillllt";
        public const string STOPWATCH_ELAPSED = "StopWatchElapsed";
        public const string STOPWATCH_START = "StartStopWatch";
        public const string STOPWATCH_STOP = "StopStopWatch";
        public const string TAIL = "tail";
        public const string TIMESTAMP = "Timestamp";
        public const string TRANSLATE = "translate";
        public const string WRITELINE = "File.writeLine";
        public const string WRITELINES = "File.writeLines";
        public const string WRITENL = "File.writenl";
        public const string WRITE_CONSOLE = "File.Write";
        public const string WRITE = "Write";

        public const string ADD_COMP_DEFINITION = "add_comp_definition";
        public const string ADD_COMP_NAMESPACE = "add_comp_namespace";
        public const string CLEAR_COMP_DEFINITIONS = "clear_comp_definitions";
        public const string CLEAR_COMP_NAMESPACES = "clear_comp_namespaces";
        public const string CSHARP_FUNCTION = "ilililtiehasdie";

        public const string ENGLISH = "en";
        public const string GERMAN = "de";
        public const string RUSSIAN = "ru";
        public const string SPANISH = "es";
        public const string SYNONYMS = "sy";

        public const string LABEL_OPERATOR = ":";
        public const string GOTO = "goto";
        public const string GOSUB = "gosub";

        public const string INCLUDE_SECURE = "includes";

        public const string ENCODE_FILE = "encodeFile";
        public const string DECODE_FILE = "decodeFile";

        public static string Language(string lang)
        {
            switch (lang)
            {
                case "English": return ENGLISH;
                case "German": return GERMAN;
                case "Russian": return RUSSIAN;
                case "Spanish": return SPANISH;
                case "Synonyms": return SYNONYMS;
                default: return ENGLISH;
            }
        }
    }
}
