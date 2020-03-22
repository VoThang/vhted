using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Hosting;
using System;

namespace VHTED.Api
{
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            try
            {
                builder
                .UseStartup<Startup>();
            }
            catch (Exception e)
            {
                LambdaLogger.Log("Exception throw in LambdaEntryPoint: " + e);
            }
        }
    }
}
