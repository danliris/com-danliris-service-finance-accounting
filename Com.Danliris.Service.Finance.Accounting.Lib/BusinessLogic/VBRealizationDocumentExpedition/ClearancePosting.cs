using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition
{
    public class ClearancePosting
    {
        public ClearancePosting()
        {
            Ids = new HashSet<int>();
        }

        public IEnumerable<int> Ids { get; set; }
        public DateTimeOffset ClearanceDate { get; set; }
    }
}
