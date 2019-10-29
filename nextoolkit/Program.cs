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

        public static string project; // project name
        public static char[] delim = { '/', '.', '\\' };
        public static string newEntity;
        public static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;


        public static IDictionary<string, string> EntityAttributes = new Dictionary<string, string>();

        public static string[] importedAttributes = { };
        public static string attributeIndicator = "{ get; set; }";
        public static string namespaceIndicator = "namespace";
        public static string[] attributeDataTypes = { "string", "int", "boolean", "datetime", "datetime?", "long", "float", "byte[]", "double", "bool" };

        //NameSpaces
        public static string referencedEntityNameSpace;
        public static List<string> optionalNameSpaces = new List<string> { };
        public static string[] appServiceNameSpaces = { };

        //AppService
        public static string appFolder = $"src\\Application\\";
        public static string appPath; //AppService Folder
        public static string appPathDto;
        public static string prefixPermission;

        //MVC
        public static string mvcProject = $"src\\Web.Mvc\\";
        public static string mvcController = $"src\\Web.Mvc\\Controllers\\";
        public static string mvcModels = $"src\\Web.Mvc\\Models\\";
        public static string mvcViews = $"src\\Web.Mvc\\Views\\";

        static void Main(string[] args)
        {

            #region Testings

            var path = getCommand("Directory to Duplicate ", true);

            var newpath = getCommand("New Directory ", true);

            var strToSearch = getCommand("String to search", true);

            var strToReplace = getCommand("String to replace", true);



            var d = new Duplicator();

            d.MapDirectoryAndFiles(path,newpath, strToSearch, strToReplace);


            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
            Environment.Exit(0);
            #endregion






            var run = "Y";

            do
            {
                Console.Clear();
                Console.WriteLine("This console program is intended to generate ABP Modules.");

                //Get Project Prefix Name
                project = getCommand("Project Prefix",true);

                //Initiate Application Folder
                if (project != "")
                {
                    appFolder = $"src\\{project}.Application\\";
                }

                //Get Entity Name or Folder
                var tempEntity = getCommand("Enter Entity Name", true);


                //Parse the entity string and get file path
                var appDirectory = entityPathParser(tempEntity);

                //Dto App
                var appPathDto = appPath + "Dto\\";

                makeDirectory(appPath);
                makeDirectory(appPathDto);

                var auth = getCommand("Add Authentication to this module ? Y / N(Y)",false,"N").ToUpper();

                //var prefixPermission = "";

                if (auth == "Y")
                {
                    prefixPermission = getCommand("Permission Prefix", false, "N");
                }

                LocateAndParseEntity();

                LocateOptionalReferences();

                var asm = new AppServiceModel
                {
                    appPath = appPath,
                    newEntity = newEntity,
                    referencedEntityNameSpace = referencedEntityNameSpace,
                    project = project,
                    prefixPermission = prefixPermission,
                    appPathDto = appPathDto,
                    optionalNameSpaces = optionalNameSpaces,
                    importedAttributes = importedAttributes,
                    appServiceNameSpaces = appServiceNameSpaces
                };

                var _appService = new AppService();

                var _appServiceDto = new AppServiceDto();

                _appService.CreateAppService(asm);

                _appService.CreateAppServiceInterface(asm);

                _appServiceDto.CreateAll(asm);


                //Apply Authentication
                createPermission();

                var fe = "";

                Console.Write("\nGenerate FrontEnd? (MVC,Angular) Just press ENTER to SKIP");
                fe = Console.ReadLine().ToUpper();

                switch (fe)
                {
                    case "MVC":
                        Console.WriteLine("Case 1");
                        break;
                    case "ANGULAR":
                        Console.WriteLine("Case 2");
                        break;
                    default:
                        Console.WriteLine("Skip?");
                        break;
                }

                Console.WriteLine("\n\n\nRun again? Y/N");
                run = Console.ReadLine().ToUpper();

            } while (run == "Y");


            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();


        }

        private static string entityPathParser(string entityString)
        {
            var n = entityString.Split(delim);

            appPath = baseDirectory + appFolder;

            if (n.Length > 1)
            {
                var e = n.Length - 1;

                for (int i = 0; i < n.Length - 1; i++)
                {
                    appPath = appPath + n[i] + '\\';
                }
                newEntity = n[e];

            }
            else
            {
                newEntity = entityString;
            }

            appPath = appPath + newEntity + "\\";

            return appPath;

        }
        private static string getCommand(string caption, Boolean isRequired, string defaultValue = "")
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
        private static void getEntityAttributes(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (line.Contains(attributeIndicator))
                {
                    if (!line.Contains("ICollection") || !line.Contains($"//public"))
                    {
                        var lineMap = line.Split(' ');
                        if (lineMap[0] == "public")
                        {
                            if (attributeDataTypes.Any(x => x == lineMap[1]))
                            {
                                EntityAttributes.Add(lineMap[1],lineMap[2]);
                            }
                            else
                            {
                                EntityAttributes.Add("int", lineMap[2]);
                            }
                            
                        }
                        Array.Resize(ref importedAttributes, importedAttributes.Length + 1);
                        importedAttributes[importedAttributes.Length - 1] = line;
                    }

                }

                if (line.Contains(namespaceIndicator))
                {
                    referencedEntityNameSpace = line.Replace(namespaceIndicator + " ", "");
                }

            }
        }
        private static void makeDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        private static List<string> getEntityAttributes()
        {



            return null;
        }
        private static void createPermission()
        {
            if (referencedEntityNameSpace != "")
            {
                string[] permissionProvider = Directory.GetFiles(baseDirectory, project + "AuthorizationProvider.cs", SearchOption.AllDirectories);

                if (permissionProvider.Count() > 0)
                {
                    Console.Write("Auth Provider Found:" + permissionProvider[0]);
                    string[] lines = File.ReadAllLines(permissionProvider[0]);
                    var newContent = new List<string> { };
                    int lastline = 0;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("CreatePermission") || lines[i].Contains("CreateChildPermission"))
                        {
                            lastline = i;
                        }
                    }

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i == lastline)
                        {
                            newContent.Add(lines[i]);
                            newContent.Add("");
                            newContent.Add("\t\t\tcontext.CreatePermission(\"" + prefixPermission + newEntity + "\",L(\"" + prefixPermission + " " + newEntity + "\"))");
                            newContent.Add("\t\t\t\t.CreateChildPermission(\"" + prefixPermission + newEntity + ".Create\",L(\"" + prefixPermission + " Create " + newEntity + "\"))");
                            newContent.Add("\t\t\t\t.CreateChildPermission(\"" + prefixPermission + newEntity + ".Read\",L(\"" + prefixPermission + " Read " + newEntity + "\"))");
                            newContent.Add("\t\t\t\t.CreateChildPermission(\"" + prefixPermission + newEntity + ".Update\",L(\"" + prefixPermission + " Update " + newEntity + "\"))");
                            newContent.Add("\t\t\t\t.CreateChildPermission(\"" + prefixPermission + newEntity + ".Delete\",L(\"" + prefixPermission + " Delete " + newEntity + "\"));");
                        }
                        else
                        {
                            newContent.Add(lines[i]);
                        }
                    }
                    File.WriteAllText(permissionProvider[0], String.Empty);
                    using (StreamWriter file = new StreamWriter(permissionProvider[0], true))
                    {
                        foreach (var line in newContent)
                        {
                            file.WriteLine(line);
                        }
                    }
                }
                //file.WriteLine("\t[AbpAuthorize(PermissionNames.Pages_" + newEntity + "CRUD)]");
            }
        }
        private static void generateMVC()
        {
            //Check Mvc project exists
            if (project != "")
            {
                mvcProject = $"src\\{project}.Web.Mvc\\";
            }


            //create Controller




            //create Model



            //register pageName


            //create View



            //create ajax






        }
        private static void displayText(string[] text, ConsoleColor bgColor, ConsoleColor foreColor, bool autoReset = true)
        {
            Console.BackgroundColor = bgColor;
            Console.ForegroundColor = foreColor;
            foreach (var t in text)
            {
                Console.Write($"\n{t}");
            }
            if (autoReset)
            {
                Console.ResetColor();
            }
        }
        private static void LocateAndParseEntity()
        {
            #region Entities and Attributes
            string[] allFiles = Directory.GetFiles(baseDirectory, newEntity + ".cs", SearchOption.AllDirectories);

            if (allFiles.Count() > 1)
            {
                var foundEntities = "";
                for (int i = 0; i < allFiles.Length; i++)
                {
                    foundEntities += $"\n[{i}] {allFiles[i]}";
                }

                var selectedEntity = getCommand("Multiple Entities Found. Please select which " + foundEntities, true);
                int parsedSelection;
                if (int.TryParse(selectedEntity, out parsedSelection))
                {
                    if (parsedSelection <= allFiles.Length - 1 && parsedSelection >= 0)
                    {
                        getEntityAttributes(allFiles[parsedSelection]);
                    }
                    else
                    {
                        string[] error = { "Input is out of range." };
                        displayText(error, ConsoleColor.Black, ConsoleColor.Red);
                    }

                }
                else
                {
                    string[] error = { "Input is not integer" };
                    displayText(error, ConsoleColor.Black, ConsoleColor.Red);
                }

            }
            else if (allFiles.Count() == 1)
            {
                getEntityAttributes(allFiles[0]);
            }
            else
            {
                string[] error = { "Entity not found." };
                displayText(error, ConsoleColor.Black, ConsoleColor.White);
            }

            #endregion
        }
        private static void LocateOptionalReferences()
        {
            #region Optional Referencing

            var optionalReferenceString = getCommand("Additional Optional References separated by comma (,) must include the file extension.", false);

            if (optionalReferenceString != "")
            {
                optionalNameSpaces = optionalReferenceString.Split(',').ToList();
                var searched = Directory.GetFiles(baseDirectory, "*.cs", SearchOption.AllDirectories);
                var refIndex = searched.Where(f => optionalNameSpaces.IndexOf(Path.GetFileName(f)) >= 0).ToArray();
                if (refIndex.Length > 0)
                {
                    foreach (var file in refIndex)
                    {
                        Console.WriteLine($"Found: {file}");
                        string[] lines = File.ReadAllLines(file);
                        foreach (var line in lines)
                        {
                            if (line.Contains(namespaceIndicator))
                            {
                                Array.Resize(ref appServiceNameSpaces, appServiceNameSpaces.Length + 1);
                                appServiceNameSpaces[appServiceNameSpaces.Length - 1] = line.Replace(namespaceIndicator + " ", "");
                            }
                        }
                    }
                }
                else
                {
                    string[] error = { "Optional References not found" };
                    displayText(error, ConsoleColor.Black, ConsoleColor.Red);
                }

            }

            #endregion
        }
    }
}
