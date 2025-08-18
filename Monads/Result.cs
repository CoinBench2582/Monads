namespace Monads
{
    /// <summary>
    /// A monad that represents a value that is the result of an operation that may fail.
    /// The result may contain either a value or an exception.
    /// This is useful for composing operations that may fail, building a chain of operations that can be unwrapped at the end.
    /// </summary>
    /// <typeparam name="TValue">The type of the value</typeparam>
    /// <typeparam name="TError">The type of the exception</typeparam>
    public class Result<TValue, TError>
        where TValue : class
        where TError : Exception
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Nedeklarujte viditelná pole instance.", Justification = "Protected field is not externally visible.")]
        protected readonly TValue? _value;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Nedeklarujte viditelná pole instance.", Justification = "Protected field is not externally visible.")]
        protected readonly TError? _error;

        /// <summary>
        /// Unwraps the result value, throwing an exception if the result is a failure
        /// </summary>
        /// <exception cref="InvalidOperationException">thrown when the result is a failure</exception>
        /// <returns>The value that was wrapped in the result</returns>
        public TValue UnwrapValue => _value ?? throw new InvalidOperationException();

        /// <summary>
        /// Unwraps the result error, throwing an exception if the result is ok
        /// </summary>
        /// <exception cref="InvalidOperationException">thrown when the result is ok</exception>
        /// <returns>The error that was wrapped in the result</returns>
        public TError UnwrapError => _error ?? throw new InvalidOperationException();

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
        /// Initializes a new instance of the <see cref="Result{TValue}"/> class with the specified value.
        /// </summary>
        /// <param name="value">The value to be encapsulated by the result.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <see langword="null"/>.</exception>
        protected Result(TValue value) => _value = value ?? throw new ArgumentNullException(nameof(value));
        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TError}"/> class with the specified error.
        /// </summary>
        /// <param name="exception">The error object representing the failure state.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is <see langword="null"/>.</exception>
        protected Result(TError exception) => _error = exception ?? throw new ArgumentNullException(nameof(exception));

        /// <summary>
        /// Creates a successful result containing the specified value.
        /// </summary>
        /// <param name="value">The value to encapsulate in the result. Cannot be <see langword="null"/>.</param>
        /// <returns>A <see cref="Result{TValue, TError}"/> representing a successful operation with the specified value.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <see langword="null"/>.</exception>
        public static Result<TValue, TError> Ok(TValue value)
            => new(value ?? throw new ArgumentNullException(nameof(value)));

        /// <summary>
        /// Creates a failed result with the specified error.
        /// </summary>
        /// <param name="exception">The error associated with the failed result. Cannot be <see langword="null"/>.</param>
        /// <returns>A <see cref="Result{TValue, TError}"/> representing a failure with the specified error.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is <see langword="null"/>.</exception>
        public static Result<TValue, TError> Failure(TError exception)
            => new(exception ?? throw new ArgumentNullException(nameof(exception)));

        /// <summary>
        /// Returns an <see cref="Option{TValue}"/> containing the value.
        /// Some if the result is ok, otherwise None.
        /// </summary>
        /// <returns>A Some containing the value if the result is ok, otherwise None.</returns>
        public Option<TValue> Value => IsOk ? Option<TValue>.Some(_value) : Option<TValue>.None();

        /// <summary>
        /// Returns an <see cref="Option{TError}"/> containing the error.
        /// Some if the result is a failure, otherwise None.
        /// </summary>
        /// <returns>A Some containing the error if the result is a failure, otherwise None.</returns>
        public Option<TError> Error => IsError ? Option<TError>.Some(_error) : Option<TError>.None();

        /// <summary>
        /// Unwraps the result, throwing an exception with the given message if the result is a failure
        /// </summary>
        /// <param name="message">The message to include in the exception</param>
        /// <exception cref="ResultException">The exception with the given message</exception>
        [Obsolete("Use `UnwrapError` or `Error` properties to get the underlying error instead.", false)]
        public void Expect(string message)
        {
            if (IsError) throw new ResultException(message, _error);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the result is Ok and the value satisfies the predicate
        /// </summary>
        /// <param name="predicate">The predicate to test the value against</param>
        /// <returns>
        /// <see langword="true"/> if the result is Ok and the value satisfies the predicate.
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool IsOkAnd(Func<TValue, bool> predicate)
            => IsOk && (predicate ?? throw new ArgumentNullException(nameof(predicate))).Invoke(_value);

        /// <summary>
        /// Returns true if the result is Error and the error satisfies the predicate
        /// </summary>
        /// <param name="predicate">The predicate to test the error against</param>
        /// <returns>
        /// <see langword="true"/> if the result is a failure and the error satisfies the predicate.
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool IsErrorAnd(Func<TError, bool> predicate)
            => IsError && (predicate ?? throw new ArgumentNullException(nameof(predicate))).Invoke(_error);

        /// <summary>
        /// Maps the value of the result to a new value if the result is ok, otherwise maps the error to a new failure
        /// </summary>
        /// <param name="operation">The function to map the value to a new value</param>
        /// <typeparam name="TResult">The type of the new value</typeparam>
        /// <returns>A new Result containing the mapped value if the result is ok, otherwise a new failure containing the mapped error</returns>
        public Result<TResult, TError> Map<TResult>(Func<TValue, TResult> operation) where TResult : class
            => IsOk
                ? new((operation ?? throw new ArgumentNullException(nameof(operation))).Invoke(_value))
                : new(_error);

        /// <summary>
        /// Maps the value of the result to a new value if the result is ok, otherwise returns a default value
        /// </summary>
        /// <param name="operation">The function to map the value to a new value</param>
        /// <param name="default">The default value to return if the result is a failure</param>
        /// <typeparam name="TResult">The type of the new value</typeparam>
        /// <returns>The mapped value if the result is ok, otherwise the default value</returns>
        public TResult MapOr<TResult>(Func<TValue, TResult> operation, TResult @default)
            => IsOk ? (operation ?? throw new ArgumentNullException(nameof(operation))).Invoke(_value) : @default;

        /// <summary>
        /// Maps the value of the result to a new value if the result is ok, otherwise maps the error to a new value
        /// </summary>
        /// <param name="operation">The function to map the value to a new value</param>
        /// <param name="else">The function to map the error to a new value</param>
        /// <typeparam name="TResult">The type of the new value</typeparam>
        /// <returns>The mapped value if the result is Ok, otherwise the mapped error</returns>
        public TResult MapOrElse<TResult>(Func<TValue, TResult> operation, Func<TError, TResult> @else)
            => IsOk
                ? (operation ?? throw new ArgumentNullException(nameof(operation))).Invoke(_value)
                : (@else ?? throw new ArgumentNullException(nameof(@else))).Invoke(_error);

        /// <summary>
        /// Maps the error of the result to a new error if the result is a failure, otherwise maps the value to a new ok result
        /// </summary>
        /// <param name="operation">The function to map the error to a new error</param>
        /// <typeparam name="TResultError">The type of the new error</typeparam>
        /// <returns>A new Error containing the mapped error if the result is Error, otherwise a new Ok containing the mapped value</returns>
        public Result<TValue, TResultError> MapError<TResultError>(Func<TError, TResultError> operation) where TResultError : Exception
            => IsOk
                ? new(_value)
                : new((operation ?? throw new ArgumentNullException(nameof(operation))).Invoke(_error));

        /// <summary>
        /// Executes an action on the value if the result is Ok
        /// </summary>
        /// <param name="operation">The action to execute on the value</param>
        /// <returns>Self</returns>
        public Result<TValue, TError> Inspect(Action<TValue> operation)
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
        public Result<TValue, TError> InspectError(Action<TError> operation)
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
    /// <typeparam name="TValue">The type of the value</typeparam>
    /// <typeparam name="TError">The type of the exception</typeparam>
    public sealed class Ok<TValue, TError>(TValue value)
        : Result<TValue, TError>(value ?? throw new ArgumentNullException(nameof(value)))
        where TValue : class
        where TError : Exception;

    /// <summary>
    /// Represents a failed result
    /// </summary>
    /// <param name="exception">Error to return</param>
    /// <typeparam name="TValue">The type of the value</typeparam>
    /// <typeparam name="TError">The type of the exception</typeparam>
    public sealed class Failure<TValue, TError>(TError exception)
        : Result<TValue, TError>(exception ?? throw new ArgumentNullException(nameof(exception)))
        where TValue : class
        where TError : Exception;

    public static class ResultExtensions
    {
        /// <summary>
        /// Executes a function and wraps the result in ok result if the function executes successfully, otherwise wraps the exception in a failure
        /// </summary>
        /// <param name="f">The function to execute</param>
        /// <returns>A new Result containing the result of the function if it executes successfully, otherwise a new failure containing the exception</returns>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <typeparam name="TException">The type of the exception</typeparam>
        /// <exception cref="ResultException">An unexpected type of exception was thrown.</exception>
        public static Result<T, TException> TryExecute<T, TException>(this Func<T> f)
            where T : class
            where TException : Exception
        {
            try
            {
                return Result<T, TException>.Ok((f ?? throw new ArgumentNullException(nameof(f))).Invoke());
            }
            catch (TException e)
            {
                return Result<T, TException>.Failure(e);
            }
            catch (Exception e)
            {
                throw new ResultException("Unexpected exception encountered", e);
            }
        }
    }
}
