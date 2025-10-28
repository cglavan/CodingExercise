using InvestmentPerformanceWebAPIExercise.Common;
using InvestmentPerformanceWebAPIExercise.Data;
using InvestmentPerformanceWebAPIExercise.Interfaces;
using InvestmentPerformanceWebAPIExercise.Models;
using InvestmentPerformanceWebAPIExercise.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Reflection;

namespace InvestmentPerformanceWebAPIExercise
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            //********************************************************************
            // Register services
            //********************************************************************
            builder.Services.AddScoped<ApiKeyAuthorizationFilter>();
            builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidationService>();

            //********************************************************************
            // Swagger configuration
            //********************************************************************
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Investment Performance API",
                    Version = "v1",
                    Description = "API for querying investment performance data"
                });


                // Add API Key security definition
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "API Key needed to access the endpoints. Use 'x-api-key: YOUR_KEY'",
                    Name = "x-api-key", // Header name
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKeyScheme"
                });

                // Add global security requirement
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });



            //********************************************************************
            // Database dependency injection
            //********************************************************************
            var connString = string.Empty;
            var env = builder.Configuration.GetSection("AppSettings")["AppConfig"];
            switch (env.ToLower())
            {
                case "local":
                    connString = Encryption.Decrypt(builder.Configuration.GetConnectionString("SQLLocalConnection"));
                    break;
                case "dev":
                    connString = Encryption.Decrypt(builder.Configuration.GetConnectionString("SQLDevConnection"));
                    break;
                case "stage":
                    connString = Encryption.Decrypt(builder.Configuration.GetConnectionString("SQLStageConnection"));
                    break;
                default:
                    connString = Encryption.Decrypt(builder.Configuration.GetConnectionString("SQLProdConnection"));
                    break;
            }
            builder.Services.AddDbContext<InvestmentDbContext>(opt => opt.UseSqlServer(connString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)).LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Error));

            //******************************************
            // Serilog configuration
            //******************************************
            var logLevelSwitch = new LoggingLevelSwitch();

            switch (builder.Configuration.GetSection("AppSettings")["LoggingLevel"]?.ToLower())
            {
                case "information":
                    logLevelSwitch.MinimumLevel = LogEventLevel.Information;
                    break;
                case "warning":
                    logLevelSwitch.MinimumLevel = LogEventLevel.Warning;
                    break;
                case "debug":
                    logLevelSwitch.MinimumLevel = LogEventLevel.Debug;
                    break;
                case "error":
                    logLevelSwitch.MinimumLevel = LogEventLevel.Error;
                    break;
                default:
                    logLevelSwitch.MinimumLevel = LogEventLevel.Verbose;
                    break;
            }

            var sinkOpts = new MSSqlServerSinkOptions
            {
                TableName = "Logs",
                AutoCreateSqlDatabase = true,
                AutoCreateSqlTable = true,
                BatchPeriod = TimeSpan.FromMinutes(1),
                LevelSwitch = logLevelSwitch
            };
            var columnOpts = new ColumnOptions();
            columnOpts.Store.Remove(StandardColumn.Properties);
            columnOpts.Store.Add(StandardColumn.LogEvent);
            columnOpts.PrimaryKey = columnOpts.Id;
            columnOpts.Id.NonClusteredIndex = true;

            Log.Logger = new LoggerConfiguration()
              .AuditTo.MSSqlServer(
                  connectionString: connString,
                  sinkOptions: sinkOpts,
                  columnOptions: columnOpts
              )
              .Enrich.FromLogContext()
              .CreateLogger();


            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(Log.Logger);

            Log.Information("***************** Logging Initiated *************");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //Add 25 seed investments
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<InvestmentDbContext>();

                if (!db.Investments.Any())
                {
                    int count = 25;
                    int shares = new Random().Next(100, 501);
                    decimal cost = Math.Round((decimal)(new Random().NextDouble() * (500 - 100) + 100), 2);
                    decimal price = Math.Round((decimal)(new Random().NextDouble() * (500 - 100) + 100), 2);
                    int months = new Random().Next(1, 24);
                    for (int i = 0; i < count; i++)
                    {
                        db.Investments.Add(new Investment
                        {
                            InvestmentId = Guid.NewGuid(),
                            UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                            Name = $"Tech Fund {i + 1}",
                            Shares = shares,
                            CostBasisPerShare = cost,
                            CurrentPrice = price,
                            PurchaseDate = DateTime.UtcNow.AddMonths(-months)
                        });

                        //Generate random values for the next round of data
                        shares = new Random().Next(100, 501);
                        cost = Math.Round((decimal)(new Random().NextDouble() * (500 - 100) + 100), 2);
                        price = Math.Round((decimal)(new Random().NextDouble() * (500 - 100) + 100), 2);
                        months = new Random().Next(1, 24);
                    }
                    db.SaveChanges();
                }
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
