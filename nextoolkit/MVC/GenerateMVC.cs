using nextoolkit.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace nextoolkit.MVC
{
    public class GenerateMVC
    {

        //private PluralizationServiceInstance psi;

        //public GenerateMVC()
        //{
        //    psi = new PluralizationServiceInstance();
        //}

        public void Run(AppServiceModel model)
        {
            //AppServiceModel model = new AppServiceModel();
            CreateController(model);
            CreateViews(model);
            CreateModels(model);
            AddToNavigation(model);
            AddToPageNames(model);
            CreateJQueryDuplicator(model);
        }

        public void CreateController(AppServiceModel model)
        {
            #region Create Controller
            //Make AppService

            Console.WriteLine($"CreateController Path: {model.controllerPath}");

            new Helpers().makeDirectory(model.controllerPath);

            var pathString = Path.Combine(model.controllerPath, model.newEntityPlural + "Controller.cs");
            using (StreamWriter file = new StreamWriter(pathString, true))
            {
                //Add the optional references
                foreach (var ns in model.appServiceNameSpaces)
                {
                    file.WriteLine("using " + ns + ";");
                }
                //Add the Entity refence
                if (model.referencedEntityNameSpace != "")
                {
                    file.WriteLine("using " + model.referencedEntityNameSpace + ";");
                }
                file.WriteLine("using Abp.Application.Services.Dto;");
                file.WriteLine("using " + model.project + ".Controllers;");
                file.WriteLine("using " + model.project + ".Nex" + model.newEntity + ";");
                file.WriteLine("using " + model.project + ".Nex" + model.newEntity + ".Dto;");
                file.WriteLine("using " + model.project + ".Web.Models." + model.newEntityPlural + ";");
                file.WriteLine("using Microsoft.AspNetCore.Mvc;");
                file.WriteLine("using Microsoft.AspNetCore.Mvc.Rendering;");

                file.WriteLine("using System;");
                file.WriteLine("using System.Collections.Generic;");
                file.WriteLine("using System.Threading.Tasks;");
                file.WriteLine("");
                file.WriteLine("namespace " + model.project + ".Web.Controllers");
                file.WriteLine("{");

                file.WriteLine("\tpublic class " + model.newEntityPlural + "Controller : ConsoleManagerControllerBase");
                file.WriteLine("\t{");
                file.WriteLine("");
                file.WriteLine("\t\tprivate readonly I" + model.newEntity + "AppService _" + model.newEntity.ToLower() + "AppService;");
                file.WriteLine("");
                file.WriteLine("\t\tpublic " + model.newEntityPlural + "Controller(I" + model.newEntity + "AppService " + model.newEntity.ToLower() + "AppService)");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\t_" + model.newEntity.ToLower() + "AppService = " + model.newEntity.ToLower() + "AppService;");
                file.WriteLine("\t\t}");
                file.WriteLine("");

                //Index
                file.WriteLine("\t\tpublic async Task<ActionResult> Index()");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\tvar " + model.newEntity.ToLower() + " = (await _" + model.newEntity.ToLower() + "AppService.GetAll(new Paged" + model.newEntity + "ResultRequestDto { MaxResultCount = int.MaxValue })).Items; // Paging not implemented yet");
                file.WriteLine("\t\t\tvar model = new " + model.newEntity + "ListViewModel");
                file.WriteLine("\t\t\t{");
                file.WriteLine("\t\t\t\t" + model.newEntityPlural + " = " + model.newEntity.ToLower() + ",");
                file.WriteLine("\t\t\t};");
                file.WriteLine("");
                foreach (var e in model.entityEnumAttributes)
                {

                file.WriteLine("\t\t\tvar " + e.Key.ToLower() + " = new List<SelectListItem>();");
                file.WriteLine("\t\t\tforeach (" + e.Value + " eVal in Enum.GetValues(typeof(" + e.Value + ")))");
                file.WriteLine("\t\t\t{");
                file.WriteLine("\t\t\t\t" + e.Key.ToLower() + ".Add(new SelectListItem { Text = Enum.GetName(typeof(" + e.Value + "), eVal), Value = eVal.ToString() });");
                file.WriteLine("\t\t\t}");
                file.WriteLine("\t\t\tViewBag." + e.Key + " = " + e.Key.ToLower() + ";");
                }
                file.WriteLine("\t\t\treturn View(model);");
                file.WriteLine("\t\t}");
                file.WriteLine("");

                file.WriteLine("\t\tpublic async Task<ActionResult> Edit" + model.newEntity + "Modal(int id)");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\tvar " + model.newEntity.ToLower() + " = await _" + model.newEntity.ToLower() + "AppService.Get(new EntityDto<int>(id));");
                file.WriteLine("\t\t\tvar model = new Edit" + model.newEntity + "ModalViewModel");
                file.WriteLine("\t\t\t{");
                file.WriteLine("\t\t\t\t" + model.newEntity + " = " + model.newEntity.ToLower() + ",");
                file.WriteLine("\t\t\t};");
                file.WriteLine("");
                foreach (var e in model.entityEnumAttributes)
                {

                    file.WriteLine("\t\t\tvar " + e.Key.ToLower() + " = new List<SelectListItem>();");
                    file.WriteLine("\t\t\tforeach (" + e.Value + " eVal in Enum.GetValues(typeof(" + e.Value + ")))");
                    file.WriteLine("\t\t\t{");
                    file.WriteLine("\t\t\t\t" + e.Key.ToLower() + ".Add(new SelectListItem { Text = Enum.GetName(typeof(" + e.Value + "), eVal), Value = eVal.ToString() });");
                    file.WriteLine("\t\t\t}");
                    file.WriteLine("\t\t\tViewBag." + e.Key + " = " + e.Key.ToLower() + ";");
                }
                file.WriteLine("\t\t\treturn View(\"_Edit" + model.newEntity + "Modal\",model);");
                file.WriteLine("\t\t}");
                file.WriteLine("");


                file.WriteLine("\t}");
                file.WriteLine("}");
            }
            Console.Write($"\nFile Generated:\n{model.newEntityPlural}Controller.cs");
            #endregion
        }

        public void CreateViews(AppServiceModel model)
        {
            #region Entity Modal View
            //Make AppService
            new Helpers().makeDirectory(model.viewPath);

            var dtoString = Path.Combine(model.viewPath, "_Edit" + model.newEntity + "Modal.cshtml");
            using (StreamWriter file = new StreamWriter(dtoString, true))
            {
                file.WriteLine("@using " + model.project + ".Web.Models.Common.Modals");
                file.WriteLine("@model " + model.project + ".Web.Models." + model.newEntityPlural + ".Edit" + model.newEntity + "ModalViewModel");
                file.WriteLine("@{");
                file.WriteLine("\tLayout = null;");
                file.WriteLine("}");
                file.WriteLine("");

                file.WriteLine("@await Html.PartialAsync(\"~/Views/Shared/Modals/_ModalHeader.cshtml\", new ModalHeaderViewModel(L(\"Edit" + model.newEntity + "\")))");
                file.WriteLine("<form name=\"" + model.newEntity + "EditForm\" role=\"form\" novalidate class=\"form-horizontal form-validation\">");
                file.WriteLine("\t<div class=\"modal-body\">");
                file.WriteLine("\t\t<input type=\"hidden\" name=\"Id\" value=\"@Model." + model.newEntity + ".Id\" />");

                foreach (var field in model.entityAttributes)
                {
                    file.WriteLine("\t\t<div class=\"form-group required\">");
                    file.WriteLine("\t\t\t<div class=\"col-md-3\">");
                    file.WriteLine("\t\t\t\t<label class=\"control-label\">@L(\"" + field.Key + "\")</label>");
                    file.WriteLine("\t\t\t</div>");
                    file.WriteLine("\t\t\t<div class=\"col-md-9\">");
                    switch (field.Value.ToLower())
                    {
                        case "string":
                            file.WriteLine("\t\t\t\t<input name=\"" + field.Key + "\" class=\"form-control\" required value=\"@Model." + model.newEntity + "." + field.Key + "\">");
                            break;
                        case "int":
                            file.WriteLine("\t\t\t\t<input type=\"number\" name=\"" + field.Key + "\" class=\"form-control\" required value=\"@Model." + model.newEntity + "." + field.Key + "\">");
                            break;
                        case "boolean":
                            file.WriteLine("\t\t\t\t<input type=\"checkbox\" name=\"" + field.Key + "\" class=\"form-check-input\" value=\"true\" @(Model." + model.newEntity + "." + field.Key + " ? Html.Raw(\"checked=" + @"\" + "\"checked" + @"\" + ") : null) /> ");
                            break;
                        case "datetime":
                            file.WriteLine("\t\t\t\t<input type=\"datetime-local\" name=\"" + field.Key + "\" class=\"form-control\" required value=\"@Model." + model.newEntity + "." + field.Key + ".ToString(\"yyyy-MM-ddThh:mm\")\">");
                            break;
                        case "datetime?":
                            file.WriteLine("\t\t\t\t<input type=\"datetime-local\" name=\"" + field.Key + "\" class=\"form-control\" required value=\"@Model." + model.newEntity + "." + field.Key + ".ToString(\"yyyy-MM-ddThh:mm\")\">");
                            break;
                        case "long":
                            file.WriteLine("\t\t\t\t<input type=\"number\" name=\"" + field.Key + "\" class=\"form-control\" required value=\"@Model." + model.newEntity + "." + field.Key + "\">");
                            break;
                        case "float":
                            file.WriteLine("\t\t\t\t<input type=\"number\" name=\"" + field.Key + "\" class=\"form-control\" required value=\"@Model." + model.newEntity + "." + field.Key + "\">");
                            break;
                        case "byte[]":
                            file.WriteLine("\t\t\t\t<input type=\"file\" name=\"" + field.Key + "\" class=\"form-control\" required value=\"@Model." + model.newEntity + "." + field.Key + "\">");
                            break;
                        case "double":
                            file.WriteLine("\t\t\t\t<input type=\"number\" name=\"" + field.Key + "\" class=\"form-control\" required value=\"@Model." + model.newEntity + "." + field.Key + "\">");
                            break;
                        case "bool":
                            file.WriteLine("\t\t\t\t<input type=\"checkbox\" name=\"" + field.Key + "\" class=\"form-check-input\" value=\"true\" @(Model." + model.newEntity + "." + field.Key + " ? Html.Raw(\"checked=" + @"\" + "\"checked" + @"\" + ") : null) /> ");
                            break;
                        case "list":
                            file.WriteLine("\t\t\t\t<select asp-items=\"ViewBag." + field.Key + "\" class=\"form-control\" name=\"" + field.Key + "\" asp-for=\"" + model.newEntity + "." + field.Key + "\"></select>");
                            break;
                        default:
                            break;
                    }
                    file.WriteLine("\t\t\t</div>");
                    file.WriteLine("\t\t</div>");
                }

                file.WriteLine("\t</div>");
                file.WriteLine("\t@await Html.PartialAsync(\"~/Views/Shared/Modals/_ModalFooterWithSaveAndCancel.cshtml\")");
                file.WriteLine("</form>");
                file.WriteLine("");
                file.WriteLine("<script src=\"~/view-resources/Views/" + model.newEntityPlural + "/_Edit" + model.newEntity + "Modal.js\" asp-append-version=\"true\"></script>");
            }
            Console.Write($"\n_Edit" + model.newEntity + "Modal.cshtml");
            #endregion


            #region Create Index
            //Index file
            var indexString = Path.Combine(model.viewPath, "Index.cshtml");
            using (StreamWriter file = new StreamWriter(indexString, true))
            {
                file.WriteLine("@using " + model.project + ".Web.Models.Common.Modals");
                file.WriteLine("@using " + model.project + ".Web.Startup");

                //Referenced Name spaces
                if (model.referencedEntityNameSpace != "")
                {
                    file.WriteLine("@using " + model.referencedEntityNameSpace + ";");
                }
                foreach (var ns in model.appServiceNameSpaces)
                {
                    file.WriteLine("@using " + ns + ";");
                }

                file.WriteLine("@model " + model.project + ".Web.Models." + model.newEntityPlural + "." + model.newEntity + "ListViewModel");

                file.WriteLine("@{");
                file.WriteLine("\tViewBag.Title = L(\"" + model.newEntityPlural + "\");");
                file.WriteLine("\tViewBag.CurrentPageName = PageNames." + model.newEntityPlural + ";");
                file.WriteLine("}");
                file.WriteLine("");
                file.WriteLine("@section scripts");
                file.WriteLine("{");
                file.WriteLine("\t<environment names=\"Development\">");
                file.WriteLine("\t<script src=\"~/view-resources/Views/" + model.newEntityPlural + "/Index.js\" asp-append-version=\"true\"></script>");
                file.WriteLine("\t</environment>");
                file.WriteLine("");
                file.WriteLine("\t<environment names=\"Staging,Production\">");
                file.WriteLine("\t<script src=\"~/view-resources/Views/" + model.newEntityPlural + "/Index.min.js\" asp-append-version=\"true\"></script>");
                file.WriteLine("\t</environment>");
                file.WriteLine("");
                file.WriteLine("\t<script src=\"~/bower_components/datatables.net/js/jquery.dataTables.min.js\"></script>");
                file.WriteLine("\t<script src=\"~/bower_components/datatables.net-bs/js/dataTables.bootstrap.min.js\"></script>");
                file.WriteLine("\t<script>");
                file.WriteLine("\t\t$(function () {");
                file.WriteLine("\t\t\t$('.datatable-auto').DataTable()");
                file.WriteLine("\t\t\t$('.datatable-custom').DataTable({");
                file.WriteLine("\t\t\t\t'paging': true,");
                file.WriteLine("\t\t\t\t'lengthChange': true,");
                file.WriteLine("\t\t\t\t'searching': true,");
                file.WriteLine("\t\t\t\t'ordering': true,");
                file.WriteLine("\t\t\t\t'info': true,");
                file.WriteLine("\t\t\t\t'autoWidth': true,");
                file.WriteLine("\t\t\t})");
                file.WriteLine("\t\t})");
                file.WriteLine("\t</script>");
                file.WriteLine("}");
                file.WriteLine("");
                file.WriteLine("@section styles");
                file.WriteLine("{");
                file.WriteLine("\t<link rel=\"stylesheet\" href=\"~/bower_components/datatables.net-bs/css/dataTables.bootstrap.min.css\">");
                file.WriteLine("}");
                file.WriteLine("");
                file.WriteLine("<div class=\"content-header clearfix\">");
                file.WriteLine("\t<h1 class=\"pull-left\">");
                file.WriteLine("\t\t@L(\"" + model.newEntityPlural + "\")");
                file.WriteLine("\t</h1>");
                file.WriteLine("\t<div class=\"pull-right\">");
                file.WriteLine("\t\t<a href=\"#\" data-toggle=\"modal\" data-target=\"#" + model.newEntity + "CreateModal\" class=\"btn bg-blue\">");
                file.WriteLine("\t\t\t<i class=\"fa fa-plus-square\"></i>");
                file.WriteLine("\t\t\t@L(\"Create\")");
                file.WriteLine("\t\t</a>");
                file.WriteLine("\t</div>");
                file.WriteLine("</div>");

                file.WriteLine("<div class=\"content\">");
                file.WriteLine("\t<div class=\"form-horizontal\">");
                file.WriteLine("\t\t<div class=\"panel-group\">");
                file.WriteLine("\t\t\t<div class=\"panel panel-default\">");
                file.WriteLine("\t\t\t\t<div class=\"panel-body\">");
                file.WriteLine("\t\t\t\t\t<div class=\"scroll-wrapper\">");
                file.WriteLine("\t\t\t\t\t\t<table class=\"table table-bordered table-striped datatable-auto\">");
                file.WriteLine("\t\t\t\t\t\t\t<thead>");
                file.WriteLine("\t\t\t\t\t\t\t\t<tr>");
                file.WriteLine("\t\t\t\t\t\t\t\t\t<th style=\"width: 150px\">@L(\"Actions\")</th>");
                foreach (var field in model.entityAttributes)
                {
                    file.WriteLine("\t\t\t\t\t\t\t\t\t<th>@L(\"" + field.Key + "\")</th>");
                }
                foreach (var field in model.entityEnumAttributes)
                {
                    file.WriteLine("\t\t\t\t\t\t\t\t\t<th>@L(\"" + field.Key + "\")</th>");
                }
                file.WriteLine("\t\t\t\t\t\t\t\t</tr>");
                file.WriteLine("\t\t\t\t\t\t\t</thead>");
                file.WriteLine("\t\t\t\t\t\t\t<tbody>");
                file.WriteLine("\t\t\t\t\t\t\t\t@foreach (var " + model.newEntity.ToLower() + " in Model." + model.newEntityPlural + ")");
                file.WriteLine("\t\t\t\t\t\t\t\t{");
                file.WriteLine("\t\t\t\t\t\t\t\t\t<tr>");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t<td style=\"width: 150px\">");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t<div class=\"dropdown\">");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t<a href=\"#\" class=\"btn btn-sm btn-primary dropdown-toggle\" data-toggle=\"dropdown\">");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t<span>@L(\"Actions\")</span>");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t<b class=\"caret\"></b>");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t</a>");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t<ul class=\"dropdown-menu\">");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t<li>");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t<a href=\"#\" class=\"edit-" + model.newEntity.ToLower() + "\" data-" + model.newEntity.ToLower() + "-id=\"@" + model.newEntity.ToLower() + ".Id\" data-toggle=\"modal\" data-target=\"#" + model.newEntity + "EditModal\">");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<i class=\"fa fa-edit\"></i>@L(\"Edit\")");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t</a>");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t</li>");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t<li>");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t<a href=\"#\" class=\"delete-" + model.newEntity.ToLower() + "\" data-" + model.newEntity.ToLower() + "-id=\"@" + model.newEntity.ToLower() + ".Id\" >");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<i class=\"fa fa-trash\"></i>@L(\"Delete\")");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t</a>");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t</li>");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t</ul>");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t\t</div>");
                file.WriteLine("\t\t\t\t\t\t\t\t\t\t</td>");
                foreach (var field in model.entityAttributes)
                {
                    if (field.Value.ToLower() == "bool" || field.Value.ToLower() == "boolean")
                    {
                        file.WriteLine("\t\t\t\t\t\t\t\t\t\t<td>@(" + model.newEntity.ToLower() + "." + field.Key + " ? Html.Raw(\"<label><span class=" + @"\" + "\"label label-primary" + @"\" + "\">TRUE</span></label>\") : null)");
                    }
                    else if (field.Value.ToLower() == "datetime" || field.Value.ToLower() == "datetime?")
                    {
                        file.WriteLine("\t\t\t\t\t\t\t\t\t\t<td>@" + model.newEntity.ToLower() + "." + field.Key + ".ToString(\"yyyy-MM-ddThh:mm\")</td>");
                    }
                    else
                    {
                        file.WriteLine("\t\t\t\t\t\t\t\t\t\t<td>@" + model.newEntity.ToLower() + "." + field.Key + "</td>");
                    }
                    
                }
                foreach (var field in model.entityEnumAttributes)
                {
                    file.WriteLine("\t\t\t\t\t\t\t\t\t\t<td>@Enum.GetName(typeof(" + field.Value + "), " + model.newEntity.ToLower() + "." + field.Key + ")</td>");
                }
                file.WriteLine("\t\t\t\t\t\t\t\t\t</tr>");
                file.WriteLine("\t\t\t\t\t\t\t\t}");
                file.WriteLine("\t\t\t\t\t\t\t</tbody>");

                file.WriteLine("\t\t\t\t\t\t</table>");
                file.WriteLine("\t\t\t\t\t</div>");
                file.WriteLine("\t\t\t\t</div>");
                file.WriteLine("\t\t\t</div>");
                file.WriteLine("\t\t</div>");
                file.WriteLine("\t</div>");
                file.WriteLine("</div>");




                file.WriteLine("<div class=\"modal fade\" id=\"" + model.newEntity + "CreateModal\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"" + model.newEntity + "CreateModalLabel\" data-backdrop=\"static\">");
                file.WriteLine("\t<div class=\"modal-dialog modal-lg\" role=\"document\">");
                file.WriteLine("\t\t<div class=\"modal-content\">");
                file.WriteLine("\t\t\t@await Html.PartialAsync(\"~/Views/Shared/Modals/_ModalHeader.cshtml\", new ModalHeaderViewModel(L(\"CreateNew" + model.newEntity + "\")))");
                file.WriteLine("\t\t\t<form name=\"" + model.newEntity + "CreateForm\" role=\"form\" novalidate class=\"form-horizontal form-validation\">");
                file.WriteLine("\t\t\t\t<div class=\"modal-body\">");
                file.WriteLine("\t\t\t\t\t<ul class=\"nav nav-tabs\" role=\"tablist\">");
                file.WriteLine("\t\t\t\t\t\t<li class=\"active\">");
                file.WriteLine("\t\t\t\t\t\t\t<a href=\"#create-" + model.newEntity.ToLower() + "-details\" data-toggle=\"tab\">@L(\"" + model.newEntity + "Details\")</a>");
                file.WriteLine("\t\t\t\t\t\t</li>");
                foreach (var item in model.entityAttributes)
                {
                    if (item.Value.ToLower() == "list")
                    {
                        file.WriteLine("\t\t\t\t\t\t<li>");
                        file.WriteLine("\t\t\t\t\t\t\t<a href=\"#create-" + model.newEntity.ToLower() + "-details\" data-toggle=\"tab\">@L(\"" + model.newEntity + "Details\")</a>");
                        file.WriteLine("\t\t\t\t\t\t</li>");
                    }
                }

                file.WriteLine("\t\t\t\t\t</ul>");
                file.WriteLine("\t\t\t\t\t<div class=\"tab-content\" style=\"padding: 20px 10px 0px 10px; \">");
                file.WriteLine("\t\t\t\t\t\t<div class=\"tab-pane active\" id=\"create-user-details\">");


                foreach (var field in model.entityAttributes)
                {
                    file.WriteLine("\t\t<div class=\"form-group required\">");
                    file.WriteLine("\t\t\t<div class=\"col-md-3\">");
                    file.WriteLine("\t\t\t\t<label class=\"control-label\">@L(\"" + field.Key + "\")</label>");
                    file.WriteLine("\t\t\t</div>");
                    file.WriteLine("\t\t\t<div class=\"col-md-9\">");
                    switch (field.Value.ToLower())
                    {
                        case "string":
                            file.WriteLine("\t\t\t\t<input name=\"" + field.Key + "\" class=\"form-control\" required>");
                            break;
                        case "int":
                            file.WriteLine("\t\t\t\t<input type=\"number\" name=\"" + field.Key + "\" class=\"form-control\" required>");
                            break;
                        case "boolean":
                            file.WriteLine("\t\t\t\t<input type=\"checkbox\" name=\"" + field.Key + "\" class=\"form-check-input\" value=\"true\" /> ");
                            break;
                        case "datetime":
                            file.WriteLine("\t\t\t\t<input type=\"datetime-local\" name=\"" + field.Key + "\" class=\"form-control\" required>");
                            break;
                        case "datetime?":
                            file.WriteLine("\t\t\t\t<input type=\"datetime-local\" name=\"" + field.Key + "\" class=\"form-control\" required>");
                            break;
                        case "long":
                            file.WriteLine("\t\t\t\t<input type=\"number\" name=\"" + field.Key + "\" class=\"form-control\" required>");
                            break;
                        case "float":
                            file.WriteLine("\t\t\t\t<input type=\"number\" name=\"" + field.Key + "\" class=\"form-control\" required>");
                            break;
                        case "byte[]":
                            file.WriteLine("\t\t\t\t<input type=\"file\" name=\"" + field.Key + "\" class=\"form-control\" required>");
                            break;
                        case "double":
                            file.WriteLine("\t\t\t\t<input type=\"number\" name=\"" + field.Key + "\" class=\"form-control\" required>");
                            break;
                        case "bool":
                            file.WriteLine("\t\t\t\t<input type=\"checkbox\" name=\"" + field.Key + "\" class=\"form-check-input\" value=\"true\" /> ");
                            break;
                        //case "list":
                        //    file.WriteLine("\t\t\t\t<select asp-items=\"ViewBag." + field.Key + "\" class=\"form-control\" name=\"" + field.Key + "\" asp-for=\"" + model.newEntity + "." + field.Key + "\"></select>");
                        //    break;
                        default:
                            break;
                    }
                    file.WriteLine("\t\t\t</div>");
                    file.WriteLine("\t\t</div>");
                }


                foreach (var field in model.entityEnumAttributes)
                {
                    file.WriteLine("\t\t<div class=\"form-group required\">");
                    file.WriteLine("\t\t\t<div class=\"col-md-3\">");
                    file.WriteLine("\t\t\t\t<label class=\"control-label\">@L(\"" + field.Key + "\")</label>");
                    file.WriteLine("\t\t\t</div>");
                    file.WriteLine("\t\t\t<div class=\"col-md-9\">");
                    file.WriteLine("\t\t\t\t<select asp-items=\"ViewBag." + field.Key + "\" class=\"form-control\" name=\"" + field.Key + "\"></select>");
                    file.WriteLine("\t\t\t</div>");
                    file.WriteLine("\t\t</div>");
                }
                file.WriteLine("\t\t\t\t\t\t</div>");
                file.WriteLine("\t\t\t\t\t</div>");

                file.WriteLine("\t\t\t\t</div>");
                file.WriteLine("\t\t\t\t@await Html.PartialAsync(\"~/Views/Shared/Modals/_ModalFooterWithSaveAndCancel.cshtml\")");
                file.WriteLine("\t\t\t</form>");
                file.WriteLine("\t\t</div>");
                file.WriteLine("\t</div>");
                file.WriteLine("</div>");
                file.WriteLine("");
                file.WriteLine("<div class=\"modal fade\" id=\"" + model.newEntity + "EditModal\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"" + model.newEntity + "EditModalLabel\" data-backdrop=\"static\">");
                file.WriteLine("\t<div class=\"modal-dialog modal-lg\" role=\"document\">");
                file.WriteLine("\t\t<div class=\"modal-content\">");
                file.WriteLine("\t\t</div>");
                file.WriteLine("\t</div>");
                file.WriteLine("</div>");

                file.WriteLine("");
            }
            Console.Write($"\n_Index.cshtml");

            #endregion



        }

        public void CreateModels(AppServiceModel model)
        {
            #region Entity Model
            //Make AppService

            new Helpers().makeDirectory(model.modelPath);


            var dtoString = Path.Combine(model.modelPath,model.newEntity + "ListViewModel.cs");
            using (StreamWriter file = new StreamWriter(dtoString, true))
            {
                file.WriteLine("using " + model.project + ".Nex" + model.newEntity + ".Dto;");
                file.WriteLine("using System.Collections.Generic;");
                file.WriteLine("");
                file.WriteLine("namespace " + model.project + ".Web.Models." + model.newEntityPlural );
                file.WriteLine("{");
                file.WriteLine("\tpublic class " + model.newEntity + "ListViewModel");
                file.WriteLine("\t{");
                file.WriteLine("");
                file.WriteLine("\t\tpublic IReadOnlyList<" + model.newEntity + "Dto> " + model.newEntityPlural + " { get; set; }");
                file.WriteLine("");
                file.WriteLine("\t}");
                file.WriteLine("}");
            }
            Console.Write($"\n{model.newEntity}ListViewModel.cs");
            #endregion

            #region Entity List Model
            //Make AppService
            var dtoStringlist = Path.Combine(model.modelPath,"Edit" + model.newEntity + "ModalViewModel.cs");
            using (StreamWriter file = new StreamWriter(dtoStringlist, true))
            {
                file.WriteLine("using " + model.project + ".Nex" + model.newEntity + ".Dto;");
                file.WriteLine("");
                file.WriteLine("namespace " + model.project + ".Web.Models." + model.newEntityPlural);
                file.WriteLine("{");
                file.WriteLine("\tpublic class Edit" + model.newEntity + "ModalViewModel");
                file.WriteLine("\t{");
                file.WriteLine("");
                file.WriteLine("\t\tpublic " + model.newEntity + "Dto " + model.newEntity + " { get; set; }");
                file.WriteLine("");
                file.WriteLine("\t}");
                file.WriteLine("}");
            }
            Console.Write($"\nEdit{model.newEntity}ModalViewModel.cs");
            #endregion
        }

        private void AddToNavigation(AppServiceModel model)
        {
            string[] permissionProvider = Directory.GetFiles(model.startupPath, model.project + "NavigationProvider.cs", SearchOption.AllDirectories);

            if (permissionProvider.Count() > 0)
            {
                //Console.Write("Auth Provider Found:" + permissionProvider[0]);
                string[] lines = File.ReadAllLines(permissionProvider[0]);
                var newContent = new List<string> { };
                int lastline = 0;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("requiredPermissionName"))
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
                        newContent.Add("\t\t\t\t.AddItem(new MenuItemDefinition(PageNames." + model.newEntityPlural + ",L(\"" + model.newEntityPlural + "\"),url: \"" + model.newEntityPlural + "\",icon: \"fa-black-tie\",");
                        newContent.Add("\t\t\t\t\t\trequiredPermissionName: \"" + model.prefixPermission + "." + model.newEntity + ".Read\"))");
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
            else
            {
                Console.WriteLine($"{model.project}NavigationProvider.cs Not Found.");
            }
            //file.WriteLine("\t[AbpAuthorize(PermissionNames.Pages_" + newEntity + "CRUD)]");
        }

        private void AddToPageNames(AppServiceModel model)
        {
            string[] permissionProvider = Directory.GetFiles(model.startupPath, "PageNames.cs", SearchOption.AllDirectories);

            if (permissionProvider.Count() > 0)
            {
                //Console.Write("Auth Provider Found:" + permissionProvider[0]);
                string[] lines = File.ReadAllLines(permissionProvider[0]);
                var newContent = new List<string> { };
                int lastline = 0;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("public const string"))
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
                        newContent.Add("\t\tpublic const string " + model.newEntityPlural + " = \"" + model.newEntityPlural + "\"; ");
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
            else
            {
                Console.WriteLine($"PageNames.cs Not Found.");
            }
            //file.WriteLine("\t[AbpAuthorize(PermissionNames.Pages_" + newEntity + "CRUD)]");
        }

        public void CreateJQueryDuplicator(AppServiceModel model)
        {
            #region _EditModal JS
            //Make _EditModal JS

            new Helpers().makeDirectory(model.ajaxPath + model.newEntityPlural);

            var dtoString = Path.Combine(model.ajaxPath + model.newEntityPlural, "_Edit" + model.newEntity + "Modal.js");
            using (StreamWriter file = new StreamWriter(dtoString, true))
            {
                file.WriteLine("(function ($) {");
                file.WriteLine("\tvar " + model.newEntity.ToLower() + "Service = abp.services.app." + model.newEntity.ToLower() + ";");
                file.WriteLine("\tvar $modal = $('#" + model.newEntity + "EditModal');");
                file.WriteLine("\tvar $form = $('form[name=" + model.newEntity + "EditForm]');");
                file.WriteLine("");
                file.WriteLine("\t$form.validate({");
                file.WriteLine("\t\thighlight: function (input) {");
                file.WriteLine("\t\t\t$(input).parents('.form-group').addClass('has-error');");
                file.WriteLine("\t\t},");
                file.WriteLine("\t\tunhighlight: function (input) {");
                file.WriteLine("\t\t$(input).parents('.form-group').removeClass('has-error');");
                file.WriteLine("\t\t},");
                file.WriteLine("\t\terrorPlacement: function (error, element) {");
                file.WriteLine("\t\t$(element).parent().append(error);");
                file.WriteLine("\t\t}");
                file.WriteLine("\t});");
                file.WriteLine("");
                file.WriteLine("\tfunction save() {");
                file.WriteLine("\t\tif (!$form.valid()) {");
                file.WriteLine("\t\t\treturn;");
                file.WriteLine("\t\t}");
                file.WriteLine("\t\tvar " + model.newEntity.ToLower() + " = $form.serializeFormToObject();");
                file.WriteLine("");
                file.WriteLine("\t\tabp.ui.setBusy($form);");
                file.WriteLine("\t\t" + model.newEntity.ToLower() + "Service.update(" + model.newEntity.ToLower() + ").done(function () {");
                file.WriteLine("\t\t\t$modal.modal('hide');");
                file.WriteLine("\t\t\tlocation.reload(true);");
                file.WriteLine("\t\t}).always(function () {");
                file.WriteLine("\t\t\tabp.ui.clearBusy($modal);");
                file.WriteLine("\t\t});");
                file.WriteLine("\t}");
                file.WriteLine("");
                file.WriteLine("\t$form.closest('div.modal-content').find(\".save-button\").click(function (e) {");
                file.WriteLine("\t\te.preventDefault();");
                file.WriteLine("\t\tsave();");
                file.WriteLine("\t});");
                file.WriteLine("");
                file.WriteLine("\t$form.find('input').on('keypress', function (e) {");
                file.WriteLine("\t\tif (e.which === 13) {");
                file.WriteLine("\t\t\te.preventDefault();");
                file.WriteLine("\t\t\tsave();");
                file.WriteLine("\t\t}");
                file.WriteLine("\t});");
                file.WriteLine("");
                file.WriteLine("\t$modal.on('shown.bs.modal', function () {");
                file.WriteLine("\t\t$form.find('input[type=text]:first').focus();");
                file.WriteLine("\t});");
                file.WriteLine("})(jQuery);");

            }
            Console.Write($"\n_Edit" + model.newEntity + "Modal.js");
            #endregion


            #region Index
            //Make Index

            new Helpers().makeDirectory(model.ajaxPath + model.newEntityPlural);

            var indexjs = Path.Combine(model.ajaxPath + model.newEntityPlural,"Index.js");
            using (StreamWriter file = new StreamWriter(indexjs, true))
            {
                file.WriteLine("(function () {");
                file.WriteLine("\t$(function () {");
                file.WriteLine("\t\tvar " + model.newEntity.ToLower() + "Service = abp.services.app." + model.newEntity.ToLower() + ";");
                file.WriteLine("\t\tvar $modal = $('#" + model.newEntity + "CreateModal');");
                file.WriteLine("\t\tvar $form = $modal.find('form');");
                file.WriteLine("");
                file.WriteLine("\t$form.validate({");
                file.WriteLine("\t\thighlight: function (input) {");
                file.WriteLine("\t\t\t$(input).parents('.form-group').addClass('has-error');");
                file.WriteLine("\t\t},");
                file.WriteLine("\t\tunhighlight: function (input) {");
                file.WriteLine("\t\t$(input).parents('.form-group').removeClass('has-error');");
                file.WriteLine("\t\t},");
                file.WriteLine("\t\terrorPlacement: function (error, element) {");
                file.WriteLine("\t\t$(element).parent().append(error);");
                file.WriteLine("\t\t}");
                file.WriteLine("\t});");
                file.WriteLine("");
                file.WriteLine("\t\t$('#RefreshButton').click(function () {");
                file.WriteLine("\t\t\trefresh" + model.newEntity + "List();");
                file.WriteLine("\t\t});");
                file.WriteLine("");
                file.WriteLine("\t\t$('.delete-" + model.newEntity.ToLower() + "').click(function () {");
                file.WriteLine("\t\t\tvar " + model.newEntity.ToLower() + "Id = $(this).attr(\"data-" + model.newEntity.ToLower() + "-id\");");
                file.WriteLine("\t\t\tvar " + model.newEntity.ToLower() + "Name = $(this).attr('data-" + model.newEntity.ToLower() + "-name');");
                file.WriteLine("\t\t\tdelete" + model.newEntity + "(" + model.newEntity.ToLower() + "Id, " + model.newEntity.ToLower() + "Name);");
                file.WriteLine("\t\t});");
                file.WriteLine("");

                file.WriteLine("\t\t$('.edit-" + model.newEntity.ToLower() + "').click(function (e) {");
                file.WriteLine("\t\t\tvar " + model.newEntity.ToLower() + "Id = $(this).attr(\"data-" + model.newEntity.ToLower() + "-id\");");
                file.WriteLine("\t\t\te.preventDefault();");
                file.WriteLine("\t\t\tabp.ajax({");
                file.WriteLine("\t\t\t\turl: abp.appPath + '" + model.newEntityPlural + "/Edit" + model.newEntity + "Modal?Id=' + " + model.newEntity.ToLower() + "Id,");
                file.WriteLine("\t\t\t\ttype: 'POST',");
                file.WriteLine("\t\t\t\tdataType: 'html',");
                file.WriteLine("\t\t\t\tsuccess: function (content) {");
                file.WriteLine("\t\t\t\t\t$('#" + model.newEntity + "EditModal div.modal-content').html(content);");
                file.WriteLine("\t\t\t\t},");
                file.WriteLine("\t\t\t\terror: function (e) { }");
                file.WriteLine("\t\t\t});");
                file.WriteLine("\t\t});");


                file.WriteLine("\t\t$form.find('button[type=\"submit\"]').click(function (e) {");
                file.WriteLine("\t\t\te.preventDefault();");
                file.WriteLine("\t\t\tif (!$form.valid()) {");
                file.WriteLine("\t\t\t\treturn;");
                file.WriteLine("\t\t\t}");


                file.WriteLine("\t\t\tvar " + model.newEntity.ToLower() + " = $form.serializeFormToObject();");
                file.WriteLine("");
                file.WriteLine("\t\t\tabp.ui.setBusy($modal);");
                file.WriteLine("\t\t\t" + model.newEntity.ToLower() + "Service.create(" + model.newEntity.ToLower() + ").done(function () {");
                file.WriteLine("\t\t\t\t$modal.modal('hide');");
                file.WriteLine("\t\t\t\tlocation.reload(true);");
                file.WriteLine("\t\t\t}).always(function () {");
                file.WriteLine("\t\t\t\tabp.ui.clearBusy($modal);");
                file.WriteLine("\t\t\t});");
                file.WriteLine("\t\t});");




                file.WriteLine("\t\t$modal.on('shown.bs.modal', function () {");
                file.WriteLine("\t\t\t$modal.find('input:not([type=hidden]):first').focus();");
                file.WriteLine("\t\t});");


                file.WriteLine("\t\tfunction refresh" + model.newEntity + "List() {");
                file.WriteLine("\t\t\tlocation.reload(true);");
                file.WriteLine("\t\t};");

                file.WriteLine("\t\tfunction delete" + model.newEntity + "(" + model.newEntity.ToLower() + "Id, " + model.newEntity.ToLower() + "Name) {");
                file.WriteLine("\t\t\tabp.message.confirm(");
                file.WriteLine("\t\t\t\tabp.utils.formatString(abp.localization.localize('AreYouSureWantToDelete', 'AbpProjectName'), " + model.newEntity.ToLower() + "Name),");
                file.WriteLine("\t\t\t\tfunction (isConfirmed) {");
                file.WriteLine("\t\t\t\t\tif (isConfirmed) {");
                file.WriteLine("\t\t\t\t\t\t" + model.newEntity.ToLower() + "Service.delete({");
                file.WriteLine("\t\t\t\t\t\t\tid: " + model.newEntity.ToLower() + "Id");
                file.WriteLine("\t\t\t\t\t\t}).done(function () {");
                file.WriteLine("\t\t\t\t\t\t\trefresh" + model.newEntity + "List();");
                file.WriteLine("\t\t\t\t\t\t});");
                file.WriteLine("\t\t\t\t\t}");
                file.WriteLine("\t\t\t\t}");
                file.WriteLine("\t\t\t);");
                file.WriteLine("\t\t}");
                file.WriteLine("\t});");
                file.WriteLine("\t})();");


            }
            Console.Write($"\n_Edit" + model.newEntity + "Modal.js");
            #endregion


        }
    }
}
