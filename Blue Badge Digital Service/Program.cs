using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue_Badge_Digital_Service
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BBDSManager manager = new BBDSManager();
            manager.getoAuthToken();
            manager.GetNewApplications();
            Console.ReadKey();
        }
    }
}