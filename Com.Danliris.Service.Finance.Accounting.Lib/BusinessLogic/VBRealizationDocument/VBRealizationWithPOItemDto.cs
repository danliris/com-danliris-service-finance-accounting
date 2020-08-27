namespace Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocument
{
    public class VBRealizationWithPOItemDto
    {
        public VBRealizationWithPOItemDto()
        {
        }

        public UnitPaymentOrderDto UnitPaymentOrder { get; internal set; }
        public int Id { get; internal set; }
    }
}