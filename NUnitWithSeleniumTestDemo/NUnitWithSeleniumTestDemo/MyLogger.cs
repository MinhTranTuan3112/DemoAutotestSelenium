using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitWithSeleniumTestDemo
{
    public class MyLogger
    {
        //Fields
        private const string filePath = @"D:\Dev\C#\NUnitWithSeleniumTestDemo\NUnitWithSeleniumTestDemo\TestLog.txt";
        private static MyLogger instance = null;
        private static readonly object instanceLock = new object();

        //Constructor
        private MyLogger()
        {

        }

        //Methods
        public static MyLogger Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance is null)
                    {
                        instance = new MyLogger();
                    }
                    return instance;
                }
            }
        }
        public void LogToFile(string text)
        {
            File.AppendAllText(filePath, text + "\n");
        }
    }
}
