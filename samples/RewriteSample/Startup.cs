﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;

namespace RewriteSample
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment hostingEnvironment)
        {
            var options = new RewriteOptions()
                //.AddRedirect("prefix/(.*)/$", "$1")
                //.AddRedirect("prefix/(.*)/$", "$1")
                //.AddRewrite(@"app/(\d+)", "app?id=$1");
                .AddRewrite(@"/(.*)/(\d+)", "products/$1?id=$2","/products");
                //.AddRewrite(@"app2/(\d+)", "app2?id=$1");
                //.AddRedirect("suffix/(.*)/$", "$1","/prefix2")
                //.AddRewrite(@"app/(\d+)", "app?id=$1");
                //.AddRedirectToHttps(302)
                //.AddIISUrlRewrite(hostingEnvironment, "UrlRewrite.xml")
                //.AddApacheModRewrite(hostingEnvironment, "Rewrite.txt");

            app.UseRewriter(options);


            //////Example of another way to use prefixes. In this case we dont have to set prefix in the options class.
            //app.UseWhen(context => context.Request.Path.StartsWithSegments("/prefix"), branch =>
            //{
            //   branch.UseRewriter(options);
            //});

            app.Run(context => context.Response.WriteAsync($"Rewritten Url: {context.Request.Path + context.Request.QueryString} \n"));
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .Build();

            host.Run();
        }
    }
}
