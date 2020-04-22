using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace PTELab.WebApi.IntegrationTests
{
    public class CompanyApiTest : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;

        public CompanyApiTest(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async void GetAllCompanies_Unathorized()
        {
            // Arrange
            var request = new
            {
                Url = "/company"
            };

            // Act
            _client.DefaultRequestHeaders.Clear();
            var response = await _client.GetAsync(request.Url);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async void GetAllCompanies_Authorized()
        {
            // Arrange
            var request = new
            {
                Url = "/company"
            };

            // Act
            var byteArray = Encoding.ASCII.GetBytes("admin:admin");
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var response = await _client.GetAsync(request.Url);
            _client.DefaultRequestHeaders.Clear();

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Theory]
        [InlineData("admin", "1234", StatusCodes.Status401Unauthorized)]
        [InlineData("1234", "1234", StatusCodes.Status401Unauthorized)]
        public async void GetAllCompanies_AuthorizeTest(string userName, string password, int expectedStatus)
        {
            // Arrange
            var request = new
            {
                Url = "/company"
            };

            // Act
            var byteArray = Encoding.ASCII.GetBytes($"{userName}:{password}");
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var response = await _client.GetAsync(request.Url);
            _client.DefaultRequestHeaders.Clear();

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(expectedStatus);
        }
    }
}
