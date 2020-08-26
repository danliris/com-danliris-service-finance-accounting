using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRequestDocument;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.VBRealizationDocumentNonPO;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocument
{
    public class VBRealizationDocumentModel : StandardEntity
    {
        public VBRealizationDocumentModel()
        {

        }

        public VBRealizationDocumentModel(VBRealizationDocumentNonPOViewModel viewModel)
        {
            Type = VBType.NonPO;
            Index = viewModel.Index;
            DocumentNo = viewModel.DocumentNo;
            Date = viewModel.Date;
            VBNonPoType = viewModel.VBNonPOType;

            if(viewModel.VBDocument != null)
            {
                VBRequestDocumentId = viewModel.VBDocument.Id;
                VBRequestDocumentNo = viewModel.VBDocument.DocumentNo;
                VBRequestDocumentDate = viewModel.VBDocument.Date;
                VBRequestDocumentRealizationEstimationDate = viewModel.VBDocument.RealizationEstimationDate;
                VBRequestDocumentCreatedBy = viewModel.VBDocument.CreatedBy;
                VBRequestDocumentAmount = viewModel.VBDocument.Amount.GetValueOrDefault();
            }

            if(viewModel.Unit != null)
            {
                SuppliantUnitCode = viewModel.Unit.Code;
                SuppliantUnitId = viewModel.Unit.Id;
                SuppliantUnitName = viewModel.Unit.Name;

                if(viewModel.Unit.Division != null)
                {
                    SuppliantDivisionCode = viewModel.Unit.Division.Code;
                    SuppliantDivisionId = viewModel.Unit.Division.Id;
                    SuppliantDivisionName = viewModel.Unit.Division.Name;
                }
            }
            
            if(viewModel.Currency != null)
            {
                CurrencyCode = viewModel.Currency.Code;
                CurrencyDescription = viewModel.Currency.Description;
                CurrencyId = viewModel.Currency.Id;
                CurrencyRate = viewModel.Currency.Rate;
                CurrencySymbol = viewModel.Currency.Symbol;
            }
        }

        public VBType Type { get; private set; }
        public int Index { get; private set; }
        [MaxLength(64)]
        public string DocumentNo { get; private set; }
        public DateTimeOffset Date { get; private set; }
        [MaxLength(32)]
        public string VBNonPoType { get; private set; }
        public int VBRequestDocumentId { get; private set; }
        [MaxLength(64)]
        public string VBRequestDocumentNo { get; private set; }
        public DateTimeOffset? VBRequestDocumentDate { get; private set; }
        public DateTimeOffset? VBRequestDocumentRealizationEstimationDate { get; private set; }
        [MaxLength(256)]
        public string VBRequestDocumentCreatedBy { get; private set; }
        public int SuppliantUnitId { get; private set; }
        [MaxLength(64)]
        public string SuppliantUnitCode { get; private set; }
        [MaxLength(64)]
        public string SuppliantUnitName { get; private set; }
        public int SuppliantDivisionId { get; private set; }
        [MaxLength(64)]
        public string SuppliantDivisionCode { get; private set; }
        [MaxLength(64)]
        public string SuppliantDivisionName { get; private set; }
        public int CurrencyId { get; private set; }
        [MaxLength(64)]
        public string CurrencyCode { get; private set; }
        [MaxLength(64)]
        public string CurrencySymbol { get; private set; }
        public double CurrencyRate { get; private set; }
        [MaxLength(256)]
        public string CurrencyDescription { get; private set; }
        public decimal VBRequestDocumentAmount { get; private set; }
    }
}
