using Com.Danliris.Service.Finance.Accounting.Lib.Models.PurchasingMemoTextile;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.PurchasingMemoTextile
{
    public class PurchasingMemoTextileService : IPurchasingMemoTextileService
    {
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;

        public PurchasingMemoTextileService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();
        }

        public int Create(FormDto form)
        {
            var model = new PurchasingMemoTextileModel(form.MemoDetail.Id,form.MemoDetail.DocumentNo, form.MemoDetail.Date, form.MemoDetail.Currency.Id, form.MemoDetail.Currency.Code, form.MemoDetail.Currency.Rate, form.AccountingBook.Id, form.AccountingBook.AccountingBookType, form.Remark, form.AccountingBook.Code);
            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
            _dbContext.PurchasingMemoTextiles.Add(model);
            _dbContext.SaveChanges();

            foreach (var item in form.Items)
            {
                var itemModel = new PurchasingMemoTextileItemModel(item.ChartOfAccount.Id, item.ChartOfAccount.Code, item.ChartOfAccount.Name, item.DebitAmount, item.CreditAmount, model.Id);
                EntityExtension.FlagForCreate(itemModel, _identityService.Username, UserAgent);
                _dbContext.PurchasingMemoTextileItems.Add(itemModel);
                _dbContext.SaveChanges();
            }

            return model.Id;
        }

        public int Delete(int id)
        {
            var deletedId = 0;

            var model = _dbContext.PurchasingMemoTextiles.FirstOrDefault(entity => entity.Id == id);

            if (model != null)
            {
                EntityExtension.FlagForDelete(model, _identityService.Username, UserAgent);
                _dbContext.PurchasingMemoTextiles.Update(model);

                var items = _dbContext.PurchasingMemoTextileItems.Where(entity => entity.PurchasingMemoTextileId == id).ToList();
                items = items.Select(item =>
                {
                    EntityExtension.FlagForDelete(item, _identityService.Username, UserAgent);
                    return item;
                }).ToList();

                _dbContext.PurchasingMemoTextileItems.UpdateRange(items);
                _dbContext.SaveChanges();
                deletedId = model.Id;
            }
            return deletedId;
        }

        public ReadResponse<IndexDto> Read(string keyword, int page = 1, int size = 25)
        {
            var query = _dbContext.PurchasingMemoTextiles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(entity => entity.MemoDetailDocumentNo.Contains(keyword));

            var count = query.Count();
            var data = query.Skip((page - 1) * size).Take(size).Select(entity => new IndexDto(entity.Id, entity.MemoDetailDocumentNo, entity.MemoDetailDate, entity.AccountingBookType, entity.MemoDetailCurrencyCode, entity.Remark)).ToList();
            return new ReadResponse<IndexDto>(data, count, new Dictionary<string, string>(), new List<string>());
        }

        public PurchasingTextileDto Read(int id)
        {
            throw new NotImplementedException();
        }

        public int Update(int id, FormDto form)
        {
            throw new NotImplementedException();
        }
    }
}
