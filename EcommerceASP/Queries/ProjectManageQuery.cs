using EcommerceASP.Constaint;
using EcommerceASP.Libraries;
using EcommerceASP.Models;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.ProjectImageManage;
using EcommerceASP.ViewModel.ProjectManage;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class ProjectManageQuery
    {
        public static ListViewModel GetListData(SearchFormViewModel objSearch)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objView = new ListViewModel();
                var lstProject = new List<ProjectManageBO>();
                lstProject = (from t in _entities.Projects
                                    join a in _entities.Accounts
                                    on t.Created_by equals a.Id into T
                                    from s in T.DefaultIfEmpty()
                                    join ac in _entities.Accounts
                                    on t.Updated_by equals ac.Id into AC
                                    from c in AC.DefaultIfEmpty()
                                    where !t.IsDeleted
                                          && (string.IsNullOrEmpty(objSearch.Title) || t.Title.Contains(objSearch.Title))
                                          && (string.IsNullOrEmpty(objSearch.Customer) || t.Customer.Contains(objSearch.Customer))
                                    select new ProjectManageBO
                                    {
                                        Id = t.Id,
                                        Title = t.Title,
                                        Slug = t.Slug,
                                        Customer = t.Customer,
                                        ThumbnailImage = t.ThumbnailImage,
                                        ImageOld = t.ThumbnailImage,
                                        Content = t.Content,
                                        JobDescription = t.JobDescription,
                                        Priority = t.Priority,
                                        Size = t.Size,
                                        TimeProject = t.TimeProject,
                                        SubTitle = t.SubTitle,
                                        ProjectCategoryId = t.ProjectCategoryId,
                                        Address = t.Address
                                        
                                    })
                           .OrderBy(m => m.Id)
                           .ToList();

                objView.Items = lstProject.ToPagedList((objSearch.PageCurrent ?? 1) - 1, objView.PageSize ?? 10);
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

        public static ProjectManageBO GetDataUpdate(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objProject = new ProjectManageBO();
                if (Id == null)
                    return objProject;

                objProject = (from t in _entities.Projects
                               where !t.IsDeleted
                                     && t.Id == Id
                               select new ProjectManageBO
                               {
                                   Id = t.Id,
                                   Title = t.Title,
                                   Slug = t.Slug,
                                   Customer = t.Customer,
                                   ThumbnailImage = t.ThumbnailImage,
                                   ImageOld = t.ThumbnailImage,
                                   Content = t.Content,
                                   JobDescription = t.JobDescription,
                                   Priority = t.Priority,
                                   Size = t.Size,
                                   TimeProject = t.TimeProject,
                                   SubTitle = t.SubTitle,
                                   ProjectCategoryId = t.ProjectCategoryId,
                                   Address = t.Address,
                                   lstProjectImages = t.ProjectImages.Where(x => x.IsDeleted)
                                                       .Select(x=> new ProjectImageBO
                                                       {
                                                           Id = x.Id,
                                                           Priority = x.Priority,
                                                           ThumbnailImage = x.ThumbnailImage
                                                       }).ToList()
                               }).FirstOrDefault();
                return objProject;
            }
            catch (Exception objEx)
            {
                return new ProjectManageBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI Update(ProjectManageBO objModel)
        {
            try
            {
                if (objModel.Id == 0)
                {
                    return CreateProject(objModel);
                }
                return UpdateProject(objModel);
            }
            catch (Exception e)
            {
                return ResponseAPI.GetFailedResponse(e.Message);
            }
        }

        public static ResponseAPI CreateProject(ProjectManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                var objPageSlug = _entities.PageSlugs.Where(m => !m.IsDeleted && m.Slug.Equals(objModel.Slug)).FirstOrDefault();
                if (objPageSlug != null && objPageSlug.PageId == (int)EnumPage.ProjectDetail)
                {
                    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại. Vui lòng nhập đường dẫn mới");
                }

                objPageSlug.PageId = (int)EnumPage.ProjectDetail;

                var objProject = new Project();
                objProject.Title = objModel.Title;
                objProject.TimeProject = objModel.TimeProject;
                objProject.Priority = objModel.Priority;
                objProject.JobDescription = objModel.JobDescription;
                objProject.Address = objModel.Address;
                objProject.Content = objModel.Content;
                objProject.Customer = objModel.Customer;
                objProject.ProjectCategoryId = 1;
                objProject.Size = objModel.Size;
                objProject.Slug = objModel.Slug;
                objProject.SubTitle = objModel.SubTitle;
                objProject.IsDeleted = false;
                objProject.Created_at = DateTime.Now;
                objProject.Created_by = 1;

                if (objModel.UploadImage != null)
                {
                    var imgNameOld = objModel.ImageOld;

                    objProject.ThumbnailImage = objModel.UploadImage.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/project/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objProject.ThumbnailImage);
                }
                _entities.Projects.Add(objProject);
                _entities.SaveChanges();

                var idxImage = 1;
                foreach (var item in objModel.UploadMultiImage)
                {
                    ProjectImage objImage = new ProjectImage();
                    objImage.ProjectId = objProject.Id;
                    objImage.ThumbnailImage = item.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/project/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }
                    item.SaveAs(pathImages + objImage.ThumbnailImage);
                    objImage.Priority = idxImage;
                    objImage.IsDeleted = false;
                    objImage.Created_at = DateTime.Now;
                    objImage.Created_by = 1;
                    _entities.ProjectImages.Add(objImage);
                    idxImage++;
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

        public static ResponseAPI UpdateProject(ProjectManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                var objPageSlug = _entities.PageSlugs.Where(m => !m.IsDeleted && m.Slug.Equals(objModel.Slug)).ToList();
                if (objPageSlug.Count > 1)
                {
                    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại. Vui lòng nhập đường dẫn mới");
                }
                if (objPageSlug.Any(m => m.PageId != (int)EnumPage.ProjectDetail))
                {
                    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại. Vui lòng nhập đường dẫn mới");
                }
                if (objPageSlug.Count == 0)
                {
                    var newPageSlug = new PageSlug();
                    newPageSlug.PageId = (int)EnumPage.ProjectDetail;
                    newPageSlug.Slug = objModel.Slug;
                    newPageSlug.IsDeleted = false;
                    newPageSlug.Created_at = DateTime.Now;
                    newPageSlug.Created_by = 1;
                    _entities.PageSlugs.Add(newPageSlug);
                }
                else
                {
                    objPageSlug[0].PageId = (int)EnumPage.ProjectDetail;
                    objPageSlug[0].Slug = objModel.Slug;
                    objPageSlug[0].IsDeleted = false;
                    objPageSlug[0].Updated_at = DateTime.Now;
                    objPageSlug[0].Updated_by = 1;
                }

                var objProject = _entities.Projects.Where(x=>x.Id == objModel.Id).FirstOrDefault();
                if (objProject == null)
                    return ResponseAPI.GetFailedResponse("Không tìm thấy dự án để cập nhật");
                objProject.Title = objModel.Title;
                objProject.TimeProject = objModel.TimeProject;
                objProject.Priority = objModel.Priority;
                objProject.JobDescription = objModel.JobDescription;
                objProject.Address = objModel.Address;
                objProject.Content = objModel.Content;
                objProject.Customer = objModel.Customer;
                objProject.ProjectCategoryId = 1;
                objProject.Size = objModel.Size;
                objProject.Slug = objModel.Slug;
                objProject.SubTitle = objModel.SubTitle;
                objProject.IsDeleted = false;
                objProject.Updated_at = DateTime.Now;
                objProject.Updated_by = 1;

                string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/project/");
                if (objModel.UploadImage != null)
                {
                    var imgNameOld = objModel.ImageOld;

                    objProject.ThumbnailImage = objModel.UploadImage.GuidName();

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objProject.ThumbnailImage);
                }

                
                if(objModel.UploadMultiImage.Count > 1 && objModel.UploadMultiImage[0] != null)
                {
                    var lstProjectImages = objProject.ProjectImages.Where(x => !x.IsDeleted).ToList();
                    foreach (var itemImage in lstProjectImages)
                    {
                        itemImage.IsDeleted = true;
                        itemImage.Updated_at = DateTime.Now;
                        itemImage.Updated_by = 1;
                        if (File.Exists(pathImages + itemImage.ThumbnailImage))
                        {
                            File.Delete(pathImages + itemImage.ThumbnailImage);
                        }
                    }

                    var idxImage = 1;
                    foreach (var item in objModel.UploadMultiImage)
                    {
                        ProjectImage objImage = new ProjectImage();
                        objImage.ProjectId = objProject.Id;
                        objImage.ThumbnailImage = item.GuidName();


                        if (!Directory.Exists(pathImages))
                        {
                            Directory.CreateDirectory(pathImages);
                        }
                        item.SaveAs(pathImages + objImage.ThumbnailImage);
                        objImage.Priority = idxImage;
                        objImage.IsDeleted = false;
                        objImage.Created_at = DateTime.Now;
                        objImage.Created_by = 1;
                        _entities.ProjectImages.Add(objImage);
                        idxImage++;
                    }
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

        public static ResponseAPI Delete(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objProject = _entities.Projects.Where(m => m.Id == Id).FirstOrDefault();
                if (objProject == null)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy loại module để xóa");
                }
                objProject.Updated_at = DateTime.Now;
                objProject.Updated_by = 1;
                objProject.IsDeleted = true;

                string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/project/");
                var lstProjectImages = objProject.ProjectImages.Where(x => !x.IsDeleted).ToList();
                foreach (var itemImage in lstProjectImages)
                {
                    itemImage.IsDeleted = true;
                    itemImage.Updated_at = DateTime.Now;
                    itemImage.Updated_by = 1;
                    if (File.Exists(pathImages + itemImage.ThumbnailImage))
                    {
                        File.Delete(pathImages + itemImage.ThumbnailImage);
                    }
                }

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