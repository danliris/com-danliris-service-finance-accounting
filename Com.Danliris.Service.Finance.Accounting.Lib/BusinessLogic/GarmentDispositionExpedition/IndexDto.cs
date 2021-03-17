using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentDispositionExpedition;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentDispositionExpedition
{
    public class IndexDto
    {
        public IndexDto(int id, string dispositionNoteNo, DateTimeOffset dispositionNoteDate, DateTimeOffset dispositionNoteDueDate, int dispositionNoteId, double currencyTotalPaid, double totalPaid, int currencyId, string currencyCode, string suppliername, string remark)
        {
            Id = id;
            DispositionNoteNo = dispositionNoteNo;
            DispositionNoteDate = dispositionNoteDate;
            DispositionNoteDueDate = dispositionNoteDueDate;
            DispositionNoteId = dispositionNoteId;
            CurrencyTotalPaid = currencyTotalPaid;
            TotalPaid = totalPaid;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            SupplierName = suppliername;
            Remark = remark;
        }

        public int Id { get; private set; }
        public string DispositionNoteNo { get; private set; }
        public DateTimeOffset DispositionNoteDate { get; private set; }
        public DateTimeOffset DispositionNoteDueDate { get; private set; }
        public int DispositionNoteId { get; private set; }
        public double CurrencyTotalPaid { get; private set; }
        public double TotalPaid { get; private set; }
        public int CurrencyId { get; private set; }
        public string CurrencyCode { get; private set; }
        public string SupplierName { get; private set; }
        public string Remark { get; private set; }
    }
}
