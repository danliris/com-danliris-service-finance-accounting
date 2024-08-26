using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CashierReport;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.CashierReport
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/cashier-report")]
    [Authorize]

    public class CashierReportController : Controller
    {
        private IIdentityService IdentityService;
        private readonly IValidateService ValidateService;
        private readonly ICashierReportService Service;
        private readonly string ApiVersion;
        private readonly IMapper Mapper;

        public CashierReportController(IIdentityService identityService, IValidateService validateService, IMapper mapper, ICashierReportService service)
        {
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
            Mapper = mapper;
            ApiVersion = "1.0.0";
        }

        [HttpGet("reports")]
        public IActionResult GetReportAll(string divisionName, string isInklaring, string account, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo)
        {
            //int offset = Convert.ToInt32(timezone);

            try
            {
                var data = Service.GetReport(divisionName, isInklaring, account, approvalDateFrom, approvalDateTo, IdentityService.TimezoneOffset);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data,
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

        [HttpGet("reports/xls")]
        public IActionResult GetXlsAll(string divisionName, string isInklaring, string account, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo)
        {

            try
            {               
                byte[] xlsInBytes;
                //int offset = Convert.ToInt32(timezone);

                var xls = Service.GenerateExcel(divisionName, isInklaring, account, approvalDateFrom, approvalDateTo, IdentityService.TimezoneOffset);

                string filename = "Laporan Kasir VB.xlsx";

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
