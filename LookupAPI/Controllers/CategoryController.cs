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

namespace LookupAPI.Controllers
{
    public class CategoryController : ApiController
    {
        private LookupEntities db = new LookupEntities();

        public class LookupCategory
        {
            public Guid  CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string Icon { get; set; }
        }

        // GET api/Category
        public List<LookupCategory> GetCategories()
        {
            List<LookupCategory> categories = new List<LookupCategory>();
            foreach (var item in db.Categories.OrderBy(w=>w.Sequence))
            {
                LookupCategory a = new LookupCategory();
                a.CategoryId = item.CategoryId;
                a.CategoryName = item.CategoryName;
                a.Icon = item.Icon;
                categories.Add(a);
            }
            return categories;
        }

        //// GET api/Category/5
        //public Category GetCategory(Guid id)
        //{
        //    Category category = db.Categories.Find(id);
        //    if (category == null)
        //    {
        //        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
        //    }

        //    return category;
        //}

        //// PUT api/Category/5
        //public HttpResponseMessage PutCategory(Guid id, Category category)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }

        //    if (id != category.CategoryId)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }

        //    db.Entry(category).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        //// POST api/Category
        //public HttpResponseMessage PostCategory(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Categories.Add(category);
        //        db.SaveChanges();

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, category);
        //        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = category.CategoryId }));
        //        return response;
        //    }
        //    else
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }
        //}

        //// DELETE api/Category/5
        //public HttpResponseMessage DeleteCategory(Guid id)
        //{
        //    Category category = db.Categories.Find(id);
        //    if (category == null)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound);
        //    }

        //    db.Categories.Remove(category);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, category);
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}