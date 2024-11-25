using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Context;
using TaskManagementSystem.FileUpload.Service.TaskManagementSystem.FileUpload;
using TaskManagementSystem.FileUpload.Service;
using TaskManagementSystem.Validation;
using Microsoft.OpenApi.Models;
using TaskManagementSystem.AutoMapping;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TaskManagementContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("BDConnection")));
//Large File Can Upload
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = long.MaxValue;
});

//builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly); // First call
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // Second call


    
// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<UserValidation>();

builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<TaskValidation>());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.EnableAnnotations(); // Enable Swagger annotations support
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());


app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();
app.UseAuthorization();

app.MapControllers();

app.Run();
