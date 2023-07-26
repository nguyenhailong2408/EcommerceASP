using EcommerceASP.Models;
using EcommerceASP.ViewModel.ComponentPageSlug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class ComponentQuery
    {
        public static List<ComponentPageSlugBO> GetComponentPageSlug(string strPageSlug)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var result = _entities.ComponentPageSlugs
                    .Where(m => !m.IsDeleted && m.PageSlug.Equals(strPageSlug))
                    .Select(m => new ComponentPageSlugBO
                    {
                        Id = m.Id,
                        Name = m.Name,
                        PageSlug = m.PageSlug,
                        Title = m.Title,
                        ComponentTypeId = m.ComponentTypeId,
                        CategoryId = m.CategoryId,
                        ReferenceId = m.ReferenceId,
                        Priority = m.Priority,
                        ComponentType = m.ComponentType
                    }).OrderBy(m => m.Priority).ToList();
                return result;
            }
            catch (Exception objEx)
            {
                throw objEx;
                //return new HomeBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }
        public static ComponentTemplateViewBO GetComponentSubDescription(int componentId, int componentTypeId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objResult = new ComponentTemplateViewBO();
                objResult.ComponentTypeId = componentTypeId;
                objResult.HtmlTemplate = _entities.ComponentTypes.Where(x => x.Id == componentTypeId).FirstOrDefault().NameFile;
                objResult.lstComponentSubDescription = _entities.ComponentSubDescriptions
                                        .Where(x => x.ComponentId == componentId)
                                        .Select(x => new ComponentSubDescriptionViewBO
                                        {
                                            ComponentId = x.ComponentId,
                                            SubTitle = x.SubTitle,
                                            Title = x.Title,
                                            Image = x.Image,
                                            Description = x.Description
                                        }).ToList();
                return objResult;
            }
            catch (Exception objEx)
            {
                throw objEx;
                //return new HomeBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }
    }
}