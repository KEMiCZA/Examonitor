using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Daishi.JsonParser;
using System.IO;
using Examonitor.Models;
using Newtonsoft.Json;

namespace Examonitor.Utility
{
    public class MonitorbeurtParser : JsonParser<MonitorBeurtModel>
    {
        public MonitorbeurtParser(Stream json, string jsonPropertyName) : base(json, jsonPropertyName) { }
 
        protected override void Build(MonitorBeurtModel parsable, JsonTextReader reader)
        {
            if (reader.Value == null)
                return;
            if (reader.Value.Equals("ExamenNaam"))
            {
                reader.Read();
                parsable.ExamenNaam = (string)reader.Value;
            }
            else if (reader.Value.Equals("BeginDatum"))
            {
                reader.Read();
                parsable.BeginDatum = DateTime.Parse((string)reader.Value);
            }
            else if (reader.Value.Equals("EindDatum"))
            {
                reader.Read();
                parsable.EindDatum = DateTime.Parse((string)reader.Value);
            }
            else if (reader.Value.Equals("Capaciteit"))
            {
                reader.Read();
                parsable.Capaciteit = Convert.ToInt32(reader.Value);
            }
            else if (reader.Value.Equals("Digitaal"))
            {
                reader.Read();
                parsable.Digitaal = Convert.ToBoolean(reader.Value);
            }
            else if (reader.Value.Equals("Campus"))
            {
                reader.Read();
                string campusNaam = (string)reader.Value;
                parsable.Campus = new Campus { Name = campusNaam };
            }

        }

        protected override bool IsBuilt(MonitorBeurtModel parsable, JsonTextReader reader)
        {
           var isBuilt = parsable.ExamenNaam != null && parsable.BeginDatum != null && parsable.Campus != null;
           return isBuilt || base.IsBuilt(parsable, reader);
        }
    }
}