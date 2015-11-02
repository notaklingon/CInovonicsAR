using CInovonics.Domain.CCure;
using CInovonics.Domain.Inovonics;
using CInovonics.Domain.LaxCo;
using CInovonicsARService.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.ServiceProcess;
using System.Threading;

namespace CInovonicsARService
{
    public partial class CInovonicsARSrv : ServiceBase
    {
        private Thread _thread;
        private GPIDevice _gpi;
        private InovonticsDevice _invDevice;
        private List<Thread> _ThreadList;
        private List<GPIDevice> _gpiList;
        private List<InovonticsDevice> _inovonicsList;

        public CInovonicsARSrv()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _ThreadList = new List<Thread>();
            _gpiList = new List<GPIDevice>();
            _inovonicsList = new List<InovonticsDevice>();

            string inovIp = Settings.Default.InovonicsIp;
            string ipCCure = Settings.Default.RelayIp;
            int colPort = Settings.Default.RelayPort;

            ThreadParam tp = new ThreadParam();
            tp.ipCCure = ipCCure;
            tp.ipInovonics = inovIp;
            tp.portCCure = colPort.ToString();

            _thread = new Thread(new ParameterizedThreadStart(thread_worker));
            _ThreadList.Add(_thread);
            _thread.Start(tp);            
        }

        protected override void OnStop()
        {
            foreach (Thread t in _ThreadList)
            {
                t.Abort();
            }

            foreach (GPIDevice g in _gpiList)
            {
                g.Dispose();
            }

            foreach (InovonticsDevice i in _inovonicsList)
            {
                i.Dispose();
            }

            _thread.Abort();
            base.OnStop();
        }

        private void thread_worker(object threadParam)
        {
            ThreadParam tp = (ThreadParam)threadParam;

            IPAddress _inovonicsDevice = null;
            IPAddress _ccureDevice = null;

            IPAddress.TryParse(tp.ipInovonics, out _inovonicsDevice);
            IPAddress.TryParse(tp.ipCCure, out _ccureDevice);

            _gpi = new GPIDevice(_ccureDevice, int.Parse(tp.portCCure));
            _gpi.OnCCureEventRaised += ReceivedMessageCCure;
            _gpiList.Add(_gpi);

            _invDevice = new InovonticsDevice(_inovonicsDevice, Settings.Default.InovonicsUser, Settings.Default.InovonicsPassword);
            _invDevice.OnInovonicsEventRaised += ReceivedMessageInovonics;
            _inovonicsList.Add(_invDevice);
        }

        private void ReceivedMessageCCure(string msg)
        {
            //We don't have two way communiciation so just swallow the messsage
            if (msg != null)
            {
                _gpi.QueueMessage(msg);
            }
        }

        private void ReceivedMessageInovonics(PanicEvent evnt)
        {
            if (evnt != null)
            {
                string newCode="0";

                switch (evnt.SCICode)
                {
                    
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        newCode = evnt.SCICode.ToString();
                        break;
                    case 10:
                        newCode = "A";
                        break;
                    case 11:
                        newCode = "B";
                        break;
                    case 12:
                        newCode = "C";
                        break;
                    case 13:
                        newCode = "D";
                        break;
                    case 14:
                        newCode = "E";
                        break;
                    case 15:
                        newCode = "F";
                        break;
                    case 16:
                        newCode = "G";
                        break;
                    case 17:
                        newCode = "H";
                        break;
                    case 18:
                        newCode = "I";
                        break;
                    case 21:
                        newCode = "J";
                        break;
                    case 125:
                        newCode = "0";
                        break;
                    default:
                        newCode = evnt.SCICode.ToString();
                        break;
                }

                String msg = String.Format("{0} {1}", evnt.Device, newCode);
                _gpi.QueueMessage(msg);
            }
        }
        
        public class ThreadParam
        {
            public string ipInovonics { get; set; }
            public string ipCCure { get; set; }
            public string portCCure { get; set; }
        }
    }
}
