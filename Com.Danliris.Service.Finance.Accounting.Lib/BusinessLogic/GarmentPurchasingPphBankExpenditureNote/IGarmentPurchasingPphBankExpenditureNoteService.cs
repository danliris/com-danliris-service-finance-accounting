using Com.Danliris.Service.Finance.Accounting.Lib.Models.GarmentPurchasingPphBankExpenditureNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentPurchasingPphBankExpenditureNoteViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingPphBankExpenditureNote
{
    public interface IGarmentPurchasingPphBankExpenditureNoteService
    {
        ReadResponse<GarmentPurchasingPphBankExpenditureNoteDataViewModel> Read(int page, int size, string order, List<string> select, string keyword, string filter);
        Task CreateAsync(FormInsert model);
        Task<GarmentPurchasingPphBankExpenditureNoteModel> ReadByIdAsync(int id);
        //Task DeleteAsync(int id);
        //Task PostingDocument(int id);
        //List<GarmentPurchasingPphBankExpenditureNoteDataViewModel> PrintInvoice(int id);
    }
}
