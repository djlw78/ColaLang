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

        protected override Variable Evaluate(ParsingScript script)
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
        protected override Variable Evaluate(ParsingScript script)
        {
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
}