using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models.Service
{
    public class ServiceIssueTotal
    {
        public int TotalCount { get; set; }
        public int DoneCount { get; set; }
        public int UndoneCount { get; set; }
        public int SetedCount { get; set; }
        public int NoSetedCount { get; set; }
        public int PlanedCount { get; set; }
        public int NoPlanedCount { get; set; }
    }
}