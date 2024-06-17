using secure_bookstore.Services;
using Microsoft.EntityFrameworkCore;
using secure_bookstore.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using AspNetCoreRateLimit;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.



var configurationBuilder = new ConfigurationBuilder()
			.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.AddXmlFile("appsettings.xml", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables()
			.AddCommandLine(args);
            
IConfiguration configuration = configurationBuilder.Build();

string CertPassword = PathManager.GetSecurityPassword(configuration);
HostConfig.CertPassword = CertPassword;
HostConfig.CertPath = configuration["CertPath"];

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var host = Dns.GetHostEntry("bookstore.io");
    serverOptions.Listen(host.AddressList[0], 5016);
     serverOptions.Listen(host.AddressList[0], 7240, listOption=>
     {
         listOption.UseHttps(HostConfig.CertPath, HostConfig.CertPassword);
     });
});

// Add services to the container.
//builder.Services.Configure<ApiKeys>(builder.Configuration.GetSection("ApiKeys"));
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(item => 
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item=>
{
    item.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = configuration["JwtSettings:Issuer"],
        ValidAudience = configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:securitykey"])),
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SamoAdmini", p=> p.RequireClaim("Neki"));
});

builder.Services.AddMemoryCache();
 builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
 builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
 builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
 builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
 builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEncryptService, EncryptService>();
// builder.Services.AddHttpsRedirection(options =>
// {
//     options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
//     options.HttpsPort = 443;
//     });

var _jwtsettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(_jwtsettings);

//builder.Services.Configure<KestrelServerOptions>(configuration.GetSection("Kestrel"));

//HSTS Security Headers
builder.Services.AddHsts(options=>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(60);
});


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.Use(async (ctx, next) =>
 {
     ctx.Response.Headers.Add("Content-Security-Policy",
     "default-src 'self';"); //report-uri /idgreport
     ctx.Response.Headers.Add("X-Content-Type-Options", "nosniff");
     await next();
 });

app.UseAuthentication(); //authentification
app.UseAuthorization(); //authorization

app.MapControllers();
app.UseIpRateLimiting();
//app.UseMiddleware<ApiKeyValidationMiddleware>();

app.Run();
