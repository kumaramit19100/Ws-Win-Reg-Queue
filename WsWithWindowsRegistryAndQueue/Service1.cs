using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Win32;
using CloudStorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount;
using Microsoft.WindowsAzure.Storage.Queue;

namespace WsWithWindowsRegistryAndQueue
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            timer.Enabled = true;
            msg("Accessing Start at : "+DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"+"\n"));
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Intel\PSIS\PSIS_DECODER");
            var value2 = registryKey.GetValue("GraphFile");
            msg("Value is : " + value2.ToString() + "\n");

            CloudStorageAccount cls = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=akstaccount;AccountKey=gkGSx+sQsRaUtEU9YAl5cxogSchyKpV7qHl8pkJ+rhuA+yiwllI+0C249yF7cRTu6JrAGX5Dr3Uo8i7rGjQqbA==;EndpointSuffix=core.windows.net");
            CloudQueueClient queueClient = cls.CreateCloudQueueClient();
            CloudQueue cloudQueue = queueClient.GetQueueReference("queuewithtimerfun");
            CloudQueueMessage queueMessage = cloudQueue.GetMessage();
            msg(queueMessage.AsString);         
            
            timer.Interval = 10000;
        }

        protected override void OnStop()
        {
            msg("Accessing Stopped at : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
        }

        private void msg(string v)
        {
            string path = @"D:\WindowsService\WsWithWindowsRegistryAndQueue.txt";
            if (!File.Exists(path))
            {
                using(StreamWriter writer = File.CreateText(path))
                {
                    writer.WriteLine(v);
                }
            }
            else
            {
                using (StreamWriter writer=File.AppendText(path)) {
                    writer.WriteLine(v);
                }
            }
        }
    }
}
