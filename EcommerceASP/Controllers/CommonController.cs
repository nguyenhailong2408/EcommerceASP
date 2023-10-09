using EcommerceASP.Libraries;
using EcommerceASP.Queries;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers
{
    public class CommonController : Controller
    {
        // GET: Common
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetBreadcrumb(string strSlug, int parentId, int pageId)
        {
            var data = CommonQuery.GetBreadcrumb(strSlug, parentId, pageId);
            return PartialView("Components/_Top_Breadcrumb", data);
        }

        public ActionResult GetActionController(int? pageId)
        {
            var data = CommonQuery.GetActionControllerByPageId(pageId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductCategoryDetail(int? productCatId)
        {
            var data = CommonQuery.GetListProductCategoryDetail(productCatId, 0);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCategoryDetail(int? CatId)
        {
            var data = CommonQuery.GetCategoryDetail(CatId, 0);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPageBySlug(string strSlug)
        {
            strSlug = strSlug.NonUnicode().Split(' ').Join("-").ToLower();
            var data = CommonQuery.GetPageBySlug(strSlug);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckExistSlug(string strSlug)
        {
            strSlug = strSlug.NonUnicode().Split(' ').Join("-").ToLower();
            var data = CommonQuery.CheckExistSlug(strSlug);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadImageProduct(HttpPostedFileBase upload)
        {
            HttpPostedFileBase file = upload;
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = Guid.NewGuid() + "";
                    fileExtension = fileExtension.ToLower();
                    string[] acceptedExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (!acceptedExtensions.Contains(fileExtension))
                    {
                        return Json(new { uploaded = 0, error = new { message = "Chỉ chấp nhận file với định dạng jpg, jpeg, png và gif" } });
                    }
                    string contentPath = Server.MapPath("~/Content/images");
                    string directory = string.Concat(contentPath, "/productContent/");
                    string path = string.Concat(directory, fileName + fileExtension);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    if (System.IO.File.Exists(path))
                    {
                        return Json(new { uploaded = 0, error = new { message = "Tên File đã tồn tại" } });
                    }
                    file.SaveAs(path);
                    return Json(new
                    {
                        uploaded = 1,
                        fileName = fileName + fileExtension,
                        url = string.Concat("/Content/images/productContent/", fileName + fileExtension)
                    });
                }
            }
            return Json(new
            {
                uploaded = 0,
                error = new { message = "Lỗi upload file" }
            });
        }

        public ActionResult UploadImageTopic(HttpPostedFileBase upload)
        {
            HttpPostedFileBase file = upload;
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = Guid.NewGuid() + "";
                    fileExtension = fileExtension.ToLower();
                    string[] acceptedExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (!acceptedExtensions.Contains(fileExtension))
                    {
                        return Json(new { uploaded = 0, error = new { message = "Chỉ chấp nhận file với định dạng jpg, jpeg, png và gif" } });
                    }
                    string contentPath = Server.MapPath("~/Content/images");
                    string directory = string.Concat(contentPath, "/topic/");
                    string path = string.Concat(directory, fileName + fileExtension);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    if (System.IO.File.Exists(path))
                    {
                        return Json(new { uploaded = 0, error = new { message = "Tên File đã tồn tại" } });
                    }
                    file.SaveAs(path);
                    return Json(new
                    {
                        uploaded = 1,
                        fileName = fileName + fileExtension,
                        url = string.Concat("/Content/images/topic/", fileName + fileExtension)
                    });
                }
            }
            return Json(new
            {
                uploaded = 0,
                error = new { message = "Lỗi upload file" }
            });
        }

        public ActionResult UploadImageBanner(HttpPostedFileBase upload)
        {
            HttpPostedFileBase file = upload;
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = Guid.NewGuid() + "";
                    fileExtension = fileExtension.ToLower();
                    string[] acceptedExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (!acceptedExtensions.Contains(fileExtension))
                    {
                        return Json(new { uploaded = 0, error = new { message = "Chỉ chấp nhận file với định dạng jpg, jpeg, png và gif" } });
                    }
                    string contentPath = Server.MapPath("~/Content/images");
                    string directory = string.Concat(contentPath, "/banner/");
                    string path = string.Concat(directory, fileName + fileExtension);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    if (System.IO.File.Exists(path))
                    {
                        return Json(new { uploaded = 0, error = new { message = "Tên File đã tồn tại" } });
                    }
                    file.SaveAs(path);
                    return Json(new
                    {
                        uploaded = 1,
                        fileName = fileName + fileExtension,
                        url = string.Concat("/Content/images/banner/", fileName + fileExtension)
                    });
                }
            }
            return Json(new
            {
                uploaded = 0,
                error = new { message = "Lỗi upload file" }
            });
        }

        public ActionResult UploadImageComponent(HttpPostedFileBase upload)
        {
            HttpPostedFileBase file = upload;
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = Guid.NewGuid() + "";
                    fileExtension = fileExtension.ToLower();
                    string[] acceptedExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (!acceptedExtensions.Contains(fileExtension))
                    {
                        return Json(new { uploaded = 0, error = new { message = "Chỉ chấp nhận file với định dạng jpg, jpeg, png và gif" } });
                    }
                    string contentPath = Server.MapPath("~/Content/images");
                    string directory = string.Concat(contentPath, "/component/");
                    string path = string.Concat(directory, fileName + fileExtension);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    if (System.IO.File.Exists(path))
                    {
                        return Json(new { uploaded = 0, error = new { message = "Tên File đã tồn tại" } });
                    }
                    file.SaveAs(path);
                    return Json(new
                    {
                        uploaded = 1,
                        fileName = fileName + fileExtension,
                        url = string.Concat("/Content/images/component/", fileName + fileExtension)
                    });
                }
            }
            return Json(new
            {
                uploaded = 0,
                error = new { message = "Lỗi upload file" }
            });
        }

        public ActionResult UploadImageProject(HttpPostedFileBase upload)
        {
            HttpPostedFileBase file = upload;
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = Guid.NewGuid() + "";
                    fileExtension = fileExtension.ToLower();
                    string[] acceptedExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (!acceptedExtensions.Contains(fileExtension))
                    {
                        return Json(new { uploaded = 0, error = new { message = "Chỉ chấp nhận file với định dạng jpg, jpeg, png và gif" } });
                    }
                    string contentPath = Server.MapPath("~/Content/images");
                    string directory = string.Concat(contentPath, "/project/");
                    string path = string.Concat(directory, fileName + fileExtension);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    if (System.IO.File.Exists(path))
                    {
                        return Json(new { uploaded = 0, error = new { message = "Tên File đã tồn tại" } });
                    }
                    file.SaveAs(path);
                    return Json(new
                    {
                        uploaded = 1,
                        fileName = fileName + fileExtension,
                        url = string.Concat("/Content/images/project/", fileName + fileExtension)
                    });
                }
            }
            return Json(new
            {
                uploaded = 0,
                error = new { message = "Lỗi upload file" }
            });
        }

        public ActionResult GetComponentType(int? componentTypeId)
        {
            var data = CommonQuery.GetComponentType(componentTypeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}