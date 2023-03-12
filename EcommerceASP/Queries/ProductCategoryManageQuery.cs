using EcommerceASP.Constaint;
using EcommerceASP.Libraries;
using EcommerceASP.Models;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.ProductCategoryManage;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace EcommerceASP.Queries
{
    public class ProductCategoryManageQuery
    {
        public static ListViewModel GetListData(SearchFormViewModel objSearch)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objView = new ListViewModel();
                var lstPost = new List<ProductCategoryManageBO>();

                lstPost = (from t in _entities.ProductCategorys
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
                           select new ProductCategoryManageBO
                           {
                               Id = t.Id,
                               Name = t.Name,
                               Priority = t.Priority,
                               Slug = t.Slug,
                               PageId = p == null ? 0 : p.PageId,
                               PageName = p == null ? String.Empty : p.Page.Name,
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

        public static ProductCategoryManageBO GetDataUpdate(int? CategoryId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objProductCategory = new ProductCategoryManageBO();
                if (CategoryId == null)
                    return objProductCategory;
                objProductCategory = (from t in _entities.ProductCategorys
                                      join p in _entities.PageSlugs
                                      on t.Slug equals p.Slug into P
                                      from p in P.DefaultIfEmpty()
                                      where !t.IsDeleted
                                            && t.Id == CategoryId
                                      select new ProductCategoryManageBO
                                      {
                                          Id = t.Id,
                                          Name = t.Name,
                                          Priority = t.Priority,
                                          Slug = t.Slug,
                                          SlugOld = t.Slug,
                                          PageId = p == null ? 0 : p.PageId,
                                          PageName = p == null ? String.Empty : p.Page.Name,
                                      }).FirstOrDefault();
                if (objProductCategory != null)
                    objProductCategory.Slug = objProductCategory.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                return objProductCategory;
            }
            catch (Exception objEx)
            {
                return new ProductCategoryManageBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI Update(ProductCategoryManageBO objModel)
        {
            try
            {
                if (objModel.Id == 0)
                {
                    return CreateProductCategory(objModel);
                }
                return UpdateProductCategory(objModel);
            }
            catch (Exception objEx)
            {
                return ResponseAPI.GetFailedResponse(objEx.Message);
            }
        }

        public static ResponseAPI CreateProductCategory(ProductCategoryManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                if (objModel.PageId != 0 && objModel.PageId != (int)EnumPage.Product)
                {
                    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại ở trang khác.\n Vui lòng nhập đường dẫn mới!");
                }
                else
                {
                    var pageSlug = _entities.PageSlugs.Where(m => m.Slug.Equals(objModel.Slug) && m.PageId == (int)EnumPage.Product).FirstOrDefault();
                    if (pageSlug == null)
                    {
                        pageSlug = new PageSlug();
                        pageSlug.PageId = (int)EnumPage.Product;
                        pageSlug.Slug = objModel.Slug;
                        pageSlug.IsDeleted = false;
                        pageSlug.Created_at = DateTime.Now;
                        pageSlug.Created_by = 1;
                        _entities.PageSlugs.Add(pageSlug);
                    }
                }
                var objProductCategory = new ProductCategory();
                objProductCategory.Name = objModel.Name;
                objProductCategory.Slug = objModel.Slug;
                objProductCategory.Priority = objModel.Priority;
                objProductCategory.IsDeleted = false;
                objProductCategory.Created_at = DateTime.Now;
                objProductCategory.Created_by = 1;

                _entities.ProductCategorys.Add(objProductCategory);
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

        public static ResponseAPI UpdateProductCategory(ProductCategoryManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                objModel.SlugOld = objModel.SlugOld.NonUnicode().Split(' ').Join("-").ToLower();
                if (objModel.PageId != 0 && objModel.PageId != (int)EnumPage.Product)
                {
                    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại ở trang khác.\n Vui lòng nhập đường dẫn mới!");
                }
                else
                {
                    var pageSlug = _entities.PageSlugs.Where(m => m.Slug.Equals(objModel.Slug) && m.PageId == (int)EnumPage.Product).FirstOrDefault();
                    if (pageSlug == null)
                    {
                        pageSlug = new PageSlug();
                        pageSlug.PageId = (int)EnumPage.Product;
                        pageSlug.Slug = objModel.Slug;
                        pageSlug.IsDeleted = false;
                        pageSlug.Created_at = DateTime.Now;
                        pageSlug.Created_by = 1;
                        _entities.PageSlugs.Add(pageSlug);
                    }
                }

                var objProductCategory = _entities.ProductCategorys.Where(m => m.Id == objModel.Id).FirstOrDefault();
                if (objProductCategory == null)
                    return ResponseAPI.GetFailedResponse("Không tìm thấy chủ đề bài viết để cập nhật");

                objProductCategory.Name = objModel.Name;
                objProductCategory.Slug = objModel.Slug;
                objProductCategory.Priority = objModel.Priority;
                objProductCategory.IsDeleted = false;
                objProductCategory.Updated_at = DateTime.Now;
                objProductCategory.Updated_by = 1;

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
                var objProductCategory = _entities.ProductCategorys.Where(m => m.Id == Id).FirstOrDefault();
                if (objProductCategory == null)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy danh mục để xóa");
                }

                objProductCategory.Updated_at = DateTime.Now;
                objProductCategory.Updated_by = 1;
                objProductCategory.IsDeleted = true;

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