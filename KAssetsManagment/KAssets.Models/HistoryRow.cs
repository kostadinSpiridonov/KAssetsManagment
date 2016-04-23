using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace KAssets.Models
{
    public class HistoryRow
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public virtual DateTime Date { get; set; }
    }
}
