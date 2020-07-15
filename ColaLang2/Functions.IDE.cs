using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SplitAndMerge
{
    class IDEConnectDebugger : ParserFunction
    {
        public static TcpClient socketForServer;
        public static NetworkStream networkStream;
        public static System.IO.StreamReader streamReader;
        public static System.IO.StreamWriter streamWriter;
        public static bool initialized = false;

        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var port = Utils.GetSafeInt(args, 0);

            socketForServer = new TcpClient("localHost", port);

            networkStream = socketForServer.GetStream();
            streamReader = new System.IO.StreamReader(networkStream);
            streamWriter = new System.IO.StreamWriter(networkStream);
            initialized = true;

            return new Variable("Connected to Debug Server on port: " + port);
        }
    }

    class IDELog : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            if (!IDEConnectDebugger.initialized)
                return Variable.EmptyInstance;

            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var reason = Utils.GetSafeString(args, 0);
            var message = Utils.GetSafeString(args, 1);

            string msg = reason + "|" + message;
            IDEConnectDebugger.streamWriter.WriteLine(msg);
            IDEConnectDebugger.streamWriter.Flush();

            return new Variable(true);
        }
    }

    class IDEGetInput : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            if (!IDEConnectDebugger.initialized)
                return Variable.EmptyInstance;

            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var text = Utils.GetSafeString(args, 0);

            IDEConnectDebugger.streamWriter.WriteLine("Input|" + text);
            IDEConnectDebugger.streamWriter.Flush();

            string theString = IDEConnectDebugger.streamReader.ReadLine();

            return new Variable(theString);
        }
    }

    class IDEDisconnect : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            if (!IDEConnectDebugger.initialized)
                return Variable.EmptyInstance;

            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 0, m_name);

            IDEConnectDebugger.streamWriter.WriteLine("exit");
            IDEConnectDebugger.streamWriter.Flush();
            IDEConnectDebugger.networkStream.Close();

            IDEConnectDebugger.initialized = false;

            return new Variable(true);
        }
    }

    class IDELogInfo : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            return new Variable("Info");
        }
    }
    class IDELogError : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            return new Variable("Error");
        }
    }
    class IDELogWarning : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            return new Variable("Warning");
        }
    }
}