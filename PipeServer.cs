using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace AsyncNamedPipe
{
    public class PipeServer
    {
        public Action<string> OnMsg, OnError, OnState;
        NamedPipeServerStream pipeServer;
        StreamWriter sw = null;
        public PipeServer()
        {
            pipeServer =
            new NamedPipeServerStream("testpipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

        }

        public void StartListen()
        {
            if (OnMsg != null)
            {
                OnMsg("pipe Established");
            }
            ThreadPool.QueueUserWorkItem(delegate
            {
                pipeServer.BeginWaitForConnection((o) =>
                {
                    if (OnMsg != null)
                    {
                        OnMsg("client conned");
                    }
                    NamedPipeServerStream pServer = (NamedPipeServerStream)o.AsyncState;
                    pServer.EndWaitForConnection(o);
                    StreamReader sr = new StreamReader(pServer);
                    string str = null;
                    while (true)
                    {
                        if (null != (str = (sr.ReadLine())))
                        {
                            if (OnMsg != null)
                            {
                                OnMsg(str);
                            }
                        }

                    }
                }, pipeServer);
            });
        }

        public void Send(string msg)
        {
            sw = new StreamWriter(pipeServer);
            try
            {
              
                if (pipeServer.IsConnected && sw != null)
                {
                    sw.AutoFlush = true;
                    sw.WriteLine(msg);
                }
            }
            catch (Exception ex)
            {
                if (OnMsg != null)
                {
                    OnMsg(ex.Message);
                }
                if (sw!=null)
                {
                    sw.Close();
                }
            }


        }
    }
}