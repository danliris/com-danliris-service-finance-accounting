using Com.Danliris.Service.Finance.Accounting.Lib.Enums;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PurchasingMemoDetailTextile;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Moonlay.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.PurchasingMemoDetailTextile
{
    public class PurchasingMemoDetailTextileService : IPurchasingMemoDetailTextileService
    {
        private const string UserAgent = "finance-service";
        private readonly FinanceDbContext _dbContext;
        private readonly IIdentityService _identityService;

        public PurchasingMemoDetailTextileService(IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetService<FinanceDbContext>();
            _identityService = serviceProvider.GetService<IIdentityService>();
        }

        private string GenerateDocumentNo(FormDto form)
        {
            var documentNo = $"{form.Date.AddHours(_identityService.TimezoneOffset):MM}{form.Date.AddHours(_identityService.TimezoneOffset):yy}.MT.0001";
            var month = form.Date.AddHours(_identityService.TimezoneOffset).Month;
            var year = form.Date.AddHours(_identityService.TimezoneOffset).Year;
            var lastDocumentNo = _dbContext.PurchasingMemoDetailTextiles.Where(entity => entity.Date.AddHours(_identityService.TimezoneOffset).Month == month && entity.Date.AddHours(_identityService.TimezoneOffset).Year == year).OrderByDescending(entity => entity.Date).Select(entity => entity.DocumentNo).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(lastDocumentNo))
            {
                var splittedDocument = lastDocumentNo.Split('.');
                var lastNumber = int.Parse(splittedDocument.LastOrDefault());
                lastNumber += 1;
                documentNo = $"{form.Date.AddHours(_identityService.TimezoneOffset):MM}{form.Date.AddHours(_identityService.TimezoneOffset):yy}.MT.{lastNumber.ToString().PadLeft(4, '0')}";
            }

            return documentNo;
        }

        public int Create(FormDto form)
        {
            var documentNo = GenerateDocumentNo(form);
            var model = new PurchasingMemoDetailTextileModel(form.Date, form.Division.Id, form.Division.Code, form.Division.Name, form.Currency.Id, form.Currency.Code, form.Currency.Rate, form.SupplierIsImport, form.Remark, form.Type, documentNo);
            EntityExtension.FlagForCreate(model, _identityService.Username, UserAgent);
            _dbContext.PurchasingMemoDetailTextiles.Add(model);
            _dbContext.SaveChanges();

            if (form.Type == PurchasingMemoType.Disposition)
            {
                foreach (var item in form.Items)
                {
                    var itemModel = new PurchasingMemoDetailTextileItemModel(item.Disposition.Id, item.Disposition.DocumentNo, item.Disposition.Date, model.Id);
                    EntityExtension.FlagForCreate(itemModel, _identityService.Username, UserAgent);
                    _dbContext.PurchasingMemoDetailTextileItems.Add(itemModel);
                    _dbContext.SaveChanges();

                    foreach (var detail in item.Disposition.Details)
                    {
                        var detailModel = new PurchasingMemoDetailTextileDetailModel(model.Id, itemModel.Id, detail.Expenditure.Id, detail.Expenditure.DocumentNo, detail.Expenditure.Date, detail.Supplier.Id, detail.Supplier.Code, detail.Supplier.Name, detail.Remark, detail.UnitPaymentOder.Id, detail.UnitPaymentOder.UnitPaymentOrderNo, detail.UnitPaymentOder.UnitPaymentOrderDate, detail.PaymentAmountCurrency, detail.PurchaseAmount, detail.PaymentAmount, detail.PaymentAmountCurrency);
                        EntityExtension.FlagForCreate(detailModel, _identityService.Username, UserAgent);
                        _dbContext.PurchasingMemoDetailTextileDetails.Add(detailModel);
                        _dbContext.SaveChanges();

                        foreach (var unitReceiptNote in detail.UnitReceiptNotes)
                        {
                            var unitReceiptNoteModel = new PurchasingMemoDetailTextileUnitReceiptNoteModel(model.Id, itemModel.Id, detailModel.Id, unitReceiptNote.Id, unitReceiptNote.UnitReceiptNoteNo, unitReceiptNote.UnitReceiptNoteDate);
                            EntityExtension.FlagForCreate(unitReceiptNoteModel, _identityService.Username, UserAgent);
                            _dbContext.PurchasingMemoDetailTextileUnitReceiptNotes.Add(unitReceiptNoteModel);
                            _dbContext.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                foreach (var detail in form.Details)
                {
                    var detailModel = new PurchasingMemoDetailTextileDetailModel(model.Id, 0, detail.Expenditure.Id, detail.Expenditure.DocumentNo, detail.Expenditure.Date, detail.Supplier.Id, detail.Supplier.Code, detail.Supplier.Name, detail.Remark, detail.UnitPaymentOder.Id, detail.UnitPaymentOder.UnitPaymentOrderNo, detail.UnitPaymentOder.UnitPaymentOrderDate, detail.PaymentAmountCurrency, detail.PurchaseAmount, detail.PaymentAmount, detail.PaymentAmountCurrency);
                    EntityExtension.FlagForCreate(detailModel, _identityService.Username, UserAgent);
                    _dbContext.PurchasingMemoDetailTextileDetails.Add(detailModel);
                    _dbContext.SaveChanges();

                    foreach (var unitReceiptNote in detail.UnitReceiptNotes)
                    {
                        var unitReceiptNoteModel = new PurchasingMemoDetailTextileUnitReceiptNoteModel(model.Id, 0, detailModel.Id, unitReceiptNote.Id, unitReceiptNote.UnitReceiptNoteNo, unitReceiptNote.UnitReceiptNoteDate);
                        EntityExtension.FlagForCreate(unitReceiptNoteModel, _identityService.Username, UserAgent);
                        _dbContext.PurchasingMemoDetailTextileUnitReceiptNotes.Add(unitReceiptNoteModel);
                        _dbContext.SaveChanges();
                    }
                }
            }

            return model.Id;
        }

        public int Delete(int id)
        {
            var deletedId = 0;

            var model = _dbContext.PurchasingMemoDetailTextiles.FirstOrDefault(entity => entity.Id == id);

            if (model != null)
            {
                EntityExtension.FlagForDelete(model, _identityService.Username, UserAgent);
                _dbContext.PurchasingMemoDetailTextiles.Update(model);

                var items = _dbContext.PurchasingMemoDetailTextileItems.Where(entity => entity.PurchasingMemoDetailTextileId == model.Id).ToList();
                var details = _dbContext.PurchasingMemoDetailTextileDetails.Where(entity => entity.PurchasingMemoDetailTextileId == model.Id).ToList();
                var unitReceiptNotes = _dbContext.PurchasingMemoDetailTextileUnitReceiptNotes.Where(entity => entity.PurchasingMemoDetailTextileId == model.Id).ToList();

                items = items.Select(element =>
                {
                    EntityExtension.FlagForDelete(element, _identityService.Username, UserAgent);
                    return element;
                }).ToList();
                _dbContext.PurchasingMemoDetailTextileItems.UpdateRange(items);

                details = details.Select(element =>
                {
                    EntityExtension.FlagForDelete(element, _identityService.Username, UserAgent);
                    return element;
                }).ToList();
                _dbContext.PurchasingMemoDetailTextileDetails.UpdateRange(details);

                unitReceiptNotes = unitReceiptNotes.Select(element =>
                {
                    EntityExtension.FlagForDelete(element, _identityService.Username, UserAgent);
                    return element;
                }).ToList();
                _dbContext.PurchasingMemoDetailTextileUnitReceiptNotes.UpdateRange(unitReceiptNotes);

                _dbContext.SaveChanges();
                deletedId = model.Id;
            }

            return deletedId;
        }

        public ReadResponse<IndexDto> Read(string keyword, int page = 1, int size = 25)
        {
            var query = _dbContext.PurchasingMemoDetailTextiles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(entity => entity.DocumentNo.Contains(keyword) || entity.DivisionName.Contains(keyword) || entity.DivisionCode.Contains(keyword) || entity.CurrencyCode.Contains(keyword));
            }

            var count = query.Select(entity => entity.Id).Count();
            var data = query.Skip((page - 1) * size).Take(size).Select(entity => new IndexDto()).ToList();
            return new ReadResponse<IndexDto>(data, count, new Dictionary<string, string>(), new List<string>());
        }

        public PurchasingMemoDetailTextileDto Read(int id)
        {
            var model = _dbContext.PurchasingMemoDetailTextiles.FirstOrDefault(entity => entity.Id == id);


            if (model != null)
            {
                var result = new PurchasingMemoDetailTextileDto(model.Date, new DivisionDto(model.DivisionId, model.DivisionCode, model.DivisionName), new CurrencyDto(model.CurrencyId, model.CurrencyCode, model.CurrencyRate), model.SupplierIsImport, model.Type, new List<FormItemDto>(), new List<FormDetailDto>(), model.Remark, model.Id);

                if (model.Type == PurchasingMemoType.Disposition)
                {
                    var items = _dbContext.PurchasingMemoDetailTextileItems.Where(entity => entity.PurchasingMemoDetailTextileId == model.Id).ToList();
                    var itemIds = items.Select(item => item.Id).ToList();
                    var details = _dbContext.PurchasingMemoDetailTextileDetails.Where(entity => entity.PurchasingMemoDetailTextileId == model.Id && itemIds.Contains(entity.PurchasingMemoDetailTextileItemId)).ToList();
                    var detailIds = details.Select(detail => detail.Id).ToList();
                    var unitReceiptNotes = _dbContext.PurchasingMemoDetailTextileUnitReceiptNotes.Where(entity => entity.PurchasingMemoDetailTextileId == model.Id && itemIds.Contains(entity.PurchasingMemoDetailTextileItemId) && detailIds.Contains(entity.PurchasingMemoDetailTextileDetailId)).ToList();

                    foreach (var item in items)
                    {
                        var itemDetails = details.Where(detail => detail.PurchasingMemoDetailTextileItemId == item.Id).ToList();
                        var disposition = new DispositionDto(item.DispositionId, item.DispositionNo, item.DispositionDate, new List<FormDetailDto>());

                        foreach (var itemDetail in itemDetails)
                        {
                            var detailUnitReceiptNotes = unitReceiptNotes.Where(unitReceiptNote => unitReceiptNote.PurchasingMemoDetailTextileDetailId == itemDetail.Id).Select(unitReceiptNote => new UnitReceiptNoteDto(unitReceiptNote.UnitReceiptNoteId, unitReceiptNote.UnitReceiptNoteNo, unitReceiptNote.UnitReceiptNoteDate)).ToList();
                            var expenditure = new ExpenditureDto(itemDetail.ExpenditureId, itemDetail.ExpenditureNo, itemDetail.ExpenditureDate);
                            var supplier = new SupplierDto(itemDetail.SupplierId, itemDetail.SupplierCode, itemDetail.SupplierName);
                            var unitPaymentOrder = new UnitPaymentOrderDto(itemDetail.UnitPaymentOrderId, itemDetail.UnitPaymentOrderNo, itemDetail.UnitPaymentOrderDate);
                            disposition.Details.Add(new FormDetailDto(expenditure, supplier, itemDetail.Remark, unitPaymentOrder, detailUnitReceiptNotes, itemDetail.PurchaseAmountCurrency, itemDetail.PurchaseAmount, itemDetail.PaymentAmountCurrency, itemDetail.PaymentAmount));
                        }

                        result.Items.Add(new FormItemDto(disposition));
                    }
                }
                else
                {
                    var details = _dbContext.PurchasingMemoDetailTextileDetails.Where(entity => entity.PurchasingMemoDetailTextileId == model.Id).ToList();
                    var detailIds = details.Select(detail => detail.Id).ToList();
                    var unitReceiptNotes = _dbContext.PurchasingMemoDetailTextileUnitReceiptNotes.Where(entity => entity.PurchasingMemoDetailTextileId == model.Id && detailIds.Contains(entity.PurchasingMemoDetailTextileDetailId)).ToList();


                    foreach (var itemDetail in details)
                    {
                        var detailUnitReceiptNotes = unitReceiptNotes.Where(unitReceiptNote => unitReceiptNote.PurchasingMemoDetailTextileDetailId == itemDetail.Id).Select(unitReceiptNote => new UnitReceiptNoteDto(unitReceiptNote.UnitReceiptNoteId, unitReceiptNote.UnitReceiptNoteNo, unitReceiptNote.UnitReceiptNoteDate)).ToList();
                        var expenditure = new ExpenditureDto(itemDetail.ExpenditureId, itemDetail.ExpenditureNo, itemDetail.ExpenditureDate);
                        var supplier = new SupplierDto(itemDetail.SupplierId, itemDetail.SupplierCode, itemDetail.SupplierName);
                        var unitPaymentOrder = new UnitPaymentOrderDto(itemDetail.UnitPaymentOrderId, itemDetail.UnitPaymentOrderNo, itemDetail.UnitPaymentOrderDate);
                        result.Details.Add(new FormDetailDto(expenditure, supplier, itemDetail.Remark, unitPaymentOrder, detailUnitReceiptNotes, itemDetail.PurchaseAmountCurrency, itemDetail.PurchaseAmount, itemDetail.PaymentAmountCurrency, itemDetail.PaymentAmount));
                    }

                }

                return result;
            }
            else
                return null;

        }

        public int Update(int id, FormDto form)
        {
            var updatedId = 0;
            var model = _dbContext.PurchasingMemoDetailTextiles.FirstOrDefault(entity => entity.Id == id);

            if (model != null)
            {
                EntityExtension.FlagForUpdate(model, _identityService.Username, UserAgent);
                _dbContext.PurchasingMemoDetailTextiles.Update(model);

                var items = _dbContext.PurchasingMemoDetailTextileItems.Where(entity => entity.PurchasingMemoDetailTextileId == model.Id).ToList();
                var details = _dbContext.PurchasingMemoDetailTextileDetails.Where(entity => entity.PurchasingMemoDetailTextileId == model.Id).ToList();
                var unitReceiptNotes = _dbContext.PurchasingMemoDetailTextileUnitReceiptNotes.Where(entity => entity.PurchasingMemoDetailTextileId == model.Id).ToList();

                items = items.Select(element =>
                {
                    EntityExtension.FlagForDelete(element, _identityService.Username, UserAgent);
                    return element;
                }).ToList();
                _dbContext.PurchasingMemoDetailTextileItems.UpdateRange(items);

                details = details.Select(element =>
                {
                    EntityExtension.FlagForDelete(element, _identityService.Username, UserAgent);
                    return element;
                }).ToList();
                _dbContext.PurchasingMemoDetailTextileDetails.UpdateRange(details);

                unitReceiptNotes = unitReceiptNotes.Select(element =>
                {
                    EntityExtension.FlagForDelete(element, _identityService.Username, UserAgent);
                    return element;
                }).ToList();
                _dbContext.PurchasingMemoDetailTextileUnitReceiptNotes.UpdateRange(unitReceiptNotes);

                _dbContext.SaveChanges();

                if (form.Type == PurchasingMemoType.Disposition)
                {
                    foreach (var item in form.Items)
                    {
                        var itemModel = new PurchasingMemoDetailTextileItemModel(item.Disposition.Id, item.Disposition.DocumentNo, item.Disposition.Date, model.Id);
                        EntityExtension.FlagForCreate(itemModel, _identityService.Username, UserAgent);
                        _dbContext.PurchasingMemoDetailTextileItems.Add(itemModel);
                        _dbContext.SaveChanges();

                        foreach (var detail in item.Disposition.Details)
                        {
                            var detailModel = new PurchasingMemoDetailTextileDetailModel(model.Id, itemModel.Id, detail.Expenditure.Id, detail.Expenditure.DocumentNo, detail.Expenditure.Date, detail.Supplier.Id, detail.Supplier.Code, detail.Supplier.Name, detail.Remark, detail.UnitPaymentOder.Id, detail.UnitPaymentOder.UnitPaymentOrderNo, detail.UnitPaymentOder.UnitPaymentOrderDate, detail.PaymentAmountCurrency, detail.PurchaseAmount, detail.PaymentAmount, detail.PaymentAmountCurrency);
                            EntityExtension.FlagForCreate(detailModel, _identityService.Username, UserAgent);
                            _dbContext.PurchasingMemoDetailTextileDetails.Add(detailModel);
                            _dbContext.SaveChanges();

                            foreach (var unitReceiptNote in detail.UnitReceiptNotes)
                            {
                                var unitReceiptNoteModel = new PurchasingMemoDetailTextileUnitReceiptNoteModel(model.Id, itemModel.Id, detailModel.Id, unitReceiptNote.Id, unitReceiptNote.UnitReceiptNoteNo, unitReceiptNote.UnitReceiptNoteDate);
                                EntityExtension.FlagForCreate(unitReceiptNoteModel, _identityService.Username, UserAgent);
                                _dbContext.PurchasingMemoDetailTextileUnitReceiptNotes.Add(unitReceiptNoteModel);
                                _dbContext.SaveChanges();
                            }
                        }
                    }
                }
                else
                {
                    foreach (var detail in form.Details)
                    {
                        var detailModel = new PurchasingMemoDetailTextileDetailModel(model.Id, 0, detail.Expenditure.Id, detail.Expenditure.DocumentNo, detail.Expenditure.Date, detail.Supplier.Id, detail.Supplier.Code, detail.Supplier.Name, detail.Remark, detail.UnitPaymentOder.Id, detail.UnitPaymentOder.UnitPaymentOrderNo, detail.UnitPaymentOder.UnitPaymentOrderDate, detail.PaymentAmountCurrency, detail.PurchaseAmount, detail.PaymentAmount, detail.PaymentAmountCurrency);
                        EntityExtension.FlagForCreate(detailModel, _identityService.Username, UserAgent);
                        _dbContext.PurchasingMemoDetailTextileDetails.Add(detailModel);
                        _dbContext.SaveChanges();

                        foreach (var unitReceiptNote in detail.UnitReceiptNotes)
                        {
                            var unitReceiptNoteModel = new PurchasingMemoDetailTextileUnitReceiptNoteModel(model.Id, 0, detailModel.Id, unitReceiptNote.Id, unitReceiptNote.UnitReceiptNoteNo, unitReceiptNote.UnitReceiptNoteDate);
                            EntityExtension.FlagForCreate(unitReceiptNoteModel, _identityService.Username, UserAgent);
                            _dbContext.PurchasingMemoDetailTextileUnitReceiptNotes.Add(unitReceiptNoteModel);
                            _dbContext.SaveChanges();
                        }
                    }
                }

                updatedId = model.Id;
            }

            return updatedId;
        }

        public List<FormItemDto> ReadDispositions(string keyword, int divisionId, bool supplierIsImport, string currencyCode)
        {
            var paymentIds = _dbContext.PaymentDispositionNotes.Where(entity => entity.CurrencyCode == currencyCode && entity.SupplierImport == supplierIsImport).Select(entity => entity.Id).Distinct().ToList();
            var query = _dbContext.PaymentDispositionNoteItems.Where(entity => paymentIds.Contains(entity.PaymentDispositionNoteId) && entity.DivisionId == divisionId).Select(entity => new { entity.DispositionId, entity.DispositionNo, entity.DispositionDate }).Distinct().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(entity => entity.DispositionNo.Contains(keyword));
            }


            var queryResult = query.Take(10).ToList();
            var dispositionIds = queryResult.Select(element => element.DispositionId).ToList();
            var paymentDispositionItems = _dbContext.PaymentDispositionNoteItems.Where(entity => dispositionIds.Contains(entity.DispositionId)).ToList();
            var paymentDispositionItemIds = paymentDispositionItems.Select(entity => entity.Id).ToList();
            var paymentDispositionIds = paymentDispositionItems.Select(entity => entity.PaymentDispositionNoteId).Distinct().ToList();
            var paymentDispositions = _dbContext.PaymentDispositionNotes.Where(entity => paymentDispositionIds.Contains(entity.Id)).ToList();
            var paymentDispositionDetails = _dbContext.PaymentDispositionNoteDetails.Where(entity => paymentDispositionItemIds.Contains(entity.PaymentDispositionNoteItemId)).ToList();
            var purchasingDispositionExpeditionIds = paymentDispositionItems.Select(element => element.PurchasingDispositionExpeditionId).ToList();
            var purchasingDispositionExpeditions = _dbContext.PurchasingDispositionExpeditions.Where(entity => purchasingDispositionExpeditionIds.Contains(entity.Id)).ToList();
            var purchasingDispositionExpeditionItems = _dbContext.PurchasingDispositionExpeditionItems.Where(entity => purchasingDispositionExpeditionIds.Contains(entity.PurchasingDispositionExpeditionId)).ToList();

            var result = new List<FormItemDto>();
            foreach (var item in queryResult)
            {
                var disposition = new DispositionDto(item.DispositionId, item.DispositionNo, item.DispositionDate, new List<FormDetailDto>());

                var itemPaymentDispositionNoteIds = paymentDispositionItems.Where(element => element.DispositionId == item.DispositionId).Select(element => element.PaymentDispositionNoteId).ToList();
                var itemPaymentDispositionNotes = paymentDispositions.Where(element => itemPaymentDispositionNoteIds.Contains(element.Id)).ToList();

                foreach (var itemPaymentDispositionNote in itemPaymentDispositionNotes)
                {
                    var expenditure = new ExpenditureDto(itemPaymentDispositionNote.Id, itemPaymentDispositionNote.PaymentDispositionNo, itemPaymentDispositionNote.PaymentDate);
                    var supplier = new SupplierDto(itemPaymentDispositionNote.SupplierId, itemPaymentDispositionNote.SupplierCode, itemPaymentDispositionNote.SupplierName);
                    var itemPaymentDispositionNoteItems = paymentDispositionItems.Where(element => element.PaymentDispositionNoteId == itemPaymentDispositionNote.Id).ToList();
                    

                    foreach (var itemPaymentDispositionNoteItem in itemPaymentDispositionNoteItems)
                    {
                        var division = new DivisionDto(itemPaymentDispositionNoteItem.DivisionId, itemPaymentDispositionNoteItem.DivisionCode, itemPaymentDispositionNoteItem.DivisionName);

                        disposition.Details.Add(new FormDetailDto(expenditure, supplier, "", new UnitPaymentOrderDto(0, "", DateTime.MinValue), new List<UnitReceiptNoteDto>(), 0, 0, 0, 0));
                    }
                }

                result.Add(new FormItemDto(disposition));

            }

            return result;
        }
    }
}
