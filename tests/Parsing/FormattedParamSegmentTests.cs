using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using System.Linq.Expressions;
using System;

namespace FastStringFormat.Parsing.Test
{
    [TestClass]
    public class FormattedParamSegmentTest
    {
        [TestMethod]
        public void TestToExpressionWithIFormattable()
        {
            // GIVEN expressions for parameters
            Mock<IParameterProvider<object>> parameterProvider = new Mock<IParameterProvider<object>>();
            Expression formatProvider = Expression.Constant(null, typeof(IFormatProvider));

            // AND the parameter provider can find the property
            Expression iformattableExpression = Expression.Constant(DateTime.Now, typeof(IFormattable));
            parameterProvider.Setup(p => p.GetParameter("IFormattableProperty")).Returns(iformattableExpression);

            // AND the parameter provider wraps the expression in a null check
            Expression wrappedIformattableExpression = Expression.Constant(new object());
            parameterProvider.Setup(p => p.WrapWithNullCheck(iformattableExpression, It.IsAny<Expression>())).Returns(wrappedIformattableExpression);

            // WHEN the param segment is converted to an expression
            FormattedParamSegment paramSegment = new FormattedParamSegment("IFormattableProperty", "yyyy-MM-dd");
            Expression result = paramSegment.ToExpression(parameterProvider.Object, formatProvider);

            // THEN the expression wrapped was a method call to the ToString method
            object toTest = parameterProvider.Invocations[1].Arguments[1];
            Assert.IsInstanceOfType(toTest , typeof(MethodCallExpression));
            MethodInfo? methodInfo2 = typeof(IFormattable).GetMethod("ToString", new Type[] { typeof(string), typeof(IFormatProvider) });
            Assert.AreSame(methodInfo2, ((MethodCallExpression)toTest ).Method);
            Assert.AreEqual(2, ((MethodCallExpression)toTest ).Arguments.Count);
            Assert.IsInstanceOfType(((MethodCallExpression)toTest ).Arguments[0], typeof(ConstantExpression));
            Assert.AreSame("yyyy-MM-dd", ((ConstantExpression)((MethodCallExpression)toTest ).Arguments[0]).Value);
            Assert.AreSame(formatProvider, ((MethodCallExpression)toTest ).Arguments[1]);

            // AND the expression contains that returned by the parameter provider
            Assert.AreSame(iformattableExpression, ((MethodCallExpression)toTest ).Object);

            // AND the expression returned is the wrapped object expression
            Assert.AreSame(wrappedIformattableExpression, result);
        }

        [TestMethod]
        public void TestToExpressionWithDateTime()
        {
            // GIVEN expressions for parameters
            Mock<IParameterProvider<object>> parameterProvider = new Mock<IParameterProvider<object>>();
            Expression formatProvider = Expression.Constant(null, typeof(IFormatProvider));

            // AND the parameter provider can find the property
            Expression dateTimeExpression = Expression.Constant(DateTime.Now);
            parameterProvider.Setup(p => p.GetParameter("DateTimeProperty")).Returns(dateTimeExpression);

            // AND the parameter provider wraps the expression in a null check
            Expression wrappedDateTimeExpression = Expression.Constant(new object());
            parameterProvider.Setup(p => p.WrapWithNullCheck(dateTimeExpression, It.IsAny<Expression>())).Returns(wrappedDateTimeExpression);

            // WHEN the param segment is converted to an expression
            FormattedParamSegment paramSegment = new FormattedParamSegment("DateTimeProperty", "yyyy-MM-dd");
            Expression result = paramSegment.ToExpression(parameterProvider.Object, formatProvider);

            // THEN the expression wrapped was a method call to the ToString method
            object toTest = parameterProvider.Invocations[1].Arguments[1];
            Assert.IsInstanceOfType(toTest, typeof(MethodCallExpression));
            MethodInfo? methodInfo2 = typeof(DateTime).GetMethod("ToString", new Type[] { typeof(string), typeof(IFormatProvider) });
            Assert.AreSame(methodInfo2, ((MethodCallExpression)toTest).Method);
            Assert.AreEqual(2, ((MethodCallExpression)toTest).Arguments.Count);
            Assert.IsInstanceOfType(((MethodCallExpression)toTest).Arguments[0], typeof(ConstantExpression));
            Assert.AreSame("yyyy-MM-dd", ((ConstantExpression)((MethodCallExpression)toTest).Arguments[0]).Value);
            Assert.AreSame(formatProvider, ((MethodCallExpression)toTest).Arguments[1]);

            // AND the expression contains that returned by the parameter provider
            Assert.AreSame(dateTimeExpression, ((MethodCallExpression)toTest).Object);

            // AND the expression returned is the wrapped object expression
            Assert.AreSame(wrappedDateTimeExpression, result);
        }

        [TestMethod]
        public void TestToExpressionWithInvalidProperty()
        {
            // GIVEN expressions for parameters
            Mock<IParameterProvider<object>> parameterProvider = new Mock<IParameterProvider<object>>();
            Expression formatProvider = Expression.Constant(null);

            // AND the parameter provider can find the property but it's not IFormattable
            Expression dateTimeExpression = Expression.Constant(new object());
            parameterProvider.Setup(p => p.GetParameter("ObjectProperty")).Returns(dateTimeExpression);

            // WHEN the param segment is converted to an expression with a property that is not IFormattable
            // THEN an exception is thrown
            FormattedParamSegment paramSegment = new FormattedParamSegment("ObjectProperty", "yyyy-MM-dd");
            Assert.ThrowsException<FormatStringSyntaxException>(() =>
                paramSegment.ToExpression(parameterProvider.Object, formatProvider),
                "Property 'ObjectProperty' does not return a type implementing IFormattable hence a format string cannot be applied to it."
            );
        }
    }
}
