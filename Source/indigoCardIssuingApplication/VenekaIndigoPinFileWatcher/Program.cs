using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenekaIndigoPinFileWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            FileProcessor processor = new FileProcessor();
            processor.readPinFile("MAILER");
           // FileProcessor processor_two = new FileProcessor();
            processor.readPinFile("LINK");
            Console.ReadKey();
        }
    }
}
