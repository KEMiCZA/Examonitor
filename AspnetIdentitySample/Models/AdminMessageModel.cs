using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Examonitor.Models
{
    public class AdminMessageModel
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Boodschap")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }
        [Display(Name = "Actief")]
        public bool Active { get; set; }
    }
}