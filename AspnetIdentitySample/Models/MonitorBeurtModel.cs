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
        [Display(Name = "Examen Naam")]
        public string ExamenNaam { get; set; }
        [Display(Name = "Begin Datum")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime BeginDatum { get; set; }
        [Display(Name = "Eind Datum")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime EindDatum { get; set; }
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
        [NotMapped]
        public string Duurtijd { get; set; }
    }
}