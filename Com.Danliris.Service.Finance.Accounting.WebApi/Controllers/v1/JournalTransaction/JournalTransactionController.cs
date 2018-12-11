using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Com.Danliris.Service.Production.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.JournalTransaction
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/journal-transactions")]
    [Authorize]
    public class JournalTransactionController : BaseController<JournalTransactionModel, JournalTransactionViewModel, IJournalTransactionService>
    {
        public JournalTransactionController(IIdentityService identityService, IValidateService validateService, IJournalTransactionService service, IMapper mapper) : base(identityService, validateService, service, mapper, "1.0.0")
        {
        }

        [HttpGet("report")]
        public IActionResult GetReport([FromQuery] int month, [FromQuery] int year, int page = 1, int size = 25)
        {
            try
            {
                VerifyUser();
                int offSet = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                //int offSet = 7;
                var data = Service.GetReport(page, size, month, year, offSet);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1.Data,
                    info = new
                    {
                        data.Item1.Count,
                        data.Item1.Order,
                        data.Item1.Selected,
                        TotalDebit = data.Item2,
                        TotalCredit = data.Item3
                    },
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                   new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                   .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("report/downloads/xls")]
        public  IActionResult GetXls([FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                VerifyUser();
                byte[] xlsInBytes;
                int offSet = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                var xls = Service.GenerateExcel(month, year, offSet);

                string fileName = string.Format("Jurnal Transaksi Periode {0} {1}", CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month), year);
                xlsInBytes = xls.ToArray();

                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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
