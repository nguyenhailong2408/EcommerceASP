using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.Constaint
{
    public class EnumConstaint
    {
    }
    public enum EnumPage
    {
        /// <summary>
        /// Trang chủ
        /// </summary>
        Home = 1,
        /// <summary>
        /// Sản phẩm
        /// </summary>
        Product = 2,
        /// <summary>
        /// Thiết kế thi công
        /// </summary>
        ConstructionDesign = 3,

        /// <summary>
        /// Tin tức
        /// </summary>
        Topic = 4,

        /// <summary>
        /// Chi tiết sản phẩm
        /// </summary>
        ProductDetail = 8,

        /// <summary>
        /// Chi tiết thiết kế nội thất
        /// </summary>
        ConstructionDesignDetail = 9,

        /// <summary>
        /// Chi tiết tin tức
        /// </summary>
        TopicDetail = 10,
    }

    public enum EnumComponentType
    {
        /// <summary>
        /// SlideShow
        /// </summary>
        SlideShow = 1,

        /// <summary>
        /// Slide dạng 3 cột
        /// </summary>
        TopicThreeCollumn = 2,

        /// <summary>
        /// Slide sản phẩm
        /// </summary>
        Product = 3,

        /// <summary>
        /// Slide bài viết 5 cột
        /// </summary>
        TopicFiveCollumn = 4,

        /// <summary>
        /// Banner
        /// </summary>
        Banner = 5,
    }
}