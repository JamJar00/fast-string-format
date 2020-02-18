using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FastStringFormat.Parsing
{
    internal class ParamSegment : ISegment
    {
        private string param;

        public ParamSegment(string param)
        {
            this.param = param;
        }

        public Expression ToExpression<T>(ParameterExpression parameter, BindingFlags bindingFlags, Expression formatProviderExpression)
        {
            MethodInfo getMethod = typeof(T).GetProperty(param, bindingFlags)?.GetGetMethod()
                ?? throw new FormatStringSyntaxException($"Property '{param}' not found on type '{nameof(T)}'. Does it have a public get accessor?");;

            Expression getExpression = Expression.Call(parameter, getMethod);

            if (getMethod.ReturnType == typeof(string))
            {
                return getExpression;
            }
            else
            {
                MethodInfo toStringMethod = typeof(T).GetMethod("ToString", new Type[0]);
                return Expression.Call(getExpression, toStringMethod);
            }
        }
    }
}