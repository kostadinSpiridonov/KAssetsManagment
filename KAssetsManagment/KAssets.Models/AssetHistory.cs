using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class AssetHistory
    {
        [Key, ForeignKey("Asset")]
        public string AssetId { get; set; }

        public virtual Asset Asset { get; set; }


        private ICollection<HistoryRow> rows;

        public AssetHistory()
        {
            this.rows = new HashSet<HistoryRow>();
        }

        public virtual ICollection<HistoryRow> Rows
        {
            get
            {
                return this.rows;
            }
            set
            {
                this.rows = value;
            }
        }
    }
}
