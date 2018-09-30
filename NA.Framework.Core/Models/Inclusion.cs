using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NA.Framework.Core.Models
{
    public class Inclusion<T>
    {
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        public List<string> IncludeStrings { get; } = new List<string>();

        public Inclusion<T> AddInclude(Expression<Func<T, object>> expression)
        {
            Includes.Add(expression);
            return this;
        }

        public Inclusion<T> AddInclude(string includeStr)
        {
            IncludeStrings.Add(includeStr);
            return this;
        }
    }
}
