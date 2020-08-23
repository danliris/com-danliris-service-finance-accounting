using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.VBRealizationDocumentExpedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/vb-realization-expeditions")]
    [Authorize]
    public class VBRealizationExpeditionController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IVBRealizationDocumentExpeditionService _service;
        private readonly IValidateService _validateService;
        private const string ApiVersion = "1.0";

        public VBRealizationExpeditionController(IServiceProvider serviceProvider)
        {
            _identityService = serviceProvider.GetService<IIdentityService>();
            _service = serviceProvider.GetService<IVBRealizationDocumentExpeditionService>();
            _validateService = serviceProvider.GetService<IValidateService>();
        }

        private void VerifyUser()
        {
            _identityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            _identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            _identityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }

        [HttpGet("vb-realization-to-verification")]
        public IActionResult GetVbRealizationToVerification([FromQuery] int vbId, [FromQuery] int vbRealizationId, [FromQuery] DateTimeOffset? realizationDate, [FromQuery] string vbRealizationRequestPerson, [FromQuery] int unitId)
        {
            try
            {
                VerifyUser();
                var data = _service.ReadRealizationToVerification(vbId, vbRealizationId, realizationDate, vbRealizationRequestPerson, unitId);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Data,
                    info = new
                    {
                        data.Count
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

        [HttpGet]
        public IActionResult Get([FromQuery] int vbId, [FromQuery] int vbRealizationId, [FromQuery] DateTimeOffset? realizationDate, [FromQuery] string vbRealizationRequestPerson, [FromQuery] int unitId, [FromQuery] int position = 0, [FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string order = "{}", [FromQuery] string keyword = "")
        {
            try
            {
                VerifyUser();
                var data = _service.Read(page, size, order, keyword, position, vbId, vbRealizationId, realizationDate, vbRealizationRequestPerson, unitId);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Data,
                    info = new
                    {
                        data.Count
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

        [HttpPut("vb-to-verification")]
        public async Task<IActionResult> Post([FromBody] VBRealizationIdListDto viewModel)
        {
            try
            {
                VerifyUser();
                _validateService.Validate(viewModel);

                await _service.SubmitToVerification(viewModel.VBRealizationIds);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
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

        [HttpPut("accept-for-verification")]
        public async Task<IActionResult> AcceptForVerification([FromBody] VBRealizationIdListDto viewModel)
        {
            try
            {
                VerifyUser();
                _validateService.Validate(viewModel);

                await _service.VerificationDocumentReceipt(viewModel.VBRealizationIds);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
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

        [HttpPut("accept-for-cashier")]
        public async Task<IActionResult> AcceptForCashier([FromBody] VBRealizationIdListDto viewModel)
        {
            try
            {
                VerifyUser();
                _validateService.Validate(viewModel);

                await _service.CashierReceipt(viewModel.VBRealizationIds);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
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

        [HttpPut("vb-verified/{id}")]
        public async Task<IActionResult> Verify([FromRoute] int id)
        {
            try
            {
                VerifyUser();

                await _service.VerifiedToCashier(id);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
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

        [HttpPut("vb-reject/{id}")]
        public async Task<IActionResult> reject([FromRoute] int id, [FromBody] VBRealizationExpeditionRejectDto viewModel)
        {
            try
            {
                VerifyUser();
                _validateService.Validate(viewModel);

                await _service.Reject(id, viewModel.Reason);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
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

        [HttpGet("reports")]
        public async Task<IActionResult> GetReport([FromQuery] int vbId, [FromQuery] int vbRealizationId, [FromQuery] string vbRequestName, [FromQuery] int unitId, [FromQuery] int divisionId, [FromQuery] DateTimeOffset? dateStart, [FromQuery] DateTimeOffset? dateEnd, [FromQuery] int page = 1, [FromQuery] int size = 25)
        {
            try
            {
                VerifyUser();

                if (dateEnd == null)
                    dateEnd = DateTimeOffset.Now;

                if (dateStart == null)
                    dateStart = dateEnd.GetValueOrDefault().AddMonths(-1);

                var reportResult = await _service.GetReports(vbId, vbRealizationId, vbRequestName, unitId, divisionId, dateStart.GetValueOrDefault(), dateEnd.GetValueOrDefault(), page, size);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = reportResult.Data,
                    info = new
                    {
                        total = reportResult.Total,
                        page,
                        size
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

        [HttpGet("reports/xls")]
        public async Task<IActionResult> GetReportXls([FromQuery] int vbId, [FromQuery] int vbRealizationId, [FromQuery] string vbRequestName, [FromQuery] int unitId, [FromQuery] int divisionId, [FromQuery] DateTimeOffset? dateStart, [FromQuery] DateTimeOffset? dateEnd)
        {
            try
            {
                VerifyUser();

                if (dateEnd == null)
                    dateEnd = DateTimeOffset.Now;

                if (dateStart == null)
                    dateStart = dateEnd.GetValueOrDefault().AddMonths(-1);

                var reportResult = await _service.GetReports(vbId, vbRealizationId, vbRequestName, unitId, divisionId, dateStart.GetValueOrDefault(), dateEnd.GetValueOrDefault(), 1, int.MaxValue);
                var stream = GenerateExcel(reportResult.Data);

                var xls = stream.ToArray();

                var file = File(xls, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Laporan Ekspedisi");

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

        private MemoryStream GenerateExcel(IList<VBRealizationDocumentExpeditionModel> data)
        {
            var timezoneoffset = _identityService.TimezoneOffset;
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "Tanggal Unit Kirim", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Mata Uang VB", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nominal VB", DataType = typeof(decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Mata Uang Realisasi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nominal Realisasi", DataType = typeof(decimal) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tanggal Terima Realisasi", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Nama Verifikator", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tanggal Kirim Kasir/Retur", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Tanggal Terima Kasir", DataType = typeof(string) });

            if (data.Count == 0)
            {
                dt.Rows.Add("", "", 0, "", 0, "", "", "", "", "");
            }
            else
            {
                foreach (var datum in data)
                {
                    var verificationReceiptDate = datum.VerificationReceiptDate.HasValue ? datum.VerificationReceiptDate.GetValueOrDefault().AddHours(timezoneoffset).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "-";
                    var verifiedToCashierDate = datum.VerifiedToCashierDate.HasValue ? datum.VerifiedToCashierDate.GetValueOrDefault().AddHours(timezoneoffset).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "-";
                    var cashierReceiptDate = datum.CashierReceiptDate.HasValue ? datum.CashierReceiptDate.GetValueOrDefault().AddHours(timezoneoffset).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "-";
                    var vbRealizationDate = datum.VBRealizationDate.AddHours(timezoneoffset).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dt.Rows.Add(
                        $"{verificationReceiptDate}",
                        $"{datum.CurrencyCode}",
                        datum.VBAmount,
                        $"{datum.CurrencyCode}",
                        datum.VBRealizationAmount,
                        $"{vbRealizationDate}",
                        $"{datum.VerificationReceiptBy}",
                        $"{verifiedToCashierDate}",
                        $"{datum.NotVerifiedReason}",
                        $"{cashierReceiptDate}"
                        );
                }
            }

            return Lib.Helpers.Excel.CreateExcelNoFilters(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(dt, "Reports") }, true);
        }
    }
}
