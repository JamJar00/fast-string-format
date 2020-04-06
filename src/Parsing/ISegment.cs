using System.Linq.Expressions;
using System.Reflection;

namespace FastStringFormat.Parsing
{
    internal interface ISegment
    {
        string? Param { get; }

        Expression ToExpression<T>(IParameterProvider<T> parameterProvider, Expression formatProviderExpression);
    }
}
