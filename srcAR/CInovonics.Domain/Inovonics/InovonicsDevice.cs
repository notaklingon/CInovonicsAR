using System;
using System.Net;
using System.Xml;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using CInovonics.Domain.LaxCo;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace CInovonics.Domain.Inovonics
{
    public class InovonticsDevice : IDisposable
    {
        public delegate void EventRaised(PanicEvent evnt);
        public event EventRaised OnInovonicsEventRaised;

        private bool _connected;

        private IPAddress _ipAddr;
        private WebRequest _webReq;
        private NetworkCredential _credentials;
        private CredentialCache _credentialCache;
        private string _userName;
        private string _password;
        private DoWorkEventArgs _dw;
        private Thread _listenThread;
        private Thread _workerThread;

        public InovonticsDevice(IPAddress addr, string username, string password)
        {
            _connected = false;
            _ipAddr = addr;
            _userName = username;
            _password = password;
            _credentials = new NetworkCredential(_userName, _password);
            _credentialCache = new CredentialCache();

            _dw = new DoWorkEventArgs(null);
            _listenThread = new Thread(() => Listen(this, _dw));
            _listenThread.Start();
        }

        private void Listen(object sender, DoWorkEventArgs e)
        {
            string url = @"http://" + _ipAddr.ToString() + "/PSIA/Metadata/stream?AreaControlEvents=true";
            _credentialCache.Add(new System.Uri(url), "Basic", _credentials);

            _webReq = WebRequest.Create(url);
            _webReq.Credentials = _credentialCache;
            _webReq.PreAuthenticate = true;

            HttpWebResponse response = (HttpWebResponse)_webReq.GetResponse();
            Stream responseStream = response.GetResponseStream();
            XmlTextReader reader = new XmlTextReader(responseStream);

            while (true) { 
                try
                {
                    reader.MoveToContent();
                    string contents = reader.ReadOuterXml();

                    if (OnInovonicsEventRaised != null)
                    {
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                        worker.RunWorkerAsync(contents);
                    }

                    reader.ResetState();
                }
                catch (System.Net.WebException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Inovonics-CONNECTION FAILED");
                    _connected = false;
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine(ex2.Message.ToString());
                }
            }            
        }

        public void Stop()
        {
            _webReq.Abort();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string docContents = (string)e.Argument;

                /* Deserializing the response takes to long using the XMLSerializer, 
                 * so much that we eneded up missing events. 
                 * 
                 * New approach is just to pull ou the tages we are looking for
                 */
                XDocument doc = XDocument.Parse(docContents);

                PanicEvent data = (from item in doc.Descendants("Info")
                                   select new PanicEvent
                                   {
                                       SCI = item.Element("SCI").Value,
                                       SCICode = Convert.ToInt32(item.Element("SCICode").Value)
                                   }).SingleOrDefault();

                if (data != null)
                {
                    data.Device = (from item in doc.Descendants("MetadataHeader")
                                   select item.Element("MetaSourceLocalID").Value).SingleOrDefault();

                    if (OnInovonicsEventRaised != null)
                    {
                        OnInovonicsEventRaised(data);
                    }
                }
            }
            catch (Exception ex)
            {
                //If there is an Error, we just want to move one
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_workerThread != null)
                {
                    if (_workerThread.IsAlive)
                    {
                        _workerThread.Abort();
                        _workerThread = null;
                    }
                }
                
                if (_listenThread.IsAlive)
                {
                    _listenThread.Abort();
                    _listenThread = null;
                }

                if (_credentialCache != null)
                {
                    _credentialCache = null;
                }

                if (_credentials != null)
                {
                    _credentials = null;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.Collect();
        }
    }
}
