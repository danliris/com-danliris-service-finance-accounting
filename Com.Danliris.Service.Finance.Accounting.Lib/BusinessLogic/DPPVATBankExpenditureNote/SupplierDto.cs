namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.DPPVATBankExpenditureNote
{
    public class SupplierDto
    {
        public SupplierDto()
        {

        }

        public SupplierDto(int supplierId, string supplierName, bool isImportSupplier)
        {
            Id = supplierId;
            Name = supplierName;
            IsImport = isImportSupplier;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsImport { get; set; }
    }
}