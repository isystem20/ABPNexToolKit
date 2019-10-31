using nextoolkit.Models;
using nextoolkit.MVC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nextoolkit.Functions
{
    public class ABPGenerator
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



        private Helpers _helper;

        private AppService _appService;

        private AppServiceDto _appServiceDto;

        private Duplicator _duplicator;
        public ABPGenerator()
        {
            _helper = new Helpers();
            _appService = new AppService();
            _appServiceDto = new AppServiceDto();
            _duplicator = new Duplicator();

        }


        public void Run()
        {
            #region Testings

            #endregion



            var run = "Y";

            do
            {
                var caption = "Make sure that you are running this program in the solution directory together with src/ folder where the projects are located.";
                _helper.displayTextNonArray(caption, ConsoleColor.Black, ConsoleColor.White);

                //Get Project Prefix Name
                project = _helper.getCommand("Project Prefix", true);

                //Initiate Application Folder
                if (project != "")
                {
                    appFolder = $"src\\{project}.Application\\";
                }

                //Get Entity Name or Folder
                var tempEntity = _helper.getCommand("Enter Entity Name", true);

                var auth = _helper.getCommand("Add Authentication to this module ? Y / N(Y)", false, "N").ToUpper();

                //var prefixPermission = "";
                if (auth == "Y")
                {
                    prefixPermission = _helper.getCommand("Permission Prefix", false, "N");
                }

                //Parse the entity string and get file path
                var appDirectory = entityPathParser(tempEntity, appFolder);

                LocateAndParseEntity();

                LocateOptionalReferences();


                //Dto App
                var appPathDto = appPath + "Dto\\";


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
                    appServiceNameSpaces = appServiceNameSpaces,
                    controllerPath = entityPathParser(tempEntity, mvcController)
                };



                //Front End

                var fe = _helper.getCommand("Generate FrontEnd? (MVC,Angular) Just press ENTER to SKIP", false).ToUpper();

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



                _helper.makeDirectory(appPath);
                _helper.makeDirectory(appPathDto);

                _appService.CreateAppService(asm);

                _appService.CreateAppServiceInterface(asm);

                _appServiceDto.CreateAll(asm);

                //Apply Authentication
                createPermission();





                Console.WriteLine("\n\n\nRun again? Y/N");
                run = Console.ReadLine().ToUpper();

            } while (run == "Y");


            //Console.WriteLine("Press any key to exit.");
            //System.Console.ReadKey();

        }



        private static void generateMVC()
        {
            //Check Mvc project exists
            if (project != "")
            {
                mvcProject = $"src\\{project}.Web.Mvc\\";
            }


            //create Controller



            //create Models


            //register pageName


            //create View



            //create ajax

            //Register navigation 




        }



        #region Private functions
        private static string entityPathParser(string entityString, string folder)
        {
            var n = entityString.Split(delim);

            appPath = baseDirectory + folder;

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
                                EntityAttributes.Add(lineMap[1], lineMap[2]);
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
        private void LocateAndParseEntity()
        {
            #region Entities and Attributes
            string[] allFiles = Directory.GetFiles(baseDirectory, newEntity + ".cs", SearchOption.AllDirectories);

            if (allFiles.Length > 1)
            {
                var foundEntities = "";
                for (int i = 0; i < allFiles.Length; i++)
                {
                    foundEntities += $"\n[{i}] {allFiles[i]}";
                }

                var selectedEntity = _helper.getCommand("Multiple Entities Found. Please select which " + foundEntities, true);
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
                        _helper.displayText(error, ConsoleColor.Black, ConsoleColor.Red);
                    }

                }
                else
                {
                    string[] error = { "Input is not integer" };
                    _helper.displayText(error, ConsoleColor.Black, ConsoleColor.Red);
                }

            }
            else if (allFiles.Length == 1)
            {
                getEntityAttributes(allFiles[0]);
            }
            else
            {
                string[] error = { "Entity not found." };
                _helper.displayText(error, ConsoleColor.Black, ConsoleColor.White);
            }

            #endregion
        }
        private void LocateOptionalReferences()
        {
            #region Optional Referencing

            var optionalReferenceString = _helper.getCommand("Additional Optional References separated by comma (,) must include the file extension.", false);

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
                    _helper.displayText(error, ConsoleColor.Black, ConsoleColor.Red);
                }

            }

            #endregion
        }
        private void RunDuplicator()
        {

            var path = _helper.getCommand("Folder to Duplicate", true);

            var newpath = _helper.getCommand("Destination directory", true);

            var strToSearch = _helper.getCommand("String to search", true);

            var strToReplace = _helper.getCommand("String to replace", true);


            var d = new Duplicator();

            d.MapDirectoryAndFiles(path, newpath, strToSearch, strToReplace);

        }
        #endregion


    }
}
