namespace Monads
{
    /// <summary>
    /// Thrown by <see cref="Result{TValue, TError}"/>
    /// and other associated classes.
    /// </summary>
    /// <remarks>
    /// Inherited from <see cref="Exception"/> with no extended behaviour.
    /// This class cannot be inherited.
    /// </remarks>
    [Serializable]
	public sealed class ResultException : Exception
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultException"/>.
        /// </summary>
		public ResultException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultException"/>
        /// with the specified error <paramref name="message"/>.
        /// </summary>
		public ResultException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultException"/>
        /// with the specified error <paramref name="message"/>
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
		public ResultException(string message, Exception inner) : base(message, inner) { }
	}
}
