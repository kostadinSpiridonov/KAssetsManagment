using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Models
{
    public class RequestToProvider
    {
        public int Id { get; set; }

        public RequestToProvider()
        {
            this.sendOffers = new HashSet<ProviderItemOffer>();
            this.wantItems = new HashSet<Item>();
            this.countItems = new HashSet<CountItems>();
            this.giveItems = new HashSet<Item>();
        }
        private ICollection<CountItems> countItems;

        public virtual ICollection<CountItems> CountItems
        {
            get
            {
                return this.countItems;
            }
            set
            {
                this.countItems = value;
            }
        }

        private ICollection<ProviderItemOffer> sendOffers;

        private ICollection<Item> wantItems;

        private ICollection<Item> giveItems;

        public virtual ICollection<ProviderItemOffer> SendOffers
        {
            get
            {
                return this.sendOffers;
            }
            set
            {
                this.sendOffers = value;
            }
        }
        
        public virtual ICollection<Item> WantItems
        {
            get
            {
                return this.wantItems;
            }
            set
            {
                this.wantItems = value;
            }
        }


        public virtual ICollection<Item> GiveItems
        {
            get
            {
                return this.giveItems;
            }
            set
            {
                this.giveItems = value;
            }
        }
      
        public bool IsApproved { get; set; }

        public bool IsSeenByApprover { get; set; }

        public bool IsFinished { get; set; }

        public virtual DateTime DateOfSend { get; set; }

        public string SentSubject { get; set; }

        public string SentBody { get; set; }


        public string FromId { get; set; }

        [ForeignKey("FromId")]
        public virtual ApplicationUser From { get; set; }


        public int ProviderId { get; set; }

        [ForeignKey("ProviderId")]
        public virtual Provider Provider { get; set; }

    }
}
