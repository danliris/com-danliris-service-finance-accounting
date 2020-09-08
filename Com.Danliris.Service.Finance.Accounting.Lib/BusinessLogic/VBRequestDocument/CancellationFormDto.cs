using System.Collections.Generic;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument
{
    public class CancellationFormDto
    {
        public CancellationFormDto()
        {
            Ids = new HashSet<int>();
        }
        public IEnumerable<int> Ids { get; set; }
    }
}