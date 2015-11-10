using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Models.Service
{
    public class EngeneerNotSeted :Exception
    {
        public EngeneerNotSeted():base()
        {

        }

        public EngeneerNotSeted(string message): base(message)
        {

        }
    }
}