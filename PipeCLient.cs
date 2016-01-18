using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;

namespace AsyncNamedPipe
{
    public class PipeCLient
    {
        public Action<string> OnMsg, OnError, OnState;
        NamedPipeClientStream pipeClient = null;
        StreamWriter sw = null;

        public PipeCLient()
        {
            pipeClient =
           new NamedPipeClientStream("localhost", "testpipe", PipeDirection.InOut, PipeOptions.Asynchronous);
            if (OnState != null)
            {
                OnState("pipe client established");
            }
        }

        public void Start()
        {
            try
            {
                pipeClient.Connect(2000);
                sw = new StreamWriter(pipeClient);
                sw.AutoFlush = true;



                Thread thListen = new Thread(new ParameterizedThreadStart(StartListen));
                thListen.Start(pipeClient);
                if (pipeClient.IsConnected && OnState != null)
                {
                    OnState("pipe client connected to server");
                }
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError("pipe client connecting failed: " + ex.Message);
                }
            }
        }

        private void StartListen(object obj)
        {
            NamedPipeClientStream client = obj as NamedPipeClientStream;
            StreamReader sr = new StreamReader(client);
           
                string revMsg = null;
                while (true)
                {
                    try
                    {
                        if (null != (revMsg = sr.ReadLine()))
                        {
                            OnMsg(revMsg);
                        }
                    }
                    catch (IOException ex)
                    {
                        if (OnError!=null)
                        {
                            OnError(ex.Message);
                        }
                        break;
                    }
                   
                }
                sr.Close();
                client.Close();
                client.Dispose();

        }

        public void Send(string msg)
        {
            if (pipeClient.IsConnected&& sw != null)
            {
                sw.WriteLine(msg);
               
            }
            else
            {
                if (OnError != null)
                {
                    OnError("client aren't connected to any server! stop send");
                }
            }
        }
    }
}
