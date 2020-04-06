using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FastStringFormat.Parsing
{
    internal class ParamSegment : ISegment
    {
        public string Param { get; }

        public ParamSegment(string param)
        {
            this.Param = param;
        }

        public Expression ToExpression<T>(IParameterProvider<T> parameterProvider, Expression _)
        {
            Expression parameter = parameterProvider.GetParameter(Param);

            Expression stringified;
            if (parameter.Type == typeof(string))
            {
                stringified = parameter;
            }
            else
            {
                MethodInfo toStringMethod = parameter.Type.GetMethod("ToString", new Type[0]);
                if (toStringMethod == null)
                    throw new FormatStringSyntaxException($"Property '{Param}' does not return a type with a ToString method on it. Is it an interface?");

                stringified = Expression.Call(parameter, toStringMethod);
            }

            return parameterProvider.WrapWithNullCheck(parameter, stringified);
        }
    }
}
