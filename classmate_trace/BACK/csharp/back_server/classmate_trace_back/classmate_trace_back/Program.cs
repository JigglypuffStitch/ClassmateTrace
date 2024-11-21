using Microsoft.EntityFrameworkCore;
using ClassmateTraceBack.Models;
using ClassmateTraceBack.Models.Home;
using System;

namespace ClassmateTraceBack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            String? connnectionString = builder.Configuration.GetConnectionString("ctDB");
            //builder.Services.AddDbContext<ctDbContext>(opt => opt.UseMySQL(connnectionString));
            builder.Services.AddScoped<PictureService>();
            builder.Services.AddScoped<AdminService>();
            builder.Services.AddScoped<ClassService>();
            builder.Services.AddScoped<ClassmateService>();
            builder.Services.AddScoped<AddressService>();
            builder.Services.AddScoped<MapService>();
            builder.Services.AddScoped<CommentsService>();
            builder.Services.AddScoped<LoginAndRegisterService>();
            builder.Services.AddScoped<PersonalService>();
            builder.Services.AddScoped<MemoryService>();
            builder.Services.AddScoped<GatheringService>();


            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://172.20.10.2:8080", "http://172.20.10.3:8080", "http://172.20.10.4:8080", "http://172.20.10.7:8080", "http://172.20.10.9:8080", "http://172.20.10.12:8080", "http://172.20.10.5:8080") // 允许的前端应用地址
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });



            var app = builder.Build();


            // Configure the HTTP request pipeline.
            //      if (app.Environment.IsDevelopment())
            //      {
            //          app.UseSwagger();
            //          app.UseSwaggerUI();
            //      }
            // ysq modified below
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseCors("AllowSpecificOrigin"); // 启用 CORS

            app.Run();
        }
    }
}