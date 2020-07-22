using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;

namespace Com.Danliris.Service.Finance.Accounting.Lib
{
    public interface IVbVerificationService
    {
        ReadResponse<VbVerificationList> Read(int page, int size, string order, List<string> select, string keyword, string filter);

        //list of verification result
        ReadResponse<VbVerificationResultList> ReadVerification(int page, int size, string order, List<string> select, string keyword, string filter);
    }
}