using InvestmentPerformanceWebAPIExercise.Data;
using InvestmentPerformanceWebAPIExercise.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;

namespace InvestmentPerformanceWebAPIExercise
{
    public class InvestmentsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        private Guid _investmentId = Guid.Empty;

        public InvestmentsControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace DbContext with in-memory version
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<InvestmentDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<InvestmentDbContext>(options =>
                        options.UseInMemoryDatabase("NuixTestDb"));

                    // Seed test data
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<InvestmentDbContext>();

                    _investmentId = Guid.NewGuid();

                    db.Investments.Add(new Investment
                    {
                        InvestmentId = _investmentId,
                        UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Name = "Test Investment",
                        Shares = 100,
                        CostBasisPerShare = 50.00m,
                        CurrentPrice = 75.00m,
                        PurchaseDate = DateTime.UtcNow.AddMonths(-14)
                    });
                    db.SaveChanges();
                });
            }).CreateClient();
        }

        [Fact]
        public async Task GetUserInvestments_ReturnsInvestmentList()
        {
            var userId = "11111111-1111-1111-1111-111111111111";

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/investments/user?userId={userId}");
            request.Headers.Add("x-api-key", "NuiX123!");

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.Contains("Test Investment", content);

        }

        [Fact]
        public async Task GetInvestmentDetails_ReturnsCorrectPerformance()
        {
            var userId = "11111111-1111-1111-1111-111111111111";
            var investmentId = _investmentId;
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/investments/user/{userId}/investment/{investmentId}");
            request.Headers.Add("x-api-key", "NuiX123!");

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<InvestmentDetailDto>();

            Assert.Equal(100, json!.Shares);
            Assert.Equal("Long Term", json.Term);
            Assert.Equal(2500.00m, json.TotalGainLoss);
        }
    }
}