using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayWorkEmailListener.Objects
{
    public class ResponseMessage
    {
        public string ErrorMessage { get; set; }
        public int Id { get; set; }
        public string Email { get; set; }
    }
}
