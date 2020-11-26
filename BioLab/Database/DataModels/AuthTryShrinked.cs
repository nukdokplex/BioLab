using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioLab.Database.DataModels
{
    public class AuthTryShrinked
    {
        public DateTime tried_at { get; set; }
        public string login { get; set; }
        public bool is_successful { get; set; }
    }
}
