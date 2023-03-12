using EcommerceASP.Constaint;
using EcommerceASP.Libraries;
using EcommerceASP.Models;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.CategoryManage;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace EcommerceASP.Queries
{
    public class CategoryManageQuery
    {
        public static ListViewModel GetListData(SearchFormViewModel objSearch)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objView = new ListViewModel();
                var lstPost = new List<CategoryManageBO>();

                lstPost = (from t in _entities.Categorys
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
                                 && (string.IsNullOrEmpty(objSearch.Slug) || t.Slug.Contains(objSearch.Slug))
                           select new CategoryManageBO
                           {
                               Id = t.Id,
                               Name = t.Name,
                               Priority = t.Priority,
                               Slug = t.Slug,
                               PageId = p == null ? 0 : p.PageId,
                               PageName = p == null ? String.Empty : p.Page.Name,
                               FolderImage = t.FolderImage,
                               Created_at = t.Created_at == null ? t.Updated_at : t.Created_at,
                               CreatedByName = string.IsNullOrEmpty(s.FullName) ? c.FullName : s.FullName
                           }).ToList();

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

        public static CategoryManageBO GetDataUpdate(int? CategoryId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objCategory = new CategoryManageBO();
                if (CategoryId == null)
                    return objCategory;
                objCategory = (from t in _entities.Categorys
                               join p in _entities.PageSlugs
                               on t.Slug equals p.Slug into P
                               from p in P.DefaultIfEmpty()
                               where !t.IsDeleted
                                     && t.Id == CategoryId
                               select new CategoryManageBO
                               {
                                   Id = t.Id,
                                   Name = t.Name,
                                   Priority = t.Priority,
                                   Slug = t.Slug,
                                   PageId = p == null ? 0 : p.PageId,
                                   PageName = p == null ? String.Empty : p.Page.Name,
                                   FolderImage = t.FolderImage
                               }).FirstOrDefault();
                if (objCategory != null)
                    objCategory.Slug = objCategory.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                return objCategory;
            }
            catch (Exception objEx)
            {
                return new CategoryManageBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI Update(CategoryManageBO objModel)
        {
            try
            {
                if (objModel.Id == 0)
                {
                    return CreateCategory(objModel);
                }
                return UpdateCategory(objModel);
            }
            catch (Exception objEx)
            {
                return ResponseAPI.GetFailedResponse(objEx.Message);
            }
        }

        public static ResponseAPI CreateCategory(CategoryManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                if (string.IsNullOrWhiteSpace(objModel.Name))
                {
                    return ResponseAPI.GetFailedResponse("Vui lòng nhập tên danh mục!");
                }
                if (string.IsNullOrWhiteSpace(objModel.Slug))
                {
                    return ResponseAPI.GetFailedResponse("Vui lòng nhập đường dẫn cho danh mục!");
                }
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();

                var lstPage = CommonQuery.GetLstPageBySlug(objModel.Slug);
                if (lstPage.Count < 1)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy đường dẫn. Vui lòng tạo nội dung cho đường dẫn trước!");
                }
                

                var objCategory = new Category();
                objCategory.Name = objModel.Name;
                objCategory.Slug = objModel.Slug;
                objCategory.Priority = objModel.Priority;
                objCategory.FolderImage = objModel.PageId == (int)EnumPage.Product ? "product" : "topic";
                objCategory.TemplateOptionId = objModel.PageId == (int)EnumPage.Product ? 0 : 1;
                objCategory.IsDeleted = false;
                objCategory.Created_at = DateTime.Now;
                objCategory.Created_by = 1;

                _entities.Categorys.Add(objCategory);
                _entities.SaveChanges();
                return ResponseAPI.GetSuccessResponse("Success", null);
            }
            catch (Exception objEx)
            {
                return ResponseAPI.GetFailedResponse(objEx.Message);
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI UpdateCategory(CategoryManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                var objCategory = _entities.Categorys.Where(m => m.Id == objModel.Id).FirstOrDefault();
                if (objCategory == null)
                    return ResponseAPI.GetFailedResponse("Không tìm thấy chủ đề bài viết để cập nhật");

                if (string.IsNullOrWhiteSpace(objModel.Name))
                {
                    return ResponseAPI.GetFailedResponse("Vui lòng nhập tên danh mục!");
                }
                if (string.IsNullOrWhiteSpace(objModel.Slug))
                {
                    return ResponseAPI.GetFailedResponse("Vui lòng nhập đường dẫn cho danh mục!");
                }
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();

                var lstPage = CommonQuery.GetLstPageBySlug(objModel.Slug);
                if (lstPage.Count < 1)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy đường dẫn. Vui lòng tạo nội dung cho đường dẫn trước!");
                }

                objCategory.Name = objModel.Name;
                objCategory.Slug = objModel.Slug;
                objCategory.Priority = objModel.Priority;
                objCategory.FolderImage = objModel.PageId == (int)EnumPage.Product ? "product": "topic";
                objCategory.TemplateOptionId = objModel.PageId == (int)EnumPage.Product ? 0 : 1;
                objCategory.IsDeleted = false;
                objCategory.Updated_at = DateTime.Now;
                objCategory.Updated_by = 1;

                _entities.SaveChanges();
                return ResponseAPI.GetSuccessResponse("Success", null);
            }
            catch (Exception e)
            {
                //foreach (var eve in e.EntityValidationErrors)
                //{
                //    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                //        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                //    foreach (var ve in eve.ValidationErrors)
                //    {
                //        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                //            ve.PropertyName, ve.ErrorMessage);
                //    }
                //}
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
                var objCategory = _entities.Categorys.Where(m => m.Id == Id).FirstOrDefault();
                if (objCategory == null)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy danh mục để xóa");
                }

                objCategory.Updated_at = DateTime.Now;
                objCategory.Updated_by = 1;
                objCategory.IsDeleted = true;

                _entities.SaveChanges();
                return ResponseAPI.GetSuccessResponse("Xóa thành công", null);
            }
            catch (Exception e)
            {
                //foreach (var eve in e.EntityValidationErrors)
                //{
                //    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                //        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                //    foreach (var ve in eve.ValidationErrors)
                //    {
                //        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                //            ve.PropertyName, ve.ErrorMessage);
                //    }
                //}
                return ResponseAPI.GetFailedResponse(e.Message);
            }
            finally
            {
                _entities.Dispose();
            }
        }
    }
}