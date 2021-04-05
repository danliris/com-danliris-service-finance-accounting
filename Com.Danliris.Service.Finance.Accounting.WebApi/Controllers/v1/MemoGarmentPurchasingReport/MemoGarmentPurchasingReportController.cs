using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MemoGarmentPurchasing;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MemoGarmentPurchasing;

namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.MemoGarmentPurchasingReport
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-purchasing/memo/report")]
    [Authorize]
    public class MemoGarmentPurchasingReportController : Controller
    {
        private readonly IIdentityService _identityService;
        //private readonly IValidateService _validateService;
        private readonly IMemoGarmentPurchasingService _service;
        private readonly IMapper _mapper;
        private const string ApiVersion = "1.0";

        public MemoGarmentPurchasingReportController(IServiceProvider serviceProvider)
        {
            _identityService = serviceProvider.GetService<IIdentityService>();
            //_validateService = serviceProvider.GetService<IValidateService>();
            _service = serviceProvider.GetService<IMemoGarmentPurchasingService>();
            _mapper = serviceProvider.GetService<IMapper>();
        }

        protected void VerifyUser()
        {
            _identityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            _identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            _identityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string filter = "{}")
        {
            try
            {
                var queryResult = _service.ReadReport(page, size, filter);
                var dataViewModel = _mapper.Map<List<MemoGarmentPurchasingDetailModel>, List<MemoGarmentPurchasingReportViewModel>>(queryResult.Data);

                var result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(_mapper, dataViewModel, page, size, queryResult.Count, queryResult.Data.Count, queryResult.Order, null);
                return Ok(result);
            }
            catch (Exception e)
            {
                var result = new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message).Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, result);
            }
        }
    }
}
