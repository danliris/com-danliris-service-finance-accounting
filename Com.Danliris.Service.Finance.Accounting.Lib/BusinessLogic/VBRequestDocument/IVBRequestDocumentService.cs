using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument
{
    public interface  IVBRequestDocumentService
    {
        Task<int> CreateNonPO(VBRequestDocumentNonPOFormDto form);
        int UpdateNonPO(int id, VBRequestDocumentNonPOFormDto form);
        Task<VBRequestDocumentNonPODto> GetNonPOById(int id);
        int DeleteNonPO(int id);

        int CreateWithPO(VBRequestDocumentWithPOFormDto form);
        int UpdateWithPO(int id, VBRequestDocumentWithPOFormDto form);
        VBRequestDocumentWithPODto GetWithPOById(int id);
        int DeleteWithPO(int id);

        ReadResponse<VBRequestDocumentModel> Get(int page, int size, string order, List<string> select, string keyword, string filter);
    }
}
