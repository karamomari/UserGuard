
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Text.Json.Serialization;
using UserGuard_API.Mappings;
using UserGuard_API.Model;
using UserGuard_API.Repositories.Implementations;


namespace UserGuard_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecritKey"]));
            builder.Services.AddSingleton(key);

            builder.Services.AddControllers().ConfigureApiBehaviorOptions(op =>
            {
                op.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddDbContext<AcademyAPI>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DB"));


            });




            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // إعدادات كلمة السر
                options.Password.RequireDigit = false; // يجب أن تحتوي على رقم
                options.Password.RequireLowercase = false; // يجب أن تحتوي على حرف صغير
                options.Password.RequireUppercase = false; // يجب أن تحتوي على حرف كبير
                options.Password.RequireNonAlphanumeric = false; // يجب أن تحتوي على رمز غير أبجدي رقمي
                options.Password.RequiredLength = 2; // طول كلمة السر (على الأقل 8 حروف)
                options.Password.RequiredUniqueChars = 1; // عدد الحروف الفريدة في كلمة السر

                // إعدادات تسجيل الدخول
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // مدة الحظر بعد عدد محاولات فاشلة
                options.Lockout.MaxFailedAccessAttempts = 4; // عدد المحاولات الفاشلة قبل الحظر

                // إعدادات التحقق من البريد الإلكتروني
                options.SignIn.RequireConfirmedAccount = false; // يتطلب تأكيد الحساب بعد التسجيل
            }).AddEntityFrameworkStores<AcademyAPI>()
             .AddDefaultTokenProviders();




            builder.Services.AddAuthentication(op =>
            {
                //شوف التوكن مش ال الكوكيز
                op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //شو تعمل اذا كان مش اوثرايز
                op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                //اي اشي ضل 
                op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(op => //هون عمليه الverfied
            {
                op.SaveToken = true; //التوكن بعده شغال
                op.RequireHttpsMetadata = false; //  لو ترو بكون جبرته ب httpS
                op.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:IssuerIP"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:AudienceIP"],
                    IssuerSigningKey =
                      new SymmetricSecurityKey(
                          Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecritKey"]))

                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("SecretaryPolicy", policy =>
                    policy.RequireRole("Secretary"));


                options.AddPolicy("TeacherPolicy", policy =>
                    policy.RequireRole("Teacher"));

                options.AddPolicy("StudentPolicy", policy =>
                   policy.RequireRole("Student"));

                options.AddPolicy("MaintenancePolicy", policy =>
                 policy.RequireRole("Maintenance"));

                options.AddPolicy("AdminOrSecretary", policy =>
                    policy.RequireRole("Admin", "Secretary"));

                options.AddPolicy("AdminOrSecretaryOrMaintenance", policy =>
                   policy.RequireRole("Admin", "Secretary", "Maintenance"));
            });



            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<IBranchRepository, BranchRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IGradeRepository, GradeRepository>();
            builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
            builder.Services.AddScoped<IMaintenanceTaskRepository, MaintenanceTaskRepository>();

            builder.Services.AddScoped<IImageService, ImageService>();

           // builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddHttpClient<ResendEmailService>();
            builder.Services.AddMediatR(typeof(Program));



            builder.Services.AddCors(op =>
            {
                op.AddPolicy("Mypolicy", policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });

            });



            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("Mypolicy");



            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
