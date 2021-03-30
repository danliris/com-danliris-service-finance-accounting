using System;
using System.Collections.Generic;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MemoGarmentPurchasing;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MemoDetailGarmentPurchasing
{
    public class MemoDetailGarmentPurchasingViewModel
    {
        public int MemoId { get; set; }
        public bool IsPosted { get; set; }
        public string Remarks { get; set; }
        public ICollection<MemoDetailGarmentPurchasingDetailViewModel> MemoDetailGarmentPurchasingDetail { get; set; }
    }
}
