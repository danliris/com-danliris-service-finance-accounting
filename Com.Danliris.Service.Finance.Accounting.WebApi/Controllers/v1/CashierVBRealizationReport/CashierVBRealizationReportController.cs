using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CashierVBRealizationReport;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.CashierVBRealizationReport
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/cashier-vb-realization-report")]
    [Authorize]

    public class CashierVBRealizationReportController : Controller
    {
        private IIdentityService IdentityService;
        private readonly IValidateService ValidateService;
        private readonly ICashierVBRealizationReportService Service;
        private readonly string ApiVersion;
        private readonly IMapper Mapper;

        public CashierVBRealizationReportController(IIdentityService identityService, IValidateService validateService, IMapper mapper, ICashierVBRealizationReportService service)
        {
            IdentityService = identityService;
            ValidateService = validateService;
            Service = service;
            Mapper = mapper;
            ApiVersion = "1.0.0";
        }

        [HttpGet("reports")]
        public IActionResult GetReportAll(string divisionName, string isInklaring,  DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, DateTimeOffset? realizeDateFrom, DateTimeOffset? realizeDateTo)
        {
            //int offset = Convert.ToInt32(timezone);

            try
            {
                var data = Service.GetReport(divisionName, isInklaring, approvalDateFrom, approvalDateTo, realizeDateFrom, realizeDateTo, IdentityService.TimezoneOffset);

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
        public IActionResult GetXlsAll(string divisionName, string isInklaring, DateTimeOffset? approvalDateFrom, DateTimeOffset? approvalDateTo, DateTimeOffset? realizeDateFrom, DateTimeOffset? realizeDateTo)
        {

            try
            {               
                byte[] xlsInBytes;
                //int offset = Convert.ToInt32(timezone);

                var xls = Service.GenerateExcel(divisionName, isInklaring, approvalDateFrom, approvalDateTo, realizeDateFrom, realizeDateTo, IdentityService.TimezoneOffset);

                string filename = "Laporan Kasir Realisasi VB.xlsx";

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
