using EcommerceASP.Models;
using EcommerceASP.ViewModel.PageSlug;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class PageSlugQuery
    {
        public static ListViewModel GetListData(SearchFormViewModel objSearch)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var list = new ListViewModel();
                var lstView = _entities.PageSlugs
                    .Where(m => !m.IsDeleted
                                && !m.Page.IsAdminPage
                                &&(objSearch.PageId==0 || m.PageId == objSearch.PageId)
                                &&(string.IsNullOrEmpty(objSearch.PageSlug) || m.Slug.Contains(objSearch.PageSlug)))
                    .Select(m => new PageSlugBO
                    {
                        Id = m.Id,
                        PageId = m.PageId,
                        PageName = m.Page.Name,
                        Slug = m.Slug,
                        Action = m.Page.Action,
                        Controller = m.Page.Controller,
                        Created_at = m.Created_at,
                        Created_by = m.Created_by
                    }).OrderBy(m=>m.PageId).ToList();

                list.Items = lstView.ToPagedList((objSearch.PageCurrent??1) - 1, list.PageSize ?? 10);
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

        public static PageSlugBO GetDataUpdate(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objPageSlug = new PageSlugBO();
                if (Id == null)
                    return objPageSlug;
                objPageSlug= _entities.PageSlugs
                    .Where(m => !m.IsDeleted && m.Id == Id)
                    .Select(m => new PageSlugBO
                    {
                        Id = m.Id,
                        PageId = m.PageId,
                        PageName = m.Page.Name,
                        Slug = m.Slug,
                        Action = m.Page.Action,
                        Controller = m.Page.Controller,
                        Created_at = m.Created_at,
                        Created_by = m.Created_by
                    }).FirstOrDefault();

                return objPageSlug;
            }
            catch (Exception objEx)
            {
                return new PageSlugBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }
    }
}