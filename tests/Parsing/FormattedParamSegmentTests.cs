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
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));
            Expression formatProvider = Expression.Constant(null, typeof(IFormatProvider));

            // WHEN the param segment is converted to an expression
            FormattedParamSegment paramSegment = new FormattedParamSegment("IFormattableProperty", "yyyy-MM-dd");
            Expression result = paramSegment.ToExpression<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, formatProvider);

            // THEN the expression is a method call
            Assert.IsInstanceOfType(result, typeof(MethodCallExpression));

            // AND the expression contains the get expression for the property
            Assert.IsInstanceOfType(((MethodCallExpression)result).Object, typeof(MethodCallExpression));
            Assert.AreSame(parameter, ((MethodCallExpression)((MethodCallExpression)result).Object).Object);
            MethodInfo? methodInfo = typeof(TestClass).GetProperty("IFormattableProperty", BindingFlags.Instance | BindingFlags.Public)?.GetGetMethod();
            Assert.AreSame(methodInfo, ((MethodCallExpression)((MethodCallExpression)result).Object).Method);

            // AND the expression contains the ToString method being accessed
            MethodInfo? methodInfo2 = methodInfo?.ReturnType.GetMethod("ToString", new Type[] { typeof(string), typeof(IFormatProvider) });
            Assert.AreSame(methodInfo2, ((MethodCallExpression)result).Method);
            Assert.AreEqual(2, ((MethodCallExpression)result).Arguments.Count);
            Assert.IsInstanceOfType(((MethodCallExpression)result).Arguments[0], typeof(ConstantExpression));
            Assert.AreSame("yyyy-MM-dd", ((ConstantExpression)((MethodCallExpression)result).Arguments[0]).Value);
            Assert.AreSame(formatProvider, ((MethodCallExpression)result).Arguments[1]);
        }

        [TestMethod]
        public void TestToExpressionWithDateTime()
        {
            // GIVEN expressions for parameters
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));
            Expression formatProvider = Expression.Constant(null, typeof(IFormatProvider));

            // WHEN the param segment is converted to an expression
            FormattedParamSegment paramSegment = new FormattedParamSegment("DateTimeProperty", "yyyy-MM-dd");
            Expression result = paramSegment.ToExpression<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, formatProvider);

            // THEN the expression is a method call
            Assert.IsInstanceOfType(result, typeof(MethodCallExpression));

            // AND the expression contains the get expression for the property
            Assert.IsInstanceOfType(((MethodCallExpression)result).Object, typeof(MethodCallExpression));
            Assert.AreSame(parameter, ((MethodCallExpression)((MethodCallExpression)result).Object).Object);
            MethodInfo? methodInfo = typeof(TestClass).GetProperty("DateTimeProperty", BindingFlags.Instance | BindingFlags.Public)?.GetGetMethod();
            Assert.AreSame(methodInfo, ((MethodCallExpression)((MethodCallExpression)result).Object).Method);

            // AND the expression contains the ToString method being accessed
            MethodInfo? methodInfo2 = methodInfo?.ReturnType.GetMethod("ToString", new Type[] { typeof(string), typeof(IFormatProvider) });
            Assert.AreSame(methodInfo2, ((MethodCallExpression)result).Method);
            Assert.AreEqual(2, ((MethodCallExpression)result).Arguments.Count);
            Assert.IsInstanceOfType(((MethodCallExpression)result).Arguments[0], typeof(ConstantExpression));
            Assert.AreSame("yyyy-MM-dd", ((ConstantExpression)((MethodCallExpression)result).Arguments[0]).Value);
            Assert.AreSame(formatProvider, ((MethodCallExpression)result).Arguments[1]);
        }

        [TestMethod]
        public void TestToExpressionWithMissingProperty()
        {
            // GIVEN expressions for parameters
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));
            Expression formatProvider = Expression.Constant(null);

            // WHEN the param segment is converted to an expression with an invalid property name
            // THEN an exception is thrown
            FormattedParamSegment paramSegment = new FormattedParamSegment("NonExistent", "yyyy-MM-dd");
            Assert.ThrowsException<FormatStringSyntaxException>(() =>
                paramSegment.ToExpression<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, formatProvider),
                "Property 'NonExistent' not found on type. Does it have a public get accessor?"
            );
        }

        [TestMethod]
        public void TestToExpressionWithInvalidProperty()
        {
            // GIVEN expressions for parameters
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));
            Expression formatProvider = Expression.Constant(null);

            // WHEN the param segment is converted to an expression with a property that is not IFormattable
            // THEN an exception is thrown
            FormattedParamSegment paramSegment = new FormattedParamSegment("ObjectProperty", "yyyy-MM-dd");
            Assert.ThrowsException<FormatStringSyntaxException>(() =>
                paramSegment.ToExpression<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, formatProvider),
                "Property 'ObjectProperty' does not return a type implementing IFormattable hence a format string cannot be applied to it."
            );
        }

        private class TestClass
        {
            public IFormattable? IFormattableProperty { get; }
            public DateTime DateTimeProperty { get; }
            public object? ObjectProperty { get; }
        }
    }
}
