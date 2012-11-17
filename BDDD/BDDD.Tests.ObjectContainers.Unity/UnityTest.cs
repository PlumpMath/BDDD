﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BDDD.Application;
using BDDD.Config;
using BDDD.ObjectContainer;
using BDDD.Tests.Common.Configuration;
using BDDD.Repository;
using FluentNHibernate.Cfg;
using BDDD.Tests.DomainModel.NHibernateMapper;
using FluentNHibernate.Conventions.Helpers;

namespace BDDD.Tests.ObjectContainers.Unity
{
    [TestClass]
    public class UnityTest
    {
        [TestMethod]
        [Description("从配置文件中获得Ioc注册信息")]
        public void GetContainerFromFile()
        {
            AppRuntime.Create(ConfigHelper.GetAppConfigSource()).Start();

            IRepositoryContext context = ServiceLocator.Instance.GetService<IRepositoryContext>();
            Assert.IsNotNull(context);
        }
    }
}
