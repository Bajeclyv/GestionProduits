using GestionProduits.API.Models.DataManager;
using GestionProduits.API.Models.DTO;
using GestionProduits.API.Models.EntityFramework;
using GestionProduits.API.Models.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace GestionProduits.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("GestionProduitsContext") ?? throw new InvalidOperationException("Connection string 'GestionProduitsContext' not found.");

            builder.Services.AddDbContext<GestionProduitsContext>(options => options.UseNpgsql(connectionString));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IDataRepository<ProduitDto>, ProduitManager>();
            builder.Services.AddScoped<IProduitRepository, ProduitManager>();

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

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
