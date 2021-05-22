using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Data.SqlClient;
using CloudPic.Web;

namespace CloudPic.Tests.IntegrationTests
{
    public class _BaseIntegrationTest
    {
        private readonly TestServer testServer;

        protected readonly IConfiguration _configuration;
        protected readonly JsonSerializerOptions _jsonSerializerOptions;

        protected readonly HttpClient _client;

        public _BaseIntegrationTest()
        {
            testServer = new TestServer(new WebHostBuilder()
                        .ConfigureAppConfiguration((context, builder) =>
                        { builder.AddJsonFile("appsettings.json"); })
                        .UseStartup<Startup>());

            _client = testServer.CreateClient();

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var config = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();
            _configuration = config;
        }
    }
}