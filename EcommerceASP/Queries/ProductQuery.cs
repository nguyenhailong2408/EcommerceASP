using EcommerceASP.Models;
using EcommerceASP.ViewModel.Prouduct;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class ProductQuery
    {
        public static ProductViewBO GetAllProduct(int PageCurrent)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                //PageCurrent = 0;
                var objView = new ProductViewBO();
                var objCategory = _entities.Categorys.Where(m => !m.IsDeleted && m.Slug == "san-pham").FirstOrDefault();

                objView.ParentSlugName = "Tất cả sản phẩm";
                objView.ParentSlug = objCategory?.Slug;
                objView.Title = "Tất cả sản phẩm";
                objView.SlugName = "Tất cả sản phẩm";
                objView.Slug = objCategory?.Slug;
                objView.IsChild = false;
                objView.PageCurrent = PageCurrent;

                var lstProduct = _entities.Products.Where(m => !m.IsDeleted).OrderByDescending(m=>m.Created_at).ToList();

                objView.lstProduct = lstProduct.ToPagedList(PageCurrent - 1, objView.PageSize ?? 9);
                return objView;
            }
            catch (Exception objEx)
            {
                return new ProductViewBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ProductViewBO GetProductList(string strSlug, int PageCurrent)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objView = new ProductViewBO();
                //var objCategory = new Category();
                if (strSlug.Equals("san-pham"))
                {
                   return objView = GetAllProduct(PageCurrent);
                }
                var objCategory = _entities.ProductCategorys.Where(m => !m.IsDeleted && m.Slug.Equals(strSlug)).FirstOrDefault();

                var lstProduct = new List<Product>();

                if (objCategory == null)
                {
                    var objProductCategoryDetail = _entities.ProductCategoryDetails.Where(m => !m.IsDeleted && m.Slug.Equals(strSlug)).FirstOrDefault();
                    if(objProductCategoryDetail != null)
                    {
                        objView.ParentSlugName = objProductCategoryDetail?.Name;
                        objView.ParentSlug = objProductCategoryDetail?.Slug;
                        objView.Title = objProductCategoryDetail?.Name;
                        objView.SlugName = objProductCategoryDetail?.Name;
                        objView.Slug = objProductCategoryDetail?.Slug;

                        var lstCatDetailId = new List<int>();
                        lstCatDetailId.Add(objProductCategoryDetail.Id);
                        if (objProductCategoryDetail.ParentId == 0)
                        {
                            var lstChild = _entities.ProductCategoryDetails.Where(m => !m.IsDeleted && m.ParentId == objProductCategoryDetail.Id).ToList();
                            if(lstChild.Count > 0)
                            {
                                lstCatDetailId.AddRange(lstChild.Select(m => m.Id).ToList());
                            }
                        }
                        lstProduct = _entities.Products.Where(m => !m.IsDeleted && lstCatDetailId.Contains(m.ProductCategoryDetailId.Value)).OrderByDescending(m => m.Created_at).ToList();
                    }
                }
                else
                {
                    objView.ParentSlugName = objCategory?.Name;
                    objView.ParentSlug = objCategory?.Slug;
                    objView.Title = objCategory?.Name;
                    objView.SlugName = objCategory?.Name;
                    objView.Slug = objCategory?.Slug;

                    lstProduct = _entities.Products.Where(m => !m.IsDeleted && m.ProductCategoryId == objCategory.Id).OrderByDescending(m => m.Created_at).ToList();
                }

                objView.PageCurrent = PageCurrent;
                objView.IsChild = false;

                objView.lstProduct = lstProduct.ToPagedList(PageCurrent - 1, objView.PageSize ?? 9);
                return objView;
            }
            catch (Exception objEx)
            {
                return new ProductViewBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ProductDetailViewBO GetProductDetail(string strSlug)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objView = new ProductDetailViewBO();

                var objProduct = _entities.Products.Where(m => !m.IsDeleted && m.Slug.Equals(strSlug)).FirstOrDefault();

                if (objProduct == null)
                {
                    return objView;
                }
                objView.Id = objProduct.Id;
                objView.Name = objProduct?.Name;
                objView.Image = objProduct?.Image;
                objView.SortDescription = objProduct?.SortDescription;
                objView.Description = objProduct?.Description;
                objView.Infomation = objProduct?.Infomation;
                objView.IsChild = true;
                objView.SlugName = objProduct?.Name;
                objView.Slug = objProduct?.Slug;
                if (objView.ProductCategoryDetailId !=null && objView.ProductCategoryDetailId != 0)
                {
                    var objCatDetail = _entities.ProductCategoryDetails
                                                .Where(m => !m.IsDeleted 
                                                            && m.Id == objView.ProductCategoryDetailId).FirstOrDefault();
                    objView.ParentSlugName = objCatDetail?.Name;
                    objView.ParentSlug = objCatDetail?.Slug;
                    objView.Title = objCatDetail?.Name;
                }
                else if(objView.ProductCategoryId != null && objView.ProductCategoryId != 0)
                {
                    var objCat = _entities.ProductCategorys
                                          .Where(m => !m.IsDeleted 
                                                      && m.Id == objView.ProductCategoryId).FirstOrDefault();
                    objView.ParentSlugName = objCat?.Name;
                    objView.ParentSlug = objCat?.Slug;
                    objView.Title = objCat?.Name;
                }
                else
                {
                    objView.ParentSlugName = objProduct?.Name;
                    objView.ParentSlug = objProduct?.Slug;
                    objView.Title = objProduct?.Name;
                    objView.IsChild = false;
                }
                return objView;
            }
            catch (Exception objEx)
            {
                return new ProductDetailViewBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }
    }
}