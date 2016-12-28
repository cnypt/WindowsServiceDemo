using System.IO;
using System.Reflection;

namespace WindowsService1
{
    public static class WriteLog
    {
        private static readonly string FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + "timetask.txt";
        public static void write(string content)
        {
            StreamWriter sw = File.AppendText(FileName);
            sw.WriteLine(content);
            sw.Flush();
            sw.Close();
        }
    }
}
