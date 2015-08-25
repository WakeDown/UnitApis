using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models
{
    public class ListResult<T>
    {
        public IEnumerable<T> List { get; set; } 
        public int TotalCount { get; set; }

        public ListResult (IEnumerable<T> list, int count)
        {
            List = list;
            TotalCount = count;
        } 
    }
}