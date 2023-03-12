using EcommerceASP.Models;
using EcommerceASP.ViewModel.Account;
using EcommerceASP.ViewModel.Base;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EcommerceASP.Queries
{
    public class AccountQuery
    {
        public static ResponseAPI LoginUser(LoginBO model)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var userInfo = _entities.Accounts.Where(m => !m.IsDeleted && m.UserName.Equals(model.Username)).FirstOrDefault();
                if(userInfo == null)
                {
                    return ResponseAPI.GetFailedResponse("Tên đăng nhập không tồn tại");
                }

                var md5Pass = md5_string(model.Password);

                if(userInfo.Password != md5Pass)
                {
                    return ResponseAPI.GetFailedResponse("Mật khẩu không chính xác");
                }
                return ResponseAPI.GetSuccessResponse("Success",null);
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

        private static string md5_string(string password)
        {
            string md5_password = string.Empty;
            using (MD5 hash = MD5.Create())
            {
                md5_password = string.Join("", hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("x2")));
            }

            return md5_password;
        }
    }
}