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
        internal List<ISegment> Segments { get => segments; }

        List<ISegment> segments = new List<ISegment>();

        public void AddTextSegment(string text)
        {
            segments.Add(new TextSegment(text));
        }

        public void AddParamSegment(string param)
        {
            segments.Add(new ParamSegment(param));
        }

        public void AddFormattedParamSegment(string param, string format)
        {
            segments.Add(new FormattedParamSegment(param, format));
        }
    }
}