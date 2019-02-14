using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Enums.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.Masters.COADataUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.Test.DataUtils.JournalTransaction
{
    public class JournalTransactionDataUtil
    {
        private readonly JournalTransactionService _Service;
        private readonly COADataUtil _COADataUtil;

        public JournalTransactionDataUtil(JournalTransactionService service, COADataUtil coaDataUtil)
        {
            _Service = service;
            _COADataUtil = coaDataUtil;
        }

        public JournalTransactionModel GetNewData()
        {
            var COA1 = Task.Run(() => _COADataUtil.GetTestData()).Result;
            JournalTransactionModel TestData = new JournalTransactionModel()
            {
                DocumentNo = Guid.NewGuid().ToString(),
                Description = "Description",
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = Guid.NewGuid().ToString(),
                Items = new List<JournalTransactionItemModel>()
                {
                    new JournalTransactionItemModel()
                    {
                        COAId = COA1.Id,
                        COA = COA1,
                        Remark = "Remark",
                        Debit = 10000.00,
                        Credit = 10000.00
                    }
                }
            };

            return TestData;
        }

        public JournalTransactionModel GetNewPostedData()
        {
            var COA1 = Task.Run(() => _COADataUtil.GetTestData()).Result;
            JournalTransactionModel TestData = new JournalTransactionModel()
            {
                DocumentNo = Guid.NewGuid().ToString(),
                Description = "Description",
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = "ReferenceNo",
                Status = JournalTransactionStatus.Posted,
                Items = new List<JournalTransactionItemModel>()
                {
                    new JournalTransactionItemModel()
                    {
                        COAId = COA1.Id,
                        COA = COA1,
                        Remark = "Remark",
                        Debit = 10000.00,
                        Credit = 10000.00
                    }
                }
            };

            return TestData;
        }

        public JournalTransactionViewModel GetDataToValidate()
        {
            var COA1 = Task.Run(() => _COADataUtil.GetTestData()).Result;
            JournalTransactionViewModel TestData = new JournalTransactionViewModel()
            {
                DocumentNo = Guid.NewGuid().ToString(),
                Description = "Description",
                Date = DateTimeOffset.UtcNow,
                ReferenceNo = Guid.NewGuid().ToString(),
                Items = new List<JournalTransactionItemViewModel>()
                {
                    new JournalTransactionItemViewModel()
                    {
                        COA = new COAViewModel()
                        {
                            Id = COA1.Id,
                            Code = COA1.Code,
                            CashAccount = COA1.CashAccount
                        },
                        Remark = "Remark",
                        Debit = 10000.00,
                        Credit = 0
                    },
                    new JournalTransactionItemViewModel()
                    {
                        COA = new COAViewModel()
                        {
                            Id = COA1.Id,
                            Code = COA1.Code,
                            CashAccount = COA1.CashAccount
                        },
                        Remark = "Remark",
                        Debit = 0,
                        Credit = 10000.00
                    },
                }
            };

            return TestData;
        }

        public async Task<JournalTransactionModel> GetTestData()
        {
            JournalTransactionModel model = GetNewData();
            await _Service.CreateAsync(model);
            return await _Service.ReadByIdAsync(model.Id);
        }

        public async Task<JournalTransactionModel> GetTestPostedData()
        {
            JournalTransactionModel model = GetNewPostedData();
            await _Service.CreateAsync(model);
            return await _Service.ReadByIdAsync(model.Id);
        }
    }
}
