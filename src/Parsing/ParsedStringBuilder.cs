using System.Collections.Generic;

namespace FastStringFormat.Parsing
{
    public interface IParsedStringBuilder
    {
        void AddFormattedParamSegment(string param, string format);
        void AddParamSegment(string param);
        void AddTextSegment(string text);
    }

    internal class ParsedStringBuilder : IParsedStringBuilder
    {
        internal List<ISegment> Segments { get; } = new List<ISegment>();

        public void AddTextSegment(string text)
        {
            Segments.Add(new TextSegment(text));
        }

        public void AddParamSegment(string param)
        {
            Segments.Add(new ParamSegment(param));
        }

        public void AddFormattedParamSegment(string param, string format)
        {
            Segments.Add(new FormattedParamSegment(param, format));
        }
    }
}
