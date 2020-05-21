namespace DataLayerApproaches
{
    using System.Reflection;
    using Data;
    using Data.Repositories;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Queries;
    using Queries.Cats;
    using Services;

    public class Startup
    {
        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services
                .AddDbContext<CatsDbContext>(opt => opt
                    .UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));

            services
                .AddTransient(typeof(IRepository<>), typeof(EfRepository<>));

            services
                .AddTransient<ICatService, CatService>();

            services
                .AddTransient<ICatQueryService, CatQueryService>()
                .AddTransient<CatsWithOwnersQuery>()
                .AddTransient<UpdateCatOwnerQuery>();

            services.AddMediatR(Assembly.GetExecutingAssembly());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
