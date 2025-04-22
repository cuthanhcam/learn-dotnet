using SalaryCalculatorApi.Models;
using SalaryCalculatorApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Thêm Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Bật Swagger ở môi trường Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Minimal API: POST /api/salary
app.MapPost("/api/salary", (Employee emp) =>
{
    var service = new SalaryService();
    var result = service.CalculateSalary(emp);
    return Results.Ok(new { emp.Name, Salary = result });
});

app.Run();
