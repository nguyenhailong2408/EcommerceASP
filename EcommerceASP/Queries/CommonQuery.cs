using EcommerceASP.Models;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.Component;
using EcommerceASP.ViewModel.PageSlug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Queries
{
    public class CommonQuery
    {
        public static BreadcrumbBO GetBreadcrumb(string strSlug, int parentId, int pageId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objBreadcrumb = new BreadcrumbBO();
                objBreadcrumb.IsChild = parentId != 0;
                var router = RouterQuery.GetRouterPage(strSlug);

                var category = _entities.Categorys.Where(m => !m.IsDeleted && m.Slug.Equals(strSlug)).FirstOrDefault();
                objBreadcrumb.ParentSlug = category?.Slug;
                objBreadcrumb.ParentSlugName = category?.Name;
                return objBreadcrumb;
            }
            catch (Exception objEx)
            {
                return new BreadcrumbBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static BreadcrumbBO GetBreadcrumbProduct(string strSlug, int parentId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objBreadcrumb = new BreadcrumbBO();
                objBreadcrumb.IsChild = parentId != 0;

                var category = _entities.Categorys.Where(m => !m.IsDeleted && m.Slug.Equals(strSlug)).FirstOrDefault();
                objBreadcrumb.ParentSlug = category?.Slug;
                objBreadcrumb.ParentSlugName = category?.Name;

                return objBreadcrumb;
            }
            catch (Exception objEx)
            {
                return new BreadcrumbBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static BreadcrumbBO GetBreadcrumbTopic(string strSlug, int parentId, int pageId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objBreadcrumb = new BreadcrumbBO();
                objBreadcrumb.IsChild = parentId != 0;
                var router = RouterQuery.GetRouterPage(strSlug);

                var category = _entities.Categorys.Where(m => !m.IsDeleted && m.Slug.Equals(strSlug)).FirstOrDefault();
                objBreadcrumb.ParentSlug = category?.Slug;
                objBreadcrumb.ParentSlugName = category?.Name;

                return objBreadcrumb;
            }
            catch (Exception objEx)
            {
                return new BreadcrumbBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static List<SelectListItem> GetListPage(int? pageId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = (from l in _entities.Pages
                            where !l.IsDeleted && !l.IsAdminPage
                            select new SelectListItem()
                            {
                                Selected = l.Id == pageId,
                                Text = l.Id.ToString() + " - " + l.Name.ToString(),
                                Value = l.Id.ToString(),
                            }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                _entities.Dispose();
            }
        }
        public static ResponseAPI GetActionControllerByPageId(int? pageId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var obj = (from l in _entities.Pages
                           where !l.IsDeleted && !l.IsAdminPage && l.Id == pageId
                           select new PageSlugBO
                           {
                               Action = l.Action,
                               Controller = l.Controller
                           }).FirstOrDefault();

                return ResponseAPI.GetSuccessResponse("Success", obj); ;
            }
            catch (Exception ex)
            {
                return ResponseAPI.GetFailedResponse(ex.Message);
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static List<SelectListItem> GetListProductCategory(int? productCatId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = (from l in _entities.ProductCategorys
                            where !l.IsDeleted
                            select new SelectListItem()
                            {
                                Selected = l.Id == productCatId,
                                Text = l.Id.ToString() + " - " + l.Name.ToString(),
                                Value = l.Id.ToString(),
                            }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                _entities.Dispose();
            }
        }
        public static List<SelectListItem> GetListProductCategoryDetail(int? productCatId, int? productCatDetailId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = (from l in _entities.ProductCategoryDetails
                            where !l.IsDeleted && (productCatId == 0 || productCatId == null || l.ProductCategoryID == productCatId)
                            select new SelectListItem()
                            {
                                Selected = l.Id == productCatDetailId,
                                Text = l.Id.ToString() + " - " + l.Name.ToString(),
                                Value = l.Id.ToString(),
                            }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static List<SelectListItem> GetListProductCategoryDetailChild(int? productCatId, int? productCatDetailId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = (from l in _entities.ProductCategoryDetails
                            where !l.IsDeleted 
                                  && (productCatId == 0 || productCatId == null || l.ProductCategoryID == productCatId)
                                  && l.ParentId !=0
                            select new SelectListItem()
                            {
                                Selected = l.Id == productCatDetailId,
                                Text = l.Id.ToString() + " - " + l.Name.ToString(),
                                Value = l.Id.ToString(),
                            }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static List<SelectListItem> GetCategoryTopic(int? CatId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = (from l in _entities.Categorys
                            where !l.IsDeleted
                            select new SelectListItem()
                            {
                                Selected = l.Id == CatId,
                                Text = l.Id.ToString() + " - " + l.Name.ToString(),
                                Value = l.Id.ToString(),
                            }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static List<SelectListItem> GetTopic(int? TopicId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = (from l in _entities.Topics
                            where !l.IsDeleted
                            select new SelectListItem()
                            {
                                Selected = l.Id == TopicId,
                                Text = l.Id.ToString() + " - " + l.Title.ToString(),
                                Value = l.Id.ToString(),
                            }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static List<SelectListItem> GetPage(int? PageId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = (from l in _entities.Pages
                            where !l.IsDeleted && !l.IsAdminPage
                            select new SelectListItem()
                            {
                                Selected = l.Id == PageId,
                                Text = l.Id.ToString() + " - " + l.Name.ToString(),
                                Value = l.Id.ToString(),
                            }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static List<SelectListItem> GetPageBySlug(string strSlug)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = (from l in _entities.PageSlugs
                            where !l.IsDeleted && l.Slug.Equals(strSlug)
                            select new SelectListItem()
                            {
                                Selected = l.Slug.Equals(strSlug),
                                Text = l.PageId.ToString() + " - " + l.Page.Name,
                                Value = l.PageId.ToString(),
                            }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                _entities.Dispose();
            }
        }
        public static List<PageSlugExistBO> CheckExistSlug(string strSlug)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = (from l in _entities.PageSlugs
                            where !l.IsDeleted && l.Slug.Equals(strSlug)
                            select new PageSlugExistBO()
                            {
                               PageId = l.PageId,
                               PageName = l.Page.Name
                            }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return new List<PageSlugExistBO>();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static List<PageSlug> GetLstPageBySlug(string strSlug)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = _entities.PageSlugs.Where(m=>m.Slug.Equals(strSlug) && !m.IsDeleted).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return new List<PageSlug>();
            }
            finally
            {
                _entities.Dispose();
            }
        }
        public static List<SelectListItem> GetCategory(int? CatId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = (from l in _entities.Categorys
                            where !l.IsDeleted
                            select new SelectListItem()
                            {
                                Selected = l.Id == CatId,
                                Text = l.Id.ToString() + " - " + l.Name.ToString(),
                                Value = l.Id.ToString(),
                            }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                _entities.Dispose();
            }
        }
        public static List<SelectListItem> GetCategoryDetail(int? CatId, int CatDetailId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = (from l in _entities.CategoryDetails
                            where !l.IsDeleted && (CatId == 0 || CatId == null || l.CategoryID == CatId)
                            select new SelectListItem()
                            {
                                Selected = l.Id == CatDetailId,
                                Text = l.Id.ToString() + " - " + l.Name.ToString(),
                                Value = l.Id.ToString(),
                            }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                return new List<SelectListItem>();
            }
            finally
            {
                _entities.Dispose();
            }
        }
    }
}