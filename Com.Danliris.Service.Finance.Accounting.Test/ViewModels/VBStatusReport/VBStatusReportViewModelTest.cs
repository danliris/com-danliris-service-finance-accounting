using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.VBStatusReport;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.ViewModels.VBStatusReport
{
    public class VBStatusReportViewModelTest
    {
        [Fact]
        public void Should_Success_Instantiate()
        {
            long Id = 1;
            string VBNo = "VBNo";
            string CreateBy = "CreateBy";
            string RealizationNo = "RealizationNo";
            string Usage = "Usage";
            int Aging = 1;
            decimal Amount = 1;
            decimal RealizationAmount = 1;
            decimal Difference = 1;
            string Status = "Status";
            Unit uvm = new Unit();
            string RealizationDate = "string";
            string Date = "string";
            string DateEstimate = "string";
            string LastModifiedUtc = "string";
            ICollection<RealizationVbDetailModel> Details = new List<RealizationVbDetailModel>();

            VBStatusReportViewModel vsrvm = new VBStatusReportViewModel();
            vsrvm.Id = Id;
            vsrvm.VBNo = VBNo;
            vsrvm.CreateBy = CreateBy;
            vsrvm.RealizationNo = RealizationNo;
            vsrvm.Usage = Usage;
            vsrvm.Aging = Aging;
            vsrvm.Amount = Amount;
            vsrvm.RealizationAmount = RealizationAmount;
            vsrvm.Difference = Difference;
            vsrvm.Status = Status;
            vsrvm.Unit = uvm;
            vsrvm.RealizationDate = RealizationDate;
            vsrvm.Date = Date;
            vsrvm.DateEstimate = DateEstimate;
            vsrvm.LastModifiedUtc = LastModifiedUtc;
            vsrvm.Details = (ICollection<RealizationVbDetailModel>)Details;


            Assert.Equal(Id, vsrvm.Id);
            Assert.Equal(VBNo, vsrvm.VBNo);
            Assert.Equal(CreateBy, vsrvm.CreateBy);
            Assert.Equal(RealizationNo, vsrvm.RealizationNo);
            Assert.Equal(Usage, vsrvm.Usage);
            Assert.Equal(Aging, vsrvm.Aging);
            Assert.Equal(Amount, vsrvm.Amount);
            Assert.Equal(RealizationAmount, vsrvm.RealizationAmount);
            Assert.Equal(Difference, vsrvm.Difference);
            Assert.Equal(Status, vsrvm.Status);
            Assert.Equal(uvm, vsrvm.Unit);
            Assert.Equal(RealizationDate, vsrvm.RealizationDate);
            Assert.Equal(Date, vsrvm.Date);
            Assert.Equal(DateEstimate, vsrvm.DateEstimate);
            Assert.Equal(LastModifiedUtc, vsrvm.LastModifiedUtc);
            Assert.Equal((IEnumerable<RealizationVbDetailModel>)Details, vsrvm.Details);
            //long Id = 1;
            //string VBNo = "VBNo";
            //string CreateBy = "CreateBy";
            //string RealizationNo = "RealizationNo";
            //string Usage = "Usage";
            //int Aging = 1;
            //decimal Amount = 1;
            //decimal RealizationAmount = 1;
            //decimal Difference = 1;
            //string Status = "Status";
            //Unit uvm = new Unit();
            //DateTimeOffset RealizationDate = DateTimeOffset.Now;
            //DateTimeOffset Date = DateTimeOffset.Now;
            //DateTimeOffset DateEstimate = DateTimeOffset.Now;
            //DateTime LastModifiedUtc = DateTime.Now;
            //ICollection<RealizationVbDetailModel> Details = new List<RealizationVbDetailModel>();

            //VBStatusReportViewModel vsrvm = new VBStatusReportViewModel();
            //vsrvm.Id = Id;
            //vsrvm.VBNo = VBNo;
            //vsrvm.CreateBy = CreateBy;
            //vsrvm.RealizationNo = RealizationNo;
            //vsrvm.Usage = Usage;
            //vsrvm.Aging = Aging;
            //vsrvm.Amount = Amount;
            //vsrvm.RealizationAmount = RealizationAmount;
            //vsrvm.Difference = Difference;
            //vsrvm.Status = Status;
            //vsrvm.Unit = uvm;
            //vsrvm.RealizationDate = RealizationDate;
            //vsrvm.Date = Date;
            //vsrvm.DateEstimate = DateEstimate;
            //vsrvm.LastModifiedUtc = LastModifiedUtc;
            //vsrvm.Details = (ICollection<RealizationVbDetailModel>)Details;


            //Assert.Equal(Id, vsrvm.Id);
            //Assert.Equal(VBNo, vsrvm.VBNo);
            //Assert.Equal(CreateBy, vsrvm.CreateBy);
            //Assert.Equal(RealizationNo, vsrvm.RealizationNo);
            //Assert.Equal(Usage, vsrvm.Usage);
            //Assert.Equal(Aging, vsrvm.Aging);
            //Assert.Equal(Amount, vsrvm.Amount);
            //Assert.Equal(RealizationAmount, vsrvm.RealizationAmount);
            //Assert.Equal(Difference, vsrvm.Difference);
            //Assert.Equal(Status, vsrvm.Status);
            //Assert.Equal(uvm, vsrvm.Unit);
            //Assert.Equal(RealizationDate, vsrvm.RealizationDate);
            //Assert.Equal(Date, vsrvm.Date);
            //Assert.Equal(DateEstimate, vsrvm.DateEstimate);
            //Assert.Equal(LastModifiedUtc, vsrvm.LastModifiedUtc);
            //Assert.Equal((IEnumerable<RealizationVbDetailModel>)Details, vsrvm.Details);

        }
    }
}