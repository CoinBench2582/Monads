namespace Monads
{
    /// <summary>
    /// A monad that represents a value that is the result of an operation that may fail.
    /// The result may contain either a value or an exception.
    /// This is useful for composing operations that may fail, building a chain of operations that can be unwrapped at the end.
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="E">The type of the exception</typeparam>
    public class Result<T, E>
        where E : Exception
        where T : class
    {
        protected T? _value;
        protected E? _error;

        protected Result(T v) => _value = v;
        protected Result(E e) => _error = e;

        /// <summary>
        /// Returns true if the result is Ok
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the result is Ok.
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool IsOk => _error is null;

        /// <summary>
        /// Returns true if the result is Err
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the result is Err.
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool IsErr => !IsOk;

        /// <summary>
        /// Unwraps the result, throwing an exception if the result is an error
        /// </summary>
        /// <exception cref="E">The error that was wrapped in the result</exception>
        /// <returns>The value that was wrapped in the result</returns>
        public T Unwrap => _error is not null ? throw _error : _value!;

        /// <summary>
        /// Unwraps the result, throwing an exception with the given message if the result is an error
        /// </summary>
        /// <param name="msg">The message to include in the exception</param>
        /// <exception cref="Exception">The exception with the given message</exception>
        public void Expect(string msg) {
            if (_error is not null) {
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if the result is Ok and the value satisfies the predicate
        /// </summary>
        /// <param name="f">The predicate to test the value against</param>
        /// <returns>
        /// <see langword="true"/> if the result is Ok and the value satisfies the predicate.
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool IsOkAnd(Func<T,bool> f) => IsOk && _value is not null ? f(_value) : false;

        /// <summary>
        /// Returns true if the result is Err and the error satisfies the predicate
        /// </summary>
        /// <param name="f">The predicate to test the error against</param>
        /// <returns>
        /// <see langword="true"/> if the result is Err and the error satisfies the predicate.
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool IsErrAnd(Func<E,bool> f) => IsErr && _error is not null ? f(_error) : false;

        /// <summary>
        /// Returns an Option containing the value. Ok if the result is Ok, otherwise None.
        /// </summary>
        /// <returns>An Option containing the value if the result is Ok, otherwise None</returns>
        public Option<T> Ok => _value is not null ? new Some<T>(_value) : new None<T>();

        /// <summary>
        /// Returns an Option containing the error. Some if the result is Err, otherwise None.
        /// </summary>
        /// <returns>An Option containing the error if the result is Err, otherwise None</returns>
        public Option<E> Err => _error is not null ? new Some<E>(_error) : new None<E>();

        /// <summary>
        /// Maps the value of the result to a new value if the result is Ok, otherwise maps the error to a new Err
        /// </summary>
        /// <param name="op">The function to map the value to a new value</param>
        /// <typeparam name="U">The type of the new value</typeparam>
        /// <returns>A new Result containing the mapped value if the result is Ok, otherwise a new Err containing the mapped error</returns>
        public Result<U,E> Map<U>(Func<T,U> op) where U:class => IsOk ? new Ok<U,E>(op(_value!)) : new Err<U,E>(_error!);

        /// <summary>
        /// Maps the value of the result to a new value if the result is Ok, otherwise returns a default value
        /// </summary>
        /// <param name="op">The function to map the value to a new value</param>
        /// <param name="def">The default value to return if the result is an error</param>
        /// <typeparam name="U">The type of the new value</typeparam>
        /// <returns>The mapped value if the result is Ok, otherwise the default value</returns>
        public U MapOr<U>(Func<T,U> op, U def) => IsOk ? op(_value!) : def;

        /// <summary>
        /// Maps the value of the result to a new value if the result is Ok, otherwise maps the error to a new value
        /// </summary>
        /// <param name="op">The function to map the value to a new value</param>
        /// <param name="cb">The function to map the error to a new value</param>
        /// <typeparam name="U">The type of the new value</typeparam>
        /// <returns>The mapped value if the result is Ok, otherwise the mapped error</returns>
        public U MapOrElse<U>(Func<T,U> op, Func<E,U> cb) => IsOk ? op(_value!) : cb(_error!);

        /// <summary>
        /// Maps the error of the result to a new error if the result is Err, otherwise maps the value to a new Ok
        /// </summary>
        /// <param name="op">The function to map the error to a new error</param>
        /// <typeparam name="F">The type of the new error</typeparam>
        /// <returns>A new Err containing the mapped error if the result is Err, otherwise a new Ok containing the mapped value</returns>
        public Result<T,F> MapErr<F>(Func<E,F> op) where F:Exception => IsOk ? new Ok<T,F>(_value!) : new Err<T,F>(op(_error!));

        /// <summary>
        /// Executes an action on the value if the result is Ok
        /// </summary>
        /// <param name="op">The action to execute on the value</param>
        /// <returns>Self</returns>
        public Result<T,E> Inspect(Action<T> op) {
            if (IsOk) {
                op(_value!);
            }
            return this;
        }

        /// <summary>
        /// Executes an action on the error if the result is Err
        /// </summary>
        /// <param name="op">The action to execute on the error</param>
        /// <returns>Self</returns>
        public Result<T,E> InspectErr(Action<E> op) {
            if (!IsOk) {
                op(_error!);
            }
            return this;
        }

        /// <summary>
        /// Executes a function and wraps the result in Ok if the function executes successfully, otherwise wraps the exception in Err
        /// </summary>
        /// <param name="f">The function to execute</param>
        /// <returns>A new Result containing the result of the function if it executes successfully, otherwise a new Err containing the exception</returns>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <typeparam name="E">The type of the exception</typeparam>
        public static Result<T,E> Exec(Func<T> f) {
            try {
                return new Ok<T,E>(f());
            } catch (E e) {
                return new Err<T,E>(e);
            }
        }
    }

    /// <summary>
    /// Represents a successful result
    /// </summary>
    /// <param name="v">Value to return</param>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="E">The type of the exception</typeparam>
    public class Ok<T, E>(T v) : Result<T, E>(v)
        where E : Exception
        where T : class;

    /// <summary>
    /// Represents a failed result
    /// </summary>
    /// <param name="e">Error to return</param>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="E">The type of the exception</typeparam>
    public class Err<T, E>(E e) : Result<T, E>(e)
        where E : Exception
        where T : class;
}
