using System.Linq.Expressions;
using System.Reflection;

namespace FastStringFormat.Parsing
{
    internal class TextSegment : ISegment
    {
        public string? Param => null;

        private readonly string text;

        public TextSegment(string text)
        {
            this.text = text;
        }

        public Expression ToExpression<T>(IParameterProvider<T> parameterProvider, Expression formatProviderExpression)
        {
            return Expression.Constant(text);
        }
    }
}
