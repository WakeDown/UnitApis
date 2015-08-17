using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Objects;

namespace ApiPrice.Models
{
    public class Category:DbModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IdParent { get; set; }

        public Category(DataRow row)
        {
            FillSelf(row);
        }

        private void FillSelf(DataRow row)
        {
            //Id = Db.DbHelper.GetValueInt(row["id"]);
            //Manager = new Employee(row["manager_sid"].ToString());
            //DateLimit = Db.DbHelper.GetValueDateTime(row["date_limit"]);
            //Descr = row["descr"].ToString();
            //DateCreate = Db.DbHelper.GetValueDateTime(row["dattim1"]);
            //State = new QueState() { Id = Db.DbHelper.GetValueInt(row["id_que_state"]), Name = row["que_state"].ToString() };
        }
    }
}