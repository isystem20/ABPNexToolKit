using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluralizationService;
using PluralizationService.English;

namespace nextoolkit
{
    public class Helpers
    {
        public string getCommand(string caption, Boolean isRequired, string defaultValue = "")
        {
            if (isRequired)
            {
                var result = "";
                do
                {
                    Console.Write("\n" + caption + " : ");
                    result = Console.ReadLine();
                } while (result == "");

                return result;

            }
            else
            {
                Console.Write("\n" + caption + " : ");
                var result = Console.ReadLine();
                return result;
            }
        }


        public void makeDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void displayText(string[] text, ConsoleColor bgColor = default, ConsoleColor foreColor = default, bool autoReset = true)
        {
            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = foreColor;
            foreach (var t in text)
            {
                Console.WriteLine($"\n{t}");
            }
            if (autoReset)
            {
                Console.ResetColor();
            }
        }

        public void displayTextNonArray(string text, ConsoleColor bgColor = default, ConsoleColor foreColor = default, bool autoReset = true)
        {
            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = foreColor;

            Console.WriteLine($"\n{text}");
            if (autoReset)
            {
                Console.ResetColor();
            }
        }

    }


    public class NexSearcher
    {
        public static List<string> GetDirectories(string path, string searchPattern = "*",
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (searchOption == SearchOption.TopDirectoryOnly)
                return Directory.GetDirectories(path, searchPattern).ToList();

            var directories = new List<string>(GetDirectories(path, searchPattern));

            for (var i = 0; i < directories.Count; i++)
                directories.AddRange(GetDirectories(directories[i], searchPattern));

            return directories;
        }

        private static List<string> GetDirectories(string path, string searchPattern)
        {
            try
            {
                return Directory.GetDirectories(path, searchPattern).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                return new List<string>();
            }
        }
    }

    //services.AddSingleton<IPluralizer, PluralizationServiceInstance>();
    public class PluralizationServiceInstance //: IPluralizer
    {
        private static readonly IPluralizationApi Api;
        private static readonly CultureInfo CultureInfo;

        static PluralizationServiceInstance()
        {
            var builder = new PluralizationApiBuilder();
            builder.AddEnglishProvider();

            Api = builder.Build();
            CultureInfo = new CultureInfo("en-US");
        }


        public string Pluralize(string name)
        {
            return Api.Pluralize(name, CultureInfo) ?? name;
        }

        public string Singularize(string name)
        {
            return Api.Singularize(name, CultureInfo) ?? name;
        }
    }





}
