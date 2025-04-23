using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using System.Reflection;
using Application.Common.Behaviors; // Hoặc namespace đúng của bạn

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AutoMapper;
using Application.Common.Mappings;
using FluentValidation;
using MediatR;
namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ModuleMappingProfile());
            }).CreateMapper());

            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            //services.AddScoped<IPermissionService, PermissionService>();

            //services.AddScoped<ISieveProcessor, SieveProcessor>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            //services.AddScoped<IAmazonS3Service, AmazonS3Service>();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
