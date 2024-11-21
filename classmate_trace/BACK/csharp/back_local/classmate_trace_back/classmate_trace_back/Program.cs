using Microsoft.EntityFrameworkCore;
using ClassmateTraceBack.Models;
using ClassmateTraceBack.Models.Home;

namespace ClassmateTraceBack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers(options =>
            {
                options.MaxModelValidationErrors = 50;
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            // 设置 Kestrel 服务器选项
            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
                serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
            });

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
                        builder.WithOrigins("http://localhost:8080") // 允许的前端应用地址
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            // 添加全局异常处理中间件
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    
                    var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                    if (exception != null)
                    {
                        var response = new
                        {
                            StatusCode = 500,
                            Message = "内部服务器错误",
                            DetailedMessage = exception.Error.Message
                        };
                        
                        await context.Response.WriteAsJsonAsync(response);
                    }
                });
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseCors("AllowSpecificOrigin"); // 启用 CORS

            app.Run();
        }
    }
}