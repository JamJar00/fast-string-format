namespace FastStringFormat
{
    /// <summary>
    /// The way null properties in the data object will be handled.
    /// </summary>
    public enum NullCheckMode
    {
        /// <summary>
        /// Indicates that no null checks will be compiled into the format function. ArgumentNullExceptions may be thrown when transforming objects to strings.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the compiled format fucntion will replace null values with an empty string.
        /// </summary>
        UseEmptyString

        // TODO Option to use 'null' string
        // TODO Option to use '<null>' string
        // TODO Option to throw ArgumentNullException
        // TODO Option to just return null
    }
}
