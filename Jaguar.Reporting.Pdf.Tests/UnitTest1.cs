using System;
using System.IO;
using System.Linq;
using Jaguar.Reporting.Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestGetString()
        {
            var pdf = new PdfGenerator();
            var report = DataRepository.GetReport();
            var data = DataRepository.GetDummyData(new string[] { "data" });

            Assert.ThrowsException<NotImplementedException>(() => pdf.GetString(report, data, null));
        }

        [TestMethod]
        public void TestFileSave()
        {
            var filePath = "report.pdf";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var pdf = new PdfGenerator();
            var report = DataRepository.GetReport();
            var data = DataRepository.GetDummyData(new string[] { "data" });

            var b = pdf.GetAllBytes(report, data, null);

            File.WriteAllBytes(filePath, b);

            Assert.IsTrue(File.Exists(filePath));
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

            var n = data.Any(x => x.TableName == "data");
            Assert.IsTrue(n);
        }

        [TestMethod]
        public void TestGetReport()
        {
            var report = DataRepository.GetReport();
            Assert.IsNotNull(report);
        }
    }
}
