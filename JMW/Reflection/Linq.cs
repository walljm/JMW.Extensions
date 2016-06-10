using System;
using System.Linq.Expressions;

namespace JMW.Reflection
{
    public static class Linq
    {
        public static string GetPropertyName<T>(Expression<Func<T, object>> propertyLambda)
        {
            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            return member.Member.Name;
        }
    }
}