using System;
using System.Linq.Expressions;

namespace JMW.Reflection
{
    public static class Linq
    {
        public static string GetPropertyName<T>(Expression<Func<T, object>> propertyLambda)
        {
            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(
                    $"Expression '{propertyLambda}' refers to a method, not a property.");

            return member.Member.Name;
        }
    }
}