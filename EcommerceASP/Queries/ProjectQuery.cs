using EcommerceASP.Constaint;
using EcommerceASP.Models;
using EcommerceASP.ViewModel.Project;
using EcommerceASP.ViewModel.ProjectManage;
using EcommerceASP.ViewModel.Topic;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class ProjectQuery
    {
        public static ProjectViewBO GetListProjects(int PageCurrent)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objTopic = _entities.Topics.Where(m => !m.IsDeleted && m.Slug.Equals("du-an")).FirstOrDefault();
                var objProjectView = new ProjectViewBO();
                var lstProjectDetail = new List<ProjectBO>();
                if (objTopic != null)
                {
                    objProjectView.ParentSlugName = "Dự án";
                    objProjectView.ParentSlug = "du-an";
                    objProjectView.Title = objTopic?.Title;
                    objProjectView.ThumbnailImage = objTopic?.ThumbnailImage;
                    objProjectView.IsChild = false;
                }
                lstProjectDetail = _entities.Projects.Where(x => !x.IsDeleted)
                                              .Select(x => new ProjectBO
                                              {
                                                  Customer = x.Customer,
                                                  Title = x.Title,
                                                  Priority = x.Priority,
                                                  ThumbnailImage = x.ThumbnailImage,
                                                  Slug = x.Slug,
                                                  Address = x.Address,
                                                  TimeProject = x.TimeProject
                                              }).OrderBy(x => x.Priority).ToList();
                objProjectView.lstProject = lstProjectDetail.ToPagedList(PageCurrent - 1, objProjectView.PageSize ?? 9);
                return objProjectView;
            }
            catch (Exception objEx)
            {
                return new ProjectViewBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ProjectBO GetProjectDetail(string strSlug)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objProject = _entities.Projects.Where(m => !m.IsDeleted && m.Slug.Equals(strSlug)).FirstOrDefault();
                
                var objProjectView = new ProjectBO();
                if (objProject != null)
                {
                    objProjectView.ParentSlugName = "Dự án";
                    objProjectView.ParentSlug = "du-an";
                    objProjectView.Slug = objProject.Slug;
                    objProjectView.SlugName = objProject.Title;
                    objProjectView.Title = objProject?.Title;
                    objProjectView.SubTitle = objProject?.SubTitle;
                    objProjectView.ThumbnailImage = objProject?.ThumbnailImage;
                    objProjectView.IsChild = true;
                    objProjectView.Address = objProject.Address;
                    objProjectView.Customer = objProject.Customer;
                    objProjectView.Size = objProject.Size;
                    objProjectView.JobDescription = objProject.JobDescription;
                    objProjectView.Content = objProject.Content;
                    objProjectView.TimeProject = objProject.TimeProject;
                    objProjectView.lstProjectImages = objProject.ProjectImages.Where(x => !x.IsDeleted)
                                                                .Select(x => new ViewModel.ProjectImageManage.ProjectImageBO
                                                                {
                                                                    Id = x.Id,
                                                                    ThumbnailImage =x.ThumbnailImage,
                                                                    Priority = x.Priority
                                                                }).OrderBy(x=>x.Priority).ToList();
                    objProjectView.lstProjectRelated = _entities.Projects.Where(m => !m.IsDeleted && !m.Slug.Equals(strSlug))
                                                                        .Take(9)
                                                                        .Select(m=> new ProjectRelatedBO
                                                                        {
                                                                            Id = m.Id,
                                                                            Slug = m.Slug,
                                                                            Customer = m.Customer,
                                                                            Title = m.Title,
                                                                            TimeProject = m.TimeProject,
                                                                            JobDescription = m.JobDescription,
                                                                            Priority = m.Priority,
                                                                            Size = m.Size,
                                                                            ThumbnailImage = m.ThumbnailImage
                                                                        }).OrderBy(m=>m.Priority).ToList();
                }

                return objProjectView;
            }
            catch (Exception objEx)
            {
                return new ProjectBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }
    }
}