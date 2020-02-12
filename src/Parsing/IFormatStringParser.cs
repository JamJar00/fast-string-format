using System.Collections.Generic;

namespace FastStringFormat.Parsing
{
    public interface IFormatStringParser
    {
        void ParseFormatString(string formatString, IParsedStringBuilder parsedStringBuilder);
    }
}