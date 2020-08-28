using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument
{
    public enum VBType
    {
        WithPO = 1,
        NonPO
    }

    public enum RealizationDocumentType
    {
        WithVB = 1,
        NonVB
    }
}
