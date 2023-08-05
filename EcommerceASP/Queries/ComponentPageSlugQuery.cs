using EcommerceASP.Libraries;
using EcommerceASP.Models;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.ComponentPageSlug;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class ComponentPageSlugQuery
    {
        public static ListViewModel GetListData(SearchFormViewModel objSearch)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objView = new ListViewModel();
                var lstData = new List<ComponentPageSlugBO>();
                lstData = (from componentpageslugs in _entities.ComponentPageSlugs
                           join accounts in _entities.Accounts
                           on componentpageslugs.Created_by equals accounts.Id into T
                           from s in T.DefaultIfEmpty()
                           join accounts2 in _entities.Accounts
                           on componentpageslugs.Updated_by equals accounts2.Id into AC
                           from c in AC.DefaultIfEmpty()
                           join componentsubdescriptions in _entities.ComponentSubDescriptions
                           on new { componentpageslugs.Id, componentpageslugs.PageSlug, componentpageslugs.IsDeleted }
                           equals new { Id = componentsubdescriptions.ComponentId, componentsubdescriptions.PageSlug, componentsubdescriptions.IsDeleted } into X
                           from d in X.DefaultIfEmpty()
                           where !componentpageslugs.IsDeleted
                                 && (string.IsNullOrEmpty(objSearch.PageSlug) || componentpageslugs.PageSlug.Contains(objSearch.PageSlug))
                           select new
                           {
                               Id = componentpageslugs.Id,
                               Name = componentpageslugs.Name,
                               Title = componentpageslugs.Title,
                               PageSlug = componentpageslugs.PageSlug,
                               Priority = componentpageslugs.Priority,
                               ComponentType = componentpageslugs.ComponentType,
                               Created_at = componentpageslugs.Created_at == null ? componentpageslugs.Updated_at : componentpageslugs.Created_at,
                               CreatedByName = string.IsNullOrEmpty(s.FullName) ? c.FullName : s.FullName,
                               d
                           })
                           .ToList()
                           .GroupBy(x => new { x.Id, x.Name, x.Title, x.PageSlug, x.Priority })
                           .Select(m => new ComponentPageSlugBO
                           {
                               Id = m.Key.Id,
                               Name = m.Key.Name,
                               Title = m.Key.Title,
                               PageSlug = m.Key.PageSlug,
                               Priority = m.Key.Priority,
                               ComponentType = m.FirstOrDefault().ComponentType,
                               Created_at = m.FirstOrDefault().Created_at,
                               CreatedByName = m.FirstOrDefault().CreatedByName,
                               lstSubDesc = m.FirstOrDefault().d == null ? new List<ComponentSubDescriptionBO>() : m.Select(z => new ComponentSubDescriptionBO
                               {
                                   Id = z.d.Id,
                                   ComponentId = z.d.ComponentId,
                                   PageSlug = z.d.PageSlug,
                                   SubTitle = z.d.SubTitle,
                                   Title = z.d.Title,
                                   Image = z.d.Image,
                                   Description = z.d.Description
                               }).ToList()
                           })
                           .OrderBy(m => m.PageSlug)
                           .ToList();

                objView.Items = lstData.ToPagedList((objSearch.PageCurrent ?? 1) - 1, objView.PageSize ?? 10);
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
        public static ComponentPageSlugBO GetDataUpdate(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objData = new ComponentPageSlugBO();
                if (Id == null)
                    return objData;

                objData = (from componentpageslugs in _entities.ComponentPageSlugs
                           join componentsubdescriptions in _entities.ComponentSubDescriptions
                           on new { componentpageslugs.Id, componentpageslugs.PageSlug, componentpageslugs.IsDeleted }
                           equals new { Id = componentsubdescriptions.ComponentId, componentsubdescriptions.PageSlug, componentsubdescriptions.IsDeleted } into X
                           from d in X.DefaultIfEmpty()
                           where componentpageslugs.Id == Id
                           select new
                           {
                               Id = componentpageslugs.Id,
                               Name = componentpageslugs.Name,
                               Title = componentpageslugs.Title,
                               PageSlug = componentpageslugs.PageSlug,
                               Priority = componentpageslugs.Priority,
                               ComponentTypeId = componentpageslugs.ComponentTypeId,
                               d
                           })
                           .ToList()
                           .GroupBy(x => new { x.Id, x.Name, x.Title, x.PageSlug, x.Priority })
                           .Select(m => new ComponentPageSlugBO
                           {
                               Id = m.Key.Id,
                               Name = m.Key.Name,
                               Title = m.Key.Title,
                               PageSlug = m.Key.PageSlug,
                               Priority = m.Key.Priority,
                               ComponentTypeId = m.FirstOrDefault().ComponentTypeId,
                               lstSubDesc = m.FirstOrDefault().d == null ? new List<ComponentSubDescriptionBO>() : m.Select(z => new ComponentSubDescriptionBO
                               {
                                   ComponentPageSlugID = m.Key.Id,
                                   Id = z.d.Id,
                                   ComponentId = z.d.ComponentId,
                                   PageSlug = z.d.PageSlug,
                                   SubTitle = z.d.SubTitle,
                                   Title = z.d.Title,
                                   Image = z.d.Image,
                                   Description = z.d.Description
                               }).ToList()
                           })
                           .FirstOrDefault();
                return objData;
            }
            catch (Exception objEx)
            {
                return new ComponentPageSlugBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI Update(ComponentPageSlugBO objModel)
        {
            try
            {
                if (objModel.Id == 0)
                {
                    return CreateComponentTypePageSlug(objModel);
                }
                return UpdateComponentTypePageSlug(objModel);
            }
            catch (Exception e)
            {
                return ResponseAPI.GetFailedResponse(e.Message);
            }
        }

        public static ResponseAPI CreateComponentTypePageSlug(ComponentPageSlugBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objData = new ComponentPageSlug();
                objData.PageSlug = objModel.PageSlug;
                objData.Name = objModel.Name;
                objData.Title = objModel.Title;
                objData.ComponentTypeId = objModel.ComponentTypeId;
                objData.QuantityRecord = objModel.QuantityRecord;
                objData.Priority = objModel.Priority;
                objData.IsDeleted = false;
                objData.Created_at = DateTime.Now;
                objData.Created_by = 1;

                _entities.ComponentPageSlugs.Add(objData);
                _entities.SaveChanges();
                return ResponseAPI.GetSuccessResponse("Thành công", null);
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

        public static ResponseAPI UpdateComponentTypePageSlug(ComponentPageSlugBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objData = _entities.ComponentPageSlugs.Where(m => m.Id == objModel.Id).FirstOrDefault();
                if (objData == null)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy thông tin để cập nhật");
                }
                objData.PageSlug = objModel.PageSlug;
                objData.Name = objModel.Name;
                objData.Title = objModel.Title;
                objData.Priority = objModel.Priority;
                objData.ComponentTypeId = objModel.ComponentTypeId;
                objData.QuantityRecord = objModel.QuantityRecord;
                objData.Updated_at = DateTime.Now;
                objData.Updated_by = 1;

                _entities.SaveChanges();
                return ResponseAPI.GetSuccessResponse("Thành công", null);
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
                var objData = _entities.ComponentPageSlugs.Where(m => m.Id == Id).FirstOrDefault();
                if (objData == null)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy loại module để xóa");
                }
                objData.Updated_at = DateTime.Now;
                objData.Updated_by = 1;
                objData.IsDeleted = true;

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
        #region ComponentSubDescription
        public static List<ComponentSubDescriptionBO> GetDataSubDescription(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objData = new List<ComponentSubDescriptionBO>();
                if (Id == null)
                    return objData;

                objData = _entities.ComponentSubDescriptions.Where(x => x.ComponentId == Id)
                            .Select(x => new ComponentSubDescriptionBO
                            {
                                Id = x.Id,
                                ComponentId = x.ComponentId,
                                PageSlug = x.PageSlug,
                                SubTitle = x.SubTitle,
                                Title = x.Title,
                                Image = x.Image,
                                ImageOld = x.Image,
                                Description = x.Description
                            })
                            .ToList();
                return objData;
            }
            catch (Exception objEx)
            {
                return new List<ComponentSubDescriptionBO>();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ComponentSubDescriptionBO GetDataUpdateSubDescription(int? Id, int? componentId, string strPageSlug)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objData = new ComponentSubDescriptionBO();
                if (Id == null || Id == 0)
                {
                    objData.ComponentId = componentId.Value;
                    objData.PageSlug = strPageSlug;
                    return objData;
                }

                objData = _entities.ComponentSubDescriptions.Where(x => x.Id == Id)
                            .Select(x => new ComponentSubDescriptionBO
                            {
                                Id = x.Id,
                                ComponentId = x.ComponentId,
                                PageSlug = x.PageSlug,
                                SubTitle = x.SubTitle,
                                Title = x.Title,
                                Image = x.Image,
                                Description = x.Description
                            })
                            .FirstOrDefault();
                return objData;
            }
            catch (Exception objEx)
            {
                return new ComponentSubDescriptionBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI UpdateSubDescription(ComponentSubDescriptionBO objModel)
        {
            try
            {
                if (objModel.Id == 0)
                {
                    return CreateComponentSubDescription(objModel);
                }
                return UpdateComponentSubDescription(objModel);
            }
            catch (Exception e)
            {
                return ResponseAPI.GetFailedResponse(e.Message);
            }
        }

        public static ResponseAPI CreateComponentSubDescription(ComponentSubDescriptionBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objData = new ComponentSubDescription();
                objData.PageSlug = objModel.PageSlug;
                objData.ComponentId = objModel.ComponentId;
                objData.SubTitle = objModel.SubTitle;
                objData.Title = objModel.Title;
                objData.Description = objModel.Description;
                objData.Priority = objModel.Priority;
                objData.ReferenceLink = objModel.ReferenceLink;
                objData.IsDeleted = false;
                objData.Created_at = DateTime.Now;
                objData.Created_by = 1;

                if (objModel.UploadImage != null)
                {
                    var imgNameOld = objModel.ImageOld;

                    objData.Image = objModel.UploadImage.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/component/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objData.Image);
                }

                _entities.ComponentSubDescriptions.Add(objData);

                _entities.SaveChanges();
                return ResponseAPI.GetSuccessResponse("Thành công", null);
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

        public static ResponseAPI UpdateComponentSubDescription(ComponentSubDescriptionBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objData = _entities.ComponentSubDescriptions.Where(m => m.Id == objModel.Id).FirstOrDefault();
                if (objData == null)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy thông tin để cập nhật");
                }
                objData.SubTitle = objModel.SubTitle;
                objData.Title = objModel.Title;
                objData.Description = objModel.Description;
                objData.Priority = objModel.Priority;
                objData.ReferenceLink = objModel.ReferenceLink;
                objData.Updated_at = DateTime.Now;
                objData.Updated_by = 1;

                if (objModel.UploadImage != null)
                {
                    var imgNameOld = objModel.ImageOld;

                    objData.Image = objModel.UploadImage.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/component/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objData.Image);
                }
                _entities.SaveChanges();
                return ResponseAPI.GetSuccessResponse("Thành công", null);
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

        public static ResponseAPI DeleteSubDescription(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objData = _entities.ComponentSubDescriptions.Where(m => m.Id == Id).FirstOrDefault();
                if (objData == null)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy loại module để xóa");
                }
                objData.Updated_at = DateTime.Now;
                objData.Updated_by = 1;
                objData.IsDeleted = true;

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
        #endregion
    }
}