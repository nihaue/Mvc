﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class ModelMetadataAttributeTest
    {
        private readonly IServiceProvider _services = TestHelper.CreateServices(nameof(ValidationWebSite));
        private readonly Action<IApplicationBuilder> _app = new ValidationWebSite.Startup().Configure;

        [Fact]
        public async Task ValidBaseClass_Product()
        {
            // Arrange
            var server = TestServer.Create(_services, _app);
            var client = server.CreateClient();
            var input = "{ \"Name\": \"MVC\", \"Contact\":\"4258959019\", \"Category\":\"Technology\"," +
                "\"CompanyName\":\"Microsoft\", \"Country\":\"USA\",\"Price\": 21, \"ProductDetails\": {\"Detail1\": \"d1\"," +
                " \"Detail2\": \"d2\", \"Detail3\": \"d3\"}}";
            var content = new StringContent(input, Encoding.UTF8, "application/json");

            var url =
                "http://localhost/ModelMetadataTypeValidation/ValidateProductViewModelInclMetadata";

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("{}", body);
        }

        [Fact]
        public async Task InvalidPropertiesAndSubPropertiesOnBaseClass_Product()
        {
            // Arrange
            var server = TestServer.Create(_services, _app);
            var client = server.CreateClient();
            var input = "{ \"Price\": 2, \"ProductDetails\": {\"Detail1\": \"d1\"}}";
            var content = new StringContent(input, Encoding.UTF8, "application/json");

            var url =
                "http://localhost/ModelMetadataTypeValidation/ValidateProductViewModelInclMetadata";

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            Assert.Equal(6, json.Count);
            Assert.Equal("CompanyName cannot be null", json["product.CompanyName"]);
            Assert.Equal("The field Price must be between 20 and 100.", json["product.Price"]);
            Assert.Equal("The Category field is required.", json["product.Category"]);
            Assert.Equal("The ContactUs field is required.", json["product.Contact"]);
            Assert.Equal("The Detail2 field is required.", json["product.ProductDetails.Detail2"]);
            Assert.Equal("The Detail3 field is required.", json["product.ProductDetails.Detail3"]);
        }

        [Fact]
        public async Task InvalidComplexTypePropertyOnBaseClass_Product()
        {
            // Arrange
            var server = TestServer.Create(_services, _app);
            var client = server.CreateClient();
            var input = "{ \"Contact\":\"4255678765\", \"Category\":\"Technology\"," +
                "\"CompanyName\":\"Microsoft\", \"Country\":\"USA\",\"Price\": 21 }";
            var content = new StringContent(input, Encoding.UTF8, "application/json");

            var url =
                "http://localhost/ModelMetadataTypeValidation/ValidateProductViewModelInclMetadata";

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            Assert.Equal(1, json.Count);
            Assert.Equal("The ProductDetails field is required.", json["product.ProductDetails"]);
        }

        [Fact]
        public async Task InvalidClassAttributeOnBaseClass_Product()
        {
            // Arrange
            var server = TestServer.Create(_services, _app);
            var client = server.CreateClient();
            var input = "{ \"Contact\":\"4258959019\", \"Category\":\"Technology\"," +
                "\"CompanyName\":\"Microsoft\", \"Country\":\"UK\",\"Price\": 21, \"ProductDetails\": {\"Detail1\": \"d1\"," +
                " \"Detail2\": \"d2\", \"Detail3\": \"d3\"}}";

            var content = new StringContent(input, Encoding.UTF8, "application/json");

            var url =
                "http://localhost/ModelMetadataTypeValidation/ValidateProductViewModelInclMetadata";

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            Assert.Equal(1, json.Count);
            Assert.Equal("Country and Name fields don't have the right values", json["product"]);
        }

        [Fact]
        public async Task ValidDerivedClass_Software()
        {
            // Arrange
            var server = TestServer.Create(_services, _app);
            var client = server.CreateClient();
            var input = "{ \"Name\": \"MVC\", \"Contact\":\"4258959019\", \"Category\":\"Technology\"," +
                "\"CompanyName\":\"Microsoft\", \"Country\":\"USA\", \"Version\":\"2\"," +
                "\"DatePurchased\": \"/Date(1297246301973)/\", \"Price\" : \"110\" }";
            var content = new StringContent(input, Encoding.UTF8, "application/json");

            var url =
                "http://localhost/ModelMetadataTypeValidation/ValidateSoftwareViewModelInclMetadata";

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal("{}", body);
        }

        [Fact]
        public async Task InvalidPropertiesOnDerivedClass_Software()
        {
            // Arrange
            var server = TestServer.Create(_services, _app);
            var client = server.CreateClient();

            var url =
                "http://localhost/ModelMetadataTypeValidation/ValidateSoftwareViewModelInclMetadata";
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var version = "2.2";
            request.Headers.TryAddWithoutValidation("version", version);

            var input = "{ \"Name\": \"MVC\", \"Contact\":\"425-895-9019\", \"Category\":\"Technology\"," +
                "\"CompanyName\":\"Microsoft\", \"Country\":\"USA\",\"Price\": 2}";

            request.Content = new StringContent(input, Encoding.UTF8, "application/json");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            Assert.Equal(2, json.Count);
            Assert.Equal("The field Price must be between 100 and 200.", json["software.Price"]);
            Assert.Equal("The field Contact must be a string with a maximum length of 10.", json["software.Contact"]);
        }

        [Fact]
        public async Task InvalidClassAttributeOnBaseClass_Software()
        {
            // Arrange
            var server = TestServer.Create(_services, _app);
            var client = server.CreateClient();
            var input = "{ \"Contact\":\"4258959019\", \"Category\":\"Technology\"," +
                "\"CompanyName\":\"Microsoft\", \"Country\":\"UK\",\"Version\":\"2\"," +
                "\"DatePurchased\": \"/Date(1297246301973)/\", \"Price\" : \"110\" }";
            var content = new StringContent(input, Encoding.UTF8, "application/json");

            var url =
                "http://localhost/ModelMetadataTypeValidation/ValidateSoftwareViewModelInclMetadata";

            // Act
            var response = await client.PostAsync(url, content);

            // Assert
            var body = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            Assert.Equal(1, json.Count);
            Assert.Equal("Country and Name fields don't have the right values", json["software"]);
        }

    }
}