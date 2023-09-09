using EcommerceASP.Models;
using EcommerceASP.ViewModel.PageSlug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class RouterQuery
    {
        public static PageSlugBO GetRouterPage(string strSlug)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                if (strSlug.Equals("gioi-thieu"))
                {
                    strSlug = "ve-chung-toi";
                }
                var objRouter = new PageSlugBO();
                objRouter = _entities.PageSlugs.Where(m => !m.IsDeleted && m.Slug.Equals(strSlug))
                    .Select(m=> new PageSlugBO
                    {
                        Id = m.Id,
                        IsAdminPage = m.Page.IsAdminPage,
                        PageId = m.PageId,
                        Slug = m.Slug,
                        AdminSlug = m.Page.Slug,
                        PageName = m.Page.Name
                    }).FirstOrDefault();
                return objRouter;
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