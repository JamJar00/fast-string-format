using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using System.Linq.Expressions;
using System;

namespace FastStringFormat.Parsing.Test
{
    [TestClass]
    public class ParamSegmentTest
    {
        [TestMethod]
        public void TestToExpressionWithString()
        {
            // GIVEN expressions for parameters
            Mock<IParameterProvider<object>> parameterProvider = new Mock<IParameterProvider<object>>();
            Expression formatProvider = Expression.Constant(null);

            // AND the parameter provider can find the property
            Expression stringExpression = Expression.Constant("I am a string!");
            parameterProvider.Setup(p => p.GetParameter("StringProperty")).Returns(stringExpression);

            // AND the parameter provider wraps the expression in a null check
            Expression wrappedStringExpression = Expression.Constant("I am a wrapped string!");
            parameterProvider.Setup(p => p.WrapWithNullCheck(stringExpression, stringExpression)).Returns(wrappedStringExpression);

            // WHEN the param segment is converted to an expression
            ParamSegment paramSegment = new ParamSegment("StringProperty");
            Expression result = paramSegment.ToExpression(parameterProvider.Object, formatProvider);

            // THEN the expression is that returned by the parameter provider
            Assert.AreSame(result, wrappedStringExpression);
        }

        [TestMethod]
        public void TestToExpressionWithObject()
        {
            // GIVEN expressions for parameters
            Mock<IParameterProvider<object>> parameterProvider = new Mock<IParameterProvider<object>>();
            Expression formatProvider = Expression.Constant(null);

            // AND the parameter provider can find the property
            Expression objectExpression = Expression.Constant(new object());
            parameterProvider.Setup(p => p.GetParameter("ObjectProperty")).Returns(objectExpression);

            // AND the parameter provider wraps the expression in a null check
            Expression wrappedObjectExpression = Expression.Constant(new object());
            parameterProvider.Setup(p => p.WrapWithNullCheck(objectExpression, It.IsAny<Expression>())).Returns(wrappedObjectExpression);

            // WHEN the param segment is converted to an expression
            ParamSegment paramSegment = new ParamSegment("ObjectProperty");
            Expression result = paramSegment.ToExpression(parameterProvider.Object, formatProvider);

            // THEN the expression wrapped was a method call to object.ToString
            object toTest = parameterProvider.Invocations[1].Arguments[1];
            Assert.IsInstanceOfType(toTest, typeof(MethodCallExpression));
            MethodInfo? methodInfo2 = typeof(object).GetMethod("ToString", new Type[0]);
            Assert.AreSame(methodInfo2, ((MethodCallExpression)toTest).Method);

            // AND the expression contains that provided by the parameter provider
            Assert.AreSame(objectExpression, ((MethodCallExpression)toTest).Object);

            // AND the expression returned is the wrapped object expression
            Assert.AreSame(wrappedObjectExpression, result);
        }
    }
}
