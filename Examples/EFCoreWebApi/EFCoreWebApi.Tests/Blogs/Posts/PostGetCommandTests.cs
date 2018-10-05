﻿using EFCoreWebApi.Blogs;
using EFCoreWebApi.Blogs.Posts;
using EFCoreWebApi.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFCoreWebApi.Tests.Blogs
{
    [TestClass]
    public class PostGetCommandTests : BaseTest
    {
        [TestMethod]
        public void NotFound()
        {
            Assert.ThrowsException<ItemNotFoundException>(() => new PostGetCommand(dbContext).Execute(1, 1111111));
        }

        [TestMethod]
        public void Execute()
        {
            var results = new PostGetCommand(dbContext).Execute(1, 1);

            //Full map is tested in Search command
            Assert.AreEqual(1, results.Id);
        }
    }
}
