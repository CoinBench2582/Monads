namespace Monads
{
    /// <summary>
    /// An exception that is thrown when one of the type arguments provided is not valid in that context.
    /// </summary>
    /// <remarks>Inherited from <see cref="ArgumentException"/> with no extended behaviour</remarks>
    [Serializable]
    public sealed class TypeArgumentException : ArgumentException
    {
        /// <inheritdoc cref="ArgumentException()"/>
        public TypeArgumentException() { }
        /// <inheritdoc cref="ArgumentException(string)"/>
        public TypeArgumentException(string message) : base(message) { }
        /// <inheritdoc cref="ArgumentException(string, Exception)"/>
        public TypeArgumentException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc cref="ArgumentException(string, string)"/>
        public TypeArgumentException(string message, string paramName) : base(message, paramName) { }
        /// <inheritdoc cref="ArgumentException(string, string, Exception)"/>
        public TypeArgumentException(string message, string paramName, Exception inner) : base(message, paramName, inner) { }
    }
}
