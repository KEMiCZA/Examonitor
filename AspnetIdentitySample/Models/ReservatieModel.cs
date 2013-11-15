using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Examonitor.Models
{
    public class ReservatieModel
    {
        
        [Key]
        public int ReservatieId { get; set; }
        public int ToezichtbeurtId { get; set; }

        public string UserName { get; set; }

        [Display(Name = "Aangemaakt op")]
        [DataType(DataType.Date)]
        public DateTime AangemaaktOp { get; set; }

        [Display(Name = "Aangepast op")]
        [DataType(DataType.Date)]
        public DateTime AangepastOp { get; set; }

        [ForeignKey("ToezichtbeurtId")]
        public virtual MonitorBeurtModel Toezichtbeurt { get; set; }
        
    }
}