using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//Step 1 - add using statements
using ToDoAPI.API.Models;   //access to the DTO's
using ToDoAPI.DATA.EF;      //access to EF
using System.Web.Http.Cors; //Added for access to modity CORS for this controller specifically

namespace ToDoAPI.API.Controllers
{
    //Step 2 -  add the line of code below to allow all origins, headers, and methods to be communicated to this controller
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CategoriesController : ApiController
    {
        //Step 3 - creeate the connection to the db
        ToDoEntities db = new ToDoEntities();

        //Step 4 - GetCategories
        //api/categories
        public IHttpActionResult GetCategories()
        {
            //Create a list of catefories from the db
            List<CategoryViewModel> cats = db.Categories.Select(c => new CategoryViewModel()
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description
            }).ToList<CategoryViewModel>();

            //handle if categories are found or not below
            if (cats.Count == 0)
            {
                return NotFound();//404
            }

            return Ok(cats);

        }//end GetCategories()

        //Step 5 - create a Get for one category
        //api/categories/id
        public IHttpActionResult GetCategories(int id)
        {
            //Create the resource object and assign it tothe results in the db
            CategoryViewModel cat = db.Categories.Where(c => c.CategoryId == id).Select(c => new CategoryViewModel()
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description
            }).FirstOrDefault();

            if (cat == null)
            {
                return NotFound();
            }

            return Ok(cat);

        }//End GetCategies

        //Step 6 - Create Functionality
        //api/categories/ (HttpPost) - POST = Create
        public IHttpActionResult PostCategory(CategoryViewModel cat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }
            db.Categories.Add(new Category()
            {
                Name = cat.Name,
                Description = cat.Description
            });

            db.SaveChanges();
            return Ok();

        }//End PostCategory

        //Step 7 - Edit Functionality
        //api/categories/ (HttpPut) Put = edit
        public IHttpActionResult PutCategory(CategoryViewModel cat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            //Category object to pass the information to the db
            Category existingCat = db.Categories.Where(c => c.CategoryId == cat.CategoryId).First();

            if (existingCat != null)
            {
                existingCat.Name = cat.Name;
                existingCat.Description = cat.Description;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }// end PutCategory

        //Step 8 - Delete Functionality
        //api/categories/id (HttpDelete)
        public IHttpActionResult DeleteCategory(int id)
        {
            Category cat = db.Categories.Where(c => c.CategoryId == id).FirstOrDefault();

            if (cat != null)
            {
                db.Categories.Remove(cat);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end DeleteCategory

        //Step 9 -  Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose(); //terminate the db object
            }

            base.Dispose(disposing);
        }
    }
}
