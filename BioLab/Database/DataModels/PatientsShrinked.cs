using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioLab.Database.DataModels
{
    public class PatientsShrinked
    {
        public long? id { get; set; }
        public string full_name { get; set; }
        public DateTime? birthdate { get; set; }
        public int? passport_serial { get; set; }
        public int? passport_number { get; set; }
    }
}
