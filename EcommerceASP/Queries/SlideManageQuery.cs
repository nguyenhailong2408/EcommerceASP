using EcommerceASP.Constaint;
using EcommerceASP.Libraries;
using EcommerceASP.Models;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.SlideManage;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class SlideManageQuery
    {
        public static ListViewModel GetListData(SearchFormViewModel objSearch)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objView = new ListViewModel();
                var lstPost = new List<SlideManageBO>();
                lstPost = (from t in _entities.Slides
                           join p in _entities.PageSlugs
                               on new { t.Slug, t.IsDeleted } equals new { p.Slug, p.IsDeleted } into P
                           from p in P.DefaultIfEmpty()
                           join a in _entities.Accounts
                           on t.Created_by equals a.Id into T
                           from s in T.DefaultIfEmpty()
                           join ac in _entities.Accounts
                           on t.Updated_by equals ac.Id into AC
                           from c in AC.DefaultIfEmpty()
                           where !t.IsDeleted
                                 && (string.IsNullOrEmpty(objSearch.Name) || t.Name.Contains(objSearch.Name))
                           select new SlideManageBO
                           {
                               Id = t.Id,
                               Name = t.Name,
                               Priority = t.Priority,
                               Slug = t.Slug,
                               Image = t.Image,
                               ImageOld = t.Image,
                               PageId = p == null ? 0 : p.PageId,
                               PageName = p == null ? String.Empty : p.Page.Name,
                           })
                           .OrderBy(m => m.Priority)
                           .ToList();

                objView.Items = lstPost.ToPagedList((objSearch.PageCurrent ?? 1) - 1, objView.PageSize ?? 10);
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

        public static SlideManageBO GetDataUpdate(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objCategory = new SlideManageBO();
                if (Id == null)
                    return objCategory;

                objCategory = (from t in _entities.Slides
                               join p in _entities.PageSlugs
                               on new { t.Slug, t.IsDeleted } equals new { p.Slug, p.IsDeleted } into P
                               from p in P.DefaultIfEmpty()
                               where !t.IsDeleted
                                     && t.Id == Id
                               select new SlideManageBO
                               {
                                   Id = t.Id,
                                   Name = t.Name,
                                   Priority = t.Priority,
                                   Slug = t.Slug,
                                   Image = t.Image,
                                   ImageOld = t.Image,
                                   PageId = p == null ? 0 : p.PageId
                               }).FirstOrDefault();
                if (objCategory != null)
                    objCategory.Slug = objCategory.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                return objCategory;
            }
            catch (Exception objEx)
            {
                return new SlideManageBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI Update(SlideManageBO objModel)
        {
            try
            {
                if (objModel.Id == 0)
                {
                    return CreateSlide(objModel);
                }
                return UpdateSlide(objModel);
            }
            catch (Exception e)
            {
                return ResponseAPI.GetFailedResponse(e.Message);
            }
        }

        public static ResponseAPI CreateSlide(SlideManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objSlide = new Slide();
                objSlide.Name = objModel.Name;
                objSlide.Priority = objModel.Priority;
                objSlide.Slug = objModel.Slug;
                objSlide.Position = "Slide Show";
                objSlide.IsDeleted = false;
                objSlide.Created_at = DateTime.Now;
                objSlide.Created_by = 1;

                if (objModel.UploadImage != null)
                {
                    var imgNameOld = objModel.ImageOld;

                    objSlide.Image = objModel.UploadImage.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/slide/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objSlide.Image);
                }

                _entities.Slides.Add(objSlide);
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

        public static ResponseAPI UpdateSlide(SlideManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objSlide = _entities.Slides.Where(m => m.Id == objModel.Id).FirstOrDefault();
                if (objSlide == null)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy thông tin slide để cập nhật");
                }
                objSlide.Name = objModel.Name;
                objSlide.Priority = objModel.Priority;
                objSlide.Slug = objModel.Slug;
                objSlide.Position = "Slide Show";
                objSlide.IsDeleted = false;
                objSlide.Updated_at = DateTime.Now;
                objSlide.Updated_by = 1;

                if (objModel.UploadImage != null)
                {
                    var imgNameOld = objModel.ImageOld;

                    objSlide.Image = objModel.UploadImage.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/slide/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objSlide.Image);
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
                var objSlide = _entities.Slides.Where(m => m.Id == Id).FirstOrDefault();
                if (objSlide == null)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy slide để xóa");
                }
                objSlide.Updated_at = DateTime.Now;
                objSlide.Updated_by = 1;
                objSlide.IsDeleted = true;

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