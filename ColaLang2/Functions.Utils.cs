﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ColaLang
{
    // Returns process info
    class PsInfoFunction : ParserFunction, IStringFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name, true);
            string pattern = args[0].AsString();

            int MAX_PROC_NAME = 26;
            Interpreter.Instance.AppendOutput(Utils.GetLine(), true);
            Interpreter.Instance.AppendOutput(String.Format("{0} {1} {2} {3} {4} {5}",
              "Process Id".PadRight(15), "Process Name".PadRight(MAX_PROC_NAME),
              "Working Set".PadRight(15), "Virt Mem".PadRight(15),
              "Start Time".PadRight(15), "CPU Time".PadRight(25)), true);

            Process[] processes = Process.GetProcessesByName(pattern);
            List<Variable> results = new List<Variable>(processes.Length);
            for (int i = 0; i < processes.Length; i++)
            {
                Process pr = processes[i];
                int workingSet = (int)(((double)pr.WorkingSet64) / 1000000.0);
                int virtMemory = (int)(((double)pr.VirtualMemorySize64) / 1000000.0);
                string procTitle = pr.ProcessName + " " + pr.MainWindowTitle.Split(null)[0];
                string startTime = pr.StartTime.ToString();
                if (procTitle.Length > MAX_PROC_NAME)
                {
                    procTitle = procTitle.Substring(0, MAX_PROC_NAME);
                }
                string procTime = string.Empty;
                try
                {
                    procTime = pr.TotalProcessorTime.ToString().Substring(0, 11);
                }
                catch (Exception) { }

                results.Add(new Variable(
                  string.Format("{0,15} {1," + MAX_PROC_NAME + "} {2,15} {3,15} {4,15} {5,25}",
                    pr.Id, procTitle,
                    workingSet, virtMemory, startTime, procTime)));
                Interpreter.Instance.AppendOutput(results.Last().String, true);
            }
            Interpreter.Instance.AppendOutput(Utils.GetLine(), true);

            return new Variable(results);
        }
    }

    // Kills a process with specified process id
    class KillFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name, true);

            Variable id = args[0];
            Utils.CheckPosInt(id, script);

            int processId = (int)id.Value;
            try
            {
                Process process = Process.GetProcessById(processId);
                process.Kill();
                Interpreter.Instance.AppendOutput("Process " + processId + " killed", true);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't kill process " + processId +
                  " (" + exc.Message + ")");
            }

            return Variable.EmptyInstance;
        }
    }

    // Starts running a new process, returning its ID
    class RunFunction : ParserFunction, INumericFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string processName = Utils.GetItem(script).String;
            Utils.CheckNotEmpty(processName, "processName");

            List<string> args = Utils.GetFunctionArgs(script);
            int processId = -1;

            try
            {
                Process pr = Process.Start(processName, string.Join("", args.ToArray()));
                processId = pr.Id;
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't start [" + processName + "]: " + exc.Message);
            }

            return new Variable(processId);
        }
    }

    // Starts running an "echo" server
    class ServerSocket : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            Variable portRes = Utils.GetItem(script);
            Utils.CheckPosInt(portRes, script);
            int port = (int)portRes.Value;

            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

                Socket listener = new Socket(AddressFamily.InterNetwork,
                                    SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(localEndPoint);
                listener.Listen(10);

                Socket handler = null;
                while (true)
                {
                    Interpreter.Instance.AppendOutput("Waiting for connections on " + port + " ...", true);
                    handler = listener.Accept();

                    // Data buffer for incoming data.
                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    string received = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    Interpreter.Instance.AppendOutput("Received from " + handler.RemoteEndPoint.ToString() +
                      ": [" + received + "]", true);

                    byte[] msg = Encoding.UTF8.GetBytes(received);
                    handler.Send(msg);

                    if (received.Contains("<EOF>"))
                    {
                        break;
                    }
                }

                if (handler != null)
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't start server: (" + exc.Message + ")");
            }

            return Variable.EmptyInstance;
        }
    }

    // Starts running an "echo" client
    class ClientSocket : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];

            List<Variable> args = script.GetFunctionArgs();

            Utils.CheckArgs(args.Count, 3, Constants.CONNECTSRV);
            Utils.CheckPosInt(args[1], script);

            string hostname = args[0].String;
            int port = (int)args[1].Value;
            string msgToSend = args[2].String;

            if (string.IsNullOrWhiteSpace(hostname) || hostname.Equals("localhost"))
            {
                hostname = Dns.GetHostName();
            }

            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(hostname);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);

                sender.Connect(remoteEP);

                Interpreter.Instance.AppendOutput("Connected to [" + sender.RemoteEndPoint.ToString() + "]", true);

                byte[] msg = Encoding.UTF8.GetBytes(msgToSend);
                sender.Send(msg);

                // Receive the response from the remote device.
                int bytesRec = sender.Receive(bytes);
                string received = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't connect to server: (" + exc.Message + ")");
            }

            return Variable.EmptyInstance;
        }
    }

    // Returns current directory name
    class PwdFunction : ParserFunction, IStringFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string path = Directory.GetCurrentDirectory();
            return new Variable(path);
        }
    }

    // Equivalent to cd.. on Windows: one directory up
    class Cd__Function : ParserFunction, IStringFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string newDir = null;

            try
            {
                string pwd = Directory.GetCurrentDirectory();
                DirectoryInfo parent = Directory.GetParent(pwd);
                if (parent == null)
                {
                    throw new ArgumentException("No parent exists.");
                }
                newDir = parent.FullName;
                Directory.SetCurrentDirectory(newDir);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't change directory: " + exc.Message);
            }

            return new Variable(newDir);
        }
    }

    // Changes directory to the passed one
    class CdFunction : ParserFunction, IStringFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            if (script.Substr().StartsWith(" .."))
            {
                script.Forward();
            }
            string newDir = Utils.GetItem(script).AsString();

            try
            {
                if (newDir == "..")
                {
                    string pwd = Directory.GetCurrentDirectory();
                    DirectoryInfo parent = Directory.GetParent(pwd);
                    if (parent == null)
                    {
                        throw new ArgumentException("No parent exists.");
                    }
                    newDir = parent.FullName;
                }
                if (newDir.Length == 0)
                {
                    newDir = Environment.GetEnvironmentVariable("HOME");
                }
                Directory.SetCurrentDirectory(newDir);

                newDir = Directory.GetCurrentDirectory();
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't change directory: " + exc.Message);
            }

            return new Variable(newDir);
        }
    }

    // Reads a file and returns all lines of that file as a "tuple" (list)
    class ReadCOLAFileFunction : ParserFunction, IArrayFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string filename = Utils.GetItem(script).AsString();
            string[] lines = Utils.GetFileLines(filename);

            List<Variable> results = Utils.ConvertToResults(lines);

            return new Variable(results);
        }
    }


    // View the contents of a text file
    class MoreFunction : ParserFunction, IArrayFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string filename = Utils.GetItem(script).AsString();
            int size = Constants.DEFAULT_FILE_LINES;

            bool sizeAvailable = Utils.SeparatorExists(script);
            if (sizeAvailable)
            {
                Variable length = Utils.GetItem(script);
                Utils.CheckPosInt(length, script);
                size = (int)length.Value;
            }

            string[] lines = Utils.GetFileLines(filename, 0, size);
            List<Variable> results = Utils.ConvertToResults(lines);

            return new Variable(results);
        }
    }

    // View the last Constants.DEFAULT_FILE_LINES lines of a file
    class TailFunction : ParserFunction, IArrayFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string filename = Utils.GetItem(script).AsString();
            int size = Constants.DEFAULT_FILE_LINES;

            bool sizeAvailable = Utils.SeparatorExists(script);
            if (sizeAvailable)
            {
                Variable length = Utils.GetItem(script);
                Utils.CheckPosInt(length, script);
                size = (int)length.Value;
            }

            string[] lines = Utils.GetFileLines(filename, -1, size);
            List<Variable> results = Utils.ConvertToResults(lines);

            return new Variable(results);
        }
    }

    // Append a line to a file
    class AppendLineFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string filename = Utils.GetItem(script).AsString();
            Variable line = Utils.GetItem(script);
            Utils.AppendFileText(filename, line.AsString() + Environment.NewLine);

            return Variable.EmptyInstance;
        }
    }

    // Apend a list of lines to a file
    class AppendLinesFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string filename = Utils.GetItem(script).AsString();
            string lines = Utils.GetLinesFromList(script);
            Utils.AppendFileText(filename, lines);

            return Variable.EmptyInstance;
        }
    }

    // Write a line to a file
    class WriteLineFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string filename = Utils.GetItem(script).AsString();
            Variable line = Utils.GetItem(script);
            Utils.WriteFileText(filename, line.AsString() + Environment.NewLine);

            return Variable.EmptyInstance;
        }
    }

    // Write a list of lines to a file
    class WriteLinesFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            //string filename = Utils.ResultToString(Utils.GetItem(script));
            string filename = Utils.GetItem(script).AsString();
            string lines = Utils.GetLinesFromList(script);
            Utils.WriteFileText(filename, lines);

            return Variable.EmptyInstance;
        }
    }

    // Find a string in files
    class FindstrFunction : ParserFunction, IArrayFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string search = Utils.GetItem(script).AsString();
            List<string> patterns = Utils.GetFunctionArgs(script);

            bool ignoreCase = true;
            if (patterns.Count > 0 && patterns.Last().Equals("case"))
            {
                ignoreCase = false;
                patterns.RemoveAt(patterns.Count - 1);
            }
            if (patterns.Count == 0)
            {
                patterns.Add("*.*");
            }

            List<Variable> results = null;
            try
            {
                string pwd = Directory.GetCurrentDirectory();
                List<string> files = Utils.GetStringInFiles(pwd, search, patterns.ToArray(), ignoreCase);

                results = Utils.ConvertToResults(files.ToArray(), true);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't find pattern: " + exc.Message);
            }

            return new Variable(results);
        }
    }

    // Find files having a given pattern
    class FindfilesFunction : ParserFunction, IArrayFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<string> patterns = Utils.GetFunctionArgs(script);
            if (patterns.Count == 0)
            {
                patterns.Add("*.*");
            }

            List<Variable> results = null;
            try
            {
                string pwd = Directory.GetCurrentDirectory();
                List<string> files = Utils.GetFiles(pwd, patterns.ToArray());

                results = Utils.ConvertToResults(files.ToArray(), true);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't list directory: " + exc.Message);
            }

            return new Variable(results);
        }
    }

    // Copy a file or a directiry
    class CopyFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string source = Utils.GetItem(script).AsString();
            script.MoveForwardIf(Constants.NEXT_ARG, Constants.SPACE);
            string dest = Utils.GetItem(script).AsString();

            string src = Path.GetFullPath(source);
            string dst = Path.GetFullPath(dest);

            List<Variable> srcPaths = Utils.GetPathnames(src);
            bool multipleFiles = srcPaths.Count > 1;
            if (dst.EndsWith("*"))
            {
                dst = dst.Remove(dst.Count() - 1);
            }
            if ((multipleFiles || Directory.Exists(src)) &&
                !Directory.Exists(dst))
            {
                try
                {
                    Directory.CreateDirectory(dst);
                }
                catch (Exception exc)
                {
                    throw new ArgumentException("Couldn't create [" + dst + "] :" + exc.Message);
                }

            }

            foreach (Variable srcPath in srcPaths)
            {
                string filename = Path.GetFileName(srcPath.String);
                //string dstPath  = Path.Combine(dst, filename);
                Utils.Copy(srcPath.String, dst);
            }

            return Variable.EmptyInstance;
        }
    }

    // Move a file or a directiry
    class MoveFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string source = Utils.GetItem(script).AsString();
            script.MoveForwardIf(Constants.NEXT_ARG, Constants.SPACE);
            string dest = Utils.GetItem(script).AsString();

            string src = Path.GetFullPath(source);
            string dst = Path.GetFullPath(dest);

            bool isFile = File.Exists(src);
            bool isDir = Directory.Exists(src);
            if (!isFile && !isDir)
            {
                throw new ArgumentException("[" + src + "] doesn't exist");
            }

            if (isFile && Directory.Exists(dst))
            {
                // If filename is missing in the destination file,
                // add it from the source.
                dst = Path.Combine(dst, Path.GetFileName(src));
            }

            try
            {
                if (isFile)
                {
                    File.Move(src, dst);
                }
                else
                {
                    Directory.Move(src, dst);
                }
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't copy: " + exc.Message);
            }

            return Variable.EmptyInstance;
        }
    }

    // Make a directory
    class MkdirFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string dirname = Utils.GetItem(script).AsString();
            try
            {
                Directory.CreateDirectory(dirname);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't create [" + dirname + "] :" + exc.Message);
            }

            return Variable.EmptyInstance;
        }
    }

    // Delete a file or a directory
    class DeleteFunction : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string pathname = Utils.GetItem(script).AsString();

            bool isFile = File.Exists(pathname);
            bool isDir = Directory.Exists(pathname);
            if (!isFile && !isDir)
            {
                throw new ArgumentException("[" + pathname + "] doesn't exist");
            }
            try
            {
                if (isFile)
                {
                    File.Delete(pathname);
                }
                else
                {
                    Directory.Delete(pathname, true);
                }
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't delete [" + pathname + "] :" + exc.Message);
            }

            return Variable.EmptyInstance;
        }
    }

    // Checks if a directory or a file exists
    class ExistsFunction : ParserFunction, INumericFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string pathname = Utils.GetItem(script).AsString();

            bool exists = false;
            try
            {
                exists = File.Exists(pathname);
                if (!exists)
                {
                    exists = Directory.Exists(pathname);
                }
            }
            catch (Exception)
            {
            }

            return new Variable(exists);
        }
    }

    // List files in a directory
    class DirFunction : ParserFunction, IArrayFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            string dirname = (!script.StillValid() || script.Current == Constants.END_STATEMENT) ?
              Directory.GetCurrentDirectory() :
              Utils.GetToken(script, Constants.NEXT_OR_END_ARRAY);

            //List<Variable> results = Utils.GetPathnames(dirname);
            List<Variable> results = new List<Variable>();

            int index = dirname.IndexOf('*');
            if (index < 0 && !Directory.Exists(dirname) && !File.Exists(dirname))
            {
                throw new ArgumentException("Directory [" + dirname + "] doesn't exist");
            }

            string pattern = Constants.ALL_FILES;

            try
            {
                string dir = index < 0 ? Path.GetFullPath(dirname) : dirname;
                if (File.Exists(dir))
                {
                    FileInfo fi = new FileInfo(dir);
                    Interpreter.Instance.AppendOutput(Utils.GetPathDetails(fi, fi.Name), true);
                    results.Add(new Variable(fi.Name));
                    return new Variable(results);
                }
                // Special dealing if there is a pattern (only * is supported at the moment)
                if (index >= 0)
                {
                    pattern = Path.GetFileName(dirname);

                    if (index > 0)
                    {
                        string prefix = dirname.Substring(0, index);
                        DirectoryInfo di = Directory.GetParent(prefix);
                        dirname = di.FullName;
                    }
                    else
                    {
                        dirname = ".";
                    }
                }
                dir = Path.GetFullPath(dirname);
                // First get contents of the directory (unless there is a pattern)
                DirectoryInfo dirInfo = new DirectoryInfo(dir);

                if (pattern == Constants.ALL_FILES)
                {
                    Interpreter.Instance.AppendOutput(Utils.GetPathDetails(dirInfo, "."), true);
                    if (dirInfo.Parent != null)
                    {
                        Interpreter.Instance.AppendOutput(Utils.GetPathDetails(dirInfo.Parent, ".."), true);
                    }
                }

                // Then get contents of all of the files in the directory
                FileInfo[] fileNames = dirInfo.GetFiles(pattern);
                foreach (FileInfo fi in fileNames)
                {
                    try
                    {
                        Interpreter.Instance.AppendOutput(Utils.GetPathDetails(fi, fi.Name), true);
                        results.Add(new Variable(fi.Name));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                // Then get contents of all of the subdirs in the directory
                DirectoryInfo[] dirInfos = dirInfo.GetDirectories(pattern);
                foreach (DirectoryInfo di in dirInfos)
                {
                    try
                    {
                        Interpreter.Instance.AppendOutput(Utils.GetPathDetails(di, di.Name), true);
                        results.Add(new Variable(di.Name));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't list directory: " + exc.Message);
            }

            return new Variable(results);
        }
    }

    class TimestampFunction : ParserFunction, IStringFunction
    {
        bool m_millis = false;
        public TimestampFunction(bool millis = false)
        {
            m_millis = millis;
        }
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();

            double timestamp = Utils.GetSafeDouble(args, 0);
            string strFormat = Utils.GetSafeString(args, 1, "yyyy/MM/dd HH:mm:ss.fff");
            Utils.CheckNotEmpty(strFormat, m_name);

            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            if (m_millis)
            {
                dt = dt.AddMilliseconds(timestamp);
            }
            else
            {
                dt = dt.AddSeconds(timestamp);
            }

            DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            DateTime localVersion = runtimeKnowsThisIsUtc.ToLocalTime();
            string when = localVersion.ToString(strFormat);
            return new Variable(when);
        }
    }

    class StopWatchFunction : ParserFunction, IStringFunction
    {
        static System.Diagnostics.Stopwatch m_stopwatch = new System.Diagnostics.Stopwatch();
        public enum Mode { START, STOP, ELAPSED, TOTAL_SECS, TOTAL_MS };

        Mode m_mode;
        public StopWatchFunction(Mode mode)
        {
            m_mode = mode;
        }

        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();

            if (m_mode == Mode.START)
            {
                m_stopwatch.Restart();
                return Variable.EmptyInstance;
            }

            string strFormat = Utils.GetSafeString(args, 0, "secs");
            string elapsedStr = "";
            double elapsed = -1.0;
            if (strFormat == "hh::mm:ss.fff")
            {
                elapsedStr = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
                  m_stopwatch.Elapsed.Hours, m_stopwatch.Elapsed.Minutes,
                  m_stopwatch.Elapsed.Seconds, m_stopwatch.Elapsed.Milliseconds);
            }
            else if (strFormat == "mm:ss.fff")
            {
                elapsedStr = string.Format("{0:D2}:{1:D2}.{2:D3}",
                    m_stopwatch.Elapsed.Minutes,
                    m_stopwatch.Elapsed.Seconds, m_stopwatch.Elapsed.Milliseconds);
            }
            else if (strFormat == "mm:ss")
            {
                elapsedStr = string.Format("{0:D2}:{1:D2}",
                    m_stopwatch.Elapsed.Minutes,
                    m_stopwatch.Elapsed.Seconds);
            }
            else if (strFormat == "ss.fff")
            {
                elapsedStr = string.Format("{0:D2}.{1:D3}",
                    m_stopwatch.Elapsed.Seconds, m_stopwatch.Elapsed.Milliseconds);
            }
            else if (strFormat == "secs")
            {
                elapsed = Math.Round(m_stopwatch.Elapsed.TotalSeconds);
            }
            else if (strFormat == "ms")
            {
                elapsed = Math.Round(m_stopwatch.Elapsed.TotalMilliseconds);
            }

            if (m_mode == Mode.STOP)
            {
                m_stopwatch.Stop();
            }

            return elapsed >= 0 ? new Variable(elapsed) : new Variable(elapsedStr);
        }
    }

    class EncodeFileFunction : ParserFunction
    {
        bool m_encode = true;

        public EncodeFileFunction(bool encode = true)
        {
            m_encode = encode;
        }

        public static string EncodeText(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            var intermidiate = System.Convert.ToBase64String(plainTextBytes);


            //return intermidiate;
            return plainText;
        }

        public static string DecodeText(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            var intermidiate = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);


            return intermidiate;
        }

        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name, true);
            string filename = args[0].AsString();
            string pathname = script.GetFilePath(filename);

            return EncodeDecode(pathname, m_encode);
        }

        public static Variable EncodeDecode(string pathname, bool encode)
        {
            string text = Utils.GetFileText(pathname);
            string newText = "";

            try
            {
                newText = encode ? EncodeText(text) : DecodeText(text);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return Variable.EmptyInstance;
            }

            Utils.WriteFileText(pathname, newText);
            return new Variable(pathname);
        }
    }

    public class WebRequestFunction : ParserFunction
    {
        static string[] s_allowedMethods = { "GET", "POST", "PUT", "DELETE", "HEAD", "OPTIONS", "TRACE" };

        public override async Task<Variable> EvaluateAsync(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);
            string method = args[0].AsString().ToUpper();
            string uri = args[1].AsString();
            string load = Utils.GetSafeString(args, 2);
            string tracking = Utils.GetSafeString(args, 3);
            string onSuccess = Utils.GetSafeString(args, 4);
            string onFailure = Utils.GetSafeString(args, 5, onSuccess);
            string contentType = Utils.GetSafeString(args, 6, "application/x-www-form-urlencoded");
            Variable headers = Utils.GetSafeVariable(args, 7);
            int timeoutMs = Utils.GetSafeInt(args, 8, 10 * 1000);
            bool justFire = Utils.GetSafeInt(args, 9) > 0;

            if (!s_allowedMethods.Contains(method))
            {
                throw new ArgumentException("Unknown web request method: " + method);
            }

            await ProcessWebRequestAsync(uri, method, load, onSuccess, onFailure, tracking, contentType, headers, timeoutMs, justFire);

            return Variable.EmptyInstance;
        }

        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);
            string method = args[0].AsString().ToUpper();
            string uri = args[1].AsString();
            string load = Utils.GetSafeString(args, 2);
            string tracking = Utils.GetSafeString(args, 3);
            string onSuccess = Utils.GetSafeString(args, 4);
            string onFailure = Utils.GetSafeString(args, 5, onSuccess);
            string contentType = Utils.GetSafeString(args, 6, "application/x-www-form-urlencoded");
            Variable headers = Utils.GetSafeVariable(args, 7);
            int timeoutMs = Utils.GetSafeInt(args, 8, 10 * 1000);
            bool justFire = Utils.GetSafeInt(args, 9) > 0;

            if (!s_allowedMethods.Contains(method))
            {
                throw new ArgumentException("Unknown web request method: " + method);
            }

            Task.Run(() => ProcessWebRequest(uri, method, load, onSuccess, onFailure, tracking,
                                             contentType, headers));

            return Variable.EmptyInstance;
        }

        static void ProcessWebRequest(string uri, string method, string load,
                                            string onSuccess, string onFailure,
                                            string tracking, string contentType,
                                            Variable headers)
        {
            try
            {
                WebRequest request = WebRequest.CreateHttp(uri);
                request.Method = method;
                request.ContentType = contentType;

                if (!string.IsNullOrWhiteSpace(load))
                {
                    var bytes = Encoding.UTF8.GetBytes(load);
                    request.ContentLength = bytes.Length;

                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                    }
                }

                if (headers != null && headers.Tuple != null)
                {
                    var keys = headers.GetKeys();
                    foreach (var header in keys)
                    {
                        var headerValue = headers.GetVariable(header).AsString();
                        request.Headers.Add(header, headerValue);
                    }
                }
                HttpWebResponse resp = request.GetResponse() as HttpWebResponse;
                string result;
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
                string responseCode = resp == null ? "" : resp.StatusCode.ToString();
                CustomFunction.Run(onSuccess, new Variable(tracking),
                                   new Variable(responseCode), new Variable(result));
            }
            catch (Exception exc)
            {
                CustomFunction.Run(onFailure, new Variable(tracking),
                                   new Variable(""), new Variable(exc.Message));
            }
        }

        static async Task ProcessWebRequestAsync(string uri, string method, string load,
                                            string onSuccess, string onFailure,
                                            string tracking, string contentType,
                                            Variable headers, int timeout,
                                            bool justFire = false)
        {
            try
            {
                WebRequest request = WebRequest.CreateHttp(uri);
                request.Method = method;
                request.ContentType = contentType;

                if (!string.IsNullOrWhiteSpace(load))
                {
                    var bytes = Encoding.UTF8.GetBytes(load);
                    request.ContentLength = bytes.Length;

                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                    }
                }

                if (headers != null && headers.Tuple != null)
                {
                    var keys = headers.GetKeys();
                    foreach (var header in keys)
                    {
                        var headerValue = headers.GetVariable(header).AsString();
                        request.Headers.Add(header, headerValue);
                    }
                }

                Task<WebResponse> task = request.GetResponseAsync();
                Task finishTask = FinishRequest(onSuccess, onFailure,
                                                tracking, task, timeout);
                if (justFire)
                {
                    return;
                }
                await finishTask;
            }
            catch (Exception exc)
            {
                await CustomFunction.RunAsync(onFailure, new Variable(tracking),
                                              new Variable(""), new Variable(exc.Message));
            }
        }

        static async Task FinishRequest(string onSuccess, string onFailure,
                                        string tracking, Task<WebResponse> responseTask,
                                        int timeoutMs)
        {
            string result = "";
            string method = onSuccess;
            HttpWebResponse response = null;
            Task timeoutTask = Task.Delay(timeoutMs);

            try
            {
                Task first = await Task.WhenAny(timeoutTask, responseTask);
                if (first == timeoutTask)
                {
                    await timeoutTask;
                    throw new Exception("Timeout waiting for response.");
                }

                response = await responseTask as HttpWebResponse;
                if ((int)response.StatusCode >= 400)
                {
                    throw new Exception(response.StatusDescription);
                }

                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch (Exception exc)
            {
                result = exc.Message;
                method = onFailure;
            }

            string responseCode = response == null ? "" : response.StatusCode.ToString();
            await CustomFunction.RunAsync(method, new Variable(tracking),
                                          new Variable(responseCode), new Variable(result));
        }
    }

    class GetVariableFromJSONFunction : ParserFunction
    {
        static char[] SEP = "\",:]}".ToCharArray();

        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            string json = args[0].AsString();

            Dictionary<int, int> d;
            json = Utils.ConvertToScript(json, out d);

            var tempScript = script.GetTempScript(json);
            Variable result = ExtractValue(tempScript);
            return result;
        }

        static Variable ExtractObject(ParsingScript script)
        {
            Variable newValue = new Variable(Variable.VarType.ARRAY);

            while (script.StillValid() && (newValue.Count == 0 || script.Current == ','))
            {
                script.Forward();
                string key = Utils.GetToken(script, SEP);
                script.MoveForwardIf(':');

                Variable valueVar = ExtractValue(script);
                newValue.SetHashVariable(key, valueVar);
            }
            script.MoveForwardIf('}');

            return newValue;
        }

        static Variable ExtractArray(ParsingScript script)
        {
            Variable newValue = new Variable(Variable.VarType.ARRAY);

            while (script.StillValid() && (newValue.Count == 0 || script.Current == ','))
            {
                script.Forward();
                Variable addVariable = ExtractValue(script);
                newValue.AddVariable(addVariable);
            }
            script.MoveForwardIf(']');

            return newValue;
        }

        static Variable ExtractValue(ParsingScript script)
        {
            if (script.TryCurrent() == '{')
            {
                return ExtractObject(script);
            }
            if (script.TryCurrent() == '[')
            {
                return ExtractArray(script);
            }
            var token = Utils.GetToken(script, SEP);
            return new Variable(token);
        }
    }


    class IncludeFileSecure : ParserFunction
    {
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name, true);

            string filename = args[0].AsString();
            string pathname = script.GetFilePath(filename);

            EncodeFileFunction.EncodeDecode(pathname, false);
            ParsingScript tempScript = script.GetIncludeFileScript(filename);
            string includeScript = tempScript.String;
            EncodeFileFunction.EncodeDecode(pathname, true);

            Variable result = null;
            if (script.Debugger != null)
            {
                result = script.Debugger.StepInIncludeIfNeeded(tempScript).Result;
            }

            while (tempScript.Pointer < includeScript.Length)
            {
                result = tempScript.Execute();
                tempScript.GoToNextStatement();
            }
            return result == null ? Variable.EmptyInstance : result;
        }
    }
}
