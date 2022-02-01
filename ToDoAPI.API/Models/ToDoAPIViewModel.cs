using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ToDoAPI.API.Models
{
    //public class ToDoAPIViewModel
    //{
    //}

    public class ToDoViewModel
    {
        [Key]
        public int TodoId { get; set; }

        [Required]
        //[StringLength(50, ErrorMessage = "** Max 50 Characters **")]
        public string Action { get; set; }

        [Required]
        public bool Done { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<System.DateTime> DateStarted { get; set; }
        public Nullable<System.DateTime> DateFinished { get; set; }

        public virtual CategoryViewModel Category { get; set; }
    }

    public class CategoryViewModel
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "** Max 50 Characters **")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "** Max 100 Characters **")]
        public string Description { get; set; }
    }

}