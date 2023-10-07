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
        private static readonly Lazy<MyLogger> lazyInstance =
        new Lazy<MyLogger>(() => new MyLogger());
        //Use your own file path
        private const string filePath = @"D:\Dev\C#\DemoAutotestSelenium\NUnitWithSeleniumTestDemo\NUnitWithSeleniumTestDemo\TestLog.txt";
        private static MyLogger instance = null;

        //Constructor
        private MyLogger()
        {

        }
        //Methods
        public static MyLogger Instance
        {
            get
            {
                return lazyInstance.Value;
            }
        }
        public void LogToFile(string text)
        {
            File.AppendAllText(filePath, text + "\n");
        }
    }
}
