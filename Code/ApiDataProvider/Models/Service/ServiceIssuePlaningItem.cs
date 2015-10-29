using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models.Service
{
    public class ServiceIssuePlaningItem
    {
        public string Sid { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int IssuesCount { get; set; }
        public string ShortName { get; set; }
        /// <summary>
        /// Вохдящие в группировку идентификаторы заявок
        /// </summary>
        public string IssuesIdList { get; set; }
        /// <summary>
        /// Вохдящие в группировку идентификаторы плана
        /// </summary>
        public string PlanIdList { get; set; }

        public ServiceIssuePlaningItem(int id, string name, int issuesCount, string shortName=null, string issuesIdList = null, string planIdList = null)
        {
            Id = id;
            Name = name;
            IssuesCount = issuesCount;
            ShortName = shortName;
            IssuesIdList = issuesIdList;
            PlanIdList = planIdList;
        }

        public ServiceIssuePlaningItem(string sid, string name, int issuesCount, string shortName = null, string issuesIdList = null, string planIdList = null)
        {
            Sid = sid;
            Name = name;
            IssuesCount = issuesCount;
            ShortName = shortName;
            IssuesIdList = issuesIdList;
            PlanIdList = planIdList;
        }
    }
}