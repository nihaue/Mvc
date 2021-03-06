﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;

namespace ContentNegotiationWebSite
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return new JsonResult("Index Method");
        }

        public User UserInfo()
        {
            return CreateUser();
        }

        [Produces(typeof(User))]
        public IActionResult UserInfo_ProducesWithTypeOnly()
        {
            return new ObjectResult(CreateUser());
        }

        [Produces("application/xml", Type = typeof(User))]
        public IActionResult UserInfo_ProducesWithTypeAndContentType()
        {
            return new ObjectResult(CreateUser());
        }

        private User CreateUser()
        {
            return new User() { Name = "John", Address = "One Microsoft Way" };
        }
    }
}