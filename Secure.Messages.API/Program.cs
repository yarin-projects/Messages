using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SecureMessagesAPI.Data;
using SecureMessagesAPI.Encryption;
using SecureMessagesAPI.Interfaces;
using SecureMessagesAPI.Repositories;
using System.Reflection;
using System.Text.Json.Serialization;

namespace SecureMessagesAPI
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
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SecureMessagesAPI",
                    Version = "v1",
                    Description = "API for Secure Messages",
                    Contact = new OpenApiContact
                    {
                        Name = "Yarin"
                    },
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddDbContext<UserContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlite(connectionString);
            });
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", options =>
                {
                    options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            builder.Services.AddScoped<IUserRespository, UserRepository>();
            builder.Services.AddSingleton<IEncryptionHelper, EncryptionHelper>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecureMessagesAPI v1"));
            }

            app.UseAuthorization();

            app.UseCors("AllowAll");

            app.MapControllers();

            app.Run();
        }
    }
}