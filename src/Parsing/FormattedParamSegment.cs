using System.Linq;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FastStringFormat.Parsing
{
    internal class FormattedParamSegment : ISegment
    {
        private readonly string param;
        private readonly string format;

        public FormattedParamSegment(string param, string format)
        {
            this.param = param;
            this.format = format;
        }

        public Expression ToExpression<T>(ParameterExpression parameter, BindingFlags bindingFlags, Expression formatProviderExpression)
        {
            MethodInfo getMethod = typeof(T).GetProperty(param, bindingFlags)?.GetGetMethod()
                ?? throw new FormatStringSyntaxException($"Property '{param}' not found on type. Does it have a public get accessor?");

            if (getMethod.ReturnType != typeof(IFormattable) && !getMethod.ReturnType.GetInterfaces().Contains(typeof(IFormattable)))
                throw new FormatStringSyntaxException($"Property '{param}' does not return a type implementing IFormattable hence a format string cannot be applied to it.");

            Expression getExpression = Expression.Call(parameter, getMethod);
            Expression formatExpression = Expression.Constant(format);
            MethodInfo toStringMethod = getMethod.ReturnType.GetMethod("ToString", new Type[] { typeof(string), typeof(IFormatProvider) });

            return Expression.Call(getExpression, toStringMethod, formatExpression, formatProviderExpression);
        }
    }
}
