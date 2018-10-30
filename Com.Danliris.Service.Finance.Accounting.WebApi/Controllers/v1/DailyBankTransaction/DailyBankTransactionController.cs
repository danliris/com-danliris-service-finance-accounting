using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Com.Danliris.Service.Production.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.DailyBankTransaction
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/daily-bank-transactions")]
    [Authorize]
    public class DailyBankTransactionController : BaseController<DailyBankTransactionModel, DailyBankTransactionViewModel, IDailyBankTransactionService>
    {
        public DailyBankTransactionController(IIdentityService identityService, IValidateService validateService, IDailyBankTransactionService service, IMapper mapper) : base(identityService, validateService, service, mapper, "1.0.0")
        {
        }

        [HttpGet("mutation/report")]
        public IActionResult GetReport(string bankId, int month, int year)
        {
            int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
            ReadResponse<DailyBankTransactionModel> Result = Service.GetReport(bankId, month, year, clientTimeZoneOffset);

            return Ok(new
            {
                apiVersion = "1.0.0",
                data = Result.Data,
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }

        [HttpGet("mutation/report/download")]
        public IActionResult GetReportXls(string bankId, int month, int year)
        {
            try
            {
                byte[] xlsInBytes;
                int clientTimeZoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);

                var xls = Service.GenerateExcel(bankId, month, year, clientTimeZoneOffset);

                string filename = String.Format("Mutasi Bank Harian - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;

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
