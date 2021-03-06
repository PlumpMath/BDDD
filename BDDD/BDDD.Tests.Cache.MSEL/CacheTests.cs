﻿using System;
using BDDD.Application;
using BDDD.Cache;
using BDDD.Cache.MSEnterpriseLibrary;
using BDDD.Config;
using BDDD.ObjectContainer;
using BDDD.Tests.Common.Configuration;
using BDDD.Tests.DomainModel;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BDDD.Tests.Cache.MSEL
{
    [TestClass]
    public class CacheTests
    {
        private static App application;

        [ClassInitialize]
        public static void Initial(TestContext context)
        {
            ManualConfigSource configSource = ConfigHelper.GetManualConfigSource();
            application = AppRuntime.Create(configSource);
            application.Start();

            var c = application.ObjectContainer.GetRealObjectContainer<UnityContainer>();
            c.RegisterType<ICache, MSELCache>();
            c.RegisterType<AbsoluteTimeExpiration, MSELAbsoluteTimeExpiration>("SCache",
                                                                               new InjectionConstructor(
                                                                                   TimeSpan.FromSeconds(5)));
            c.RegisterType<AbsoluteTimeExpiration, MSELAbsoluteTimeExpiration>("S1Cache",
                                                                               new InjectionConstructor(
                                                                                   TimeSpan.FromSeconds(15)));
        }

        [TestMethod]
        [Description("测试添加缓存")]
        public void AddCache()
        {
            var c = new Customer("scott1", 10);
            CacheManager.AddCache("test1", c, ServiceLocator.Instance.GetService<AbsoluteTimeExpiration>("SCache"));

            var cachedCustomer = CacheManager.GetCache<Customer>("test1");
            Assert.IsNotNull(cachedCustomer);
            Assert.AreEqual(cachedCustomer.Name, "scott1");
        }

        [TestMethod]
        [Description("测试移除缓存")]
        public void RemoveCache()
        {
            var c = new Customer("scott1", 10);
            CacheManager.AddCache("test1", c,
                                  ServiceLocator.Instance.GetService<AbsoluteTimeExpiration>("SCache"));

            var cachedCustomer = CacheManager.GetCache<Customer>("test1");
            Assert.IsNotNull(cachedCustomer);
            Assert.AreEqual(cachedCustomer.Name, "scott1");

            CacheManager.RemoveCache("test1");
            var shouldNull = CacheManager.GetCache<Customer>("test1");
            Assert.IsNull(shouldNull);
        }

        [TestMethod]
        [Description("测试缓存的过期策略_绝对时间")]
        public void AddCache_AbsoluteExpiration()
        {
            var c = new Customer("scott1", 10);
            var expiration = ServiceLocator.Instance.GetService<AbsoluteTimeExpiration>("S1Cache");
            CacheManager.AddCache("test1", c, expiration);

            //todo:暂时没有找到好的方法进行单元测试
            //实际在debug的时候确实会过期，但是如果使用sleep运行的
            //话就不会过期

            //int i = 0;
            //while (i++ < 10)
            //{
            //    Customer cachedCustomer = BDDD.Cache.CacheManager.GetCache<Customer>("test1");

            //    Console.WriteLine(DateTime.Now.ToString());
            //    Assert.IsNotNull(cachedCustomer);
            //    Assert.AreEqual<string>(cachedCustomer.Name, "scott1");

            //    Thread.Sleep(1000);
            //}
        }
    }
}