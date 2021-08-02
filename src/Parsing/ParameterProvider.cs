using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FastStringFormat.Parsing
{
    internal interface IParameterProvider<T>
    {
        Expression GetParameter(string param);
        Expression WrapWithNullCheck(Expression nullableExpression, Expression processedExpression);
    }

    /// <summary>
    /// Helper for getting parameters.
    /// </summary>
    internal class ParameterProvider<T> : IParameterProvider<T>
    {
        private readonly ParameterExpression parameter;

        private readonly BindingFlags bindingFlags;

        private readonly NullCheckMode nullCheckMode;

        internal ParameterProvider(ParameterExpression parameter, BindingFlags bindingFlags, NullCheckMode nullCheckMode)
        {
            this.parameter = parameter;
            this.bindingFlags = bindingFlags;
            this.nullCheckMode = nullCheckMode;
        }

        /// <summary>
        /// Returns a parameter with any null checks necesary for the current NullCheckMode.
        /// If the parameter name is a chain of properties, this method returns an expression which represents a chain of get property calls.
        /// </summary>
        /// <param name="param">The parameter to search for. Can be a chain of property names.</param>
        /// <returns>The expression for the parameter.</returns>
        public Expression GetParameter(string param)
        {
            string[] props = param.Split('.');
            Type type = typeof(T);
            Expression callInstance = parameter;

            foreach (var prop in props)
            {
                MethodInfo getMethod = type.GetProperty(prop, bindingFlags)?.GetGetMethod()
                    ?? throw new FormatStringSyntaxException($"Property '{prop}' not found on type '{type}'. Does it have a public get accessor?");

                callInstance = Expression.Call(callInstance, getMethod);
                type = callInstance.Type;
            }

            return callInstance;
        }

        /// <summary>
        /// Wraps an expression in the appropriate null check.
        /// </summary>
        /// <param name="nullableExpression">The expression that may be null that is used in the processedExpression.</param>
        /// <param name="processedExpression">The expression transformed assuming that nullableExpression is not null.</param>
        /// <returns>A null check wrapping the expression</returns>
        public Expression WrapWithNullCheck(Expression nullableExpression, Expression processedExpression)
        {
            // If nullableExpression can't actually be null then we can skip the check
            if (!IsNullable(nullableExpression.Type))
                return processedExpression;

            // Select the correct check to use
            switch (nullCheckMode)
            {
                case NullCheckMode.UseEmptyString:
                    // TODO if this parameter is used more than once we should probably assign this to a variable instead
                    // TODO this calls the get method multiple times which is inefficient and may not always return the same value
                    return Expression.Condition(
                        Expression.Equal(nullableExpression, Expression.Constant(null)),
                        Expression.Constant(""),
                        processedExpression
                    );

                default:
                    return processedExpression;
            }
        }

        private static bool IsNullable(Type type)
        {
            // Is it a ref type?
            if (!type.IsValueType)
                return true;

            // Is it a Nullable<T>?
            if (Nullable.GetUnderlyingType(type) != null)
                return true;

            // It's a value type
            return false;
        }
    }
}
