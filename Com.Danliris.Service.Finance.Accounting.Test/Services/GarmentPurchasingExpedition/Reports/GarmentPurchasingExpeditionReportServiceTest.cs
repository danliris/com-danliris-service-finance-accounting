using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.GarmentPurchasingExpedition.Reports;
using Com.Danliris.Service.Finance.Accounting.Lib.Enums.Expedition;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Services.GarmentPurchasingExpedition.Reports
{
    public class GarmentPurchasingExpeditionReportServiceTest
    {
        private const string Entity = "GarmentPurchasingExpeditionReport";

        private string GetCurrentAsyncMethod([CallerMemberName] string methodName = "")
        {
            var method = new StackTrace()
                .GetFrames()
                .Select(frame => frame.GetMethod())
                .FirstOrDefault(item => item.Name == methodName);

            return method.Name;

        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IIdentityService)))
                .Returns(new IdentityService() { TimezoneOffset = 0, Token = "token", Username = "username" });

            return serviceProviderMock;
        }

        private FinanceDbContext GetDbContext(string testName)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseInternalServiceProvider(serviceProvider);

            return new FinanceDbContext(optionsBuilder.Options);
        }

        [Fact]
        public void Should_Success_Generate_Excel()
        {
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
               .Returns(dbContext);

            var service = new GarmentPurchasingExpeditionReportService(serviceProviderMock.Object);

            var reportResponse = service.GenerateExcel(1, 1, GarmentPurchasingExpeditionPosition.VerificationAccepted, DateTimeOffset.Now, DateTimeOffset.Now);
            Assert.NotNull(reportResponse);
        }

        [Fact]
        public void Should_Success_Get_Report()
        {
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
               .Returns(dbContext);

            var service = new GarmentPurchasingExpeditionReportService(serviceProviderMock.Object);

            var reportResponse = service.GetReport(1, 1, GarmentPurchasingExpeditionPosition.VerificationAccepted, DateTimeOffset.Now, DateTimeOffset.Now);
            Assert.NotNull(reportResponse);
        }

        [Fact]
        public void Should_Success_Get_Report_ViewModel()
        {
            var dbContext = GetDbContext(GetCurrentAsyncMethod());
            var serviceProviderMock = GetServiceProvider();

            serviceProviderMock
               .Setup(serviceProvider => serviceProvider.GetService(typeof(FinanceDbContext)))
               .Returns(dbContext);

            var service = new GarmentPurchasingExpeditionReportService(serviceProviderMock.Object);

            var reportResponse = service.GetReportViewModel(1, 1, GarmentPurchasingExpeditionPosition.VerificationAccepted, DateTimeOffset.Now, DateTimeOffset.Now);
            Assert.NotNull(reportResponse);
        }
    }
}
