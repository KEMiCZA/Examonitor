using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Examonitor.Models
{
    public class MonitorBeurtModel
    {

        
        [Key] public int MonitorBeurtId { get; set; }
        [Display(Name = "Aangemaakt Op")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Datum { get; set; }
        public string Start { get; set; }
        public string Einde { get; set; }
        public string Duurtijd { get; set; }
        public int Capaciteit { get; set; }
        public int Gereserveerd { get; set; }
        [Display(Name = "Aangemaakt Op")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AangemaaktOp { get; set; }
        [Display(Name = "Aangepast Op")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Aangepast { get; set; }
        public string Soort { get; set; }
        public string Campus { get; set; }
        public string Departement { get; set; }
    }
}