using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LookupAPI.Models;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace LookupAPI.Controllers
{
    public class LookupCategory
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int CategorySequence { get; set; }
        public string Icon { get; set; }
        public List<LookUpContact> Contacts { get; set; }
    }
    public class LookUpContact
    {
        public Guid ContactID { get; set; }
        public string ContactName { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public List<LookupContactDetail> Details { get; set; }
    }
    public class LookupContactDetail
    {
        public string Type { get; set; }
        public string Detail { get; set; }
        public string Icon { get; set; }
        public string Conditions { get; set; }
        public bool IsTollFree { get; set; }
    }
    public class ContactController : ApiController
    {
        private LookupEntities db = new LookupEntities();
        private string countryName = "", countryCode = "", stateName = "", stateCode = "", cityName = "";

        // GET api/Contact
        public List<LookupCategory> GetContacts(double latitude, double longitude)
        {
            GetLocation();
            var countrylevelcontacts = db.Contacts.Where(w => w.Country.CountryName == countryName && w.State == null && w.City == null);
            var statelevelcontacts = db.Contacts.Where(w => w.Country.CountryName == countryName && w.State.StateName == stateName && w.City == null);
            var citylevelcontacts = db.Contacts.Where(w => w.Country.CountryName == countryName && w.State.StateName == stateName && w.City.CityName == cityName);
            List<LookupCategory> categories = new List<LookupCategory>();
            FormatContacts(countrylevelcontacts, categories);
            FormatContacts(statelevelcontacts, categories);
            FormatContacts(citylevelcontacts, categories);
            return categories;
        }


        private static void FormatContacts(IQueryable<Contact> countrylevelcontacts, List<LookupCategory> categories)
        {
            List<string> categorynames = new List<string>();
            LookupCategory currentCategory = new LookupCategory();
            foreach (Contact contact in countrylevelcontacts)
            {
                if (!categorynames.Contains(contact.Category.CategoryName))
                {
                    LookupCategory category = new LookupCategory();
                    category.CategoryId = contact.CategoryId;
                    category.CategoryName = contact.Category.CategoryName;
                    category.CategorySequence = (int)contact.Category.Sequence;
                    category.Icon = contact.Category.Icon;
                    category.Contacts = new List<LookUpContact>();
                    categories.Add(category);
                    categorynames.Add(category.CategoryName);
                    currentCategory = category;
                }
                else
                {
                    currentCategory = categories.Where(w => w.CategoryName == contact.Category.CategoryName).FirstOrDefault();
                }
                LookUpContact lookupContact = new LookUpContact();
                lookupContact.ContactID = contact.ContactId;
                lookupContact.ContactName = contact.ContactName;
                lookupContact.Description = contact.ContactDescription;
                lookupContact.Details = new List<LookupContactDetail>();
                foreach (ContactDetail detail in contact.ContactDetails)
                {
                    LookupContactDetail lookupDetail = new LookupContactDetail();
                    lookupDetail.Detail = detail.ContactDetails;
                    lookupDetail.Conditions = detail.Conditions;
                    lookupDetail.Icon = detail.Icon;
                    lookupDetail.IsTollFree = (bool)((detail.IsTollFree == null) ? false : detail.IsTollFree);
                    lookupDetail.Type = detail.ContactType.ContactTypeName;
                    lookupContact.Details.Add(lookupDetail);
                }
                currentCategory.Contacts.Add(lookupContact);
            }
        }

        private void GetLocation()
        {
            string url = "http://maps.googleapis.com/maps/api/geocode/json?sensor=true&latlng=" + 37.78 + "," + -122.42;
            WebRequest request = WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var responseObject = new LookupAPI.Models.LatLongToLocation.RootObject();
            var jsonSerializer = new DataContractJsonSerializer(typeof(LookupAPI.Models.LatLongToLocation.RootObject));
            responseObject = (LookupAPI.Models.LatLongToLocation.RootObject)jsonSerializer.ReadObject(response.GetResponseStream());

            string rawJson = new StreamReader(response.GetResponseStream()).ReadToEnd();
            foreach (LookupAPI.Models.LatLongToLocation.Result item in responseObject.results)
            {
                if (item.types.Contains("country"))
                {
                    countryName = ((List<LookupAPI.Models.LatLongToLocation.AddressComponent>)item.address_components)[0].long_name;
                    countryCode = ((List<LookupAPI.Models.LatLongToLocation.AddressComponent>)item.address_components)[0].short_name;
                }
                if (item.types.Contains("administrative_area_level_1"))
                {
                    stateName = ((List<LookupAPI.Models.LatLongToLocation.AddressComponent>)item.address_components)[0].long_name;
                    stateCode = ((List<LookupAPI.Models.LatLongToLocation.AddressComponent>)item.address_components)[0].short_name;
                }
                if (item.types.Contains("administrative_area_level_2"))
                {
                    cityName = ((List<LookupAPI.Models.LatLongToLocation.AddressComponent>)item.address_components)[0].long_name;
                }
            }
        }

        // GET api/Contact/5
        public Contact GetContact(Guid id)
        {
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return contact;
        }

        ////// PUT api/Contact/5
        ////public HttpResponseMessage PutContact(Guid id, Contact contact)
        ////{
        ////    if (!ModelState.IsValid)
        ////    {
        ////        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        ////    }

        ////    if (id != contact.ContactId)
        ////    {
        ////        return Request.CreateResponse(HttpStatusCode.BadRequest);
        ////    }

        ////    db.Entry(contact).State = EntityState.Modified;

        ////    try
        ////    {
        ////        db.SaveChanges();
        ////    }
        ////    catch (DbUpdateConcurrencyException ex)
        ////    {
        ////        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        ////    }

        ////    return Request.CreateResponse(HttpStatusCode.OK);
        ////}

        ////// POST api/Contact
        ////public HttpResponseMessage PostContact(Contact contact)
        ////{
        ////    if (ModelState.IsValid)
        ////    {
        ////        db.Contacts.Add(contact);
        ////        db.SaveChanges();

        ////        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, contact);
        ////        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = contact.ContactId }));
        ////        return response;
        ////    }
        ////    else
        ////    {
        ////        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        ////    }
        ////}

        ////// DELETE api/Contact/5
        ////public HttpResponseMessage DeleteContact(Guid id)
        ////{
        ////    Contact contact = db.Contacts.Find(id);
        ////    if (contact == null)
        ////    {
        ////        return Request.CreateResponse(HttpStatusCode.NotFound);
        ////    }

        ////    db.Contacts.Remove(contact);

        ////    try
        ////    {
        ////        db.SaveChanges();
        ////    }
        ////    catch (DbUpdateConcurrencyException ex)
        ////    {
        ////        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        ////    }

        ////    return Request.CreateResponse(HttpStatusCode.OK, contact);
        ////}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}