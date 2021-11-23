using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SendIPV6address
{
    public partial class Service1 : ServiceBase
    {
        System.Timers.Timer createOrderTimer;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            createOrderTimer = new System.Timers.Timer();
            createOrderTimer.Elapsed += new System.Timers.ElapsedEventHandler(Page_Load);
            createOrderTimer.Interval = 20000;
            createOrderTimer.Enabled = true;
            createOrderTimer.AutoReset = true;
            createOrderTimer.Start();
        }

        protected override void OnStop()
        {
        }

        protected void Page_Load(object sender, System.Timers.ElapsedEventArgs args)
        {
            Ping ping = new Ping();
            PingReply pr = ping.Send("bing.com");
            if (pr.Status == IPStatus.Success)
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("sender@qq.com");
                mailMessage.To.Add(new MailAddress("receiver@gmail.com"));
                
                string name = Dns.GetHostName();
                IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
                mailMessage.Subject = name;
                mailMessage.Body = "IPv6:   ";
                foreach (IPAddress ipa in ipadrlist)
                {
                    if (ipa.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        mailMessage.Body += ipa.ToString();
                        mailMessage.Body += "    ";
                    }
                }

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.qq.com";
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                //验证发件人身份(发件人的邮箱，邮箱里的生成授权码);
                client.Credentials = new NetworkCredential("sender7@qq.com", "password");
                client.Send(mailMessage);
                StopWindowsService("SendIPv6adressOnStartup");
            }
        }
        /// <summary>
        /// 开启服务
        /// </summary>
        /// <param name="windowsServiceName">服务名称</param>
        static void StartWindowsService(string windowsServiceName)
        {
            using (System.ServiceProcess.ServiceController control = new System.ServiceProcess.ServiceController(windowsServiceName))
            {
                if (control.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine("服务启动......");
                    control.Start();
                    Console.WriteLine("服务已经启动......");
                }
                else if (control.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                {
                    Console.WriteLine("服务已经启动......");
                }
            }

        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="windowsServiceName">服务名称</param>
        static void StopWindowsService(string windowsServiceName)
        {
            using (System.ServiceProcess.ServiceController control = new System.ServiceProcess.ServiceController(windowsServiceName))
            {
                if (control.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                {
                    Console.WriteLine("服务停止......");
                    control.Stop();
                    Console.WriteLine("服务已经停止......");
                }
                else if (control.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine("服务已经停止......");
                }
            }
        }
    }
}
