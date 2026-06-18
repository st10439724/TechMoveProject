
using TechMove.Services;
using Xunit;

namespace TechMove.Tests
{
    public class UnitTest1
    {
        private readonly ValidationService _validationService;

        public UnitTest1()
        {
            _validationService = new ValidationService();
        }

        [Fact]
        public void ConvertUsdToZar_CorrectAmount_ReturnsExpectedResult()
        {
            decimal usdAmount = 100;
            decimal exchangeRate = 18.50m;
            decimal result = _validationService.ConvertUsdToZar(usdAmount, exchangeRate);
            Assert.Equal(1850.00m, result);
        }

        [Fact]
        public void IsValidPdfFile_WithPdfExtension_ReturnsTrue()
        {
            string fileName = "agreement.pdf";
            bool result = _validationService.IsValidPdfFile(fileName);
            Assert.True(result);
        }

        [Fact]
        public void IsValidPdfFile_WithExeExtension_ReturnsFalse()
        {
            string fileName = "malware.exe";
            bool result = _validationService.IsValidPdfFile(fileName);
            Assert.False(result);
        }

        [Fact]
        public void CanRaiseServiceRequest_ActiveContract_ReturnsTrue()
        {
            string contractStatus = "Active";
            bool result = _validationService.CanRaiseServiceRequest(contractStatus);
            Assert.True(result);
        }

        [Fact]
        public void CanRaiseServiceRequest_ExpiredContract_ReturnsFalse()
        {
            string contractStatus = "Expired";
            bool result = _validationService.CanRaiseServiceRequest(contractStatus);
            Assert.False(result);
        }

        [Fact]
        public void CanRaiseServiceRequest_OnHoldContract_ReturnsFalse()
        {
            string contractStatus = "On Hold";
            bool result = _validationService.CanRaiseServiceRequest(contractStatus);
            Assert.False(result);
        }
    }
}






















































//namespace TechMove.Tests
//{
//    public class UnitTest1
//    {
//        [Fact]
//        public void Test1()
//        {

//        }
//    }
//}