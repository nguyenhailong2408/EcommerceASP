using EcommerceASP.Constaint;
using EcommerceASP.Libraries;
using EcommerceASP.Models;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.CategoryDetailManage;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class CategoryDetailManageQuery
    {
        public static ListViewModel GetListData(SearchFormViewModel objSearch)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objView = new ListViewModel();
                var lstPost = new List<CategoryDetailManageBO>();
                lstPost = (from t in _entities.CategoryDetails
                           join t2 in _entities.CategoryDetails
                           on t.ParentId equals t2.Id into C
                           from tt2 in C.DefaultIfEmpty()
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
                                 && (objSearch.CategoryId == 0 || t.CategoryID == objSearch.CategoryId)
                                 && (objSearch.ParentId == 0 || t.ParentId == objSearch.ParentId)
                                 && (string.IsNullOrEmpty(objSearch.Name) || t.Name.Contains(objSearch.Name))
                                 && (string.IsNullOrEmpty(objSearch.Slug) || t.Slug.Contains(objSearch.Slug))
                           select new CategoryDetailManageBO
                           {
                               Id = t.Id,
                               Name = t.Name,
                               Priority = t.Priority,
                               Slug = t.Slug,
                               PageId = p.PageId,
                               PageInfo = p.PageId + " - " + p.Page.Name,
                               CategoryName = t.Category.Name,
                               CategoryID = t.CategoryID,
                               ParentId = t.ParentId,
                               ParentName = t.ParentId == 0 ? t.Category.Name : tt2.Name,
                               IsHasBanner = t.IsHasBanner,
                               BannerImage = t.BannerImage,
                               Created_at = t.Created_at == null ? t.Updated_at : t.Created_at,
                               CreatedByName = string.IsNullOrEmpty(s.FullName) ? c.FullName : s.FullName
                           })
                           .OrderBy(m => m.CategoryID)
                           .ThenBy(m => m.ParentId)
                           .ThenBy(m => m.Priority)
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

        public static CategoryDetailManageBO GetDataUpdate(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objCategory = new CategoryDetailManageBO();
                if (Id == null)
                    return objCategory;

                objCategory = (from t in _entities.CategoryDetails
                               join p in _entities.PageSlugs
                               on new { t.Slug, t.IsDeleted } equals new { p.Slug, p.IsDeleted } into P
                               from p in P.DefaultIfEmpty()
                               where !t.IsDeleted
                                     && t.Id == Id
                               select new CategoryDetailManageBO
                               {
                                   Id = t.Id,
                                   Name = t.Name,
                                   Priority = t.Priority,
                                   Slug = t.Slug,
                                   PageId = p.PageId,
                                   CategoryName = t.Category.Name,
                                   CategoryID = t.CategoryID,
                                   ParentId = t.ParentId,
                                   ImageOld = t.BannerImage,
                                   IsHasBanner = t.IsHasBanner,
                                   BannerImage = t.BannerImage
                               }).FirstOrDefault();
                if (objCategory != null)
                    objCategory.Slug = objCategory.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                return objCategory;
            }
            catch (Exception objEx)
            {
                return new CategoryDetailManageBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI Update(CategoryDetailManageBO objModel)
        {
            try
            {
                if (objModel.Id == 0)
                {
                    return CreateCategory(objModel);
                }
                return UpdateCategory(objModel);
            }
            catch (Exception e)
            {
                return ResponseAPI.GetFailedResponse(e.Message);
            }
        }

        public static ResponseAPI CreateCategory(CategoryDetailManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();

                var lstProductCat = new List<string>();
                var lstProductCatName = new List<string>();
                var lstProductCatPriority = new List<int>();
                lstProductCat.Add(objModel.Slug);
                lstProductCatName.Add(objModel.Name);
                lstProductCatPriority.Add(objModel.Priority);
                var productCatId = 0;

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
                // Phần này tự động duyệt tạo mới /  bổ sung cho danh mục sản phẩm còn thiếu
                if (objModel.ParentId == 0) // Nếu cha = 0 thì kiểm tra đã có tạo danh mục sản phẩm chưa
                {
                    var objProductCat = _entities.ProductCategorys.Where(m => m.Slug == objModel.Slug).FirstOrDefault();
                    if (objProductCat == null)
                    {
                        ProductCategory productCategory = new ProductCategory();
                        productCategory.Name = objModel.Name;
                        productCategory.Slug = objModel.PageId == 2 ? objModel.Slug : String.Empty;
                        productCategory.Priority = objModel.Priority;
                        productCategory.IsDeleted = false;
                        productCategory.Created_at = DateTime.Now;
                        productCategory.Created_by = 1;
                        _entities.ProductCategorys.Add(productCategory);
                        _entities.SaveChanges();

                        productCatId = productCategory.Id;
                    }
                    else
                    {
                        productCatId = objProductCat.Id;
                    }
                }
                else
                {
                    // Nếu là trang sản phẩm thì duyệt ngược lấy danh sách cha => rồi tự động tạo thêm danh mục sản phẩm
                    var pageSlug = _entities.PageSlugs.Where(m => !m.IsDeleted && m.Slug.Equals(objModel.Slug) && m.PageId == (int)EnumPage.Product).FirstOrDefault();
                    if (pageSlug != null)
                    {
                        int count = 0;
                        int parentTempId = objModel.ParentId;
                        for (int i = 0; i <= 20; i++)
                        {
                            count++;
                            var objCat = _entities.CategoryDetails.Where(m => m.Id == parentTempId).FirstOrDefault();
                            parentTempId = objCat.ParentId;
                            lstProductCat.Add(objCat.Slug.NonUnicode().Split(' ').Join("-").ToLower());
                            lstProductCatName.Add(objCat.Name);
                            lstProductCatPriority.Add(objCat.Priority);
                            if (objCat.ParentId == 0)
                                break;
                        }

                        var productCatDetailId = 0;
                        for (int i = lstProductCat.Count; i > 0; i--)
                        {
                            string slugTemp = lstProductCat[i - 1];
                            string nameTemp = lstProductCatName[i - 1];
                            int priorityTemp = lstProductCatPriority[i - 1];
                            if (i == lstProductCat.Count)
                            {
                                var objProductCat = _entities.ProductCategorys.Where(m => m.Name == nameTemp).FirstOrDefault();
                                if (objProductCat == null)
                                {
                                    ProductCategory productCategory = new ProductCategory();
                                    productCategory.Name = nameTemp;
                                    productCategory.Slug = objModel.PageId == 2 ? slugTemp : String.Empty;
                                    productCategory.Priority = priorityTemp;
                                    productCategory.IsDeleted = false;
                                    productCategory.Created_at = DateTime.Now;
                                    productCategory.Created_by = 1;
                                    _entities.ProductCategorys.Add(productCategory);
                                    _entities.SaveChanges();

                                    productCatId = productCategory.Id;
                                }
                                else
                                {
                                    productCatId = objProductCat.Id;
                                }
                            }
                            else
                            {
                                var productCatDetail = _entities.ProductCategoryDetails.Where(m => m.Name == nameTemp).FirstOrDefault();
                                if (productCatDetail == null)
                                {
                                    ProductCategoryDetail productCategoryDetail = new ProductCategoryDetail();
                                    productCategoryDetail.Name = nameTemp;
                                    productCategoryDetail.ProductCategoryID = productCatId;
                                    productCategoryDetail.Slug = objModel.PageId == 2 ? slugTemp : String.Empty;
                                    productCategoryDetail.Priority = priorityTemp;
                                    productCategoryDetail.ParentId = productCatDetailId;
                                    productCategoryDetail.IsDeleted = false;
                                    productCategoryDetail.Created_at = DateTime.Now;
                                    productCategoryDetail.Created_by = 1;
                                    _entities.ProductCategoryDetails.Add(productCategoryDetail);
                                    _entities.SaveChanges();

                                    productCatDetailId = productCategoryDetail.Id;
                                }
                                else
                                {
                                    productCatDetailId = productCatDetail.Id;
                                }
                            }
                        }
                    }
                }

                var objCategory = new CategoryDetail();
                objCategory.Name = objModel.Name;
                objCategory.Slug = objModel.Slug;
                objCategory.CategoryID = objModel.CategoryID;
                objCategory.ParentId = objModel.ParentId;
                objCategory.Priority = objModel.Priority;
                if (productCatId != 0)
                {
                    objCategory.ProductCategoryId = productCatId;
                }
                objCategory.IsHasBanner = objModel.IsHasBanner;
                objCategory.IsDeleted = false;
                objCategory.Created_at = DateTime.Now;
                objCategory.Created_by = 1;

                if (objModel.UploadImage != null && objModel.IsHasBanner)
                {
                    var imgNameOld = objModel.ImageOld;

                    objCategory.BannerImage = objModel.UploadImage.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/banner/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objCategory.BannerImage);
                }

                _entities.CategoryDetails.Add(objCategory);
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

        public static ResponseAPI UpdateCategory(CategoryDetailManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();

                var lstProductCatSlug = new List<string>();
                var lstProductCatName = new List<string>();
                var lstProductCatPriority = new List<int>();
                lstProductCatSlug.Add(objModel.Slug);
                lstProductCatName.Add(objModel.Name);
                lstProductCatPriority.Add(objModel.Priority);
                var productCatId = 0;

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

                if (objModel.ParentId == 0)
                {
                    var objProductCat = _entities.ProductCategorys.Where(m => m.Name.Equals(objModel.Name)).FirstOrDefault();
                    if (objProductCat == null)
                    {
                        ProductCategory productCategory = new ProductCategory();
                        productCategory.Name = objModel.Name;
                        productCategory.Slug = objModel.PageId == 2 ? objModel.Slug : String.Empty;
                        productCategory.Priority = objModel.Priority;
                        productCategory.IsDeleted = false;
                        productCategory.Created_at = DateTime.Now;
                        productCategory.Created_by = 1;
                        _entities.ProductCategorys.Add(productCategory);
                        _entities.SaveChanges();

                        productCatId = productCategory.Id;
                    }
                    else
                    {
                        productCatId = objProductCat.Id;
                    }
                }
                else
                {
                    // Nếu là trang sản phẩm thì duyệt ngược lấy danh sách cha => rồi tự động tạo thêm danh mục sản phẩm
                    var pageSlug = _entities.PageSlugs.Where(m => !m.IsDeleted && m.Slug.Equals(objModel.Slug) && m.PageId == (int)EnumPage.Product).FirstOrDefault();
                    if (pageSlug != null)
                    {
                        int count = 0;
                        int parentTempId = objModel.ParentId;

                        // Xử lý duyệt liên tục lấy nhiều cấp cha
                        for (int i = 0; i <= 20; i++)
                        {
                            count++;
                            var objCat = _entities.CategoryDetails.Where(m => m.Id == parentTempId).FirstOrDefault();
                            // Nếu ID cha khác 0 thì gán lại tìm kiếm tiếp
                            parentTempId = objCat.ParentId;
                            lstProductCatSlug.Add(objCat.Slug.NonUnicode().Split(' ').Join("-").ToLower());
                            lstProductCatName.Add(objCat.Name);
                            lstProductCatPriority.Add(objCat.Priority);
                            if (objCat.ParentId == 0) // Id cha  = 0 => đây là cấp cao nhất của danh mục
                                break;
                        }

                        var productCatDetailId = 0;
                        // Duyệt ngược slug tạo danh mục sản phẩm theo phân cấp
                        for (int i = lstProductCatSlug.Count; i > 0; i--)
                        {
                            string slugTemp = lstProductCatSlug[i - 1];
                            string nameTemp = lstProductCatName[i - 1];
                            int priorityTemp = lstProductCatPriority[i - 1];
                            if (i == lstProductCatSlug.Count)
                            {
                                var objProductCat = _entities.ProductCategorys.Where(m => m.Name == nameTemp).FirstOrDefault();
                                if (objProductCat == null)
                                {
                                    ProductCategory productCategory = new ProductCategory();
                                    productCategory.Name = nameTemp;
                                    productCategory.Slug = objModel.PageId == 2 ? slugTemp : String.Empty;
                                    productCategory.Priority = priorityTemp;
                                    productCategory.IsDeleted = false;
                                    productCategory.Created_at = DateTime.Now;
                                    productCategory.Created_by = 1;
                                    _entities.ProductCategorys.Add(productCategory);
                                    _entities.SaveChanges();

                                    productCatId = productCategory.Id;
                                }
                                else
                                {
                                    productCatId = objProductCat.Id;
                                }
                            }
                            else
                            {
                                var productCatDetail = _entities.ProductCategoryDetails.Where(m => m.Name == nameTemp).FirstOrDefault();
                                if (productCatDetail == null)
                                {
                                    ProductCategoryDetail productCategoryDetail = new ProductCategoryDetail();
                                    productCategoryDetail.Name = nameTemp;
                                    productCategoryDetail.ProductCategoryID = productCatId;
                                    productCategoryDetail.Slug = objModel.PageId == 2 ? slugTemp : String.Empty;
                                    productCategoryDetail.Priority = priorityTemp;
                                    productCategoryDetail.ParentId = productCatDetailId;
                                    productCategoryDetail.IsDeleted = false;
                                    productCategoryDetail.Created_at = DateTime.Now;
                                    productCategoryDetail.Created_by = 1;
                                    _entities.ProductCategoryDetails.Add(productCategoryDetail);
                                    _entities.SaveChanges();

                                    productCatDetailId = productCategoryDetail.Id;
                                }
                                else
                                {
                                    productCatDetailId = productCatDetail.Id;
                                }
                            }
                        }
                    }
                }

                var objCategory = _entities.CategoryDetails.Where(m => m.Id == objModel.Id).FirstOrDefault();
                if (objCategory == null)
                    return ResponseAPI.GetFailedResponse("Không tìm thấy danh mục để cập nhật");

                objCategory.Name = objModel.Name;
                objCategory.Slug = objModel.Slug;
                objCategory.CategoryID = objModel.CategoryID;
                objCategory.ParentId = objModel.ParentId;
                if (productCatId != 0)
                {
                    objCategory.ProductCategoryId = productCatId;
                }
                objCategory.Priority = objModel.Priority;
                objCategory.IsHasBanner = objModel.IsHasBanner;
                objCategory.IsDeleted = false;
                objCategory.Updated_at = DateTime.Now;
                objCategory.Updated_by = 1;

                if (objModel.UploadImage != null && objModel.IsHasBanner)
                {
                    var imgNameOld = objModel.ImageOld;

                    objCategory.BannerImage = objModel.UploadImage.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/banner/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objCategory.BannerImage);
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
                var objCategory = _entities.CategoryDetails.Where(m => m.Id == Id).FirstOrDefault();
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