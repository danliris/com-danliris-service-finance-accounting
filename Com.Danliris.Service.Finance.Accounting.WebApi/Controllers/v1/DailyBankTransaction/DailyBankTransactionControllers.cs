using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.DailyBankTransaction
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/daily-bank-transactions")]
    [Authorize]
    public class DailyBankTransactionControllers : Controller
    {
        private readonly string _ApiVersion = "1.0.0";
        private readonly IServiceProvider _ServiceProvider;
        private readonly IDailyBankTransactionService _DailyBankTransactionService;
        private readonly IdentityService _IdentityService;
        private readonly IMapper _Mapper;

        public DailyBankTransactionControllers(IServiceProvider serviceProvider, IDailyBankTransactionService DailyBankTransactionFacade, IMapper mapper)
        {
            _ServiceProvider = serviceProvider;
            _DailyBankTransactionService = DailyBankTransactionFacade;
            _Mapper = mapper;
            _IdentityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            ReadResponse<DailyBankTransactionModel> Response = _DailyBankTransactionService.Read(page, size, order, keyword, filter);

            return Ok(new
            {
                apiVersion = "1.0.0",
                data = Response.Data,
                info = new Dictionary<string, object>
                {
                    { "count", Response.Data.Count },
                    { "total", Response.Count },
                    { "order", Response.Order },
                    { "page", page },
                    { "size", size }
                },
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            try
            {
                var model = await _DailyBankTransactionService.ReadById(Id);
                DailyBankTransactionViewModel viewModel = _Mapper.Map<DailyBankTransactionViewModel>(model);

                if (model == null)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(_ApiVersion, General.NOT_FOUND_STATUS_CODE, General.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(Result);
                }

                return Ok(new
                {
                    apiVersion = _ApiVersion,
                    data = viewModel,
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(_ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DailyBankTransactionViewModel viewModel)
        {
            _IdentityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");
            _IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            IValidateService validateService = (IValidateService)_ServiceProvider.GetService(typeof(IValidateService));

            try
            {
                validateService.Validate(viewModel);

                DailyBankTransactionModel model = _Mapper.Map<DailyBankTransactionModel>(viewModel);

                int result = await _DailyBankTransactionService.Create(model, _IdentityService.Username);

                Dictionary<string, object> Result =
                    new ResultFormatter(_ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return Created(String.Concat(Request.Path, "/", 0), Result);
            }
            catch (ServiceValidationException e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(_ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(_ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("mutation/report")]
        public IActionResult GetReport(string bankId, DateTimeOffset? dateFrom, DateTimeOffset? dateTo)
        {
            int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
            ReadResponse<DailyBankTransactionModel> Result = _DailyBankTransactionService.GetReport(bankId, dateFrom, dateTo, clientTimeZoneOffset);

            return Ok(new
            {
                apiVersion = "1.0.0",
                data = Result.Data,
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }

        [HttpGet("mutation/report/download")]
        public IActionResult GetReportXls(string bankId, DateTimeOffset? dateFrom, DateTimeOffset? dateTo)
        {
            try
            {
                byte[] xlsInBytes;
                int clientTimeZoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);

                var xls = _DailyBankTransactionService.GenerateExcel(bankId, dateFrom, dateTo, clientTimeZoneOffset);

                string filename = String.Format("Mutasi Bank Harian - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(_ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}
