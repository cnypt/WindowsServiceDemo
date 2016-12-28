using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WindowsService1
{
    public partial class MainService : ServiceBase
    {
        readonly Timer _timer;
        private static readonly string FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + "timetask.txt";
        public MainService()
        {
            InitializeComponent();
            _timer = new Timer(1000 * 10)
            {
                AutoReset = true,
                Enabled = true
            };
            _timer.Elapsed += delegate (object sender, ElapsedEventArgs e)
              {
                  WriteLog.write(string.Format("Run DateTime {0}", DateTime.Now));
                  string sql = string.Format("insert into WriteLog(LogTime,LogMessage) values ('{0}','{1}')", DateTime.Now.ToString(), "测试");
                  SqlHelper.ExecuteSql(sql, CommandType.Text);
              };

           
        }

        protected override void OnStart(string[] args)
        {
            WriteLog.write(string.Format("Start DateTime {0}", DateTime.Now));
        }

        protected override void OnStop()
        {
            WriteLog.write(string.Format("End DateTime {0}", DateTime.Now));
        }
    }
}
