using nextoolkit.Functions;
using nextoolkit.Models;
using nextoolkit.MVC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace nextoolkit
{
    public class Program
    {

        static void Main(string[] args)
        {

            string[] acceptableStrings = { "exit", "help", "abpgen", "duplicator" };

            string command;

            string caption;

            Helpers _helper = new Helpers();

            do
            {

                Console.Clear();

                command = _helper.getCommand("Command", true).ToLower();

                switch (command)
                {

                    case "help":
                        //Show Help
                        caption = "Welcome to Nex Toolkit";
                        _helper.displayTextNonArray(caption, ConsoleColor.Black, ConsoleColor.White);
                        break;

                    case "exit":
                        Console.ReadKey();
                        Environment.Exit(0);
                        break;

                    case "abpgen":
                        new ABPGenerator().Run();
                        break;

                    case "duplicator":
                        new Duplicator().Run();
                        break;


                }

                Console.ReadKey();


            } while (acceptableStrings.Contains(command));



        }

    }
}
