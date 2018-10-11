﻿using EF6AspNetWebApi.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace EF6AspNetWebApi.Web
{
    public class ServiceActivator : IHttpControllerActivator
    {
        public ServiceActivator(HttpConfiguration configuration)
        {
            var services = new ServiceCollection();

            var controllerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(ApiController)));
            foreach (var type in controllerTypes)
            {
                services.AddTransient(type);
            }

            Application.InitializeAndBuildProvider(services);
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var scope = Application.ServiceProvider.CreateScope();
            request.RegisterForDispose(scope);

            return scope.ServiceProvider.GetRequiredService(controllerType) as IHttpController;
        }
    }
}