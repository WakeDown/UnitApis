using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataProvider.Objects
{
    public class ItemExistsException:Exception
    {
        public ItemExistsException():base()
        {
            
        }

        public ItemExistsException(string message): base(message)
        {
            
        }
    }
}