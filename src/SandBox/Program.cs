using CInovonics.Domain.CCure;
using CInovonics.Domain.Inovonics;
using CInovonics.Domain.Events;
using SandBox.Properties;
using System;
using System.Net;

namespace SandBox
{
    class Program
    {
        private static GPIDevice gpi; 
       
        static void Main(string[] args)
        {
            IPAddress inovonticsDevice = null;
            IPAddress ccureDevice = null;

            try
            {
                #if DEBUG
                Console.WriteLine("Connectioning to CCure Device");
                #endif
                IPAddress.TryParse(Settings.Default.CCureIP, out ccureDevice);

                gpi = new GPIDevice(ccureDevice, Settings.Default.CCurePort);
                gpi.OnEventRaised += CCUREReceivedMessage;

                #if DEBUG
                Console.WriteLine("Connecting to Inovontics Device");
                #endif

                IPAddress.TryParse(Settings.Default.InovonicsIP, out inovonticsDevice);

                InovonticsDevice d = new InovonticsDevice(inovonticsDevice, Settings.Default.InovonicsUsr, Settings.Default.InovonicsPwd);
                d.OnEventRaised += ReceivedMessage;

                Console.ReadLine();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void ReceivedMessage(PanicEvent evnt)
        {
            if(evnt != null)
            {
                #if DEBUG
                Console.WriteLine(evnt);
                #endif

                String msg = String.Format("{0} {1}", evnt.Device, evnt.SCICode);
                gpi.QueueMessage(msg);
            }
        }

        static void CCUREReceivedMessage(string msg)
        {
            #if DEBUG
            Console.WriteLine(msg);
            #endif
        }
    }
}
