using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditBalance;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CreditBalance;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.CreditorAccount;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.DailyBankTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.JournalTransaction;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Services.Master;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.WebApi.Utilities;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Com.Danliris.Service.Finance.Accounting.WebApi
{
    public class Startup
    {
        private readonly string[] EXPOSED_HEADERS = new string[] { "Content-Disposition", "api-version", "content-length", "content-md5", "content-type", "date", "request-id", "response-time" };
        private readonly string FINANCE_POLICY = "FinancePolicy";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void RegisterEndpoint()
        {
            APIEndpoint.Core = Configuration.GetValue<string>("CoreEndpoint") ?? Configuration["CoreEndpoint"];
            APIEndpoint.Inventory = Configuration.GetValue<string>("InventoryEndpoint") ?? Configuration["InventoryEndpoint"];
            APIEndpoint.Purchasing = Configuration.GetValue<string>("PurchasingEndpoint") ?? Configuration["PurchasingEndpoint"];
            APIEndpoint.Finishing = Configuration.GetValue<string>("FinishingEndpoint") ?? Configuration["FinishingEndpoint"];
            //APIEndpoint.Production = Configuration.GetValue<string>("ProductionEndpoint") ?? Configuration["ProductionEndpoint"];
        }

        private void RegisterServices(IServiceCollection services)
        {
            services
                .AddScoped<IIdentityService, IdentityService>()
                .AddScoped<IValidateService, ValidateService>();
        }

        private void RegisterBusinessServices(IServiceCollection services)
        {
            services
                .AddTransient<ICOAService, COAService>()
                .AddTransient<ICreditorAccountService, CreditorAccountService>()
                .AddTransient<IJournalTransactionService, JournalTransactionService>()
                .AddTransient<ICreditBalanceService, CreditBalanceService>()
                .AddTransient<IDailyBankTransactionService, DailyBankTransactionService>();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString(Constant.DEFAULT_CONNECTION) ?? Configuration[Constant.DEFAULT_CONNECTION];

            services.AddDbContext<FinanceDbContext>(options => options.UseSqlServer(connectionString, c => c.CommandTimeout(60)));

            #region Register
            RegisterServices(services);

            RegisterBusinessServices(services);

            RegisterEndpoint();

            services.AddAutoMapper();

            #endregion

            #region Authentication
            string Secret = Configuration.GetValue<string>(Constant.SECRET) ?? Configuration[Constant.SECRET];
            SymmetricSecurityKey Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = false,
                        IssuerSigningKey = Key
                    };
                });
            #endregion

            #region CORS

            services.AddCors(options => options.AddPolicy(FINANCE_POLICY, builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders(EXPOSED_HEADERS);
            }));

            #endregion

            #region API

            services
                .AddApiVersioning(options => options.DefaultApiVersion = new ApiVersion(1, 0))
                .AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<FinanceDbContext>();
                context.Database.Migrate();
            }

            app.UseAuthentication();
            app.UseCors(FINANCE_POLICY);
            app.UseMvc();
        }
    }
}
