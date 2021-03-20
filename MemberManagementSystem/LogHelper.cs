using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MemberManagementSystem
{
    class LogHelper
    {
        public static void WriteToLog(string logMessage, string path = null, string name = null)
        {
            if(name== null) 
            {
                name = "log.txt";
            }
            
            if(path == null) 
            {
                path = "";
            }

            using (StreamWriter w = File.AppendText(path + name))
            {
                w.Write("\r\nLog Entry : ");
                w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                w.WriteLine("  :");
                w.WriteLine($"  :{logMessage}");
                w.WriteLine("-------------------------------");
            }
        }
        public static void WriteToLogWithInfos(string logMessage,string className,string methodName, string path = null, string name = null)
        {
            if(name== null) 
            {
                name = "log.txt";
            }
            
            if(path == null) 
            {
                path = "";
            }

            using (StreamWriter w = File.AppendText(path + name))
            {
                w.Write("\r\nLog Entry : ");
                w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                w.WriteLine("  :");
                w.WriteLine("Class : " + className);
                w.WriteLine("Method : " + methodName);
                w.WriteLine($"  :{logMessage}");
                w.WriteLine("-------------------------------");
            }
        }

        public static void DumpLog(string path = null, string name = null)
        {
            if (String.IsNullOrEmpty(name))
            {
                name = "log.txt";
            }

            if (String.IsNullOrEmpty(path))
            {
                path = "";
            }

            var test = path + name;

            using (StreamReader r = File.OpenText(path + name))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
