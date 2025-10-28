using Microsoft.AspNetCore.Mvc;

namespace InvestmentPerformanceWebAPIExercise.Common
{
    public class Attributes
    {
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class ApiKeyAttribute : ServiceFilterAttribute
        {
            public ApiKeyAttribute() : base(typeof(ApiKeyAuthorizationFilter)) { }
        }
    }
}
