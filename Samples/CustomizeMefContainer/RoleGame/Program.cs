using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleGame
{
    using RoleGame.Application;

    class Program
    {
        static void Main(string[] args)
        {
            var shell = new RoleGameShell();
            shell.StartAppAsync();

            Console.ReadLine();
        }
    }
}
