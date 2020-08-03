using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition
{
    public enum VBRealizationPosition
    {
        Purchasing = 1,
        PurchasingToVerification,
        Verification,
        VerifiedToCashier,
        Cashier,
        NotVerified
    }

    public enum VBRealizationExpeditionStatus
    {
        Cashier = 1,
        Return
    }
}
