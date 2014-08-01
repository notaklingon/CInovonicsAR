using System;
using System.Net;
using System.Xml;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using CInovonics.Domain.Events;

namespace CInovonics.Domain.Inovonics
{
    public class InovonticsDevice
    {
        public delegate void EventRaised(PanicEvent evnt);
        public event EventRaised OnEventRaised;

        private IPAddress _ipAddr;
        private WebRequest _req;
        private NetworkCredential _credentials;
        private CredentialCache credentialCache;
        private BackgroundWorker _listener;
        
        public InovonticsDevice(IPAddress addr, string username, string password)
        {
            _ipAddr = addr;
            _credentials = new NetworkCredential(username, password);
            credentialCache = new CredentialCache();

            _listener = new BackgroundWorker();
            _listener.DoWork += new DoWorkEventHandler(Listen);
            _listener.RunWorkerAsync();
       }

        private void Listen(object sender, DoWorkEventArgs e)
        {
            string url = @"http://" + _ipAddr.ToString() + "/PSIA/Metadata/stream?AreaControlEvents=true";
            credentialCache.Add(new System.Uri(url), "Basic", _credentials);

            _req = WebRequest.Create(url);
            _req.Credentials = credentialCache;
            _req.PreAuthenticate = true;

            HttpWebResponse response = (HttpWebResponse)_req.GetResponse();
            Stream responseStream = response.GetResponseStream();
            XmlTextReader reader = new XmlTextReader(responseStream);

            while (true)
            {
                reader.MoveToContent();
                string contents = reader.ReadOuterXml();

                if (OnEventRaised != null)
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                    worker.RunWorkerAsync(contents);
                }

                reader.ResetState();
            }
        }

        public void Stop()
        {
            _req.Abort();
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

                if(data != null)
                {
                    data.Device = (from item in doc.Descendants("MetadataHeader")
                                   select item.Element("MetaSourceLocalID").Value).SingleOrDefault();

                    if (OnEventRaised != null)
                    {
                        OnEventRaised(data);
                    }
                }
            }catch(Exception ex)
            {
                //If there is an Error, we just want to move one
            }
        }
    }
}
