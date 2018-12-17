using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Test.Controller.Utils;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.JournalTransaction;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.JournalTransaction
{
    public class JournalTransactionControllerTest : BaseControllerTest<JournalTransactionController, JournalTransactionModel, JournalTransactionViewModel, IJournalTransactionService>
    {
        [Fact]
        public void GetReport_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(), It.IsAny<int>())).Returns((new ReadResponse<JournalTransactionReportViewModel>(new List<JournalTransactionReportViewModel>(), 1, new Dictionary<string, string>(), new List<string>()), 0, 0));

            var response = GetController(mocks).GetReport();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void GetReport_ThrowException()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GetReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(), It.IsAny<int>())).Throws(new Exception());

            var response = GetController(mocks).GetReport();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void GetReportExcel_ReturnFile()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(), It.IsAny<int>())).Returns(new MemoryStream());

            var response =  GetController(mocks).GetXls();
            Assert.NotNull(response);

            var response2 = GetController(mocks).GetXls(DateTimeOffset.UtcNow);
            Assert.NotNull(response2);

            var response3 = GetController(mocks).GetXls(null, DateTimeOffset.UtcNow);
            Assert.NotNull(response3);

            var response4 = GetController(mocks).GetXls(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
            Assert.NotNull(response4);

        }
        [Fact]
        public void GetReportExcel_ThrowException()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.GenerateExcel(It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(), It.IsAny<int>())).Throws(new Exception());

            var response = GetController(mocks).GetXls();
            Assert.NotNull(response);
        }
    }
}
