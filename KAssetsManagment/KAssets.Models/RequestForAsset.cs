using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class RequestForAsset
    {
        public int Id { get; set; }

        public RequestForAsset()
        {
            this.assets = new HashSet<Asset>();
            this.givenAssets = new HashSet<Asset>();
            this.approvedAssets = new HashSet<Asset>();
        }

        private ICollection<Asset> assets;
        private ICollection<Asset> approvedAssets;
        private ICollection<Asset> givenAssets;


        [Required]
        public string FromId { get; set; }
     
        [ForeignKey("FromId")]
        public virtual ApplicationUser From { get; set; }


        public virtual ICollection<Asset> Assets
        {
            get
            {
                return this.assets;
            }
            set
            {
                this.assets = value;
            }
        }

        public virtual ICollection<Asset> ApprovedAssets
        {
            get
            {
                return this.approvedAssets;
            }
            set
            {
                this.approvedAssets = value;
            }
        }

        public virtual ICollection<Asset> GivenAssets
        {
            get
            {
                return this.givenAssets;
            }
            set
            {
                this.givenAssets = value;
            }
        }

        public virtual DateTime DateOfSend { get; set; }

        public bool IsApproved { get; set; }

        public bool AreAssetsGave { get; set; }

        public bool Finished { get; set; }
        

        public int? PackingSlipId { get; set; }

        [ForeignKey("PackingSlipId")]
        public virtual PackingSlip PackingSlip { get; set; }
    }
}
