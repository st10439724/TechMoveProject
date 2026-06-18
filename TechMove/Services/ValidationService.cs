namespace TechMove.Services
{
    public class ValidationService
    {
        public decimal ConvertUsdToZar(decimal usdAmount, decimal exchangeRate)
        {
            return usdAmount * exchangeRate;
        }

        public bool IsValidPdfFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var extension = Path.GetExtension(fileName).ToLower();
            return extension == ".pdf";
        }

        public bool CanRaiseServiceRequest(string contractStatus)
        {
            if (contractStatus == "Expired" || contractStatus == "On Hold")
                return false;

            return true;
        }
    }
}