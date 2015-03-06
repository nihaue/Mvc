// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if ASPNET50
using System;
using System.Threading.Tasks;
using AutofacWebSite;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.TestHost;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class DependencyResolverTests
    {
        [Theory]
        [InlineData("http://localhost/di", "<p>Builder Output: Hello from builder.</p>")]
        [InlineData("http://localhost/basic", "<p>Hello From Basic View</p>")]
        public async Task AutofacDIContainerCanUseMvc(string url, string expectedResponseBody)
        {
            // Arrange
            Action<IApplicationBuilder> app = new Startup().Configure;

            // Act & Assert (does not throw)
            // This essentially calls into the Startup.Configuration method
            var server = TestServer.Create(app, AddServices);

            // Make a request to start resolving DI pieces
            var response = await server.CreateClient().GetAsync(url);

            var actualResponseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedResponseBody, actualResponseBody);
        }

        private static void AddServices(IServiceCollection services)
        {
            TestHelper.AddServices(services, nameof(AutofacWebSite));
        }
    }
}
#endif
