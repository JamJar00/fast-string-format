namespace FastStringFormat.Parsing.Parsers
{
    public class DefaultFormatParser : IFormatStringParser
    {
        public void ParseFormatString(string formatString, IParsedStringBuilder parsedStringBuilder)
        {
            int ptr = 0;
            while (ptr < formatString.Length)
            {
                // Seek to open brace
                int openBraceAt = formatString.IndexOf('{', ptr);
                
                // If there was no open brace we've hit the end of the string
                if (openBraceAt == -1)
                {
                    parsedStringBuilder.AddTextSegment(formatString.Substring(ptr));
                    return;
                }

                // Else if there was an open brace then store previous text segment if there was any
                int textSegmentLength = openBraceAt - ptr;
                if (textSegmentLength != 0)
                    parsedStringBuilder.AddTextSegment(formatString.Substring(ptr, textSegmentLength));

                // Seek to close brace and colon
                int closeBraceAt = formatString.IndexOf('}', openBraceAt);
                int colonAt = formatString.IndexOf(':', openBraceAt);

                // If a colon was used before the closing brace consume as a formatted param
                if (colonAt != -1 && colonAt < closeBraceAt)
                {
                    string paramSegment = formatString.Substring(openBraceAt + 1, colonAt - openBraceAt - 1);
                    string formatSegment = formatString.Substring(colonAt + 1, closeBraceAt - colonAt - 1);

                    if (paramSegment.Length == 0)
                        throw new FormatStringSyntaxException($"Empty parameter at position {openBraceAt}.");

                    if (formatSegment.Length == 0)
                        throw new FormatStringSyntaxException($"Empty format at position {openBraceAt}.");

                    parsedStringBuilder.AddFormattedParamSegment(paramSegment, formatSegment);
                    
                    ptr = closeBraceAt + 1;
                    continue;
                }

                // Else if there was no colon just consume the param
                if (closeBraceAt != -1)
                {
                    string paramSegment = formatString.Substring(openBraceAt + 1, closeBraceAt - openBraceAt - 1);
                    
                    if (paramSegment.Length == 0)
                        throw new FormatStringSyntaxException($"Empty parameter at position {openBraceAt}.");
                    
                    parsedStringBuilder.AddParamSegment(paramSegment);
                    
                    ptr = closeBraceAt + 1;
                    continue;
                }

                // No matching close brace *brrrpp*
                throw new FormatStringSyntaxException($"Missing '}}' to match '{{' at position {openBraceAt}.");
            }
        }
    }
}