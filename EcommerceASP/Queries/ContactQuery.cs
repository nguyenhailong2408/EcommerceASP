using EcommerceASP.Models;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.Queries
{
    public class ContactQuery
    {
        public static ContactBO GetContact(string strSlug)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                var objCategory = _entities.Categorys.Where(m => !m.IsDeleted && m.Slug.Equals(strSlug)).FirstOrDefault();

                var objContact = new ContactBO();
                objContact.Title = objCategory?.Name;
                objContact.SlugName = objCategory?.Name;
                objContact.Slug = objCategory?.Slug;
                objContact.ParentSlugName = objCategory?.Name;
                objContact.ParentSlug = objCategory?.Slug;
                objContact.IsChild = false;


                return objContact;
            }
            catch (Exception objEx)
            {
                return new ContactBO();
            }
            finally
            {
                _entities.Dispose();
            }
        }

        public static ResponseAPI Update(ContactBO objModel)
        {
            EcommerceEntities _entities = new EcommerceEntities();
            try
            {
                Contact contact = new Contact();
                contact.FullName = objModel.FullName;
                contact.Email = objModel.Email;
                contact.Address = objModel.Address;
                contact.Content = objModel.Content;
                contact.Phone = objModel.Phone;
                contact.StateId = 0;
                contact.IsDeleted = false;
                contact.Created_at = DateTime.Now;
                contact.Created_by = 1;
                _entities.Contacts.Add(contact);
                _entities.SaveChanges();

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
    }
}