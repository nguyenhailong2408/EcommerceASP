using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EcommerceASP.Libraries
{
    public static class Common
    {
        public static string GuidName(this HttpPostedFileBase x)
        {
            string guid = Guid.NewGuid().ToString();
            return x.FileName.Replace(Path.GetFileNameWithoutExtension(x.FileName), guid);
        }

        public static string NonUnicode(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ", "đ", "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ", "í", "ì", "ỉ", "ĩ", "ị", "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ", "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự", "ý", "ỳ", "ỷ", "ỹ", "ỵ", };
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "d", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "i", "i", "i", "i", "i", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "y", "y", "y", "y", "y", };
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

        public static string JoinTextWithCharacter(string[] text, string character)
        {
            var textTemp = string.Empty;
            foreach (var item in text)
            {
                textTemp += item + character;
            }
            return textTemp;
        }

        public static string Join(this IEnumerable<string> listObject, string separator = ",")
        {
            if (listObject == null || listObject.Count() == 0)
            {
                return string.Empty;
            }
            else
            {
                return string.Join(separator, listObject);
            }
        }

        public static string JoinTrim(this IEnumerable<string> listObject, string separator = ",")
        {
            if (listObject == null || listObject.Count() == 0)
            {
                return string.Empty;
            }
            else
            {
                return string.Join(separator, listObject.Select(s => s.Trim()));
            }
        }

        public static string ConvertDateTimeToStrVietNamese(this DateTime? dateTime)
        {
            Dictionary<int, string> dicMonths = new Dictionary<int, string>();
            dicMonths.Add(0, "Không tìm thấy");
            dicMonths.Add(1, "Tháng một");
            dicMonths.Add(2, "Tháng hai");
            dicMonths.Add(3, "Tháng ba");
            dicMonths.Add(4, "Tháng bốn");
            dicMonths.Add(5, "Tháng năm");
            dicMonths.Add(6, "Tháng sáu");
            dicMonths.Add(7, "Tháng bảy");
            dicMonths.Add(8, "Tháng tám");
            dicMonths.Add(9, "Tháng chín");
            dicMonths.Add(10, "Tháng mười");
            dicMonths.Add(11, "Tháng mười một");
            dicMonths.Add(12, "Tháng mười hai");
            if (dateTime == null)
                dateTime = DateTime.Now;
            var day = dateTime?.Day;
            var month = dateTime == null ? 0: dateTime?.Month;
            var year = dateTime?.Year;

            return day + " " + dicMonths[month.Value] + ", " + year;
        }
    }
}