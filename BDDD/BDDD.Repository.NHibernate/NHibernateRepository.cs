﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BDDD.Repository.NHibernate.Properties;
using BDDD.Specification;
using NHibernate;
using NHibernate.Linq;

namespace BDDD.Repository.NHibernate
{
    public class NHibernateRepository<TAggregateRoot> : Repository<TAggregateRoot> where TAggregateRoot : class
    {
        private readonly ISession session;

        public NHibernateRepository(IRepositoryContext context)
            : base(context)
        {
            if (context is INHibernateContext)
                session = (context as INHibernateContext).Session;
            else
                throw new RepositoryException(Resource.EX_INVALID_CONTEXT_TYPE);
        }

        #region 实现Repository中的抽象方法

        protected override void DoAdd(TAggregateRoot aggregateRoot)
        {
            Context.RegisterNew(aggregateRoot);
        }

        protected override bool DoExists(ISpecification<TAggregateRoot> specification)
        {
            return DoGetSignal(specification) != null;
        }

        protected override void DoRemove(TAggregateRoot aggregateRoot)
        {
            Context.RegisterDeleted(aggregateRoot);
        }

        protected override void DoUpdate(TAggregateRoot aggregateRoot)
        {
            Context.RegisterModified(aggregateRoot);
        }

        protected override TAggregateRoot DoGetByKey(object key)
        {
            return session.Get<TAggregateRoot>(key);
        }

        protected override TAggregateRoot DoGetSignal(ISpecification<TAggregateRoot> specification)
        {
            return session.Query<TAggregateRoot>().Where(specification.GetExpression()).FirstOrDefault();
        }

        protected override IEnumerable<TAggregateRoot> DoGetAll()
        {
            return session.QueryOver<TAggregateRoot>().List();
        }

        protected override IEnumerable<TAggregateRoot> DoGetAll(ISpecification<TAggregateRoot> specification)
        {
            return session.Query<TAggregateRoot>().Where(specification.GetExpression());
        }

        protected override IEnumerable<TAggregateRoot> DoGetAll(ISpecification<TAggregateRoot> specification,
                                                                int pageNumber, int pageSize,
                                                                Expression<Func<TAggregateRoot, object>> sortPredicate,
                                                                SortOrder sortOrder)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "PageNumber必须大于0");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "pageSize必须大于0");
            IQueryable<TAggregateRoot> query = session.Query<TAggregateRoot>().Where(specification.GetExpression());
            int skip = (pageNumber - 1) * pageSize;
            int take = pageSize;
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    if (sortPredicate != null)
                        query = query.OrderBy(sortPredicate).Skip(skip).Take(take);
                    break;
                case SortOrder.Descending:
                    if (sortPredicate != null)
                        query = query.OrderByDescending(sortPredicate).Skip(skip).Take(take);
                    break;
            }
            return query.ToList();
        }

        protected override IEnumerable<TAggregateRoot> DoGetAll(Expression<Func<TAggregateRoot, bool>> specification)
        {
            return session.Query<TAggregateRoot>().Where(specification);
        }

        protected override IEnumerable<TAggregateRoot> DoGetAll(Expression<Func<TAggregateRoot, bool>> specification,
                                                                int pageNumber, int pageSize,
                                                                Expression<Func<TAggregateRoot, object>> sortPredicate,
                                                                SortOrder sortOrder)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "PageNumber必须大于0");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "pageSize必须大于0");
            IQueryable<TAggregateRoot> query = session.Query<TAggregateRoot>().Where(specification);
            int skip = (pageNumber - 1) * pageSize;
            int take = pageSize;
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    if (sortPredicate != null)
                        query = query.OrderBy(sortPredicate).Skip(skip).Take(take);
                    break;
                case SortOrder.Descending:
                    if (sortPredicate != null)
                        query = query.OrderByDescending(sortPredicate).Skip(skip).Take(take);
                    break;
            }
            return query.ToList();
        }

        protected override IEnumerable<TAggregateRoot> DoGetAll(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "PageNumber必须大于0");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "pageSize必须大于0");
            IQueryable<TAggregateRoot> query = session.Query<TAggregateRoot>();
            int skip = (pageNumber - 1) * pageSize;
            int take = pageSize;
            return query.Skip(skip).Take(take).ToList();
        }

        protected override IEnumerable<TAggregateRoot> DoGetAll(int pageNumber, int pageSize,
                                                                Expression<Func<TAggregateRoot, object>> sortPredicate,
                                                                SortOrder sortOrder)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "PageNumber必须大于0");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "pageSize必须大于0");
            IQueryable<TAggregateRoot> query = session.Query<TAggregateRoot>();
            int skip = (pageNumber - 1) * pageSize;
            int take = pageSize;
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    if (sortPredicate != null)
                        query = query.OrderBy(sortPredicate).Skip(skip).Take(take);
                    break;
                case SortOrder.Descending:
                    if (sortPredicate != null)
                        query = query.OrderByDescending(sortPredicate).Skip(skip).Take(take);
                    break;
            }
            return query.ToList();
        }

        #endregion
    }
}