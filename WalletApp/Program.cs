using ApplicationCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using Infrastructure.Configuration;
using WalletApp.Resource;

namespace WalletApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            builder.Services.Configure<ParamConfiguration>(builder.Configuration.GetSection("Configuration"));

            // SmarContract
            builder.Services.AddTransient((smartContract) =>
            {
                var myConfig = builder.Configuration.GetSection("Configuration");
                
                return new Infrastructure.Ethereum.SmartContractContext(myConfig);
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();

            app.UseCors(options => options.WithOrigins("*").AllowAnyMethod().AllowAnyHeader());

            app.MapControllers();

            app.Run();
        }
    }
}