using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models.Service
{
    public class ServiceIssuePlaningItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IssuesCount { get; set; }

        public ServiceIssuePlaningItem(int id, string name, int issuesCount)
        {
            Id = id;
            Name = name;
            IssuesCount = issuesCount;
        }
    }
}