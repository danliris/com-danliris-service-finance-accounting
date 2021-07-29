using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentFinance.MemorialDetail
{
    public class GarmentFinanceMemorialDetailModel : StandardEntity
    {
        [MaxLength(20)]
        public string MemorialNo { get; set; }
        public int MemorialId { get; set; }
        public DateTimeOffset MemorialDate { get; set; }

        public virtual ICollection<GarmentFinanceMemorialDetailItemModel> Items { get; set; }

    }
}
