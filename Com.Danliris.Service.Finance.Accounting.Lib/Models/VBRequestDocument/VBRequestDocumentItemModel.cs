using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRequestDocument
{
    public class VBRequestDocumentItemModel : StandardEntity
    {
        public VBRequestDocumentItemModel()
        {

        }

        public VBRequestDocumentItemModel(
            int vbRequestDocumentId,
            int unitId,
            string unitName,
            string unitCode,
            int divisionId,
            string divisionName,
            string divisionCode,
            int epoId,
            string epoNo,
            bool useIncomeTax,
            int incomeTaxId,
            string incomeTaxName,
            double incomeTaxRate,
            string incomeTaxBy,
            double amountByUnit,
            bool isSelected,
            int vbDocumentLayoutOrder
            )
        {
            VBRequestDocumentId = vbRequestDocumentId;
            UnitId = unitId;
            UnitName = unitName;
            UnitCode = unitCode;
            DivisionId = divisionId;
            DivisionName = divisionName;
            DivisionCode = divisionCode;
            EPOId = epoId;
            EPONo = epoNo;
            UseIncomeTax = useIncomeTax;
            IncomeTaxId = incomeTaxId;
            IncomeTaxName = incomeTaxName;
            IncomeTaxRate = incomeTaxRate;
            IncomeTaxBy = incomeTaxBy;
            AmountByUnit = amountByUnit;
            IsSelected = isSelected;

            VBDocumentLayoutOrder = vbDocumentLayoutOrder;
        }

        public int VBRequestDocumentId { get; private set; }
        public int UnitId { get; private set; }
        [MaxLength(256)]
        public string UnitName { get; private set; }
        [MaxLength(64)]
        public string UnitCode { get; private set; }
        public int DivisionId { get; private set; }
        [MaxLength(256)]
        public string DivisionName { get; private set; }
        [MaxLength(64)]
        public string DivisionCode { get; private set; }
        public int EPOId { get; private set; }
        [MaxLength(64)]
        public string EPONo { get; private set; }
        public bool UseIncomeTax { get; private set; }
        public int IncomeTaxId { get; private set; }
        [MaxLength(64)]
        public string IncomeTaxName { get; private set; }
        public double IncomeTaxRate { get; private set; }
        [MaxLength(64)]
        public string IncomeTaxBy { get; private set; }
        public double AmountByUnit { get; private set; }
        public bool IsSelected { get; private set; }

        public int VBDocumentLayoutOrder { get; private set; }
    }
}
