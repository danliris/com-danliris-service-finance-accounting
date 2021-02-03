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
        Task<FormInsert> ReadByIdAsync(int id);
        Task DeleteAsync(int id);
        Task PostingDocument(int id);
        //List<GarmentPurchasingPphBankExpenditureNoteDataViewModel> PrintInvoice(int id);
        ReadResponse<GarmentPurchasingPphBankExpenditureNoteReportViewDto> GetReportView(int page, int size, string order, GarmentPurchasingPphBankExpenditureNoteFilterReportDto filter);
        List<GarmentPurchasingPphBankExpenditureNoteLoaderInternNote> GetLoaderInterNotePPH(string keyword);
        List<GarmentPurchasingPphBankExpenditureLoaderSupplierDto> GetLoaderSupplier(string keyword);
        List<GarmentPurchasingPphBankExpenditureNoteLoaderInvoiceDto> GetLoaderInvoice(string keyword);
        List<GarmentPurchasingPphBankExpenditureNoteLoaderInvoiceOutDto> GetLoaderInvoiceOut(string keyword);
    }
}
