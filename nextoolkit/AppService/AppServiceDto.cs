using nextoolkit.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nextoolkit
{
    public class AppServiceDto
    {
        public void CreateAll(AppServiceModel model)
        {
            CreateEntityDto(model);

            CreateEntityDtoRegister(model);

            CreateEntityDtoModify(model);

            CreateEntityDtoPaged(model);
        }

        public void CreateEntityDto(AppServiceModel model)
        {
            #region EntityDto
            //Make AppService
            var dtoString = Path.Combine(model.appPathDto, model.newEntity + "Dto.cs");
            using (StreamWriter file = new StreamWriter(dtoString, true))
            {
                //Add the Entity refence
                if (model.referencedEntityNameSpace != "")
                {
                    file.WriteLine("using " + model.referencedEntityNameSpace + ";");
                }
                foreach (var ns in model.appServiceNameSpaces)
                {
                    file.WriteLine("using " + ns + ";");
                }
                file.WriteLine("using Abp.Application.Services;");
                file.WriteLine("using Abp.AutoMapper;");
                file.WriteLine("using Abp.Application.Services.Dto;");
                file.WriteLine("using System;");
                file.WriteLine("");
                file.WriteLine("namespace " + model.project + ".Nex" + model.newEntity + ".Dto");
                file.WriteLine("{");
                file.WriteLine("\t[AutoMapFrom(typeof(" + model.newEntity + "))]");
                file.WriteLine("\tpublic class " + model.newEntity + "Dto : EntityDto<int>");
                file.WriteLine("\t{");
                file.WriteLine("");
                foreach (var l in model.importedAttributes)
                {
                    if (!l.Contains("//public"))
                    {
                        file.WriteLine(l);
                    }
                }
                file.WriteLine("");
                file.WriteLine("\t}");
                file.WriteLine("}");
            }
            Console.Write($"\n{model.newEntity}Dto.cs");
            #endregion


        }

        public void CreateEntityDtoRegister(AppServiceModel model)
        {

            #region Create EntityDto
            //Make Create Entity Dto
            var cdtoString = Path.Combine(model.appPathDto, "Create" + model.newEntity + "Dto.cs");
            using (StreamWriter file = new StreamWriter(cdtoString, true))
            {
                //Add the Entity refence
                if (model.referencedEntityNameSpace != "")
                {
                    file.WriteLine("using " + model.referencedEntityNameSpace + ";");
                }
                foreach (var ns in model.appServiceNameSpaces)
                {
                    file.WriteLine("using " + ns + ";");
                }
                file.WriteLine("using Abp.AutoMapper;");
                file.WriteLine("using System.ComponentModel.DataAnnotations;");
                file.WriteLine("using System;");
                file.WriteLine("");
                file.WriteLine("namespace " + model.project + ".Nex" + model.newEntity + ".Dto");
                file.WriteLine("{");
                file.WriteLine("\t[AutoMapTo(typeof(" + model.newEntity + "))]");
                file.WriteLine("\tpublic class Create" + model.newEntity + "Dto");
                file.WriteLine("\t{");
                file.WriteLine("");
                foreach (var l in model.importedAttributes)
                {
                    if (!l.Contains("//public"))
                    {
                        file.WriteLine("\t\t[Required]");
                        file.WriteLine(l);
                    }

                }
                file.WriteLine("");
                file.WriteLine("\t}");
                file.WriteLine("}");
            }
            Console.Write($"\nCreate{model.newEntity}Dto.cs");
            #endregion

        }

        public void CreateEntityDtoModify(AppServiceModel model)
        {
            #region Update EntityDto
            //Make Update EntityDto
            var udtoString = Path.Combine(model.appPathDto, "Update" + model.newEntity + "Dto.cs");
            using (StreamWriter file = new StreamWriter(udtoString, true))
            {
                //Add the Entity refence
                if (model.referencedEntityNameSpace != "")
                {
                    file.WriteLine("using " + model.referencedEntityNameSpace + ";");
                }
                file.WriteLine("using Abp.Application.Services.Dto;");
                file.WriteLine("using System.ComponentModel.DataAnnotations;");
                file.WriteLine("using System;");
                file.WriteLine("using Abp.AutoMapper;");
                file.WriteLine("");
                file.WriteLine("namespace " + model.project + ".Nex" + model.newEntity + ".Dto");
                file.WriteLine("{");
                file.WriteLine("\t[AutoMapTo(typeof(" + model.newEntity + "))]");
                file.WriteLine("\tpublic class Update" + model.newEntity + "Dto : Create" + model.newEntity + "Dto, IEntityDto");
                file.WriteLine("\t{");
                file.WriteLine("");
                file.WriteLine("\t\tpublic int Id { get; set; }");
                file.WriteLine("");
                file.WriteLine("\t}");
                file.WriteLine("}");
            }
            Console.Write($"\nUpdate{model.newEntity}Dto.cs");
            #endregion
        }

        public void CreateEntityDtoPaged(AppServiceModel model)
        {

            #region Paged Entity Dto
            //Make Update EntityDto
            var pdtoString = Path.Combine(model.appPathDto, "Paged" + model.newEntity + "ResultRequestDto.cs");
            using (StreamWriter file = new StreamWriter(pdtoString, true))
            {
                foreach (var ns in model.appServiceNameSpaces)
                {
                    file.WriteLine("using " + ns + ";");
                }
                file.WriteLine("using Abp.Application.Services.Dto;");
                file.WriteLine("");
                file.WriteLine("namespace " + model.project + ".Nex" + model.newEntity + ".Dto");
                file.WriteLine("{");
                file.WriteLine("\tpublic class Paged" + model.newEntity + "ResultRequestDto : PagedResultRequestDto");
                file.WriteLine("\t{");
                file.WriteLine("");
                file.WriteLine("\t\tpublic string Keyword { get; set; }");
                file.WriteLine("\t\tpublic RecordStatus? Status { get; set; }");
                file.WriteLine("");
                file.WriteLine("\t}");
                file.WriteLine("}");
            }
            Console.Write($"\nPaged{model.newEntity}ResultRequestDto.cs");
            #endregion
        }


    }
}
