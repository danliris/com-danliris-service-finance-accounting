using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.VBStatusReport;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.HttpClientService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.VBStatusReport;
using Com.Danliris.Service.Finance.Accounting.Test.DataUtils.VBStatusReport;
using Com.Danliris.Service.Finance.Accounting.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.VBStatusReport
{
    public class VBStatusReportServiceTest
    {
        private const string ENTITY = "VBStatusReprt";

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private FinanceDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<FinanceDbContext> optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            FinanceDbContext dbContext = new FinanceDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private VBStatusReportDataUtil _dataUtil(VBStatusReportService service)
        {
            return new VBStatusReportDataUtil(service);
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test", TimezoneOffset = 7 });


            return serviceProvider;
        }


        [Fact]
        public async Task Should_Success_GetReport()
        {
            VBStatusReportService service = new VBStatusReportService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData_Outstanding_ById();

            var Response = service.GetReport(data.UnitId, data.Id, true, data.Date, data.Date, data.Date, data.Date, 7);
            Assert.NotNull(Response);

            Response = service.GetReport(data.UnitId, data.Id, null, null, null, null, null, 7);
            Assert.NotNull(Response);

            Response = service.GetReport(data.UnitId, data.Id, false, data.Date, null, data.Date, null, 7);
            Assert.NotNull(Response);

            Response = service.GetReport(data.UnitId, data.Id, false, null, data.Date, null, data.Date, 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_GenerateExcel_Realisasi()
        {
            VBStatusReportService service = new VBStatusReportService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData_Realisasi_ById();

            var dataRealisation = new RealizationVbModel()
            {
                DateEstimate = DateTimeOffset.Now,
                VBNoRealize = "VBNoRealize",
                VBNo = "VBNo",
                Date = DateTimeOffset.Now,
                Amount = 100,
                isVerified = false,
                LastModifiedUtc = DateTime.Now,
            };
            service._DbContext.RealizationVbs.Add(dataRealisation);
            service._DbContext.SaveChanges();

            var Response = service.GenerateExcel(data.UnitId, data.Id, data.Realization_Status, null, null, null, null, 7);
            Assert.NotNull(Response);

            Response = service.GenerateExcel(data.UnitId, data.Id, false, null, null, null, null, 7);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_GenerateExcel_Outstanding()
        {
            VBStatusReportService service = new VBStatusReportService(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var data = await _dataUtil(service).GetTestData_Outstanding_ById();

            var dataRealisation = new RealizationVbModel()
            {
                DateEstimate = DateTimeOffset.Now,
                VBNoRealize = "VBNoRealize",
                VBNo = "VBNo",
                Date = DateTimeOffset.Now,
                Amount = 100,
                isVerified = false,
                LastModifiedUtc = DateTime.Now,
            };
            service._DbContext.RealizationVbs.Add(dataRealisation);
            service._DbContext.SaveChanges();
            
            var Response = service.GenerateExcel(data.UnitId, data.Id, data.Realization_Status, null, null, null, null, 7);
            Assert.NotNull(Response);

            Response = service.GenerateExcel(data.UnitId, data.Id, true, null, null, null, null, 7);
            Assert.NotNull(Response);
        }
    }
}
