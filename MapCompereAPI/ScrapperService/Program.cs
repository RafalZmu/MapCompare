
using ScrapperService.Connectors;
using ScrapperService.Services.UNSDScrapper;
using ScrapperService.Services.WebScrapper;

namespace ScrapperService
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
            builder.Services.AddSingleton<IScrapperService, UNSDScrapperService>();
			builder.Services.AddSingleton<ILLMServiceConnector, LLMServiceConnector>();
            builder.Services.AddSingleton<IDataProcessor, DataProcessor>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
