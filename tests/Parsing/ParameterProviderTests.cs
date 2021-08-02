using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastStringFormat.Parsing
{
    [TestClass]
    public class ParameterProviderTests
    {
        [TestMethod]
        public void TestGetParameter()
        {
            // GIVEN an expression for the parameter
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));

            // WHEN a param is requested and the null check mode is set to None
            ParameterProvider<TestClass> parameterProvider = new ParameterProvider<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, NullCheckMode.None);
            Expression result = parameterProvider.GetParameter("StringProperty");

            // THEN the returned expression is a method call
            Assert.IsInstanceOfType(result, typeof(MethodCallExpression));

            // AND the expression contains the correct parameter
            Assert.AreSame(parameter, ((MethodCallExpression)result).Object);

            // AND the expression contains the getter being accessed
            MethodInfo? methodInfo = typeof(TestClass).GetProperty("StringProperty", BindingFlags.Instance | BindingFlags.Public)?.GetGetMethod();
            Assert.AreSame(methodInfo, ((MethodCallExpression)result).Method);
        }

        [TestMethod]
        public void TestGetParameterWithNestedProperty()
        {
            // GIVEN an expression for the parameter
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));

            // WHEN a nested param is requested and the null check mode is set to None
            ParameterProvider<TestClass> parameterProvider = new ParameterProvider<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, NullCheckMode.None);
            Expression result = parameterProvider.GetParameter("NestedTestClassProperty.StringProperty");

            // THEN the returned expression is a method call
            Assert.IsInstanceOfType(result, typeof(MethodCallExpression));

            // AND the expression contains the getter being accessed
            MethodInfo? methodInfo = typeof(TestClass)
                .GetProperty("NestedTestClassProperty", BindingFlags.Instance | BindingFlags.Public)?
                .PropertyType
                .GetProperty("StringProperty", BindingFlags.Instance | BindingFlags.Public)?
                .GetGetMethod();

            Assert.AreSame(methodInfo, ((MethodCallExpression)result).Method);
        }

        [TestMethod]
        public void TestGetParameterWithMissingProperty()
        {
            // GIVEN an expression for the parameter
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));

            // WHEN a param is requested with a property name missing from the object
            // THEN an exception is thrown
            ParameterProvider<TestClass> parameterProvider = new ParameterProvider<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, NullCheckMode.None);
            FormatStringSyntaxException e = Assert.ThrowsException<FormatStringSyntaxException>(() => parameterProvider.GetParameter("NonExistent"));
            Assert.AreEqual("Property 'NonExistent' not found on type 'FastStringFormat.Parsing.ParameterProviderTests+TestClass'. Does it have a public get accessor?", e.Message);
        }

        [TestMethod]
        public void TestGetParameterWithMissingNestedProperty()
        {
            // GIVEN an expression for the parameter
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));

            // WHEN a param is requested with a nested property name missing from the object
            // THEN an exception is thrown
            ParameterProvider<TestClass> parameterProvider = new ParameterProvider<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, NullCheckMode.None);
            FormatStringSyntaxException e = Assert.ThrowsException<FormatStringSyntaxException>(() => parameterProvider.GetParameter("NestedTestClassProperty.NonExistent"));
            Assert.AreEqual("Property 'NonExistent' not found on type 'FastStringFormat.Parsing.ParameterProviderTests+NestedTestClass'. Does it have a public get accessor?", e.Message);
        }


        [TestMethod]
        public void TestWrapWithNullCheckWithNoneNullable()
        {
            // GIVEN an expression for the parameter
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));

            // AND a nullable expression
            Expression nullableExpression = Expression.Constant(10);
            Expression processedExpression = Expression.Constant("Hello");

            // WHEN wrapping a nullable
            ParameterProvider<TestClass> parameterProvider = new ParameterProvider<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, NullCheckMode.None);
            Expression result = parameterProvider.WrapWithNullCheck(nullableExpression, processedExpression);

            // THEN the returned expression is the processedExpression
            Assert.AreSame(processedExpression, result);
        }

        [TestMethod]
        public void TestWrapWithNullCheckWithNoneNullCheckMode()
        {
            // GIVEN an expression for the parameter
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));

            // AND a non-nullable expression
            Expression nullableExpression = Expression.Constant(new object());
            Expression processedExpression = Expression.Constant("Hello");

            // WHEN wrapping a reference type
            ParameterProvider<TestClass> parameterProvider = new ParameterProvider<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, NullCheckMode.None);
            Expression result = parameterProvider.WrapWithNullCheck(nullableExpression, processedExpression);

            // THEN the returned expression is the processedExpression
            Assert.AreSame(processedExpression, result);
        }

        [TestMethod]
        public void TestWrapWithNullCheckWithUseEmptyStringNullCheckModeAndNullable()
        {
            // GIVEN expressions for parameters
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));

            // AND a nullable expression
            Expression nullableExpression = Expression.Constant(false, typeof(bool?));
            Expression processedExpression = Expression.Constant("Hello");

            // WHEN the null check mode is set to UseEmptyString and we pass a nullable value type
            ParameterProvider<TestClass> parameterProvider = new ParameterProvider<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, NullCheckMode.UseEmptyString);
            Expression result = parameterProvider.WrapWithNullCheck(nullableExpression, processedExpression);

            // THEN the returned expression is a conditional
            Assert.IsInstanceOfType(result, typeof(ConditionalExpression));

            // AND the test is an equals comparison against null
            Expression test = ((ConditionalExpression)result).Test;
            Assert.IsInstanceOfType(test, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.Equal, ((BinaryExpression)test).NodeType);

            Assert.AreSame(nullableExpression, ((BinaryExpression)test).Left);

            Assert.IsInstanceOfType(((BinaryExpression)test).Right, typeof(ConstantExpression));
            Assert.IsNull(((ConstantExpression)((BinaryExpression)test).Right).Value);

            // AND the true branch is a constant of an empty string
            Expression trueBranch = ((ConditionalExpression)result).IfTrue;
            Assert.IsInstanceOfType(trueBranch, typeof(ConstantExpression));
            Assert.AreEqual("", ((ConstantExpression)trueBranch).Value);

            // AND the false branch is the processed expression
            Expression falseBranch = ((ConditionalExpression)result).IfFalse;
            Assert.AreSame(processedExpression, falseBranch);
        }

        [TestMethod]
        public void TestWrapWithNullCheckWithUseEmptyStringNullCheckModeAndReferenceType()
        {
            // GIVEN expressions for parameters
            ParameterExpression parameter = Expression.Parameter(typeof(TestClass));

            // AND a nullable expression
            Expression nullableExpression = Expression.Constant(new object());
            Expression processedExpression = Expression.Constant("Hello");

            // WHEN the null check mode is set to UseEmptyString and we pass a referenec type
            ParameterProvider<TestClass> parameterProvider = new ParameterProvider<TestClass>(parameter, BindingFlags.Instance | BindingFlags.Public, NullCheckMode.UseEmptyString);
            Expression result = parameterProvider.WrapWithNullCheck(nullableExpression, processedExpression);

            // THEN the returned expression is a conditional
            Assert.IsInstanceOfType(result, typeof(ConditionalExpression));

            // AND the test is an equals comparison against null
            Expression test = ((ConditionalExpression)result).Test;
            Assert.IsInstanceOfType(test, typeof(BinaryExpression));
            Assert.AreEqual(ExpressionType.Equal, ((BinaryExpression)test).NodeType);

            Assert.AreSame(nullableExpression, ((BinaryExpression)test).Left);

            Assert.IsInstanceOfType(((BinaryExpression)test).Right, typeof(ConstantExpression));
            Assert.IsNull(((ConstantExpression)((BinaryExpression)test).Right).Value);

            // AND the true branch is a constant of an empty string
            Expression trueBranch = ((ConditionalExpression)result).IfTrue;
            Assert.IsInstanceOfType(trueBranch, typeof(ConstantExpression));
            Assert.AreEqual("", ((ConstantExpression)trueBranch).Value);

            // AND the false branch is the processed expression
            Expression falseBranch = ((ConditionalExpression)result).IfFalse;
            Assert.AreSame(processedExpression, falseBranch);
        }

        private class NestedTestClass
        {
            public string? StringProperty { get; }
        }

        private class TestClass
        {
            public string? StringProperty { get; }
            public bool BooleanProperty { get; }
            public bool? NullableProperty { get; }
            public NestedTestClass? NestedTestClassProperty { get; }
        }
    }
}
