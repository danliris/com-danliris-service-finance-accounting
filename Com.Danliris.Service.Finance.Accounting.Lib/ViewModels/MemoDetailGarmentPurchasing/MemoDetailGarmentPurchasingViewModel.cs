using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities.BaseClass;

namespace Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MemoDetailGarmentPurchasing
{
    public class MemoDetailGarmentPurchasingViewModel : IValidatableObject
    {
        public int MemoId { get; set; }
        public bool IsPosted { get; set; }
        public string Remarks { get; set; }
        public ICollection<MemoDetailGarmentPurchasingDetailViewModel> MemoDetailGarmentPurchasingDetail { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MemoId == 0) {
                yield return new ValidationResult("Nomor Memo harus diisi", new List<string> { "MemoId" });
            }

            if (MemoDetailGarmentPurchasingDetail.Count > 0)
            {
                int CountItemsError = 0;
                string ItemsError = "[";

                foreach (var memoDetail in MemoDetailGarmentPurchasingDetail)
                {
                    ItemsError += "{ ";
                    if (memoDetail.GarmentDeliveryOrderId <= 0 || string.IsNullOrWhiteSpace(memoDetail.GarmentDeliveryOrderNo))
                    {
                        CountItemsError++;
                        ItemsError += "'GarmentDeliveryOrderNo': 'No. Surat Jalan harus diisi', ";
                    }

                    if (string.IsNullOrWhiteSpace(memoDetail.RemarksDetail))
                    {
                        CountItemsError++;
                        ItemsError += "'RemarksDetail': 'Keterangan harus diisi', ";
                    }

                    if (memoDetail.PaymentRate <= 0)
                    {
                        CountItemsError++;
                        ItemsError += "'PaymentRate': 'Rate Bayar harus diisi', ";
                    }

                    if (memoDetail.MemoAmount <= 0)
                    {
                        CountItemsError++;
                        ItemsError += "'MemoAmount': 'Jumlah harus diisi', ";
                    }

                    ItemsError += "}, ";
                }

                ItemsError += "]";

                if (CountItemsError > 0)
                    yield return new ValidationResult(ItemsError, new List<string> { "MemoGarmentPurchasingDetails" });
            }
        }

    }
}
