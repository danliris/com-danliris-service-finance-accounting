using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.VbNonPORequest
{
    public class VbRequestDetailModel : StandardEntity
    {
        [MaxLength(64)]
        public string VBNo { get; set; }
        public int POId { get; set;}
        [MaxLength(64)]
        public string PONo { get; set; }
        public int UnitId { get; set; }
        [MaxLength(64)]
        public string UnitName { get; set; }
        [MaxLength(64)]
        public string DetailOthers { get; set; }

        public int VBId { get; set; }

        [ForeignKey("VBId")]
        public virtual VbRequestModel VbRequestDetail { get; set; }
    }
}
