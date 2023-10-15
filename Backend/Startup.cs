using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Backend.Data;
using Backend.Models;
using Backend.Repositories;

namespace Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityMongoDbProvider<User, MongoRole>(
                identityOptions => { identityOptions.SignIn.RequireConfirmedEmail = false; },
                mongoIdentityOptions =>
                {
                    mongoIdentityOptions.ConnectionString = 
                        Configuration.GetSection("MongoSettings:Connection").Value + "/" + 
                        Configuration.GetSection("MongoSettings:DatabaseName").Value;
                });
            
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(150); 
                options.LoginPath = "/login"; // Change this path if necessary
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder
                        .WithOrigins("http://localhost:5173")  // Adjust this to your front-end application's URL
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
            services.AddTransient<UserRepository>();
            services.AddTransient<FavoritesRepository>(); 
            services.AddControllersWithViews();
            services.AddSingleton<MongoDbContext>();
            services.AddControllers();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend v1"));
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseCors("AllowSpecificOrigin");

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Cross-Origin-Opener-Policy", "same-origin");
                context.Response.Headers.Add("Cross-Origin-Embedder-Policy", "require-corp");
                await next();
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
