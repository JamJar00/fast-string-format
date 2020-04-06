using System.Linq;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FastStringFormat.Parsing
{
    internal class FormattedParamSegment : ISegment
    {
        public string Param { get; }

        private readonly string format;

        public FormattedParamSegment(string param, string format)
        {
            this.Param = param;
            this.format = format;
        }

        public Expression ToExpression<T>(IParameterProvider<T> parameterProvider, Expression formatProviderExpression)
        {
            Expression parameter = parameterProvider.GetParameter(Param);

            if (parameter.Type != typeof(IFormattable) && !parameter.Type.GetInterfaces().Contains(typeof(IFormattable)))
                throw new FormatStringSyntaxException($"Property '{Param}' does not return a type implementing IFormattable hence a format string cannot be applied to it.");

            Expression formatExpression = Expression.Constant(format);
            MethodInfo toStringMethod = parameter.Type.GetMethod("ToString", new Type[] { typeof(string), typeof(IFormatProvider) });

            Expression stringified = Expression.Call(parameter, toStringMethod, formatExpression, formatProviderExpression);

            return parameterProvider.WrapWithNullCheck(parameter, stringified);
        }
    }
}
