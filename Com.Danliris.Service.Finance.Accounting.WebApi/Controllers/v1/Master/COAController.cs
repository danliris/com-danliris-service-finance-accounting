using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.MasterCOA;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using Com.Danliris.Service.Production.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.Master
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/master/chart-of-accounts")]
    [Authorize]
    public class COAController : BaseController<COAModel, COAViewModel, ICOAService>
    {
        public COAController(IIdentityService identityService, IValidateService validateService, ICOAService service, IMapper mapper) : base(identityService, validateService, service, mapper, "1.0.0")
        {

        }

        [HttpPost("upload")]
        public async Task<IActionResult> PostCSVFileAsync(IFormFile file)
        {
            try
            {
                var viewModels = CSV<COAViewModel>.ReadFile(file);
                var data = Mapper.Map<List<COAModel>>(viewModels);
                await Service.UploadData(data.ToList());

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return Created(String.Concat(Request.Path, "/", 0), Result);
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
