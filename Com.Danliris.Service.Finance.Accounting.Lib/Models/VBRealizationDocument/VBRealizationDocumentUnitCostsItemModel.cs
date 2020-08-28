using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.VBRealizationDocumentNonPO;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocument
{
    public class VBRealizationDocumentUnitCostsItemModel : StandardEntity
    {
        public VBRealizationDocumentUnitCostsItemModel()
        {

        }

        public VBRealizationDocumentUnitCostsItemModel(int vbRealizationDocumentId, VBRealizationDocumentNonPOUnitCostViewModel viewModel)
        {
            if(viewModel.Unit != null)
            {
                UnitId = viewModel.Unit.Id;
                UnitName = viewModel.Unit.Name;
                UnitCode = viewModel.Unit.Code;

                if(viewModel.Unit.Division != null)
                {
                    DivisionId = viewModel.Unit.Division.Id;
                    DivisionName = viewModel.Unit.Division.Name;
                    DivisionCode = viewModel.Unit.Division.Code;

                }

                VBDocumentLayoutOrder = viewModel.Unit.VBDocumentLayoutOrder;
            }

            Amount = viewModel.Amount;
            IsSelected = viewModel.IsSelected;

            VBRealizationDocumentId = vbRealizationDocumentId;
        }

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
        public decimal Amount { get; private set; }
        public bool IsSelected { get; private set; }
        public int VBDocumentLayoutOrder { get; private set; }

        public int VBRealizationDocumentId { get; private set; }

        public void SetUnit(int newUnitId, string newUnitName, string newUnitCode, string user, string userAgent)
        {
            if (newUnitId != UnitId)
            {
                UnitId = newUnitId;
                this.FlagForUpdate(user, userAgent);
            }

            if (newUnitName != UnitName)
            {
                UnitName = newUnitName;
                this.FlagForUpdate(user, userAgent);
            }

            if (newUnitCode != UnitCode)
            {
                UnitCode = newUnitCode;
                this.FlagForUpdate(user, userAgent);
            }
        }

        public void SetDivision(int newDivisionId, string newDivisionName, string newDivisionCode, string user, string userAgent)
        {
            if (newDivisionId != DivisionId)
            {
                DivisionId = newDivisionId;
                this.FlagForUpdate(user, userAgent);
            }

            if (newDivisionName != DivisionName)
            {
                DivisionName = newDivisionName;
                this.FlagForUpdate(user, userAgent);
            }

            if (newDivisionCode != DivisionCode)
            {
                DivisionCode = newDivisionCode;
                this.FlagForUpdate(user, userAgent);
            }
        }

        public void SetIsSelected(bool newFlagIsSelected, string user, string userAgent)
        {
            if (newFlagIsSelected != IsSelected)
            {
                IsSelected = newFlagIsSelected;
                this.FlagForUpdate(user, userAgent);
            }
        }

        public void SetAmount(decimal newAmount, string user, string userAgent)
        {
            if (newAmount != Amount)
            {
                Amount = newAmount;
                this.FlagForUpdate(user, userAgent);
            }
        }

    }
}
