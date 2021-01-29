using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.GarmentPurchasingPphBankExpenditureNoteViewModels
{
    public class FormAdd
    {
        public string INNo { get; set; }
        public string Remark { get; set; }
        public DateTimeOffset INDate { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public int CurrencyRate { get; set; }
        public int SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public List<Item> Items { get; set; }
        public bool IsCreatedVB { get; set; }
        public int Position { get; set; }
        public string UId { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAgent { get; set; }
        public DateTimeOffset LastModifiedUtc { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedAgent { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset DeletedUtc { get; set; }
        public string DeletedBy { get; set; }
        public string DeletedAgent { get; set; }
        public int Id { get; set; }
    }

    public class Item
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTimeOffset InvoiceDate { get; set; }
        public int TotalAmount { get; set; }
        public List<Detail> Details { get; set; }
        public int GarmentINId { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAgent { get; set; }
        public DateTimeOffset LastModifiedUtc { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedAgent { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset DeletedUtc { get; set; }
        public string DeletedBy { get; set; }
        public string DeletedAgent { get; set; }
        public int Id { get; set; }
    }

    public class Detail
    {
        public int DOId { get; set; }
        public string DONo { get; set; }
        public int EPOId { get; set; }
        public string EPONo { get; set; }
        public string POSerialNumber { get; set; }
        public string RONo { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentType { get; set; }
        public int PaymentDueDays { get; set; }
        public DateTimeOffset PaymentDueDate { get; set; }
        public DateTimeOffset DODate { get; set; }
        public int InvoiceDetailId { get; set; }
        public string ProductCode { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public int UOMId { get; set; }
        public string UOMUnit { get; set; }
        public int PricePerDealUnit { get; set; }
        public int PriceTotal { get; set; }
        public int GarmentItemINId { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAgent { get; set; }
        public DateTimeOffset LastModifiedUtc { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedAgent { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset DeletedUtc { get; set; }
        public string DeletedBy { get; set; }
        public string DeletedAgent { get; set; }
        public int Id { get; set; }
    }
}
