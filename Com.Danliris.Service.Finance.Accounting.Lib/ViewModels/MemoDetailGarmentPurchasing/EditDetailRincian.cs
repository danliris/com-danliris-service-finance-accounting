using System;
using System.Collections.Generic;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MemoDetailGarmentPurchasing
{
    public class EditDetailRincian
    {
        public int MemoId { get; set; }
        public string Remarks { get; set; }
        public ICollection<EditDetailRincianItems> Items { get; set; }
    }
}
