using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using ToDoListAPI.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApi(
		options => builder.Configuration.Bind("AzureAd", options),
		options => builder.Configuration.Bind("AzureAd", options));

builder.Services.AddDbContext<ToDoContext>(options =>
	options.UseInMemoryDatabase("ToDos"));
builder.Services.AddControllers();
builder.Services.AddCors(options => options.AddPolicy("default", policy =>
	policy.AllowAnyOrigin()
		.AllowAnyMethod()
		.AllowAnyHeader()));

var app = builder.Build();

app.UseCors("default");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
