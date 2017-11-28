using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceStack.Redis.Tests
{
    [TestClass]
    public class ServiceStackTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var Client = new ServiceStack.Redis.RedisClient("www.ajiao.net", 6379, "123456");
            Client.GetAllKeys();
        }
    }
}
