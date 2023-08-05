using EcommerceASP.Libraries;
using EcommerceASP.Models;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.ComponentTypeManage;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class ComponentTypeManageQuery
    {
        public static ListViewModel GetListData(SearchFormViewModel objSearch)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objView = new ListViewModel();
                var lstComponentType = new List<ComponentTypeManageBO>();
                lstComponentType = (from t in _entities.ComponentTypes
                           join a in _entities.Accounts
                           on t.Created_by equals a.Id into T
                           from s in T.DefaultIfEmpty()
                           join ac in _entities.Accounts
                           on t.Updated_by equals ac.Id into AC
                           from c in AC.DefaultIfEmpty()
                           where !t.IsDeleted
                                 && (string.IsNullOrEmpty(objSearch.Name) || t.Name.Contains(objSearch.Name))
                                 && (string.IsNullOrEmpty(objSearch.Description) || t.Name.Contains(objSearch.Description))
                           select new ComponentTypeManageBO
                           {
                               Id = t.Id,
                               Name = t.Name,
                               NameFile = t.NameFile,
                               Description = t.Description,
                               DescriptionImage = t.DescriptionImage,
                               Row = t.Row,
                               Collumn = t.Collumn,
                               IsSlide = t.IsSlide
                           })
                           .OrderBy(m => m.Id)
                           .ToList();

                objView.Items = lstComponentType.ToPagedList((objSearch.PageCurrent ?? 1) - 1, objView.PageSize ?? 10);
                return objView;
            }
            catch (Exception objEx)
            {
                return new ListViewModel();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ComponentTypeManageBO GetDataUpdate(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objCategory = new ComponentTypeManageBO();
                if (Id == null)
                    return objCategory;
                
                objCategory = (from t in _entities.ComponentTypes
                               where !t.IsDeleted
                                     && t.Id == Id
                               select new ComponentTypeManageBO
                               {
                                   Id = t.Id,
                                   Name = t.Name,
                                   NameFile = t.NameFile,
                                   Description = t.Description,
                                   DescriptionImage = t.DescriptionImage,
                                   ImageOld = t.DescriptionImage,
                                   Row = t.Row,
                                   Collumn = t.Collumn,
                                   IsSlide = t.IsSlide
                               }).FirstOrDefault();
                return objCategory;
            }
            catch (Exception objEx)
            {
                return new ComponentTypeManageBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI Update(ComponentTypeManageBO objModel)
        {
            try
            {
                if (objModel.Id == 0)
                {
                    return CreateComponentType(objModel);
                }
                return UpdateComponentType(objModel);
            }
            catch (Exception e)
            {
                return ResponseAPI.GetFailedResponse(e.Message);
            }
        }

        public static ResponseAPI CreateComponentType(ComponentTypeManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objComponentType = new ComponentType();
                objComponentType.Name = objModel.Name;
                objComponentType.NameFile = objModel.NameFile;
                objComponentType.Description = objModel.Description;
                objComponentType.IsSlide = objModel.IsSlide;
                objComponentType.Row = objModel.IsSlide ? 1 : objModel.Row ?? 1;
                objComponentType.Collumn = objModel.Collumn;
                objComponentType.IsDeleted = false;
                objComponentType.Created_at = DateTime.Now;
                objComponentType.Created_by = 1;

                if (objModel.UploadImage != null)
                {
                    var imgNameOld = objModel.ImageOld;

                    objComponentType.DescriptionImage = objModel.UploadImage.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/component/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objComponentType.DescriptionImage);
                }

                _entities.ComponentTypes.Add(objComponentType);
                _entities.SaveChanges();
                return ResponseAPI.GetSuccessResponse("Success", null);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return ResponseAPI.GetFailedResponse(e.Message);
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI UpdateComponentType(ComponentTypeManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objComponentType = _entities.ComponentTypes.Where(m => m.Id == objModel.Id).FirstOrDefault();
                if (objComponentType == null)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy thông tin loại module để cập nhật");
                }
                objComponentType.Name = objModel.Name;
                objComponentType.NameFile = objModel.NameFile;
                objComponentType.Description = objModel.Description;
                objComponentType.IsSlide = objModel.IsSlide;
                objComponentType.Row = objModel.IsSlide ? 1 : objModel.Row ?? 1;
                objComponentType.Collumn = objModel.Collumn;
                objComponentType.IsDeleted = false;
                objComponentType.Updated_at = DateTime.Now;
                objComponentType.Updated_by = 1;

                if (objModel.UploadImage != null)
                {
                    var imgNameOld = objModel.ImageOld;

                    objComponentType.DescriptionImage = objModel.UploadImage.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/component/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objComponentType.DescriptionImage);
                }

                _entities.SaveChanges();
                return ResponseAPI.GetSuccessResponse("Success", null);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return ResponseAPI.GetFailedResponse(e.Message);
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI Delete(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objComponentType = _entities.ComponentTypes.Where(m => m.Id == Id).FirstOrDefault();
                if (objComponentType == null)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy loại module để xóa");
                }
                objComponentType.Updated_at = DateTime.Now;
                objComponentType.Updated_by = 1;
                objComponentType.IsDeleted = true;

                _entities.SaveChanges();
                return ResponseAPI.GetSuccessResponse("Xóa thành công", null);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return ResponseAPI.GetFailedResponse(e.Message);
            }
            finally
            {
                _entities.Dispose();
            }
        }
    }
}