using System.Linq.Expressions;
using System.Reflection;

namespace FastStringFormat.Parsing
{
    internal class TextSegment : ISegment
    {
        private string text;

        public TextSegment(string text)
        {
            this.text = text;
        }

        public Expression ToExpression<T>(ParameterExpression parameter, BindingFlags bindingFlags, Expression formatProviderExpression)
        {
            return Expression.Constant(text);
        }
    }
}