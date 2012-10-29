﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using BDDD.ObjectContainer;
using BDDD.Config;

namespace BDDD.Application
{
    public sealed class App
    {
        private IConfigSource configSource;
        private IObjectContainer objectContainer;
        private List<IInterceptor> interceptors;

        public delegate void AppInitHandle(IConfigSource source, IObjectContainer objectContainer);
        public event AppInitHandle AppInitEvent;

        public App(IConfigSource configSource)
        {
            if (configSource == null)
                throw new ArgumentNullException("configSource 为空");
            if (configSource.Config == null)
                throw new ConfigException("没有定义配置信息");
            if (configSource.Config.ObjectContainer == null)
                throw new ConfigException("当前配置文件中没有定义ObjectContainer信息");
            if (string.IsNullOrEmpty(configSource.Config.ObjectContainer.Provider))
                throw new ConfigException("当前配置文件中没有定义ObjectContainer的Provider信息");
            this.configSource = configSource;

            //从配置文件中加载ObjectContainer
            Type objectContainerType = Type.GetType(configSource.Config.ObjectContainer.Provider);
            if (objectContainerType == null)
                throw new ConfigException("没有找到类型为{0}的ObjectContainer", configSource.Config.ObjectContainer.Provider);
            this.objectContainer = Activator.CreateInstance(objectContainerType) as ObjectContainer.ObjectContainer;

            //从配置文件中加载Interceptors
            interceptors = new List<IInterceptor>();
            if (configSource.Config.Interception != null && configSource.Config.Interception.Interceptors != null)
            {
                foreach (InterceptorElement interceptorElement in configSource.Config.Interception.Interceptors)
                {
                    Type interceptorType = Type.GetType(interceptorElement.Type);
                    if (interceptorType == null)
                        throw new ConfigException("找不到类型为{0}的拦截器", interceptorElement.Type);
                    IInterceptor interceptor = (IInterceptor)Activator.CreateInstance(interceptorType);
                    interceptors.Add(interceptor);
                }
            }

        }

        public IConfigSource ConfigSource
        {
            get { return configSource; }
        }

        public IObjectContainer ObjectContainer
        {
            get { return objectContainer; }
        }

        public IEnumerable<IInterceptor> Interceptors
        {
            get { return interceptors; }
        }

        private void HandleAppInitEvent()
        {
            if (AppInitEvent != null)
            {
                AppInitEvent(configSource, objectContainer);
            }
        }

        public void Start()
        {
            HandleAppInitEvent();
        }
    }
}
