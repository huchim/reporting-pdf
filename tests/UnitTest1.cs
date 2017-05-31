using System;
using Jaguar.Reporting.Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var pdf = new PdfGenerator();
            var report = DataRepository.GetReport();
            var data = DataRepository.GetDummyData(new string[] { "data" });

            Assert.ThrowsException<NotImplementedException>(() => pdf.GetString(report, data, null));
        }

        [TestMethod]
        public void MyTestGetAllBytes()
        {
            var pdf = new PdfGenerator();
            var report = DataRepository.GetReport();
            var data = DataRepository.GetDummyData(new string[] { "data" });
            var b = pdf.GetAllBytes(report, data, null);
        }
        
        [TestMethod]
        public void TestGetDummyData()
        {
            var data = DataRepository.GetDummyData(new string[] { "data" });
        }

        [TestMethod]
        public void TestGetReport()
        {
            var report = DataRepository.GetReport();
        }
    }
}
