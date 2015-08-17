using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Stuff.Objects;

namespace Stuff.Models
{
    public class DocMeetLinkList:DbModel
    {
        public int IdDocument { get; set; }
        public IEnumerable<int> IdDepartments { get; set; }
        public IEnumerable<int> IdPositions { get; set; }
        public IEnumerable<int> IdEmployees { get; set; }
       
    }
}