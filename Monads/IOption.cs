
namespace Monads
{
    /// <summary>
    /// An interface used to unite monad types that may or may not contain a value of the specified type.
    /// </summary>
    /// <typeparam name="T">Type of the underlying value</typeparam>
    public interface IOption<T>
        where T : notnull
    {
        /// <summary>
        /// Returns the underlying value
        /// </summary>
        /// <returns>the contained value</returns>
        /// <exception cref="InvalidOperationException">there is no underlying value</exception>
        T Value { get; }

        /// <summary>
        /// Returns a value indicating whether the underlying value or is <see langword="null"/> or not.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the underlying value exists.
        /// <see langword="false"/> if there is no underlying value.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(Value))]
        bool HasValue { get; }

        /// <summary>
        /// Creates an <see cref="IOption{T}"/> that contains a value of type <typeparamref name="T"/>
        /// </summary>
        /// <param name="value">value to store</param>
        /// <returns>An <see cref="IOption{T}"/> with some value of type <typeparamref name="T"/></returns>
        static abstract IOption<T> Some(T value);

        /// <summary>
        /// Creates an <see cref="IOption{T}"/> with no underlying value
        /// </summary>
        /// <returns>An <see cref="IOption{T}"/> containing nothing</returns>
        static abstract IOption<T> None();

        /// <summary>
        /// Tries to commit an operation with the possible underlying value.
        /// </summary>
        /// <typeparam name="R">Type of the result of the operation</typeparam>
        /// <param name="func">operation to perform</param>
        /// <returns>
        /// If there was an underlying value, returns an <see cref="IOption{R}.Some(R)"/>.
        /// If there was no value, returns an <see cref="IOption{R}.None"/>.
        /// </returns>
        IOption<R> Bind<R>(Func<T, R> func) where R : notnull;

        /// <summary>
        /// Performs one of the actions,
        /// depending on whether there exists an underlying value or not.
        /// </summary>
        /// <param name="some">action to perform if a value exists</param>
        /// <param name="none">action to perform if there is no value</param>
        void Inspect(Action<T> some, Action none);
        /// <summary>
        /// Performs the action <paramref name="some"/> only if there exists an underlying value.
        /// </summary>
        /// <param name="some">the action to perform if a value exists</param>
        void Inspect(Action<T> some);
        /// <summary>
        /// Performs the action <paramref name="none"/> only if there doesn't exist any value.
        /// </summary>
        /// <param name="none">the action to perform if there is no value</param>
        void Inspect(Action none);

        /// <summary>
        /// Performs one of the operations,
        /// depending on if there exists an underlying value or not.
        /// </summary>
        /// <param name="some">operation to perform if the value exists</param>
        /// <param name="none">operation to perform if there is no value</param>
        /// <returns>
        /// If there is a value, it is passed to <paramref name="some"/> and its result is returned.
        /// If there is no value, the result of <paramref name="none"/> is returned.
        /// </returns>
        R Map<R>(Func<T, R> some, Func<R> none);

        /// <summary>
        /// Tries to return the underlying value.
        /// </summary>
        /// <param name="orElse">Value to default to if <see langword="this"/> doesn't have an underlying value</param>
        /// <returns>
        /// If there is an underlying value, it is returned.
        /// Otherwise, <paramref name="orElse"/> is returned.
        /// </returns>
        T ValueOrDefault(T orElse);
    }
}