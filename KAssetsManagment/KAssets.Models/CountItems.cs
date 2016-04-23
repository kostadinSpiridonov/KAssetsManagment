using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class CountItems
    {
        public int Id { get; set; }

        public int Key { get; set; }

        public int Want { get; set; }

        public int Approved { get; set; }

        public int Give { get; set; }
    }
}
