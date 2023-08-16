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
using System.IO;
using System.Linq;
using System.Web;

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
                    objTopicView.SubTitle = objTopic?.SubTitle;
                    objTopicView.ThumbnailImage = objTopic?.ThumbnailImage;
                    objTopicView.IsChild = false;
                    objTopicView.Content = objTopic?.Content;

                    if (strSlug.Equals("tin-tuc"))
                    {
                        objTopicView.ParentSlug = objTopic?.Slug;
                        var lstCatTopics = _entities.Topics.Where(x => x.CategoryId == (int)EnumPage.Topic);
                        lstTopicDetail = lstCatTopics.SelectMany(x => x.TopicDetails.Where(z => !z.IsDeleted)
                                                        .Select(z => new TopicDetailBO
                                                        {
                                                            Id = z.Id,
                                                            Name = z.Name,
                                                            Title = z.Title,
                                                            Content = z.Content,
                                                            ThumbnailImage = z.ThumbnailImage,
                                                            Slug = z.Slug,
                                                            Description = z.Description,
                                                            Priority = z.Priority,
                                                            Created_at = z.Updated_at == null ? z.Created_at
                                                                        : z.Updated_at == null ? DateTime.Now : z.Updated_at
                                                        })).OrderByDescending(h => h.Created_at).ToList();
                    }
                    else
                    {
                        var lstCatTopicIds = _entities.Topics.Where(x => x.CategoryId == (int)EnumPage.Topic).Select(x=>x.Id).ToList();
                        if(lstCatTopicIds.Any(x=>x == objTopic.Id))
                        {
                            objTopicView.ParentSlug = "tin-tuc";
                        }
                        objTopicView.IsChild = true;
                        objTopicView.SlugName = objTopic?.Title;
                        objTopicView.Slug = objTopic?.Slug;

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
                                        Created_at = m.Updated_at == null ? m.Created_at : m.Updated_at == null ? DateTime.Now : m.Updated_at
                                    }).OrderByDescending(m => m.Created_at).ToList();
                    }
                    objTopicView.ParentSlugName = objTopic?.Category.Name;
                    
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
                                            Created_at = m.Updated_at == null ? m.Created_at : m.Updated_at == null ? DateTime.Now : m.Updated_at
                                        }).OrderByDescending(m => m.Created_at).ToList();
                    }
                    objTopicView.ParentSlugName = objTopic?.Category.Name;
                    objTopicView.ParentSlug = objTopic?.Slug;
                }
               
                //if (!lstCatTopicIds.Contains(objTopic.Id))
                //{
                //    objTopicView.ParentSlug = "tin-tuc";
                //}
                objTopicView.lstComponentPageSlug = _entities.ComponentPageSlugs
                                                    .Where(x => x.PageSlug.Equals(strSlug) && !x.IsDeleted).OrderBy(x => x.Priority).ToList();
                objTopicView.PageCurrent = PageCurrent;

                objTopicView.lstTopicDetail = lstTopicDetail.ToPagedList(PageCurrent - 1, objTopicView.PageSize ?? 9);
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
                               SubTitle = t.SubTitle,
                               Content = t.Content,
                               ThumbnailImage = t.ThumbnailImage,
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
                        SubTitle = t.SubTitle,
                        ThumbnailImage = t.ThumbnailImage,
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
                objPost.SubTitle = objModel.SubTitle;
                objPost.Slug = objModel.Slug;
                objPost.Content = objModel.Content;
                objPost.Priority = objModel.Priority;
                objPost.IsDeleted = false;
                objPost.Created_at = DateTime.Now;
                objPost.Created_by = 1;

                if (objModel.UploadImage != null)
                {
                    var imgNameOld = objModel.ImageOld;

                    objPost.ThumbnailImage = objModel.UploadImage.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/topic/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objPost.ThumbnailImage);
                }

                _entities.Topics.Add(objPost);
                _entities.SaveChanges();
                return ResponseAPI.GetSuccessResponse("Thành công", null);
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
                objTopic.SubTitle = objModel.SubTitle;
                objTopic.Slug = objModel.Slug;
                objTopic.Content = objModel.Content;
                objTopic.Priority = objModel.Priority;
                objTopic.IsDeleted = false;
                objTopic.Updated_at = DateTime.Now;
                objTopic.Updated_by = 1;

                if (objModel.UploadImage != null)
                {
                    var imgNameOld = objModel.ImageOld;

                    objTopic.ThumbnailImage = objModel.UploadImage.GuidName();
                    string pathImages = HttpContext.Current.Server.MapPath("~/Content/Images/topic/");

                    if (!Directory.Exists(pathImages))
                    {
                        Directory.CreateDirectory(pathImages);
                    }

                    if (File.Exists(pathImages + imgNameOld))
                    {
                        File.Delete(pathImages + imgNameOld);
                    }

                    objModel.UploadImage.SaveAs(pathImages + objTopic.ThumbnailImage);
                }

                _entities.SaveChanges();
                return ResponseAPI.GetSuccessResponse("Thành công", null);
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

        public static TopicViewBO GetComponent(string strSlug, int PageCurrent)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objTopicView = new TopicViewBO();
                var lstTopicDetail = new List<TopicDetailBO>();
                var objTopicDetail = _entities.TopicDetails.Where(m => !m.IsDeleted && m.Slug.Equals(strSlug)).FirstOrDefault();
                if (objTopicDetail != null)
                {
                    objTopicView.Title = objTopicDetail?.Title;
                    objTopicView.IsChild = true;
                    objTopicView.Content = objTopicDetail?.Content;
                    objTopicView.SlugName = objTopicDetail?.Title;
                    objTopicView.Slug = objTopicDetail?.Slug;

                    lstTopicDetail = objTopicDetail?.Topic.TopicDetails
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
                                        Created_at = m.Updated_at == null ? m.Created_at : m.Updated_at == null ? DateTime.Now : m.Updated_at
                                    }).OrderByDescending(m => m.Created_at).ToList();
                }
                objTopicView.ParentSlugName = objTopicDetail?.Topic.Category.Name;
                objTopicView.ParentSlug = objTopicDetail?.Topic.Slug;

                objTopicView.lstComponentPageSlug = _entities.ComponentPageSlugs
                                                    .Where(x => x.PageSlug.Equals(strSlug) && !x.IsDeleted).OrderBy(x => x.Priority).ToList();
                objTopicView.PageCurrent = PageCurrent;

                objTopicView.lstTopicDetail = lstTopicDetail.ToPagedList(PageCurrent - 1, objTopicView.PageSize ?? 9);
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

    }
}