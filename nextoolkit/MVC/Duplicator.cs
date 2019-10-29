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
            List<string> olddirectories = NexSearcher.GetDirectories(path);

            List<string> newDirectories = new List<string>();

            var psi = new PluralizationServiceInstance();

            string pluralizedToString = psi.Pluralize(toReplace);
            string pluralizedStr = psi.Pluralize(str);

            foreach (var item in olddirectories)
            {
                var files = Directory.GetFiles(item);

                foreach (var file in files)
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





                }

                Console.WriteLine();

                //var n = item.Replace(path, newPath);
                //newDirectories.Add(n);
                //Console.WriteLine(n);
            }



        }




    }
}
