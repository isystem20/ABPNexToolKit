using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace nextoolkit.MVC
{
    public class Duplicator
    {
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

                    var newFilePath = offsetPath + file.Replace(path,"");

                    if (newFilePath.Contains(pluralizedStr))
                    {
                        newFilePath = newFilePath.Replace(pluralizedStr, pluralizedToString);
                    }
                    else if (newFilePath.Contains(str))
                    {
                        newFilePath = newFilePath.Replace(str, toReplace);
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
