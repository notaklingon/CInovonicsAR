using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CInovonics.Domain.CCure;
using CInovonics.Domain.Inovonics;
using CInovonics.Domain.LaxCo;
using System.Net;
using System.Threading;
using System.Collections.Specialized;
using SandboxWindows.Properties;

namespace SandboxWindows
{
    public partial class Form1 : Form
    {
        private Thread _thread;
        private GPIDevice _gpi;
        private InovonticsDevice _invDevice;
        private List<Thread> _ThreadList;
        private List<GPIDevice> _gpiList;
        private List<InovonticsDevice> _inovonicsList;
        delegate void SetTextCallback(string text);

        public Form1()
        {
            InitializeComponent();
        }

        private void UpdateListBox(string text)
        {
            if (this.listBoxMessage.InvokeRequired)
            {
                SetTextCallback d=new SetTextCallback(UpdateListBox);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.listBoxMessage.Items.Add(text);
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            UpdateListBox("Starting Relay Service...");
            _ThreadList=new List<Thread>();
            _gpiList = new List<GPIDevice>();
            _inovonicsList=new List<InovonticsDevice>();

            string colIP =Settings.Default.InovonicsIp;
            string ipCCure = Settings.Default.RelayIp;
            string colPort = Settings.Default.RelayPort;

            ThreadParam tp = new ThreadParam();
            tp.ipCCure = ipCCure;
            tp.ipInovonics = colIP;
            tp.portCCure = colPort;
                    
            _thread = new Thread(new ParameterizedThreadStart(thread_worker));
            _ThreadList.Add(_thread);
            _thread.Start(tp);            
        }

        private void thread_worker(object threadParam)
        {
            ThreadParam tp = (ThreadParam)threadParam;

            IPAddress _inovonicsDevice = null;
            IPAddress _ccureDevice = null;

            IPAddress.TryParse(tp.ipInovonics, out _inovonicsDevice);
            IPAddress.TryParse(tp.ipCCure, out _ccureDevice);

            _gpi = new GPIDevice(_ccureDevice, int.Parse(tp.portCCure));
            _gpi.OnCCureEventRaised += ReceivedMessageCCURE;
            _gpiList.Add(_gpi);

            _invDevice = new InovonticsDevice(_inovonicsDevice, Settings.Default.InovonicsUser, Settings.Default.InovonicsPassword);
            _invDevice.OnInovonicsEventRaised += ReceivedMessageInovonics;
            _inovonicsList.Add(_invDevice);
        }

        private void ReceivedMessageCCURE(string msg)
        {
            UpdateListBox("CCure: " + msg);
        }

        private void ReceivedMessageInovonics(PanicEvent evnt)
        {
            if (evnt != null)
            {
                string newCode = "0";

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
                UpdateListBox(msg);
            }
        }

        public class ThreadParam
        {
            public string ipInovonics { get; set; }
            public string ipCCure { get; set; }
            public string portCCure { get; set; }
        }

        private void buttonStop_Click(object sender, EventArgs e)
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
        }
    }
}
