using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace SplitAndMerge
{
    class COLA_NET
    {
        public static void Init()
        {
            //ParserFunction.RegisterFunction("connectTest", new ConnectTest());
        }


    }

    class ConnectTest : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var connection = Utils.GetSafeString(args, 0);
            var sport = Utils.GetSafeString(args, 1);
            int port = int.Parse(sport);
            TcpListener tcp = new TcpListener(IPAddress.Parse(connection), port);
            tcp.Start();
            Console.WriteLine("internet test");
            return Variable.EmptyInstance;
        }
    }
}
