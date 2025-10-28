using InvestmentPerformanceWebAPIExercise.Interfaces;

namespace InvestmentPerformanceWebAPIExercise.Services
{
    public class ApiKeyValidationService : IApiKeyValidator
    {
        private readonly IConfiguration _configuration;

        public ApiKeyValidationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsValid(string apiKey)
        {
            var validKey = _configuration["AppSettings:ApiKey"];
            return apiKey == validKey;
        }
    }

}
