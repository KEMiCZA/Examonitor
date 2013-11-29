using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Examonitor.Models
{
    public class MonitorBeurtModel
    {
        [Key] public int MonitorBeurtId { get; set; }
        [Display(Name = "Examen")]
        public string ExamenNaam { get; set; }
        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Datum { get; set; }
        public string Start { get; set; }
        public string Einde { get; set; }
        public string Duurtijd { get; set; }
        public int Capaciteit { get; set; }
        public int Gereserveerd { get; set; }
        public bool Digitaal { get; set; }      
        public virtual Campus Campus { get; set; }

        // Databinding, this will be set before going to the view to style the layout correctly
        [NotMapped]
        public bool ReservedByCurrentUser { get; set; }
        [NotMapped]
        public bool Available { get; set; }
        [NotMapped]
        public int CurrentRegistratieID { get; set; }
    }
}