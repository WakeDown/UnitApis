using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stuff.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static IEnumerable<Position> GetSelectionList()
        {
            var lst = new List<Position>();

            lst.Add(new Position() { Id = 1, Name = "Pos1" });
            lst.Add(new Position() { Id = 2, Name = "Pos2" });
            lst.Add(new Position() { Id = 3, Name = "Pos3" });

            return lst;
        }
    }

    
}