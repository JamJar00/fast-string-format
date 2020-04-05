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
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));
            Expression formatProvider = Expression.Constant(null);

            // WHEN the param segment is converted to an expression
            ParamSegment paramSegment = new ParamSegment("StringProperty");
            Expression result = paramSegment.ToExpression<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, formatProvider);

            // THEN the expression is a method call
            Assert.IsInstanceOfType(result, typeof(MethodCallExpression));

            // AND the expression contains the correct parameter
            Assert.AreSame(((MethodCallExpression)result).Object, parameter);

            // AND the expression contains the getter being accessed
            MethodInfo? methodInfo = typeof(TestClass).GetProperty("StringProperty", BindingFlags.Instance | BindingFlags.Public)?.GetGetMethod();
            Assert.AreSame(methodInfo, ((MethodCallExpression)result).Method);
        }

        [TestMethod]
        public void TestToExpressionWithObject()
        {
            // GIVEN expressions for parameters
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));
            Expression formatProvider = Expression.Constant(null);

            // WHEN the param segment is converted to an expression
            ParamSegment paramSegment = new ParamSegment("ObjectProperty");
            Expression result = paramSegment.ToExpression<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, formatProvider);

            // THEN the expression is a method call
            Assert.IsInstanceOfType(result, typeof(MethodCallExpression));

            // AND the expression contains the get expression for the property
            Assert.IsInstanceOfType(((MethodCallExpression)result).Object, typeof(MethodCallExpression));
            Assert.AreSame(parameter, ((MethodCallExpression)((MethodCallExpression)result).Object).Object);
            MethodInfo? methodInfo = typeof(TestClass).GetProperty("ObjectProperty", BindingFlags.Instance | BindingFlags.Public)?.GetGetMethod();
            Assert.AreSame(methodInfo, ((MethodCallExpression)((MethodCallExpression)result).Object).Method);

            // AND the expression contains the ToString method being accessed
            MethodInfo? methodInfo2 = methodInfo?.ReturnType.GetMethod("ToString", new Type[0]);
            Assert.AreSame(methodInfo2, ((MethodCallExpression)result).Method);
        }

        [TestMethod]
        public void TestToExpressionWithMissingProperty()
        {
            // GIVEN expressions for parameters
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));
            Expression formatProvider = Expression.Constant(null);

            // WHEN the param segment is converted to an expression with an invalid property name
            // THEN an exception is thrown
            ParamSegment paramSegment = new ParamSegment("NonExistent");
            Assert.ThrowsException<FormatStringSyntaxException>(() =>
                paramSegment.ToExpression<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, formatProvider),
                "Property 'NonExistent' not found on type. Does it have a public get accessor?"
            );
        }

        private class TestClass
        {
            public string? StringProperty { get; }
            public object? ObjectProperty { get; }
        }
    }
}
