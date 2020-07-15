using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Net.NetworkInformation;

namespace SplitAndMerge
{
    class COLA_NET
    {
        public static List<NetClient> ncs = new List<NetClient>(); // its NetClients not No Copyright Sounds :P
        public static List<NetListener> nls = new List<NetListener>();
        public static List<NetSocket> nsts = new List<NetSocket>();
        public static void Init()
        {
            //ParserFunction.RegisterFunction("connectTest", new ConnectTest());
        }


    }

    #region Server / Client

    class NetTCPClient : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var address = Utils.GetSafeString(args, 0);
            var port = Utils.GetSafeInt(args, 1);

            NetClient nc = new NetClient(address, port, COLA_NET.ncs.Count);
            COLA_NET.ncs.Add(nc);

            return new Variable(nc);
        }
    }

    class NetTCPListener : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var port = Utils.GetSafeInt(args, 0);

            NetListener nl = new NetListener(port, COLA_NET.nls.Count);
            COLA_NET.nls.Add(nl);

            return new Variable(nl);
        }
    }

    class NetTCPSocket : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var nls = Utils.GetSafeString(args, 0);

            NetListener nl = NetListener.Parse(nls);
            NetSocket nst = new NetSocket(nl, COLA_NET.nsts.Count);
            COLA_NET.nsts.Add(nst);

            return new Variable(nst);
        }
    }

    class NetWriteLine : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var nostring = Utils.GetSafeString(args, 0);
            var data = Utils.GetSafeString(args, 1);

            if (nostring.StartsWith("NC"))
            {
                NetClient no = NetClient.Parse(nostring);
                no.streamWriter.WriteLine(data);
            }
            else if (nostring.StartsWith("NST"))
            {
                NetSocket no = NetSocket.Parse(nostring);
                no.streamWriter.WriteLine(data);
            }

            return Variable.EmptyInstance;
        }
    }

    class NetReadLine : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var nostring = Utils.GetSafeString(args, 0);

            string callback = "";

            if (nostring.StartsWith("NC"))
            {
                NetClient nc = NetClient.Parse(nostring);
                callback = nc.streamReader.ReadLine();
            }
            else if (nostring.StartsWith("NST"))
            {
                NetSocket no = NetSocket.Parse(nostring);
                callback = no.streamReader.ReadLine();
            }

            return new Variable(callback);
        }
    }

    class NetFlush : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var nostring = Utils.GetSafeString(args, 0);

            if (nostring.StartsWith("NC"))
            {
                NetClient nc = NetClient.Parse(nostring);
                nc.streamWriter.Flush();
            }
            else if (nostring.StartsWith("NST"))
            {
                NetSocket no = NetSocket.Parse(nostring);
                no.streamWriter.Flush();
            }
                return Variable.EmptyInstance;
        }
    }

    class NetDisconnect : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var nostring = Utils.GetSafeString(args, 0);

            if (nostring.StartsWith("NC"))
            {
                NetClient nc = NetClient.Parse(nostring);
                nc.tcp.Close();
                nc.networkStream.Close();
                nc.streamReader.Close();
                nc.streamWriter.Close();
            }
            else if (nostring.StartsWith("NST"))
            {
                NetSocket no = NetSocket.Parse(nostring);
                no.sfc.Close();
                no.networkStream.Close();
                no.streamReader.Close();
                no.streamWriter.Close();
            }

            return Variable.EmptyInstance;
        }
    }

    #endregion

    #region Web

    class NetPing : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var address = Utils.GetSafeString(args, 0);
            var timeout = Utils.GetSafeInt(args, 1);

            Ping pingSender = new Ping();

            string data = "cola-madeby-ProfessorDJ&RedCube";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            PingOptions options = new PingOptions(64, true);

            PingReply reply = pingSender.Send(address, timeout, buffer, options);

            if (reply.Status == IPStatus.Success)
            {
                return new Variable("Address: " + reply.Address.ToString() + " Ping: " + reply.RoundtripTime + " [SUCCESS]");
            }
            else
            {
                return new Variable(reply.Status);
            }
        }
    }

    class NetReadFromURL : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var address = Utils.GetSafeString(args, 0);

            using (WebClient client = new WebClient())
            {
                string s = client.DownloadString(address);
                return new Variable(s);
            }
        }
    }

    class NetDownloadFile : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var address = Utils.GetSafeString(args, 0);
            var path = Utils.GetSafeString(args, 1);

            WebClient WebC = new WebClient();
            WebC.DownloadFile(address, path);

            return new Variable(true);
        }
    }

    #endregion

    /*class ConnectTest : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
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
    }*/

    public class NetClient
    {
        public TcpClient tcp;
        public NetworkStream networkStream;
        public StreamWriter streamWriter;
        public StreamReader streamReader;

        int id;
        string address;
        int port;

        public NetClient(string ip, int _port, int _id)
        {
            id = _id;
            address = ip;
            port = _port;
            tcp = new TcpClient(ip, _port);

            networkStream = tcp.GetStream();
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);
        }
        public static NetClient Parse (string s)
        {
            string sid0 = s.Split('[')[1];
            string sid1 = sid0.Substring(0, sid0.Length - 1);
            int index = int.Parse(sid1);
            return COLA_NET.ncs[index];
        }
        public override string ToString ()
        {
            return "NC " + address + ", " + port + " [" + id + "]";
        }
    }

    public class NetListener
    {
        public TcpListener tcp;

        int id;
        int port;

        public NetListener(int _port, int _id)
        {
            id = _id;
            port = _port;
            tcp = new TcpListener(_port);
            tcp.Start();
        }
        public static NetListener Parse(string s)
        {
            string sid0 = s.Split('[')[1];
            string sid1 = sid0.Substring(0, sid0.Length - 1);
            int index = int.Parse(sid1);
            return COLA_NET.nls[index];
        }
        public override string ToString()
        {
            return "NL " + port + " [" + id + "]";
        }
    }

    public class NetSocket
    {
        public Socket sfc;
        public NetworkStream networkStream;
        public StreamWriter streamWriter;
        public StreamReader streamReader;

        int id;

        public NetSocket(NetListener tcp, int _id)
        {
            id = _id;
            sfc = tcp.tcp.AcceptSocket();
            networkStream = new NetworkStream(sfc);
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);
        }
        public static NetSocket Parse(string s)
        {
            string sid0 = s.Split('[')[1];
            string sid1 = sid0.Substring(0, sid0.Length - 1);
            int index = int.Parse(sid1);
            return COLA_NET.nsts[index];
        }
        public override string ToString()
        {
            return "NST " + sfc.Connected + " [" + id + "]";
        }
    }
}
