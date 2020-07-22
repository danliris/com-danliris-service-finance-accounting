using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib
{
    public class VbVerificationResultList
    {
        public DateTimeOffset DateVerified { get; set; }
        public string RealizeNo { get; set; }
        public DateTimeOffset DateRealize { get; set; }
        public string RequestName { get; set; }
        public string UnitRequest { get; set; }
        public string SendTo { get; set; }
        public string VbNo { get; set; }
        public string VBCategory { get; set; }
        public string Currency { get; set; }
        public bool isVerified { get; set; }
    }
}