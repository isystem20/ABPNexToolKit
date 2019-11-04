using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace nextoolkit.Functions
{

    public class Duplicator
    {

        private Helpers _helper;
        public Duplicator()
        {
            _helper = new Helpers();

        }

        public void Run()
        {
            var path = _helper.getCommand("Folder to Duplicate", true);

            var newpath = _helper.getCommand("Destination directory", true);

            var strToSearch = _helper.getCommand("String to search", true);

            var strToReplace = _helper.getCommand("String to replace", true);

            var enablePluralize = _helper.getCommand("Enable Pluralization? (Y/N)", true).ToUpper();

            bool plural = false;

            if (enablePluralize == "Y")
            {
                plural = true;
            }

            MapDirectoryAndFiles(path, newpath, strToSearch, strToReplace, plural);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(0);
        }

        public void MapDirectoryAndFiles(string path, string newPath, string str, string toReplace, bool pluralize = true)
        {
            var offsetPath = new DirectoryInfo(path).Name;

            var psi = new PluralizationServiceInstance();

            string pluralizedToString = psi.Pluralize(toReplace);
            string pluralizedStr = psi.Pluralize(str);

            if (newPath == "..")
            {
                newPath = path.Replace("\\" + offsetPath,"");
            }

            var allFiles = Directory.GetFiles(path,"*.*", SearchOption.AllDirectories);


            foreach (var file in allFiles)
                {

                    List<string> lines = File.ReadAllLines(file).ToList();

                    List<string> newLines = new List<string>();

                    foreach (var line in lines)
                    {
                        if (pluralize == true)
                        {
                            if (line.Contains(pluralizedStr))
                            {
                                newLines.Add(line.Replace(pluralizedStr, pluralizedToString));
                            }
                            else if (line.Contains(str))
                            {
                                newLines.Add(line.Replace(str, toReplace));
                            }
                            else
                            {
                                newLines.Add(line);
                            }
                        }
                        else
                        {
                            if (line.Contains(str))
                            {
                                newLines.Add(line.Replace(str, toReplace));
                            }
                            else
                            {
                                newLines.Add(line);
                            }
                        }
                        
                    }

                    var newFilePath = offsetPath + file.Replace(path,"");

                    if (pluralize == true)
                    {
                        if (newFilePath.Contains(pluralizedStr))
                        {
                            newFilePath = newFilePath.Replace(pluralizedStr, pluralizedToString);
                        }
                        else if (newFilePath.Contains(str))
                        {
                            newFilePath = newFilePath.Replace(str, toReplace);
                        }

                    }
                    else
                    {
                        if (newFilePath.Contains(str))
                        {
                            newFilePath = newFilePath.Replace(str, toReplace);
                        }
                    }


                    newFilePath = newPath + "\\" + newFilePath;

                    var newFileDir = Path.GetDirectoryName(newFilePath);

                    if (!Directory.Exists(newFileDir))
                    {
                        Directory.CreateDirectory(newFileDir);
                    }


                    if (!File.Exists(newFilePath))
                    {
                        File.Create(newFilePath).Dispose();

                        using (StreamWriter f = new StreamWriter(newFilePath))
                        {
                            foreach (var newline in newLines)
                            {
                                f.WriteLine(newline);
                            }
                        }

                    }
                    else if (File.Exists(newFilePath))
                    {
                        using (StreamWriter f = new StreamWriter(newFilePath))
                        {
                            foreach (var newline in newLines)
                            {
                                f.WriteLine(newline);
                            }
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"New File Created: {newFilePath}");
                }

        }




    }
}
