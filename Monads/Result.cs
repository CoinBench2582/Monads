namespace Monads
{
    /// <summary>
    /// A monad that represents a value that is the result of an operation that may fail.
    /// The result may contain either a value or an exception.
    /// This is useful for composing operations that may fail, building a chain of operations that can be unwrapped at the end.
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="TError">The type of the exception</typeparam>
    public class Result<T, TError>
        where T : class
        where TError : Exception
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Nedeklarujte viditelná pole instance.", Justification = "Protected field is not externally visible.")]
        protected readonly T? _value;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Nedeklarujte viditelná pole instance.", Justification = "Protected field is not externally visible.")]
        protected readonly TError? _error;

        protected Result(T value) => _value = value;
        protected Result(TError exception) => _error = exception;

        /// <summary>
        /// Returns true if the result is Ok
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the result is Ok.
        /// <see langword="false"/> otherwise.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(_value))]
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, nameof(_error))]
        public bool IsOk => _error is null;

        /// <summary>
        /// Returns true if the result is Error
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the result is Error.
        /// <see langword="false"/> otherwise.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(_error))]
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, nameof(_value))]
        public bool IsError => _error is not null;

        /// <summary>
        /// Unwraps the result, throwing an exception if the result is an error
        /// </summary>
        /// <exception cref="TError">The error that was wrapped in the result</exception>
        /// <returns>The value that was wrapped in the result</returns>
        public T Unwrap => _value ?? throw _error!;

        /// <summary>
        /// Unwraps the result, throwing an exception with the given message if the result is an error
        /// </summary>
        /// <param name="message">The message to include in the exception</param>
        /// <exception cref="Exception">The exception with the given message</exception>
        public void Expect(string message)
        {
            if (IsError) throw new Exception(message);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the result is Ok and the value satisfies the predicate
        /// </summary>
        /// <param name="predicate">The predicate to test the value against</param>
        /// <returns>
        /// <see langword="true"/> if the result is Ok and the value satisfies the predicate.
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool IsOkAnd(Func<T, bool> predicate)
            => IsOk && (predicate ?? throw new ArgumentNullException(nameof(predicate))).Invoke(_value);

        /// <summary>
        /// Returns true if the result is Error and the error satisfies the predicate
        /// </summary>
        /// <param name="predicate">The predicate to test the error against</param>
        /// <returns>
        /// <see langword="true"/> if the result is Error and the error satisfies the predicate.
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool IsErrorAnd(Func<TError, bool> predicate)
            => IsError && (predicate ?? throw new ArgumentNullException(nameof(predicate))).Invoke(_error);

        /// <summary>
        /// Returns an Option containing the value. Ok if the result is Ok, otherwise None.
        /// </summary>
        /// <returns>An Option containing the value if the result is Ok, otherwise None</returns>
        public Option<T> Ok => IsOk ? new Some<T>(_value) : new None<T>();

        /// <summary>
        /// Returns an Option containing the error. Some if the result is Error, otherwise None.
        /// </summary>
        /// <returns>An Option containing the error if the result is Error, otherwise None</returns>
        public Option<TError> Error => IsError ? new Some<TError>(_error) : new None<TError>();

        /// <summary>
        /// Maps the value of the result to a new value if the result is Ok, otherwise maps the error to a new Error
        /// </summary>
        /// <param name="operation">The function to map the value to a new value</param>
        /// <typeparam name="TResult">The type of the new value</typeparam>
        /// <returns>A new Result containing the mapped value if the result is Ok, otherwise a new Error containing the mapped error</returns>
        public Result<TResult, TError> Map<TResult>(Func<T, TResult> operation) where TResult : class
            => IsOk
                ? new Ok<TResult, TError>((operation ?? throw new ArgumentNullException(nameof(operation))).Invoke(_value))
                : new Error<TResult, TError>(_error);

        /// <summary>
        /// Maps the value of the result to a new value if the result is Ok, otherwise returns a default value
        /// </summary>
        /// <param name="operation">The function to map the value to a new value</param>
        /// <param name="default">The default value to return if the result is an error</param>
        /// <typeparam name="TResult">The type of the new value</typeparam>
        /// <returns>The mapped value if the result is Ok, otherwise the default value</returns>
        public TResult MapOr<TResult>(Func<T, TResult> operation, TResult @default)
            => IsOk ? (operation ?? throw new ArgumentNullException(nameof(operation))).Invoke(_value) : @default;

        /// <summary>
        /// Maps the value of the result to a new value if the result is Ok, otherwise maps the error to a new value
        /// </summary>
        /// <param name="operation">The function to map the value to a new value</param>
        /// <param name="else">The function to map the error to a new value</param>
        /// <typeparam name="TResult">The type of the new value</typeparam>
        /// <returns>The mapped value if the result is Ok, otherwise the mapped error</returns>
        public TResult MapOrElse<TResult>(Func<T, TResult> operation, Func<TError, TResult> @else)
            => IsOk
                ? (operation ?? throw new ArgumentNullException(nameof(operation))).Invoke(_value)
                : (@else ?? throw new ArgumentNullException(nameof(@else))).Invoke(_error);

        /// <summary>
        /// Maps the error of the result to a new error if the result is Error, otherwise maps the value to a new Ok
        /// </summary>
        /// <param name="operation">The function to map the error to a new error</param>
        /// <typeparam name="TResultError">The type of the new error</typeparam>
        /// <returns>A new Error containing the mapped error if the result is Error, otherwise a new Ok containing the mapped value</returns>
        public Result<T, TResultError> MapError<TResultError>(Func<TError, TResultError> operation) where TResultError : Exception
            => IsOk
                ? new Ok<T, TResultError>(_value)
                : new Error<T, TResultError>((operation ?? throw new ArgumentNullException(nameof(operation))).Invoke(_error));

        /// <summary>
        /// Executes an action on the value if the result is Ok
        /// </summary>
        /// <param name="operation">The action to execute on the value</param>
        /// <returns>Self</returns>
        public Result<T, TError> Inspect(Action<T> operation)
        {
            if (IsOk)
                (operation ?? throw new ArgumentNullException(nameof(operation))).Invoke(_value);
            return this;
        }

        /// <summary>
        /// Executes an action on the error if the result is Error
        /// </summary>
        /// <param name="operation">The action to execute on the error</param>
        /// <returns>Self</returns>
        public Result<T, TError> InspectError(Action<TError> operation)
        {
            if (IsError)
                (operation ?? throw new ArgumentNullException(nameof(operation))).Invoke(_error);
            return this;
        }
    }

    /// <summary>
    /// Represents a successful result
    /// </summary>
    /// <param name="value">Value to return</param>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="TError">The type of the exception</typeparam>
    public class Ok<T, TError>(T value)
        : Result<T, TError>(value ?? throw new ArgumentNullException(nameof(value)))
        where T : class
        where TError : Exception;

    /// <summary>
    /// Represents a failed result
    /// </summary>
    /// <param name="exception">Error to return</param>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="TError">The type of the exception</typeparam>
    public class Error<T, TError>(TError exception)
        : Result<T, TError>(exception ?? throw new ArgumentNullException(nameof(exception)))
        where T : class
        where TError : Exception;

    public static class ResultExtensions
    {
        /// <summary>
        /// Executes a function and wraps the result in Ok if the function executes successfully, otherwise wraps the exception in Error
        /// </summary>
        /// <param name="f">The function to execute</param>
        /// <returns>A new Result containing the result of the function if it executes successfully, otherwise a new Error containing the exception</returns>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <typeparam name="TException">The type of the exception</typeparam>
        /// <exception cref="Exception">An unexpected type of exception was thrown.</exception>
        public static Result<T, TException> TryExecute<T, TException>(this Func<T> f)
            where T : class
            where TException : Exception
        {
            try
            {
                return new Ok<T, TException>((f ?? throw new ArgumentNullException(nameof(f))).Invoke());
            }
            catch (TException e)
            {
                return new Error<T, TException>(e);
            }
            catch (Exception e)
            {
                throw new Exception("Unexpected exception encountered", e);
            }
        }
    }
}
