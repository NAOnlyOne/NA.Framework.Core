using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NA.Framework.Core.Models
{
    /// <summary>
    /// Where条件构造器
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class WhereBuilder<TEntity> where TEntity : class
    {
        private Expression _linkedExp;
        private readonly IEnumerable<ParameterExpression> _args;
        public static WhereBuilder<TEntity> Base => new WhereBuilder<TEntity>(e => true);

        public WhereBuilder(Expression<Func<TEntity, bool>> baseExp)
        {
            _args = baseExp.Parameters;
            _linkedExp = Expression.Invoke(baseExp, _args);
        }

        public WhereBuilder() : this(e => true) { }

        public static WhereBuilder<TEntity> CreateBase(Expression<Func<TEntity, bool>> baseExp)
        {
            var builder = new WhereBuilder<TEntity>(baseExp);
            return builder;
        }

        public WhereBuilder<TEntity> IfAnd(bool condition, Expression<Func<TEntity, bool>> exp)
        {
            if (condition)
            {
                var invokeExp = Expression.Invoke(exp, _args);
                _linkedExp = Expression.AndAlso(_linkedExp, invokeExp);
            }
            return this;
        }

        public WhereBuilder<TEntity> IfElseAnd(bool condition, Expression<Func<TEntity, bool>> ifTrue, Expression<Func<TEntity, bool>> ifFalse)
        {
            var invokeExp = Expression.Invoke(condition ? ifTrue : ifFalse, _args);
            _linkedExp = Expression.AndAlso(_linkedExp, invokeExp);
            return this;
        }

        public WhereBuilder<TEntity> IfOr(bool condition, Expression<Func<TEntity, bool>> exp)
        {
            if (condition)
            {
                var invokeExp = Expression.Invoke(exp, _args);
                _linkedExp = Expression.OrElse(_linkedExp, invokeExp);
            }
            return this;
        }

        public WhereBuilder<TEntity> IfElseOr(bool condition, Expression<Func<TEntity, bool>> ifTrue, Expression<Func<TEntity, bool>> ifFalse)
        {
            var invokeExp = Expression.Invoke(condition ? ifTrue : ifFalse, _args);
            _linkedExp = Expression.OrElse(_linkedExp, invokeExp);
            return this;
        }

        public Expression<Func<TEntity, bool>> Build()
        {
            var finalExp = Expression.Lambda<Func<TEntity, bool>>(_linkedExp, _args);
            return finalExp;
        }
    }
}
