using System.Collections.Generic;

namespace Com.Danliris.Service.Finance.Accounting.Lib
{
    public class VbWithPORequestDetailViewModel
    {
        public string no { get; set; }
        public Unit unit { get; set; } 
        public ICollection<VbWithPORequestDetailItemsViewModel> Details { get; set; }
    }
}