﻿using System;
using System.Linq.Expressions;

namespace JMW.Reflection;

public static class Linq
{
    public static string GetPropertyName<T>(Expression<Func<T, object>> propertyLambda)
    {
        return propertyLambda.Body is not MemberExpression member
            ? throw new ArgumentException(
                $"Expression '{propertyLambda}' refers to a method, not a property.")
            : member.Member.Name;
    }
}