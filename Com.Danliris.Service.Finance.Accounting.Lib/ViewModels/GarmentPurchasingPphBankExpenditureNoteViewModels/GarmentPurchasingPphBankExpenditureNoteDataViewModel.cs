using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingPphBankExpenditureNote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentPurchasingPphBankExpenditureNoteViewModels
{
    public class GarmentPurchasingPphBankExpenditureNoteDataViewModel
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string No { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public string BankAccountName { get; set; }
        public string IncomeTaxName { get; set; }
        public double IncomeTaxRate { get; set; }
        public Int64 TotalDpp { get; set; }
        public double TotalIncomeTax { get; set; }
        public string Currency { get; set; }
        public DateTimeOffset LastModifiedUtc { get; set; }
        public bool IsPosted { get; set; }

        public GarmentPurchasingPphBankExpenditureNoteDataViewModel()
        {

        }

        public GarmentPurchasingPphBankExpenditureNoteDataViewModel(GarmentPurchasingPphBankExpenditureNoteModel model)
        {
            Id = model.Id;
            Date = model.InvoiceOutDate;
            No = model.InvoiceOutNumber;
            CreatedUtc = model.CreatedUtc;
            BankAccountName = model.BankName;
            IncomeTaxName = model.IncomeTaxName;
            IncomeTaxRate = model.IncomeTaxRate;
            TotalIncomeTax = model.Items.Sum(element => element.IncomeTaxTotal);
            Currency = model.BankCurrencyCode;
            LastModifiedUtc = model.LastModifiedUtc;
            IsPosted = model.IsPosted


        }
    }
}
