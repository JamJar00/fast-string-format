using System.Linq.Expressions;
using System.Reflection;

namespace FastStringFormat.Parsing
{
    internal interface ISegment
    {
        Expression ToExpression<T>(ParameterExpression parameter, BindingFlags bindingFlags, Expression formatProviderExpression);
    }
}