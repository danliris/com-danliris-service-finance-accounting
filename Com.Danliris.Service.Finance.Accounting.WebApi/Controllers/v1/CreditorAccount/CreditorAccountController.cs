using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.CreditorAccount
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/creditor-account")]
    public class CreditorAccountController : Controller
    {
        private IIdentityService IdentityService;
        private readonly IValidateService ValidateService;
        private readonly ICreditorAccountService Service;
        private readonly string ApiVersion;

        public CreditorAccountController(IIdentityService identityService, IValidateService validateService, ICreditorAccountService service, string apiVersion)
        {
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
            ApiVersion = apiVersion;
        }

        private void VerifyUser()
        {
            IdentityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            IdentityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
        }

        [HttpPost("unit-receipt-note")]
        public async Task<ActionResult> Post([FromBody] CreditorAccountPostedViewModel viewModel)
        {
            try
            {
                VerifyUser();
                ValidateService.Validate(viewModel);

                CreditorAccountModel model = new CreditorAccountModel()
                {
                    UnitReceiptNotePPN = viewModel.PPN,
                    FinalBalance = viewModel.DPP * viewModel.PPN,
                    InvoiceNo = viewModel.InvoiceNo,
                    SupplierCode = viewModel.SupplierCode,
                    SupplierName = viewModel.SupplierName,
                    UnitReceiptNoteNo = viewModel.Code,
                    UnitReceiptNoteDPP = viewModel.DPP,
                    UnitReceiptNoteDate = viewModel.Date,
                    UnitReceiptMutation = viewModel.DPP * viewModel.PPN
                };

                await Service.CreateAsync(model);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return Created(String.Concat(Request.Path, "/", 0), Result);
            }
            catch (ServiceValidationException e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPut("unit-receipt-note")]
        public async Task<IActionResult> Put([FromBody] CreditorAccountPostedViewModel viewModel)
        {
            try
            {
                VerifyUser();
                ValidateService.Validate(viewModel);


                CreditorAccountModel model = new CreditorAccountModel()
                {
                    UnitReceiptNotePPN = viewModel.PPN,
                    FinalBalance = viewModel.DPP * viewModel.PPN,
                    InvoiceNo = viewModel.InvoiceNo,
                    SupplierCode = viewModel.SupplierCode,
                    SupplierName = viewModel.SupplierName,
                    UnitReceiptNoteNo = viewModel.Code,
                    UnitReceiptNoteDPP = viewModel.DPP,
                    UnitReceiptNoteDate = viewModel.Date,
                    UnitReceiptMutation = viewModel.DPP * viewModel.PPN
                };

                await Service.UpdateFromUnitReceiptNoteAsync(viewModel.SupplierCode, viewModel.Code, viewModel.InvoiceNo, model);

                return NoContent();
            }
            catch (ServiceValidationException e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}
