using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.API.Models;   //access to the DTO's
using ToDoAPI.DATA.EF;      //access to EF
using System.Web.Http.Cors; //Added for access to modity CORS for this controller specifically

namespace ToDoAPI.API.Controllers
{
    //Step 5 - Create the first controller
    //Upon creating this controller, we need to add some functionality to the API controller.
    //1. We need to access the ViewModel thru a using statement
    //2. Access the data layer thru using statement
    //3. Install Cross Origin Resource Sharing (CORS) functionality into the API layer
    //3a. Install-Package Microsoft.AspNet.WebApi.Cors
    //3b. Navigate to the App_Start/WebApiConfig.cs and add a line of code to allow for CORS across this API functionality
    //3c. Add a using statement to allow CORS in this controller
    //3d. Add a metadata tag to configure CORS for this controller

    //Origins: Who can connect? * means all
    //Headers: What type of data? XML, JSON, etc.
    //Methods: GET(read), POST(create), PUT(edit), DELETE
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class ToDoController : ApiController
    {
        //Crate an object that will connect to the db
        ToDoEntities db = new ToDoEntities();

        //Read - Get Functionality
        //api/todo
        public IHttpActionResult GetTodos()
        {
            List<ToDoViewModel> todos = db.TodoItems.Include("Category").Select(t => new ToDoViewModel()
            {
                TodoId = t.TodoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                DateStarted = t.DateStarted,
                DateFinished = t.DateFinished,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description,
                }
            }).ToList<ToDoViewModel>();

            if (todos.Count == 0)
            {
                return NotFound();
            }

            return Ok(todos);
        }//end GetTodos

        //api/todo/id
        public IHttpActionResult GetTodo(int id)
        {
            //Create a resource view model object to house our data
            ToDoViewModel todo = db.TodoItems.Include("Category").Where(t => t.TodoId == id).Select(t => new ToDoViewModel()
            {
                TodoId = t.TodoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                DateStarted = t.DateStarted,
                DateFinished = t.DateFinished,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description,
                }
            }).FirstOrDefault();

            if (todo == null)
            {
                return NotFound();
            }

            return Ok(todo);

        }//end GetTodo(id)

        //api/todo (HttpPost) - Post = Create
        public IHttpActionResult PostTodo(ToDoViewModel todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            //Translate the DTO info a Resource object to send the request to create the new object to the db
            TodoItem newTodo = new TodoItem()
            {
                Action = todo.Action,
                Done = todo.Done,
                CategoryId = todo.CategoryId,
                DateStarted = todo.DateStarted,
                DateFinished = todo.DateFinished
            };

            //Add the resource to the db table
            db.TodoItems.Add(newTodo);
            //Save Changes
            db.SaveChanges();

            return Ok(newTodo);

        }//end PostTodo

        //api/todo (HttpPut) Put = edit
        public IHttpActionResult PutTodo(ToDoViewModel todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            TodoItem existingTodo = db.TodoItems.Where(t => t.TodoId == todo.TodoId).FirstOrDefault();

            if (existingTodo != null)
            {
                existingTodo.TodoId = todo.TodoId;
                existingTodo.Action = todo.Action;
                existingTodo.CategoryId = todo.CategoryId;
                existingTodo.DateStarted = todo.DateStarted;
                existingTodo.DateFinished = todo.DateFinished;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end PutTodo

        public IHttpActionResult DeleteTodo(int id)
        {
            //get the resource
            TodoItem todo = db.TodoItems.Where(t => t.TodoId == id).FirstOrDefault();

            //if the resource is not null, delete the resource
            if (todo != null)
            {
                db.TodoItems.Remove(todo);
                db.SaveChanges();
                return Ok();
            }

            //if null - return NotFound()
            else
            {
                return NotFound();
            }

        }//end DeleteResource

        //We use Dispose() below to dispose of any connection to the db after we are done with them - best
        //practice to handle performance - dispose of instance of the controller and the instance of a db
        //connection when we are done with it
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose(); //terminate the db object
            }
            //Below dispose of hte instance of the controller
            base.Dispose(disposing);
        }

    }
}
