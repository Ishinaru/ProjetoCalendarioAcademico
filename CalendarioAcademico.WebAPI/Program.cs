using CalendarioAcademico.Data.ConfigGeral;
using CalendarioAcademico.Data.DBContext;
using CalendarioAcademico.Data.DBContext.Repositories.Calendario;
using CalendarioAcademico.Data.DBContext.Repositories.Generic;
using CalendarioAcademico.Data.DBContext.UnitOfWork;
using CalendarioAcademico.WebAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Mapster;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.Extensions.PlatformAbstractions;
using CalendarioAcademico.WebAPI.Services.Dropdown;
using CalendarioAcademico.WebAPI.Middleware;
using CalendarioAcademico.WebAPI.Services.DiasLetivos;
namespace CalendarioAcademico.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Build
            var builder = WebApplication.CreateBuilder(args);

            // DbContext como Scoped
            builder.Services.AddDbContext<CalendarioAcademicoContext>(options => 
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStrings")), ServiceLifetime.Scoped);

            //DbContextFactory como Scoped
            builder.Services.AddDbContextFactory<CalendarioAcademicoContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStrings")), ServiceLifetime.Scoped);


            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IDropdownService, DropdownService>();
            builder.Services.AddScoped<IDiasLetivosService, DiasLetivosService>();
            builder.Services.AddControllers();

            builder.Services.RegisterMaps();

            // swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options => 
            {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Documentação Calendário Acadêmico",
                        Description = "Documentação da API responsável pelo gerenciamento do calendário acadêmico da UESC.",

                    });
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Calendário Acadêmico");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
