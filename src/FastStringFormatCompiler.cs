using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Globalization;
using FastStringFormat.Parsing;
using FastStringFormat.Parsing.Parsers;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FastStringFormat.Test")]

namespace FastStringFormat
{
    public partial class FastStringFormatCompiler
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;

        public IFormatStringParser Parser { get; }

        public FastStringFormatCompiler()
            : this (new DefaultFormatParser())
        {

        }

        public FastStringFormatCompiler(IFormatStringParser parser)
        {
            Parser = parser;
        }

        public Func<T, string> Compile<T>(string? formatString)
        {
            return Compile<T>(formatString, CultureInfo.CurrentCulture);
        }

        // TODO remove nullableness
        public Func<T, string> Compile<T>(string? formatString, IFormatProvider? formatProvider)
        {
            // Guard, since we currently target a version of .NET that doesn't understand nullable reference types
            if (formatString == null)
                throw new ArgumentNullException(nameof(formatString));

            if (formatProvider == null)
                throw new ArgumentNullException(nameof(formatProvider));

            // Parse the string to a set of segments
            IEnumerable<ISegment> segments = ParseFormatString(formatString);

            // Create necessary constant expressions
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            Expression formatProviderExpression = Expression.Constant(formatProvider);

            // Compile segments to constituent expressions
            IEnumerable<Expression> segmentExpressions = segments.Select(s => s.ToExpression<T>(parameter, BINDING_FLAGS, formatProviderExpression));

            // Select the best method of forming the string
            // TODO May be faster to nest concat operations up until a certain point before resorting to allocating an array object in the process. Perhaps a user preference?
            Expression formatExpression;
            if (segments.Count() == 1)
                formatExpression = CompileToSingleton<T>(segmentExpressions.First());
            else if (segments.Count() <= 4)
                formatExpression = CompileToSingleConcat<T>(segmentExpressions);
            else
                formatExpression = CompileToArrayConcat<T>(segmentExpressions);

            // Construct a lambda function from the compiled expression
            return Expression.Lambda<Func<T, string>>(
                formatExpression,
                "FastStringFormatAutogenerated",
                new ParameterExpression[] { parameter }
            ).Compile();
        }

        private IEnumerable<ISegment> ParseFormatString(string formatString)
        {
            ParsedStringBuilder parsedStringBuilder = new ParsedStringBuilder();

            Parser.ParseFormatString(formatString, parsedStringBuilder);

            return parsedStringBuilder.Segments;
        }

        private Expression CompileToSingleton<T>(Expression expression)
        {
            // Most people don't want a null input to return null, they want an empty string instead
            return Expression.Coalesce(expression, Expression.Constant(""));
        }

        private static Expression CompileToSingleConcat<T>(IEnumerable<Expression> segmentExpressions)
        {
            MethodInfo concatMethodInfo = typeof(string).GetMethod("Concat", Enumerable.Repeat(typeof(string), segmentExpressions.Count()).ToArray());

            return Expression.Call(null, concatMethodInfo, segmentExpressions);
        }

        private static Expression CompileToArrayConcat<T>(IEnumerable<Expression> segmentExpressions)
        {
            MethodInfo concatMethodInfo = typeof(string).GetMethod("Concat", new Type[] { typeof(string[]) });
            Expression parameterExpressionArray = Expression.NewArrayInit(typeof(string), segmentExpressions);

            return Expression.Call(null, concatMethodInfo, parameterExpressionArray);
        }
    }
}
