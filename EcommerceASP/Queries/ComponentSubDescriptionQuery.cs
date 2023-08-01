using EcommerceASP.Models;
using EcommerceASP.ViewModel.ComponentSubDescription;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class ComponentSubDescriptionQuery
    {
        public static ListViewModel GetListData(SearchFormViewModel objSearch)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objView = new ListViewModel();
                var lstData = new List<ComponentSubDescriptionBO>();
                lstData = (from t in _entities.ComponentSubDescriptions
                           join a in _entities.Accounts
                           on t.Created_by equals a.Id into T
                           from s in T.DefaultIfEmpty()
                           join ac in _entities.Accounts
                           on t.Updated_by equals ac.Id into AC
                           from c in AC.DefaultIfEmpty()
                           where !t.IsDeleted
                                 && (string.IsNullOrEmpty(objSearch.PageSlug) || t.PageSlug.Contains(objSearch.PageSlug))
                           select new ComponentSubDescriptionBO
                           {
                               Id = t.Id,
                               ComponentId = t.ComponentId,
                               Title = t.Title,
                               SubTitle = t.SubTitle,
                               PageSlug = t.PageSlug,
                               Image = t.Image,
                               Description = t.Description,
                               Created_at = t.Created_at == null ? t.Updated_at : t.Created_at,
                               CreatedByName = string.IsNullOrEmpty(s.FullName) ? c.FullName : s.FullName
                           })
                           .OrderBy(m => m.PageSlug)
                           .ToList();

                objView.Items = lstData.ToPagedList((objSearch.PageCurrent ?? 1) - 1, objView.PageSize ?? 10);
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
    }
}