using System;

namespace Com.Danliris.Service.Finance.Accounting.Lib
{
    public class VbRequestList
    {
        public int Id { get; set; }
        public string VBNo { get; set; }
        public DateTimeOffset Date { get; set; }
        public string UnitLoad { get; set; }
        public string CreateBy { get; set; }
        public bool Status_Post { get; set; }
        public bool Approve_Status { get; set; }
        public bool Complete_Status { get; set; }
        public string VBRequestCategory { get; set; }
    }
}