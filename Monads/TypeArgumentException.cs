namespace Monads
{
    /// <summary>
    /// An exception that is thrown when one of the type arguments provided is not valid in that context.
    /// </summary>
    /// <remarks>
    /// Inherited from <see cref="ArgumentException"/> with no extended behaviour.
    /// This class cannot be inherited.
    /// </remarks>
    [Serializable]
    public sealed class TypeArgumentException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeArgumentException"/>.
        /// </summary>
        public TypeArgumentException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeArgumentException"/>
        /// with the specified error <paramref name="message"/>.
        /// </summary>
        public TypeArgumentException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeArgumentException"/>
        /// with the specified error <paramref name="message"/>
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public TypeArgumentException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeArgumentException"/>
        /// with the specified error <paramref name="message"/>
        /// and the name of the parameter that caused this exception.
        /// </summary>
        public TypeArgumentException(string message, string paramName) : base(message, paramName) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeArgumentException"/>
        /// with the specified error <paramref name="message"/>,
        /// the name of the parameter that caused this exception,
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public TypeArgumentException(string message, string paramName, Exception inner) : base(message, paramName, inner) { }
    }
}
