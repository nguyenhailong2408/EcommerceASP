using EcommerceASP.Constaint;
using EcommerceASP.Libraries;
using EcommerceASP.Models;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.PostDetailManage;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class TopicDetailQuery
    {
        #region TopicDetail Manage

        public static ListViewModel GetListData(SearchFormViewModel objSearch)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objView = new ListViewModel();
                var lstPost = new List<PostDetailManageBO>();
                lstPost = (from t in _entities.TopicDetails
                           join tp in _entities.Topics
                           on new { Id = t.TopicId.Value, t.IsDeleted } equals new { tp.Id, tp.IsDeleted }
                           join a in _entities.Accounts
                           on t.Created_by equals a.Id into T
                           from s in T.DefaultIfEmpty()
                           where !t.IsDeleted
                                 && (string.IsNullOrEmpty(objSearch.Title) || t.Title.Contains(objSearch.Title))
                                 && (string.IsNullOrEmpty(objSearch.Slug) || t.Slug.Contains(objSearch.Slug))
                                 && (objSearch.TopicId == 0 || t.TopicId == objSearch.TopicId)
                                 && (objSearch.CategoryId == 0 || t.Topic.CategoryId == objSearch.CategoryId)
                           select new PostDetailManageBO
                           {
                               Id = t.Id,
                               Title = t.Title,
                               Name = t.Name,
                               Content = t.Content,
                               Priority = t.Priority,
                               Slug = t.Slug,
                               Description = t.Description,
                               ThumbnailImage = t.ThumbnailImage,
                               ImageOld = t.ThumbnailImage,
                               TopicId = t.TopicId.Value,
                               TopicInfo = t.Topic.Id + " - " + t.Topic.Title,
                               Created_at = t.Created_at == null ? t.Updated_at : t.Created_at,
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

        public static PostDetailManageBO GetDataUpdate(int? topicId)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objPost = new PostDetailManageBO();
                if (topicId == null)
                    return objPost;
                objPost = _entities.TopicDetails.Where(t => t.Id == topicId)
                    .Select(t => new PostDetailManageBO
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Name = t.Name,
                        Content = t.Content,
                        Priority = t.Priority,
                        Slug = t.Slug,
                        Description = t.Description,
                        ThumbnailImage = t.ThumbnailImage,
                        ImageOld = t.ThumbnailImage,
                        TopicId = t.TopicId.Value,
                    }).FirstOrDefault();
                return objPost;
            }
            catch (Exception objEx)
            {
                return new PostDetailManageBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI Update(PostDetailManageBO objModel)
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

        public static ResponseAPI CreateTopic(PostDetailManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                var objPageSlug = _entities.PageSlugs.Where(m => !m.IsDeleted
                                                            && m.Slug.Equals(objModel.Slug)
                                                            && (m.PageId == (int)EnumPage.TopicDetail
                                                              || m.PageId == (int)EnumPage.ConstructionDesignDetail))
                                                     .FirstOrDefault();
                if (objPageSlug != null)
                {
                    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại. Vui lòng nhập đường dẫn mới");
                }
                var topicInfo = _entities.Topics.Where(m => m.Id == objModel.TopicId).FirstOrDefault();
                objPageSlug = new PageSlug();
                objPageSlug.PageId = topicInfo.Category.PageCategories.FirstOrDefault().PageId;
                objPageSlug.Slug = objModel.Slug;
                objPageSlug.IsDeleted = false;
                objPageSlug.Created_at = DateTime.Now;
                objPageSlug.Created_by = 1;
                _entities.PageSlugs.Add(objPageSlug);

                var objPost = new TopicDetail();
                objPost.TopicId = objModel.TopicId;
                objPost.Title = objModel.Title;
                objPost.Slug = objModel.Slug;
                objPost.Content = objModel.Content;
                objPost.Description = objModel.Description;
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

                _entities.TopicDetails.Add(objPost);
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

        public static ResponseAPI UpdateTopic(PostDetailManageBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                objModel.Slug = objModel.Slug.NonUnicode().Split(' ').Join("-").ToLower();
                var objPageSlug = _entities.PageSlugs.Where(m => !m.IsDeleted && m.Slug.Equals(objModel.Slug)).ToList();
                //if (objPageSlug.Count > 1)
                //{
                //    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại. Vui lòng nhập đường dẫn mới");
                //}
                //if (objPageSlug.Any(m => (m.PageId != (int)EnumPage.TopicDetail && m.PageId != (int)EnumPage.ConstructionDesignDetail)))
                //{
                //    return ResponseAPI.GetFailedResponse("Đường dẫn đã tồn tại. Vui lòng nhập đường dẫn mới");
                //}

                var topicInfo = _entities.Topics.Where(m => m.Id == objModel.TopicId).FirstOrDefault();
                if (objPageSlug.Count == 0)
                {
                    var newPageSlug = new PageSlug();
                    newPageSlug.PageId = topicInfo.Category.PageCategories.FirstOrDefault().PageId;
                    newPageSlug.Slug = objModel.Slug;
                    newPageSlug.IsDeleted = false;
                    newPageSlug.Created_at = DateTime.Now;
                    newPageSlug.Created_by = 1;
                    _entities.PageSlugs.Add(newPageSlug);
                }
                else
                {
                    objPageSlug[0].PageId = topicInfo.Category.PageCategories.FirstOrDefault().PageId;
                    objPageSlug[0].Slug = objModel.Slug;
                    objPageSlug[0].IsDeleted = false;
                    objPageSlug[0].Updated_at = DateTime.Now;
                    objPageSlug[0].Updated_by = 1;
                }

                var objTopic = _entities.TopicDetails.Where(m => m.Id == objModel.Id).FirstOrDefault();
                if (objTopic == null)
                    return ResponseAPI.GetFailedResponse("Không tìm thấy  bài viết để cập nhật");

                objTopic.TopicId = objModel.TopicId;
                objTopic.Title = objModel.Title;
                objTopic.Slug = objModel.Slug;
                objTopic.Content = objModel.Content;
                objTopic.Description = objModel.Description;
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

        public static ResponseAPI Delete(int? Id)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objTopic = _entities.Topics.Where(m => m.Id == Id).FirstOrDefault();
                if (objTopic == null)
                {
                    return ResponseAPI.GetFailedResponse("Không tìm thấy bài viết để xóa");
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

        #endregion TopicDetail Manage
    }
}