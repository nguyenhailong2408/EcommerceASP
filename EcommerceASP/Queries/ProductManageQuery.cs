using EcommerceASP.Constaint;
using EcommerceASP.Libraries;
using EcommerceASP.Models;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.ProductManage;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class ProductManageQuery
    {
        public static ListViewModel GetListData(SearchFormViewModel objSearch)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = new ListViewModel();
                var lstView = (from m in _entities.Products
                               join p in _entities.ProductCategorys
                               on m.ProductCategoryId equals p.Id into P
                               from p in P.DefaultIfEmpty()
                               join d in _entities.ProductCategoryDetails
                               on m.ProductCategoryDetailId equals d.Id into D
                               from d in D.DefaultIfEmpty()
                               where !m.IsDeleted
                               && (string.IsNullOrEmpty(objSearch.NameProduct) || m.Name.Equals(objSearch.NameProduct))
                               && (string.IsNullOrEmpty(objSearch.Slug) || m.Slug.Equals(objSearch.Slug))
                               && (objSearch.ProductCategoryId == 0 || m.ProductCategoryId == objSearch.ProductCategoryId)
                               && (objSearch.ProductCategoryDetailId == 0 || m.ProductCategoryDetailId == objSearch.ProductCategoryDetailId)
                               select new ProductManageBO
                               {
                                   Id = m.Id,
                                   Image = m.Image,
                                   Infomation = m.Infomation,
                                   Name = m.Name,
                                   Price = m.Price,
                                   Price_sale = m.Price_sale,
                                   Slug = m.Slug,
                                   SortDescription = m.SortDescription,
                                   Description = m.Description,
                                   ProductCategoryInfo = m.ProductCategoryId + " - " + p.Name,
                                   ProductCategoryDetailInfo = m.ProductCategoryDetailId + " - " + d.Name
                               }).ToList();
                list.Items = lstView.ToPagedList((objSearch.PageCurrent ?? 1) - 1, list.PageSize ?? 10);
                return list;
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

        public static ProductManageBO GetDataUpdate(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objProduct = new ProductManageBO();
                if (Id == null)
                    return objProduct;
                objProduct = _entities.Products
                    .Where(m => !m.IsDeleted && m.Id == Id)
                    .Select(m => new ProductManageBO
                    {
                        Id = m.Id,
                        Image = m.Image,
                        ImageOld = m.Image,
                        Infomation = m.Infomation,
                        Name = m.Name,
                        Price = m.Price,
                        Price_sale = m.Price_sale,
                        Slug = m.Slug,
                        SortDescription = m.SortDescription,
                        Description = m.Description,
                        ProductCategoryId = m.ProductCategoryId,
                        ProductCategoryDetailId = m.ProductCategoryDetailId,
                        ProductCategoryInfo = m.ProductCategoryId.ToString(),
                        ProductCategoryDetailInfo = m.ProductCategoryDetailId.ToString()
                    }).FirstOrDefault();

                return objProduct;
            }
            catch (Exception objEx)
            {
                return new ProductManageBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI Update(ProductManageBO objModel)
        {
            try
            {
                if (objModel.Id == 0)
                {
                    return CreateProduct(objModel);
                }
                return UpdateProduct(objModel);
            }
            catch (Exception objEx)
            {
                return ResponseAPI.GetFailedResponse(objEx.Message);
            }
        }

        public static ResponseAPI CreateProduct(ProductManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                if (string.IsNullOrWhiteSpace(objModel.Name))
                {
                    return ResponseAPI.GetFailedResponse("Vui lòng nhập tên sản phẩm");
                }
                if (string.IsNullOrWhiteSpace(objModel.Slug))
                {
                    return ResponseAPI.GetFailedResponse("Vui lòng nhập đường dẫn đến trang chi tiết sản phẩm");
                }
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                var objPageSlug = _entities.PageSlugs.Where(m => !m.IsDeleted && m.Slug.Equals(objModel.Slug)).FirstOrDefault();
                if (objPageSlug != null)
                {
                    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại. Vui lòng nhập đường dẫn mới");
                }

                objPageSlug = new PageSlug();
                objPageSlug.PageId = (int)EnumPage.ProductDetail;
                objPageSlug.Slug = objModel.Slug;
                objPageSlug.IsDeleted = false;
                objPageSlug.Created_at = DateTime.Now;
                objPageSlug.Created_by = 1;
                _entities.PageSlugs.Add(objPageSlug);


                var objProduct = new Product();
                objProduct.Name = objModel.Name;
                objProduct.ProductCategoryId = objModel.ProductCategoryId;
                objProduct.ProductCategoryDetailId = objModel.ProductCategoryDetailId;
                objProduct.Slug = objModel.Slug;
                //objProduct.SortDescription = objModel.SortDescription;
                objProduct.Description = objModel.Description;
                objProduct.Infomation = objModel.Infomation;
                objProduct.StockQuantity = 1;
                objProduct.Created_at = DateTime.Now;
                objProduct.Created_by = 1;
                objProduct.IsDeleted = false;

                if (objModel.UploadImage != null)
                {
                    var imgNameOld = objModel.ImageOld;

                    objModel.Image = objModel.UploadImage.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/product/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objModel.Image);
                }
                _entities.Products.Add(objProduct);
                _entities.SaveChanges();
                return ResponseAPI.GetSuccessResponse("Success", null);
            }
            catch (DbEntityValidationException objEx)
            {
                return ResponseAPI.GetFailedResponse(objEx.Message);
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI UpdateProduct(ProductManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                if (string.IsNullOrWhiteSpace(objModel.Name))
                {
                    return ResponseAPI.GetFailedResponse("Vui lòng nhập tên sản phẩm");
                }
                if (string.IsNullOrWhiteSpace(objModel.Slug))
                {
                    return ResponseAPI.GetFailedResponse("Vui lòng nhập đường dẫn đến trang chi tiết sản phẩm");
                }
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                var objPageSlug = _entities.PageSlugs.Where(m => !m.IsDeleted && m.Slug.Equals(objModel.Slug)).ToList();
                if (objPageSlug.Count > 1)
                {
                    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại. Vui lòng nhập đường dẫn mới");
                }
                if (objPageSlug.Any(m => m.PageId != (int)EnumPage.ProductDetail))
                {
                    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại. Vui lòng nhập đường dẫn mới");
                }
                if (objPageSlug.Count == 0)
                {
                    var newPageSlug = new PageSlug();
                    newPageSlug.PageId = (int)EnumPage.ProductDetail;
                    newPageSlug.Slug = objModel.Slug;
                    newPageSlug.IsDeleted = false;
                    newPageSlug.Created_at = DateTime.Now;
                    newPageSlug.Created_by = 1;
                    _entities.PageSlugs.Add(newPageSlug);
                }
                else
                {
                    objPageSlug[0].PageId = (int)EnumPage.ProductDetail;
                    objPageSlug[0].Slug = objModel.Slug;
                    objPageSlug[0].IsDeleted = false;
                    objPageSlug[0].Updated_at = DateTime.Now;
                    objPageSlug[0].Updated_by = 1;
                }


                var objProduct = _entities.Products.Where(m => m.Id == objModel.Id).FirstOrDefault();
                if (objProduct == null)
                    return ResponseAPI.GetFailedResponse("Không tìm thấy sản phẩm để cập nhật");
                objProduct.Name = objModel.Name;
                objProduct.ProductCategoryId = objModel.ProductCategoryId;
                objProduct.ProductCategoryDetailId = objModel.ProductCategoryDetailId;
                objProduct.Slug = objModel.Slug;
                //objProduct.SortDescription = objModel.SortDescription;
                objProduct.Description = objModel.Description;
                objProduct.Infomation = objModel.Infomation;
                objProduct.StockQuantity = 1;
                objProduct.Price = 1;
                objProduct.Price_sale = 1;
                objProduct.Updated_at = DateTime.Now;
                objProduct.Updated_by = 1;
                objProduct.IsDeleted = false;
                if (objModel.UploadImage != null)
                {
                    var imgNameOld = objModel.ImageOld;

                    objModel.Image = objModel.UploadImage.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/product/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objModel.Image);
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

        public static ResponseAPI DeleteProduct(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objProduct = _entities.Products.Where(m => m.Id == Id).FirstOrDefault();
                if (objProduct == null)
                {
                    return ResponseAPI.GetSuccessResponse("Xóa thành công", null);
                }
                var lstPageSlug = _entities.PageSlugs.Where(m => !m.IsDeleted
                                                                && m.Slug.Equals(objProduct.Slug)
                                                                && m.PageId == (int)EnumPage.ProductDetail).ToList();
                foreach (var itemPageSlug in lstPageSlug)
                {
                    itemPageSlug.IsDeleted = true;
                    itemPageSlug.Updated_at = DateTime.Now;
                    itemPageSlug.Updated_by = 1;
                }

                objProduct.Updated_at = DateTime.Now;
                objProduct.Updated_by = 1;
                objProduct.IsDeleted = true;

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