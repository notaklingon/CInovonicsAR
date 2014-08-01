using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace CInovonics.Domain.CCure
{
    public class GPIDevice
    {
        public delegate void EventRaised(String msg);
        public event EventRaised OnEventRaised;

        public const int MAX_BUFFER_SIZE = 1024;
        public byte Acknowledgement = Convert.ToByte('\x0006');
        public string MSG_HEADER = "<";
        public string MSG_FOOTER = ">";

        private BlockingCollection<string> _msgQueue;

        private int _port;
        private IPAddress _ipAddr;
        private TcpListener _srv;

        private BackgroundWorker _listener;

        public GPIDevice(IPAddress addr, int Port)
        {
            _port = Port;
            _ipAddr = addr;

            _srv = new TcpListener(_ipAddr, _port);
            _srv.Start();

            _msgQueue = new BlockingCollection<string>();

            _listener = new BackgroundWorker();
            _listener.DoWork += new DoWorkEventHandler(listen);
            _listener.RunWorkerAsync();
        }

        public void QueueMessage(string msg)
        {
            _msgQueue.TryAdd(msg);
        }

        public void listen(object sender, DoWorkEventArgs e)
        {
            try
            {
                Byte[] buffer = new Byte[MAX_BUFFER_SIZE];

                while(true)
                {
                    TcpClient client = _srv.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();

                    try
                    {
                        int i = 0;
                        // Loop to receive all the data sent by the client. 
                        while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            //Send our Acknowledgement
                            byte[] reply = new byte[1];
                            reply[0] = Acknowledgement;
                            stream.Write(reply, 0, reply.Length);

                            //Hand it off to whomever is listening
                            if(OnEventRaised != null)
                            {
                                OnEventRaised(System.Text.Encoding.ASCII.GetString(buffer, 0, i));
                            }
                            
                            foreach(string m in _msgQueue.GetConsumingEnumerable())
                            {
                                string encMsg = String.Format("{0}{1}{2}", MSG_HEADER, m, MSG_FOOTER);
                                byte[] qMsg = System.Text.Encoding.ASCII.GetBytes(encMsg);
                                stream.Write(qMsg, 0, qMsg.Length);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
  
                    }

                    stream.Dispose();
                    stream.Close();
                    client.Close();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _srv.Stop();
            }
        }
    }
}
