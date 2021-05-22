using CloudPic.Models.VM;
using NPoco;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace CloudPic.Tests.IntegrationTests
{
    public class ViewPermissionsTests : _BaseIntegrationTest, IDisposable
    {
        public void Dispose()
        {
        }

        [Fact]
        public async Task GetExploreShouldReturnOK()
        {
            var response = await _client.GetAsync("explore");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMyPhotosShouldRedirectToLogin()
        {
            var response = await _client.GetAsync("photos");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }

        [Fact]
        public async Task GetAdminIndexShouldRedirectToLogin()
        {
            var response = await _client.GetAsync("admin");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }

        [Fact]
        public async Task LoginShouldReturnOK()
        {
            var user = new LoginUserVM
            {
                 Email = "ivana.loncar@gmail.com",
                 Password = "lozinka"
            };
            var content = new StringContent(user.Serialize, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await _client.PostAsync("account/login", content);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        }

    }
}
