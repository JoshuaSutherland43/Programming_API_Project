using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models;
using Claim = System.Security.Claims.Claim;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Data.SqlClient;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<DbApiContext>(options => options.UseSqlServer("Data Source = labVMH8OX\\SQLEXPRESS; Initial Catalog = PROG; Integrated Security = true"));

            //#region Set Up Region
            //builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<DbApiContext>().AddDefaultTokenProviders();
            //#endregion

            //builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddScoped<Data>();

            #region Add bearer to top
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Prog API", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter valid token in the text input below. \n\n Example: \" abcdef12345\"",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
            });

            #endregion

            #region Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "API",
                    ValidAudience = "API", 
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("d7s8h484987wfw8fh9w8hf74wgfw948h94w8fh98w4hf98w74hgw8gh98w4w048g"))
                };
            });
            #endregion

            var app = builder.Build();

           

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();
            //app.UseAuthentication(); //Need this don't forget to add    
            //app.UseAuthorization();

            #region Sign Up
            app.MapPost("/signup", (string email, string password, string firstName, string surname) =>
            {
                try
                {
                    var data = new Data();

                    data.SignUp(email, password, firstName, surname);

                    return Results.Ok("User successfully registered.");
                }
                catch (SqlException ex)
                {
                    return Results.Problem($"Database error: {ex.Message}", statusCode: 500);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Unexpected error: {ex.Message}", statusCode: 500);
                }
            }).WithName("SignUp").WithOpenApi();
            #endregion

            #region Login
            app.MapPost("/login", (string email, string password) =>
            {
                try
                {
                    var data = new Data();
                    var user = data.LoginUser(email, password);

                    if (user == null)
                    {
                        return Results.BadRequest("Invalid email or password.");
                    }

                    // Ensure UserID is retrieved correctly from the user object
                    var token = GenerateJwtToken(user.Email, user.RoleId, user.UserID); // Pass UserID
                    return Results.Ok(new { Token = token, User = user });
                }
                catch (SqlException ex)
                {
                    return Results.Problem($"Database error: {ex.Message}", statusCode: 500);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Unexpected error: {ex.Message}", statusCode: 500);
                }
            }).WithName("LoginUser").WithOpenApi();
            #endregion


            #region Upload Claim
            app.MapPost("/uploadClaim", (DateTime uploadDate, int hours, double rate, string? notes, int userId, string filePath, string status) =>
          {
             var data = new Data();
             data.UploadClaim(uploadDate, hours, rate, notes, userId, filePath, status);
              return Results.Ok("Claim uploaded");
          }).WithName("UploadClaim").WithOpenApi();
          #endregion

            #region Get User Claims
            app.MapGet("/getUserClaims", (int userId) =>
            {
                var data = new Data();
                var claims = data.GetUserClaims(userId);
                return Results.Ok(claims);
            }).WithName("GetUserClaims").WithOpenApi();
            #endregion

            #region Update Claim
            app.MapPut("/updateClaim",(int claimId, string status) =>
            {
                var data = new Data();
                data.UpdateClaim(claimId, status);
                return Results.Ok("Claim updated");
            }).WithName("UpdateClaim").WithOpenApi();
            #endregion

            #region Get all Claims
            app.MapGet("/getAllClaims", () =>
            {
                var data = new Data();
                var claims = data.GetAllClaims();
                return Results.Ok(claims);
            }).WithName("GetAllClaims").WithOpenApi();
            #endregion

            #region Get User FullName by Claim
            app.MapGet("/getUserFullNameByClaim", (int claimId) =>
            {
                var data = new Data();
                var fullName = data.GetUserFullNameByClaim(claimId);
                return Results.Ok(fullName);
            }).WithName("GetUserFullNameByClaim").WithOpenApi();
            #endregion

            #region Get User FullName from lecturer
            app.MapGet("/getLecturerFullName",(int lecturerId) =>
            {
                var data = new Data();
                var fullName = data.GetLecturerFullNameFromLecturer(lecturerId);
                return Results.Ok(fullName);
            }).WithName("GetLecturerFullName").WithOpenApi();
            #endregion

            #region Amount Earned
            app.MapGet("/getAmountEarned", (int claimId) =>
            {
                var data = new Data();
                var amount = data.GetAmountEarned(claimId);
                return Results.Ok(amount);
            }).WithName("GetAmountEarned").WithOpenApi();
            #endregion

            #region Get Total Amount Earned
            app.MapGet("/getTotalAmountEarned", (int lecturerId) =>
            {
                var data = new Data();
                var total = data.GetTotalAmountEarnedByLecturer(lecturerId);
                return Results.Ok(total);
            }).WithName("GetTotalAmountEarned").WithOpenApi();
            #endregion

            #region Upload Invoice
            app.MapPost("/uploadInvoice", (int lecturerId, int claimId) =>
            {
                var data = new Data();
                data.UploadInvoice(lecturerId, claimId);
                return Results.Ok("Invoice uploaded");
            }).WithName("UploadInvoice").WithOpenApi();
            #endregion

            #region Get All Invoices
            app.MapGet("/getAllInvoices", () =>
            {
                var data = new Data();
                var invoices = data.GetAllInvoices();
                return Results.Ok(invoices);
            }).WithName("GetAllInvoices").WithOpenApi();
            #endregion

            #region Get All Claims from Invoice
            app.MapGet("/getAllClaimsFromInvoice",  () =>
            {
                var data = new Data();
                var claims = data.GetAllClaimsFromInvoice();
                return Results.Ok(claims);
            }).WithName("GetAllClaimsFromInvoice").WithOpenApi();
            #endregion

            #region Get All Processed Claims
            app.MapGet("/getAllProcessedClaims", () =>
            {
                var data = new Data();
                var claims = data.GetAllProcessedClaims();
                return Results.Ok(claims);
            }).WithName("GetAllProcessedClaims").WithOpenApi();
            #endregion

            #region Get Processed Claims by Lecturer
            app.MapGet("/getProcessedClaimsByLecturer/{lecturerId}", (int lecturerId) =>
            {
                var data = new Data();
                var claims = data.GetProcessedClaimsByLecturer(lecturerId);
                return Results.Ok(claims);
            }).WithName("GetProcessedClaimsByLecturer").WithOpenApi();
            #endregion

            #region Get Approved Claims by Lecturer
            app.MapGet("/getApprovedClaimsByLecturer/{lecturerId}", (int lecturerId) =>
            {
                var data = new Data();
                var claims = data.GetApprovedClaimsByLecturer(lecturerId);
                return Results.Ok(claims);
            }).WithName("GetApprovedClaimsByLecturer").WithOpenApi();
            #endregion

            app.Run();
        }

        #region Generate JWTtoken with user rights
        private static string GenerateJwtToken(string email, string role, int userId)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role),
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()) 
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("d7s8h484987wfw8fh9w8hf74wgfw948h94w8fh98w4hf98w74hgw8gh98w4w048g"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "API",
                audience: "API",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion


    }
}