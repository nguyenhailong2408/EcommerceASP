using EcommerceASP.Constaint;
using EcommerceASP.Libraries;
using EcommerceASP.Models;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.PostManage;
using EcommerceASP.ViewModel.Topic;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace EcommerceASP.Queries
{
    public class TopicQuery
    {
        public static TopicViewBO GetTopic(string strSlug, int PageCurrent)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                //var objRouter = new PageSlug();
                //objRouter = _entities.PageSlugs.Where(m => !m.IsDeleted && m.Slug.Equals(strSlug)).FirstOrDefault();
                var objTopic = _entities.Topics.Where(m => !m.IsDeleted && m.Slug.Equals(strSlug)).FirstOrDefault();

                var objTopicView = new TopicViewBO();
                var lstTopicDetail = new List<TopicDetailBO>();
                if (objTopic != null)
                {
                    objTopicView.Title = objTopic?.Title;
                    objTopicView.IsChild = false;
                    objTopicView.Content = objTopic?.Content;
                    lstTopicDetail = objTopic.TopicDetails
                                    .Where(m => !m.IsDeleted)
                                    .Select(m => new TopicDetailBO
                                    {
                                        Id = m.Id,
                                        Name = m.Name,
                                        Title = m.Title,
                                        Content = m.Content,
                                        ThumbnailImage = m.ThumbnailImage,
                                        Slug = m.Slug,
                                        Description = m.Description,
                                        Priority = m.Priority,
                                        Created_at = m.Created_at
                                    }).OrderByDescending(m => m.Created_at).ToList();
                }
                else
                {
                    var objTopicDetail = _entities.TopicDetails.Where(m => !m.IsDeleted && m.Slug.Equals(strSlug)).FirstOrDefault();
                    if (objTopicDetail != null)
                    {
                        objTopicView.Title = objTopicDetail?.Title;
                        objTopicView.IsChild = true;
                        objTopic = objTopicDetail?.Topic;
                        objTopicView.Content = objTopicDetail?.Content;
                        objTopicView.SlugName = objTopicDetail?.Title;
                        objTopicView.Slug = objTopicDetail?.Slug;

                        lstTopicDetail = objTopic.TopicDetails
                                        .Where(m => !m.IsDeleted && m.Slug.Equals(strSlug))
                                        .Select(m => new TopicDetailBO
                                        {
                                            Id = m.Id,
                                            Name = m.Name,
                                            Title = m.Title,
                                            Content = m.Content,
                                            ThumbnailImage = m.ThumbnailImage,
                                            Slug = m.Slug,
                                            Description = m.Description,
                                            Priority = m.Priority,
                                            Created_at = m.Created_at
                                        }).OrderByDescending(m => m.Created_at).ToList();
                    }
                }
                objTopicView.ParentSlugName = objTopic?.Category.Name;
                objTopicView.ParentSlug = objTopic?.Slug;
                objTopicView.lstComponentPageSlug = _entities.ComponentPageSlugs
                                                    .Where(x => x.PageSlug.Equals(strSlug) && !x.IsDeleted).ToList();
                objTopicView.PageCurrent = PageCurrent;

                objTopicView.lstTopicDetail = lstTopicDetail.ToPagedList(PageCurrent - 1, objTopicView.PageSize ?? 10);
                return objTopicView;
            }
            catch (Exception objEx)
            {
                return new TopicViewBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        #region TopicManage

        public static ListViewModel GetListData(SearchFormViewModel objSearch)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objView = new ListViewModel();
                var lstPost = new List<PostManageBO>();
                lstPost = (from t in _entities.Topics
                           join a in _entities.Accounts
                           on t.Created_by equals a.Id into T
                           from s in T.DefaultIfEmpty()
                           where !t.IsDeleted
                                 && (string.IsNullOrEmpty(objSearch.Title) || t.Title.Contains(objSearch.Title))
                                 && (string.IsNullOrEmpty(objSearch.Slug) || t.Slug.Contains(objSearch.Slug))
                                 && (objSearch.CategoryId == 0 || t.CategoryId == objSearch.CategoryId)
                           select new PostManageBO
                           {
                               Id = t.Id,
                               CategoryInfo = t.CategoryId + " - " + t.Category.Name,
                               Title = t.Title,
                               Content = t.Content,
                               Priority = t.Priority,
                               Slug = t.Slug,
                               Created_at = t.Created_at,
                               CreatedByName = s.UserName
                           }).ToList();

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

        public static PostManageBO GetDataUpdate(int? topicId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objPost = new PostManageBO();
                if (topicId == null)
                    return objPost;
                objPost = _entities.Topics.Where(t => t.Id == topicId)
                    .Select(t => new PostManageBO
                    {
                        Id = t.Id,
                        CategoryId = t.CategoryId,
                        CategoryInfo = t.CategoryId + " - " + t.Category.Name,
                        Title = t.Title,
                        Content = t.Content,
                        Priority = t.Priority,
                        Slug = t.Slug,
                        Created_at = t.Created_at,
                    }).FirstOrDefault();
                return objPost;
            }
            catch (Exception objEx)
            {
                return new PostManageBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI Update(PostManageBO objModel)
        {
            try
            {
                if (objModel.Id == 0)
                {
                    return CreateTopic(objModel);
                }
                return UpdateTopic(objModel);
            }
            catch (Exception objEx)
            {
                return ResponseAPI.GetFailedResponse(objEx.Message);
            }
        }

        public static ResponseAPI CreateTopic(PostManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                var objPageSlug = _entities.PageSlugs.Where(m => !m.IsDeleted && m.Slug.Equals(objModel.Slug)).FirstOrDefault();
                if (objPageSlug != null)
                {
                    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại. Vui lòng nhập đường dẫn mới");
                }

                objPageSlug = new PageSlug();
                objPageSlug.PageId = objModel.CategoryId == 3 ? (int)EnumPage.Topic : (int)EnumPage.ConstructionDesign;
                objPageSlug.Slug = objModel.Slug;
                objPageSlug.IsDeleted = false;
                objPageSlug.Created_at = DateTime.Now;
                objPageSlug.Created_by = 1;
                _entities.PageSlugs.Add(objPageSlug);

                var objPost = new Topic();
                objPost.CategoryId = objModel.CategoryId;
                objPost.Title = objModel.Title;
                objPost.Slug = objModel.Slug;
                objPost.Content = objModel.Content;
                objPost.Priority = objModel.Priority;
                objPost.IsDeleted = false;
                objPost.Created_at = DateTime.Now;
                objPost.Created_by = 1;

                _entities.Topics.Add(objPost);
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

        public static ResponseAPI UpdateTopic(PostManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                var objPageSlug = _entities.PageSlugs.Where(m => !m.IsDeleted && m.Slug.Equals(objModel.Slug)).ToList();
                if (objPageSlug.Count > 1)
                {
                    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại. Vui lòng nhập đường dẫn mới");
                }
                if (objPageSlug.Any(m => m.PageId != (int)EnumPage.Topic && m.PageId != (int)EnumPage.ConstructionDesign))
                {
                    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại. Vui lòng nhập đường dẫn mới");
                }
                if (objPageSlug.Count == 0)
                {
                    var newPageSlug = new PageSlug();
                    newPageSlug.PageId = objModel.CategoryId == 3 ? (int)EnumPage.Topic : (int)EnumPage.ConstructionDesign;
                    newPageSlug.Slug = objModel.Slug;
                    newPageSlug.IsDeleted = false;
                    newPageSlug.Created_at = DateTime.Now;
                    newPageSlug.Created_by = 1;
                    _entities.PageSlugs.Add(newPageSlug);
                }
                else
                {
                    objPageSlug[0].PageId = objModel.CategoryId == 3 ? (int)EnumPage.Topic : (int)EnumPage.ConstructionDesign;
                    objPageSlug[0].Slug = objModel.Slug;
                    objPageSlug[0].IsDeleted = false;
                    objPageSlug[0].Updated_at = DateTime.Now;
                    objPageSlug[0].Updated_by = 1;
                }

                var objTopic = _entities.Topics.Where(m => m.Id == objModel.Id).FirstOrDefault();
                if (objTopic == null)
                    return ResponseAPI.GetFailedResponse("Không tìm thấy chủ đề bài viết để cập nhật");

                objTopic.CategoryId = objModel.CategoryId;
                objTopic.Title = objModel.Title;
                objTopic.Slug = objModel.Slug;
                objTopic.Content = objModel.Content;
                objTopic.Priority = objModel.Priority;
                objTopic.IsDeleted = false;
                objTopic.Updated_at = DateTime.Now;
                objTopic.Updated_by = 1;

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

        public static ResponseAPI DeleteTopic(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objTopic = _entities.Topics.Where(m => m.Id == Id).FirstOrDefault();
                if (objTopic == null)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy chủ đề bài viết để xóa");
                }
                var lstPageSlug = _entities.PageSlugs.Where(m => !m.IsDeleted
                                                                && m.Slug.Equals(objTopic.Slug)
                                                                && (m.PageId == (int)EnumPage.Topic
                                                                    || m.PageId == (int)EnumPage.ConstructionDesign))
                                                                .ToList();
                foreach (var itemPageSlug in lstPageSlug)
                {
                    itemPageSlug.IsDeleted = true;
                    itemPageSlug.Updated_at = DateTime.Now;
                    itemPageSlug.Updated_by = 1;
                }

                objTopic.Updated_at = DateTime.Now;
                objTopic.Updated_by = 1;
                objTopic.IsDeleted = true;

                //foreach(var item in objTopic.TopicDetails)
                //{
                //    item.IsDeleted = true;
                //    item.Updated_at = DateTime.Now;
                //    item.Updated_by = 1;
                //}
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

        #endregion TopicManage
    }
}