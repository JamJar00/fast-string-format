using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using FastStringFormat.Parsing;
using System.Linq.Expressions;

namespace FastStringFormat.Parsing.Test
{
    [TestClass]
    public class TextSegmentTest
    {
        [TestMethod]
        public void TestToExpression()
        {
            // GIVEN unused expressions for parameters
            ParameterExpression parameter = Expression.Parameter(typeof(object));
            Expression formatProvider = Expression.Constant(null);

            // WHEN the text segment is converted to an expression
            TextSegment textSegment = new TextSegment("a string");
            Expression result = textSegment.ToExpression<object>(parameter, default, formatProvider);

            // THEN the expression is a constant
            Assert.IsInstanceOfType(result, typeof(ConstantExpression));

            // AND the expression contains the correct string
            Assert.AreEqual("a string", ((ConstantExpression)result).Value);
        }
    }
}
