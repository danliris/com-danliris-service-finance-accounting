using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocument
{
    public class VBRealizationDocumentExpenditureItemModel : StandardEntity
    {
        public DateTimeOffset Date { get; private set; }
        public string Remark { get; private set; }
        public double Amount { get; private set; }
        public bool UseVat { get; private set; }
        public bool UseIncomeTax { get; private set; }
        public int IncomeTaxId { get; private set; }
        [MaxLength(64)]
        public string IncomeTaxName { get; private set; }
        public double IncomeTaxRate { get; private set; }
        [MaxLength(64)]
        public string IncomeTaxBy { get; private set; }
    }
}
