using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models.Service
{
    public class ServiceIssuePlaningResult
    {
        public ServiceIssuePlaningResult(ServiceIssuePlaningCityItem[] citiesList)
        {
            CitiesList = citiesList;
        }
        public ServiceIssuePlaningCityItem[] CitiesList { get; set; }

        public class ServiceIssuePlaningCityItem
        {
            public ServiceIssuePlaningCityItem(int id, string name, int issuesCount)
            {
                Id = id;
                Name = name;
                IssuesCount = issuesCount;

                AddressList = new ServiceIssuePlaningAddressItem[0];
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public int IssuesCount { get; set; }
            public ServiceIssuePlaningAddressItem[] AddressList { get; set; }

            public class ServiceIssuePlaningAddressItem
            {
                public ServiceIssuePlaningAddressItem(int id, string name, int issuesCount)
                {
                    Id = id;
                    Name = name;
                    IssuesCount = issuesCount;
                    ClientList = new ServiceIssuePlaningClientItem[0];
                }

                public int Id { get; set; }
                public string Name { get; set; }
                public int IssuesCount { get; set; }
                public ServiceIssuePlaningClientItem[] ClientList { get; set; }

                public class ServiceIssuePlaningClientItem
                {
                    public ServiceIssuePlaningClientItem(int id, string name, int issuesCount)
                    {
                        Id = id;
                        Name = name;
                        IssuesCount = issuesCount;
                        DeviceList = new ServiceIssuePlaningDeviceItem[0];
                    }

                    public int Id { get; set; }
                    public string Name { get; set; }
                    public int IssuesCount { get; set; }
                    public ServiceIssuePlaningDeviceItem[] DeviceList { get; set; }

                    public class ServiceIssuePlaningDeviceItem
                    {
                        public ServiceIssuePlaningDeviceItem(int id, string name, int issuesCount)
                        {
                            Id = id;
                            Name = name;
                            IssuesCount = issuesCount;
                        }

                        public int Id { get; set; }
                        public string Name { get; set; }
                        public int IssuesCount { get; set; }
                    }
                }
            }
        }
    }


}