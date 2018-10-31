using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.Enums;
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
        private readonly IMapper Mapper;

        public CreditorAccountController(IIdentityService identityService, IValidateService validateService, IMapper mapper, ICreditorAccountService service, string apiVersion)
        {
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
            Mapper = mapper;
            ApiVersion = apiVersion;
        }

        private void VerifyUser()
        {
            IdentityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            IdentityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
        }

        [HttpPost("unit-receipt-note")]
        public async Task<ActionResult> UnitReceiptNotePost([FromBody] CreditorAccountUnitReceiptNotePostedViewModel viewModel)
        {
            try
            {
                VerifyUser();
                ValidateService.Validate(viewModel);
                
                await Service.CreateFromUnitReceiptNoteAsync(viewModel);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return Created(string.Concat(Request.Path, "/", 0), Result);
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
        public async Task<IActionResult> UnitReceiptNotePut([FromBody] CreditorAccountUnitReceiptNotePostedViewModel viewModel)
        {
            try
            {
                VerifyUser();
                ValidateService.Validate(viewModel);
                
                await Service.UpdateFromUnitReceiptNoteAsync(viewModel);

                return NoContent();
            }
            catch (NotFoundException)
            {
                Dictionary<string, object> Result =
                       new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                       .Fail();
                return BadRequest(Result);
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

        [HttpGet("unit-receipt-note")]
        public async Task<IActionResult> UnitReceiptNoteGet([FromQuery] string supplierCode, [FromQuery] string code, [FromQuery] string invoiceNo)
        {
            try
            {
                CreditorAccountUnitReceiptNotePostedViewModel vm = await Service.GetByUnitReceiptNote(supplierCode,  code, invoiceNo);

                if (vm == null)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.NOT_FOUND_STATUS_CODE, General.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(Result);
                }
                else
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                        .Ok(Mapper, vm);
                    return Ok(Result);
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpDelete("unit-receipt-note/{Id}")]
        public async Task<IActionResult> UnitReceiptNoteDelete([FromRoute] int id)
        {
            try
            {
                VerifyUser();

                await Service.DeleteFromUnitReceiptNoteAsync(id);

                return NoContent();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPost("bank-expenditure-note")]
        public async Task<ActionResult> BankExpenditureNotePost([FromBody] CreditorAccountBankExpenditureNotePostedViewModel viewModel)
        {
            try
            {
                VerifyUser();
                ValidateService.Validate(viewModel);
                

                await Service.CreateFromBankExpenditureNoteAsync(viewModel);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return Created(string.Concat(Request.Path, "/", 0), Result);
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

        [HttpPut("bank-expenditure-note")]
        public async Task<IActionResult> BankExpenditureNotePut([FromBody] CreditorAccountBankExpenditureNotePostedViewModel viewModel)
        {
            try
            {
                VerifyUser();
                ValidateService.Validate(viewModel);
                
                await Service.UpdateFromBankExpenditureNoteAsync(viewModel);

                return NoContent();
            }
            catch (NotFoundException)
            {
                Dictionary<string, object> Result =
                       new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                       .Fail();
                return BadRequest(Result);
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

        [HttpGet("bank-expenditure-note")]
        public async Task<IActionResult> BankExpenditureNoteGet([FromQuery] string supplierCode, [FromQuery] string code, [FromQuery] string invoiceNo)
        {
            try
            {
                CreditorAccountBankExpenditureNotePostedViewModel vm = await Service.GetByBankExpenditureNote(supplierCode,  code, invoiceNo);

                if (vm == null)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.NOT_FOUND_STATUS_CODE, General.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(Result);
                }
                else
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                        .Ok(Mapper, vm);
                    return Ok(Result);
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpDelete("bank-expenditure-note/{Id}")]
        public async Task<IActionResult> BankExpenditureNoteDelete([FromRoute] int id)
        {
            try
            {
                VerifyUser();

                await Service.DeleteFromBankExpenditureNoteAsync(id);

                return NoContent();
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
