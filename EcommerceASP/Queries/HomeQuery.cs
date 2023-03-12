using EcommerceASP.Constaint;
using EcommerceASP.Models;
using EcommerceASP.ViewModel;
using EcommerceASP.ViewModel.Category;
using EcommerceASP.ViewModel.Component;
using EcommerceASP.ViewModel.Topic;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class HomeQuery
    {
        public static HomeBO GetComponent()
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objHome = new HomeBO();
                objHome.lstComponent = _entities.Components
                    .Where(m => !m.IsDeleted && m.PageId == ((int)EnumPage.Home))
                    .Select(m => new ComponentBO
                    {
                        Id = m.Id,
                        Name = m.Name,
                        PageId = m.PageId,
                        Title = m.Title,
                        ComponentTypeId = m.ComponentTypeId,
                        CategoryId = m.CategoryId,
                        ReferenceId = m.ReferenceId,
                        Priority = m.Priority,
                        ComponentType = m.ComponentType
                    }).OrderBy(m=>m.Priority).ToList();
                return objHome;
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

        public static List<CategoryBO> GetCategory()
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var listCategory = _entities.Categorys
                    .Where(m => !m.IsDeleted)
                    .Select(m => new CategoryBO
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Slug = m.Slug,
                        IsDeleted = m.IsDeleted,
                        TemplateOptionId = m.TemplateOptionId ?? 1,
                        lstCategoryDetail = m.CategoryDetails.Where(x => !x.IsDeleted)
                        .Select(x => new CategoryDetailBO
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Slug = x.Slug,
                            Priority = x.Priority,
                            ParentId = x.ParentId,
                            ProductCategoryId = x.ProductCategoryId,
                            CategoryID = x.CategoryID,
                            IsNewLabel = x.IsNewLabel ?? false,
                            IsHasBanner = x.IsHasBanner,
                            BannerImage = x.BannerImage
                        }).ToList()

                    }).ToList();
                return listCategory;
            }
            catch (Exception objEx)
            {
                return new List<CategoryBO>();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ComponentViewBO GetDataComponent(ComponentBO Component)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objCategory = _entities.Categorys
                    .Where(m => m.Id == Component.CategoryId)
                    .Select(m => m).FirstOrDefault();
                var objComponent = new ComponentViewBO()
                {
                    Id = Component.Id,
                    Name = Component.Name,
                    Title = Component.Title,
                    ParentSlug = objCategory?.Slug,
                    Rows = Component.ComponentType.Row ?? 1,
                    Collumns = Component.ComponentType.Collumn ?? 1,
                    IsSlide = Component.ComponentType.IsSlide
                };

                switch (Component.ComponentTypeId)
                {
                    case (int)EnumComponentType.SlideShow:
                        objComponent.lstDetailComponent = GetComponentSlideShow(Component);
                        break;
                    case (int)EnumComponentType.Product:
                        objComponent.lstDetailComponent = GetComponentProuduct(Component, objCategory?.Slug, objCategory?.FolderImage);
                        break;
                    case (int)EnumComponentType.TopicThreeCollumn:
                        objComponent.lstDetailComponent = GetComponentTopic(Component, objCategory?.Slug, objCategory?.FolderImage);
                        break;
                    case (int)EnumComponentType.TopicFiveCollumn:
                        objComponent.lstDetailComponent = GetComponentTopic(Component, objCategory?.Slug, objCategory?.FolderImage);
                        break;
                    default:
                        break;
                }
                
                return objComponent;
            }
            catch (Exception objEx)
            {
                return new ComponentViewBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }
        public static List<ComponentDetailViewBO> GetComponentProuduct(ComponentBO Component, string strParentSlug,
            string strFolderImage)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objComponent = new List<ComponentDetailViewBO>();
                objComponent = (from c in _entities.ComponentItems
                                join p in _entities.Products
                                on new { Id = c.ItemId, c.IsDeleted } equals new { p.Id, p.IsDeleted }
                                where !c.IsDeleted && c.ComponentId == Component.Id
                                join a in _entities.Accounts on p.Created_by equals a.Id into A
                                from s in A.DefaultIfEmpty()
                                select new ComponentDetailViewBO
                                {
                                    ReferenceId = p.Id,
                                    Title = p.Name,
                                    Name = p.Name,
                                    Description = p.SortDescription,
                                    ParentSlug = "san-pham",
                                    Slug = p.Slug,
                                    ImageName = p.Image,
                                    FolderImage = strFolderImage,
                                    Price = p.Price,
                                    PriceSale = p.Price_sale,
                                    Priority = c.Priority,
                                    Created_at = p.Created_at,
                                    Created_by = s.FullName
                                })
                                .OrderByDescending(m => m.Created_at)
                                .ToList();


                return objComponent;
            }
            catch (Exception objEx)
            {
                return new List<ComponentDetailViewBO>();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static List<ComponentDetailViewBO> GetComponentTopic(ComponentBO Component, string strParentSlug,
            string strFolderImage)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objComponent = new List<ComponentDetailViewBO>();
                objComponent = (from t in _entities.Topics
                                join td in _entities.TopicDetails
                                on new { t.Id, t.IsDeleted } equals new { Id = td.TopicId.Value, td.IsDeleted }
                                where !t.IsDeleted && t.Id == Component.ReferenceId
                                join a in _entities.Accounts on td.Created_by equals a.Id into A
                                from s in A.DefaultIfEmpty()
                                select new ComponentDetailViewBO
                                {
                                    ReferenceId = td.Id,
                                    Title = td.Title,
                                    Name = td.Name,
                                    Description = td.Description,
                                    Slug = td.Slug,
                                    ImageName = td.ThumbnailImage,
                                    FolderImage = strFolderImage,
                                    Priority = td.Priority,
                                    Created_at = td.Created_at,
                                    Created_by = s.FullName
                                })
                                .OrderByDescending(m => m.Created_at)
                                .ToList();
                return objComponent;
            }
            catch (Exception objEx)
            {
                return new List<ComponentDetailViewBO>();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static List<ComponentDetailViewBO> GetComponentSlideShow(ComponentBO Component)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objComponent = new List<ComponentDetailViewBO>();
                objComponent = _entities.Slides.Where(p=>!p.IsDeleted).Select(p => new ComponentDetailViewBO
                                {
                                    ReferenceId = p.Id,
                                    Title = p.Name,
                                    Name = p.Name,
                                    Description = p.Position,
                                    Slug = p.Slug,
                                    ImageName = p.Image,
                                    FolderImage = "slide",
                                    Priority = p.Priority,
                                })
                                .OrderByDescending(p => p.Priority)
                                .ToList();


                return objComponent;
            }
            catch (Exception objEx)
            {
                return new List<ComponentDetailViewBO>();
            }
            finally
            {
                _entities.Dispose();
            }
        }
    }
}