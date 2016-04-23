using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KAssets.Areas.AssetsActions.Models
{
    public class AssetHistoryViewModel
    {
        public string Id { get; set; }

        public List<HistoryRowViewModel> Rows { get; set; }
    }

    public class HistoryRowViewModel
    {
        public string Content { get; set; }

        public string Date { get; set; }
    }
}