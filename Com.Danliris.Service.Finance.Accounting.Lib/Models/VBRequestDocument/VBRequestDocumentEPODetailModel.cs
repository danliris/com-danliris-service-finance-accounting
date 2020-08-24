using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRequestDocument
{
    public class VBRequestDocumentEPODetailModel : StandardEntity
    {
        public VBRequestDocumentEPODetailModel()
        {

        }

        public VBRequestDocumentEPODetailModel(
            int vbRequestDocumentItemId,
            int epoId,
            int productId,
            string productCode,
            string productName,
            double defaultQuantity,
            int defaultUOMId,
            string defaultUOMUnit,
            double dealQuantity,
            int dealUOMId,
            string dealUOMUnit,
            double conversion,
            double price,
            bool useVat,
            string remark
            )
        {
            VBRequestDocumentItemId = vbRequestDocumentItemId;
            EPOId = epoId;
            ProductId = productId;
            ProductCode = productCode;
            ProductName = productName;
            DefaultQuantity = defaultQuantity;
            DefaultUOMId = defaultUOMId;
            DefaultUOMUnit = defaultUOMUnit;
            DealQuantity = dealQuantity;
            DealUOMId = dealUOMId;
            DealUOMUnit = dealUOMUnit;
            Conversion = conversion;
            Price = price;
            UseVat= useVat;
            Remark = remark;
        }

        public int VBRequestDocumentItemId { get; private set; }
        public int EPOId { get; private set; }
        public int ProductId { get; private set; }
        [MaxLength(64)]
        public string ProductCode { get; private set; }
        [MaxLength(512)]
        public string ProductName { get; private set; }
        public double DefaultQuantity { get; private set; }
        public int DefaultUOMId { get; private set; }
        [MaxLength(64)]
        public string DefaultUOMUnit { get; private set; }
        public double DealQuantity { get; private set; }
        public int DealUOMId { get; private set; }
        [MaxLength(64)]
        public string DealUOMUnit { get; private set; }
        public double Conversion { get; private set; }
        public double Price { get; private set; }
        public bool UseVat { get; private set; }
        public string Remark { get; }
    }
}
