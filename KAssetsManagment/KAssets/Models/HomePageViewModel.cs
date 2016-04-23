using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KAssets.Models
{
    public class HomePageViewModel
    {
        public int Events { get; set; }

        public int Employess { get; set; }

        public int Items { get; set; }

        public int Assets { get; set; }

        public List<DataTypeMorrisArea<int,string>> AquiredAssets { get; set; }

        public List<DataTypeMorrisArea<int,string>> ScrappedAssets { get; set; }

        public List<DataTypeMorrisArea<int,string>> RenovatedAssets { get; set; }

        public List<DataTypeMorrisArea<int, string>> IssuedInvoices { get; set; }

        public List<DataTypeMorrisArea<int, string>> SystemAccidents { get; set; }
    }

    public class DataTypeMorrisArea<T,T1>
    {
        public T Value { get; set; }

        public T1 Date { get; set; }
    }

    public class CountNewThingsViewModel
    {
        public int ItemOrderRequestsForApproving { get; set; }

        public int ItemOrderApprovedRequests { get; set; }

        public int ItemOrderRequestsForFinishing { get; set; }


        public int AssetOrderRequestsForApproving { get; set; }

        public int AssetOrderApprovedRequests { get; set; }

        public int AssetOrderRequestsForFinishing { get; set; }


        public int ProviderOrderRequestsForApproving { get; set; }

        public int ProviderOrderApprovedRequests { get; set; }


        public int ScrappingRequests { get; set; }


        public int RelocationRequestsForApproving { get; set; }

        public int RelocationsForIssue { get; set; }

        public int RelocationForIssueAll { get; set; }

        public int RelocationReceive { get; set; }

        public int RelocationReceiveAll { get; set; }

       
        
        public int RenovationRequestsForApproving { get; set; }

        public int RenovationApprovedRequests { get; set; }

        public int RenovationAssetForRenovating { get; set; }

        public int RenovationReturnedAssets { get; set; }


        public int InvoicesForApproving { get; set; }

        public int InvoicesForPaid { get; set; }

        public int AccidentsForAnswering { get; set; }

        public int AccidnetsAnswers { get; set; }
    }
}