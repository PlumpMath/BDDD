﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using BDDD.Interception;
using BDDD.Application;

namespace BDDD.ObjectContainer
{
    public abstract class ObjectContainer:IObjectContainer
    {
        #region 变量

        private readonly IInterceptorSelector interceptorSelector = new InterceptorSelector();
        private readonly ProxyGenerator proxyGenerator = new ProxyGenerator();
        private readonly ProxyGenerationOptions proxyGenerationOptions;

        #endregion

        public ObjectContainer()
        {
            proxyGenerationOptions = new ProxyGenerationOptions { Selector = interceptorSelector };
        }

        private object GetProxyObject(Type targetType, object targetObject)
        {
            IInterceptor[] interceptors = AppRuntime.Instance.CurrentApplication.Interceptors.ToArray();

            if (interceptors == null ||
                interceptors.Length == 0)
                return targetObject;

            if (targetType.IsInterface)
            {
                object obj = null;
                ProxyGenerationOptions proxyGenerationOptionsForInterface = new ProxyGenerationOptions();
                proxyGenerationOptionsForInterface.Selector = interceptorSelector;
                Type targetObjectType = targetObject.GetType();
                if (targetObjectType.IsDefined(typeof(BaseTypeForInterfaceProxyAttribute), false))
                {
                    BaseTypeForInterfaceProxyAttribute baseTypeForIPAttribute = targetObjectType.GetCustomAttributes(typeof(BaseTypeForInterfaceProxyAttribute), false)[0] as BaseTypeForInterfaceProxyAttribute;
                    proxyGenerationOptionsForInterface.BaseTypeForInterfaceProxy = baseTypeForIPAttribute.BaseType;
                }
                if (targetObjectType.IsDefined(typeof(AdditionalInterfaceToProxyAttribute), false))
                {
                    List<Type> intfTypes = targetObjectType.GetCustomAttributes(typeof(AdditionalInterfaceToProxyAttribute), false)
                                                           .Select(p =>
                                                           {
                                                               AdditionalInterfaceToProxyAttribute attrib = p as AdditionalInterfaceToProxyAttribute;
                                                               return attrib.InterfaceType;
                                                           }).ToList();
                    obj = proxyGenerator.CreateInterfaceProxyWithTarget(targetType, intfTypes.ToArray(), targetObject, proxyGenerationOptionsForInterface, interceptors);
                }
                else
                    obj = proxyGenerator.CreateInterfaceProxyWithTarget(targetType, targetObject, proxyGenerationOptionsForInterface, interceptors);
                return obj;
            }
            else
                return proxyGenerator.CreateClassProxyWithTarget(targetType, targetObject, proxyGenerationOptions, interceptors);
        }

        protected abstract T DoGetService<T>() where T : class;
        protected abstract object DoGetService(Type serviceType);
        protected abstract T DoGetRealObjectContainer<T>() where T : class;

        #region IObjectContainer接口

        public T GetService<T>() where T : class
        {
             T serviceImpl =DoGetService<T>();
             return GetProxyObject(typeof(T), serviceImpl) as T;
        }

        public object GetService(Type serviceType)
        {
            object serviceImpl = DoGetService(serviceType);
            return GetProxyObject(serviceType, serviceImpl);
        }

        public T GetRealObjectContainer<T>() where T:class
        {
           T serviceImpl = DoGetRealObjectContainer<T>();
           return GetProxyObject(typeof(T), serviceImpl) as T;
        }

        #endregion



    }
}
