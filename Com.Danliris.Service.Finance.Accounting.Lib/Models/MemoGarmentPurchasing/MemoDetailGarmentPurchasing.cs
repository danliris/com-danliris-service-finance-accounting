using System;
using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.MemoGarmentPurchasing
{
    public class MemoDetailGarmentPurchasing: StandardEntity
    {
        public int MemoId { get; set; }
        [ForeignKey("MemoId")]
        public int IsPosted { get; set; }
        public string Remarks { get; set; }
    }
}
