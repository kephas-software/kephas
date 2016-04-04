using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleModel.App.Console
{
    using SimpleModel.App.Console.Application;

    using Console = System.Console;

    class Program
    {
        static void Main(string[] args)
        {
            var shell = new Shell();
            shell.StartAppAsync();

            Console.ReadLine();
        }
    }
}
