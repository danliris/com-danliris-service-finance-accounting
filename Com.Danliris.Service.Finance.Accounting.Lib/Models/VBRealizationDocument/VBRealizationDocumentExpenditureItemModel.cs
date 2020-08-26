using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.VBRealizationDocumentNonPO;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocument
{
    public class VBRealizationDocumentExpenditureItemModel : StandardEntity
    {

        public VBRealizationDocumentExpenditureItemModel()
        {

        }

        public VBRealizationDocumentExpenditureItemModel(int vbRealizationDocumentId, VBRealizationDocumentNonPOExpenditureItemViewModel viewModel)
        {
            Date = viewModel.DateDetail.GetValueOrDefault();
            Remark = viewModel.Remark;
            Amount = viewModel.Amount;
            UseVat = viewModel.IsGetPPn;
            UseIncomeTax = viewModel.IsGetPPh;
            if(viewModel.IncomeTax != null)
            {
                IncomeTaxId = viewModel.IncomeTax.Id;
                IncomeTaxName = viewModel.IncomeTax.Name;
                IncomeTaxRate = viewModel.IncomeTax.Rate;
            }
            IncomeTaxBy = viewModel.IncomeTaxBy;
            VBRealizationDocumentId = vbRealizationDocumentId;
        }

        public DateTimeOffset Date { get; private set; }
        public string Remark { get; private set; }
        public decimal Amount { get; private set; }
        public bool UseVat { get; private set; }
        public bool UseIncomeTax { get; private set; }
        public int IncomeTaxId { get; private set; }
        [MaxLength(64)]
        public string IncomeTaxName { get; private set; }
        public double IncomeTaxRate { get; private set; }
        [MaxLength(64)]
        public string IncomeTaxBy { get; private set; }

        public int VBRealizationDocumentId { get; private set; }
    }
}
