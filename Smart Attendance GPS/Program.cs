using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Smart_Attendance_GPS.Helpers;
using Smart_Attendance_GPS.Models;
using Smart_Attendance_GPS.Models.Context;
using Smart_Attendance_GPS.Seeders;
using Smart_Attendance_GPS.Services;
using Smart_Attendance_GPS.Services.IService;
using System.Text;
using FaceRecognition.Core;
using FaceRecognition.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http, 
        Scheme = "bearer",               
        BearerFormat = "JWT",           
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token."
    });

    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
    //, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    ));

builder.Services.AddIdentity<ApplicationUser,IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

// Configure FaceRecognition services
builder.Services.Configure<FaceRecognitionOptions>(
    builder.Configuration.GetSection("FaceRecognition"));
builder.Services.AddFaceRecognition();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Any",
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();

        });

});

builder.Services.AddScoped<DbSeeder>();  // Register the DbSeeder class
//services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService,AdminService >();
builder.Services.AddScoped<ICompanyService,CompanyService >();
builder.Services.AddScoped<IAttendanceService,AttendanceService>();
builder.Services.AddScoped<IFaceEnrollmentService, FaceEnrollmentService>();
builder.Services.AddScoped<IFaceVerificationAppService, FaceVerificationAppService>();



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JWT").Get<JWT>();
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                // Prevent default 401 response
                context.HandleResponse();
                // Throw exception for error handling middleware
                //throw new UnauthorizedAccessException("User not authenticated");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\": \"User not authenticated\"}");
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("Any");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Attendance GPS API V1");
    c.RoutePrefix = string.Empty; // Swagger UI at root URL (e.g., https://localhost:5001/)
});


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//db seeder
//using (var scope = app.Services.CreateScope())
//{
//    var dbSeeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
//    await dbSeeder.SeedAsync();
//}
app.Run();
