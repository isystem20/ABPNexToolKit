using nextoolkit.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nextoolkit
{
    public class AppService
    {
        public void CreateAppService(AppServiceModel model)
        {
            #region Create App Service
            //Make AppService
            var pathString = Path.Combine(model.appPath, model.newEntity + "AppService.cs");
            using (StreamWriter file = new StreamWriter(pathString, true))
            {
                //Add the Entity refence
                if (model.referencedEntityNameSpace != "")
                {
                    file.WriteLine("using " + model.referencedEntityNameSpace + ";");
                }
                file.WriteLine("using Abp.Application.Services;");
                file.WriteLine("using Abp.Application.Services.Dto;");
                file.WriteLine("using Abp.Authorization;");
                file.WriteLine("using " + model.project + ".Authorization;");
                file.WriteLine("using Abp.Collections.Extensions;");
                file.WriteLine("using Abp.Domain.Repositories;");
                file.WriteLine("using Abp.Extensions;");
                file.WriteLine("using " + model.project + ".Nex" + model.newEntity + ".Dto;");
                file.WriteLine("using System;");
                file.WriteLine("using System.Collections.Generic;");
                file.WriteLine("using System.Linq;");
                file.WriteLine("using System.Text;");
                file.WriteLine("using System.Threading.Tasks;");
                file.WriteLine("using Abp.Linq.Extensions;");
                file.WriteLine("");
                file.WriteLine("namespace " + model.project + ".Nex" + model.newEntity);
                file.WriteLine("{");

                //Apply Authentication if desired
                if (model.referencedEntityNameSpace != "")
                {
                    file.WriteLine("\t//[AbpAuthorize(\"" + model.prefixPermission + model.newEntity + "\")]");
                }

                file.WriteLine("\tpublic class " + model.newEntity + "AppService : AsyncCrudAppService<" + model.newEntity + ", " + model.newEntity + "Dto, int, Paged" + model.newEntity + "ResultRequestDto, Create" + model.newEntity + "Dto, Update" + model.newEntity + "Dto>, I" + model.newEntity + "AppService");
                file.WriteLine("\t{");
                file.WriteLine("");
                file.WriteLine("\t\tprivate readonly IRepository<" + model.newEntity + "> _r" + model.newEntity + ";");
                file.WriteLine("");
                file.WriteLine("\t\tpublic " + model.newEntity + "AppService(IRepository<" + model.newEntity + "> r" + model.newEntity + ") : base(r" + model.newEntity + ")");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\t_r" + model.newEntity + " = r" + model.newEntity + ";");
                file.WriteLine("\t\t}");
                file.WriteLine("");
                //Apply Authentication for READ
                if (model.referencedEntityNameSpace != "")
                {
                    file.WriteLine("\t\t[AbpAuthorize(\"" + model.prefixPermission + model.newEntity + ".Read\")]");
                }
                file.WriteLine("\t\tprotected override IQueryable<" + model.newEntity + "> CreateFilteredQuery(Paged" + model.newEntity + "ResultRequestDto input)");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\treturn Repository.GetAll()");
                file.WriteLine("\t\t\t\t.WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword))");
                file.WriteLine("\t\t\t\t.WhereIf(input.Status.HasValue, x => x.Status == input.Status);");
                file.WriteLine("\t\t}");

                file.WriteLine("");
                //Apply Authentication for CREATE
                if (model.referencedEntityNameSpace != "")
                {
                    file.WriteLine("\t\t[AbpAuthorize(\"" + model.prefixPermission + model.newEntity + ".Create\")]");
                }
                file.WriteLine("\t\tpublic override async Task<" + model.newEntity + "Dto> Create(Create" + model.newEntity + "Dto input)");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\tvar entity = MapToEntity(input);");
                file.WriteLine("");
                file.WriteLine("\t\t\tawait Repository.InsertAsync(entity);");
                file.WriteLine("\t\t\tawait CurrentUnitOfWork.SaveChangesAsync();");
                file.WriteLine("");
                file.WriteLine("\t\t\treturn MapToEntityDto(entity);");
                file.WriteLine("\t\t}");
                file.WriteLine("");

                //Apply Authentication for UPDATE
                if (model.referencedEntityNameSpace != "")
                {
                    file.WriteLine("\t\t[AbpAuthorize(\"" + model.prefixPermission + model.newEntity + ".Update\")]");
                }
                file.WriteLine("\t\tpublic override async Task<" + model.newEntity + "Dto> Update(Update" + model.newEntity + "Dto input)");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\tvar entity = await GetEntityByIdAsync(input.Id);");
                file.WriteLine("");
                file.WriteLine("\t\t\tMapToEntity(input, entity);");
                file.WriteLine("\t\t\tawait CurrentUnitOfWork.SaveChangesAsync();");
                file.WriteLine("");
                file.WriteLine("\t\t\treturn MapToEntityDto(entity);");
                file.WriteLine("\t\t}");

                //Apply Authentication for DELETE
                if (model.referencedEntityNameSpace != "")
                {
                    file.WriteLine("\t\t[AbpAuthorize(\"" + model.prefixPermission + model.newEntity + ".Delete\")]");
                }
                file.WriteLine("\t\tpublic override async Task Delete(EntityDto<int> input)");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\tvar record = await _r" + model.newEntity + ".FirstOrDefaultAsync(input.Id);");
                file.WriteLine("");
                file.WriteLine("\t\t\tawait _r" + model.newEntity + ".DeleteAsync(record);");
                file.WriteLine("\t\t}");

                //Apply Authentication for READ SINGLE
                if (model.referencedEntityNameSpace != "")
                {
                    file.WriteLine("\t\t[AbpAuthorize(\"" + model.prefixPermission + model.newEntity + ".Read\")]");
                }
                file.WriteLine("\t\tprotected override async Task<" + model.newEntity + "> GetEntityByIdAsync(int id)");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\treturn await Repository.GetAsync(id);");
                file.WriteLine("\t\t}");

                file.WriteLine("");
                file.WriteLine("\t}");
                file.WriteLine("}");
            }
            Console.Write($"\nFile Generated:\n{model.newEntity}AppService.cs");
            #endregion
        }
        public void CreateAppServiceInterface(AppServiceModel model)
        {
            #region App Service Interface
            //Make Update EntityDto
            var aidtoString = Path.Combine(model.appPath, "I" + model.newEntity + "AppService.cs");
            using (StreamWriter file = new StreamWriter(aidtoString, true))
            {
                file.WriteLine("using Abp.Application.Services;");
                file.WriteLine("using " + model.project + ".Nex" + model.newEntity + ".Dto;");
                file.WriteLine("");
                file.WriteLine("namespace " + model.project + ".Nex" + model.newEntity + "");
                file.WriteLine("{");
                file.WriteLine("\tpublic interface I" + model.newEntity + "AppService : IAsyncCrudAppService<" + model.newEntity + "Dto, int, Paged" + model.newEntity + "ResultRequestDto, Create" + model.newEntity + "Dto, Update" + model.newEntity + "Dto>");
                file.WriteLine("\t{");
                file.WriteLine("");
                file.WriteLine("\t\t//Put something if applicable");
                file.WriteLine("");
                file.WriteLine("\t}");
                file.WriteLine("}");
            }
            Console.Write($"\nI" + model.newEntity + "AppService.cs");
            #endregion
        }

    }
}
