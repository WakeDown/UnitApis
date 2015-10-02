using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models
{
    public class ListResult<T>
    {
        public IEnumerable<T> List { get; set; }
        /// <summary>
        /// Количество сьолк всего, если применен фильтр то количество с учетом фильтра, НО без ограничения по количеству строк 
        /// </summary>
        public int TotalCount { get; set; }
        
        public ListResult (IEnumerable<T> list, int totalCount)
        {
            List = list;
            TotalCount = totalCount;
        } 
    }
}