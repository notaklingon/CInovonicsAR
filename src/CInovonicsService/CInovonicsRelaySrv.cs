using CInovonics.Domain.CCure;
using CInovonics.Domain.Inovonics;
using CInovonics.Domain.Events;
using CInovonicsService.Properties;
using System;
using System.Net;
using System.ServiceProcess;
using System.Threading;

namespace CInovonicsService
{
    public partial class CInovonicsRelaySrv : ServiceBase
    {

        private Thread _thread;
        private GPIDevice _gpi;
        private InovonticsDevice _invDevice;
        public CInovonicsRelaySrv()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _thread = new Thread(thread_worker);
            _thread.Start();
        }

        private void thread_worker()
        {
            IPAddress _inovonicsDevice = null;
            IPAddress _ccureDevice = null;

            IPAddress.TryParse(Settings.Default.InovonicsIP, out _inovonicsDevice);
            IPAddress.TryParse(Settings.Default.CCureIP, out _ccureDevice);

            _gpi = new GPIDevice(_ccureDevice, Settings.Default.CCurePort);
            _gpi.OnEventRaised += CCUREReceivedMessage;

            _invDevice = new InovonticsDevice(_inovonicsDevice, Settings.Default.InovonicsUser, Settings.Default.InovonicsPasswd);
            _invDevice.OnEventRaised += ReceivedMessage;
        }

        protected override void OnStop()
        {
            _thread.Abort();
            base.OnStop();
        }

        private void CCUREReceivedMessage(string msg)
        {
            //We don't have two way communiciation so just swallow the messsage
        }

        private void ReceivedMessage(PanicEvent evnt)
        {
            if (evnt != null)
            {
                String msg = String.Format("{0} {1}", evnt.Device, evnt.SCICode);
                _gpi.QueueMessage(msg);
            }
        }
    }
}
