namespace SecurityBlanket
{
    /// <summary>
    /// Represents one validation failure
    /// </summary>
    public class BlanketError
    {
        /// <summary>
        /// The type of validation failure that occurred
        /// </summary>
        public FailureType Failure { get; internal set; }

        /// <summary>
        /// The object or element that failed validation
        /// </summary>
        public object Value { get; internal set; }

        /// <summary>
        /// The path to get to the object from the root of the validation
        /// </summary>
        public string Path { get; internal set; }
    }
}
