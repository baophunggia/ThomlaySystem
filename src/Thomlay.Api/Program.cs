using Microsoft.EntityFrameworkCore;
using Thomlay.Application.Abstractions.Repositories;
using Thomlay.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowThomlayBase", policy =>
    {
        policy.WithOrigins("https://thomlay.com", "https://www.thomlay.com") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 1. Cấu hình Database (PostgreSQL / Supabase)
builder.Services.AddDbContext<ThomlayDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SupabaseConnection")));

// 2. Đăng ký Repositories
builder.Services.AddScoped<IDeploymentOrderRepository, DeploymentOrderRepository>();
// Đăng ký Handler của Vanilla CQRS
builder.Services.AddScoped<Thomlay.Application.Commands.Orders.CreateDeploymentOrderCommandHandler>();

// 3. Đăng ký các API Controllers
builder.Services.AddControllers();

// 4. Cấu hình Swagger để test API nhanh chóng
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Đăng ký Secret Key của Stripe
Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

var app = builder.Build();

// 5. Cấu hình HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowThomlayBase");
app.UseAuthorization();
app.MapControllers();

app.Run();