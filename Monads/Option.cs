﻿namespace Monads
{
    /// <summary>
    /// The monad type used to represent that there might be a value, but doesn't have to be as well.
    /// </summary>
    /// <remarks>Also provides simple shorthands for ways of manipulating with them.</remarks>
    /// <typeparam name="T">the underlying value</typeparam>
    public class Option<T> : IEquatable<Option<T>>
        where T : class
    {
        protected T? _value;

        /// <summary>
        /// Returns the underlying value
        /// </summary>
        /// <returns>the contained value</returns>
        /// <exception cref="InvalidOperationException">there is no underlying value</exception>
        public T Value => _value ?? throw new InvalidOperationException();

        /// <summary>
        /// Returns a value indicating whether the underlying value or is <see langword="null"/> or not.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the underlying value exists.
        /// <see langword="false"/> if there is no underlying value.
        /// </returns>
        //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(_value))]
        public bool HasValue => _value is not null;

        protected Option(T? value = null) => _value = value;

        /// <summary>
        /// Creates an <see cref="Option{T}"/> that contains a value of type <typeparamref name="T"/>
        /// </summary>
        /// <param name="value">value to store</param>
        /// <returns>An <see cref="Option{T}"/> with some value of type <typeparamref name="T"/></returns>
        public static Option<T> Some(T value) => new(value);

        /// <summary>
        /// Creates an <see cref="Option{T}"/> with no underlying value
        /// </summary>
        /// <returns>An <see cref="Option{T}"/> containing nothing</returns>
        public static Option<T> None() => new(null);

        /// <summary>
        /// Tries to commit an operation with the possible underlying value.
        /// </summary>
        /// <typeparam name="R">Type of the result of the operation</typeparam>
        /// <param name="func">operation to perform</param>
        /// <returns>
        /// If there was an underlying value, returns an <see cref="Option{R}.Some(R)"/>.
        /// If there was no value, returns an <see cref="Option{R}.None"/>.
        /// </returns>
        public Option<R> Bind<R>(Func<T, R> func) where R : class
            => HasValue ? new(func(_value)) : new();

        /// <summary>
        /// Performs one of the actions,
        /// depending on if there exists an underlying value or not.
        /// </summary>
        /// <param name="some">action to perform if a value exists</param>
        /// <param name="none">action to perform if there is no value</param>
        public void Inspect(Action<T> some, Action none)
        {
            if (HasValue)
                some(_value);
            else none();
        }

        /// <summary>
        /// Performs one of the operations,
        /// depending on if there exists an underlying value or not.
        /// </summary>
        /// <param name="some">operation to perform if the value exists</param>
        /// <param name="none">operation to perform if there is no value</param>
        /// <returns>
        /// If there is a value, it is sent to <paramref name="some"/> and its result is returned.
        /// If there is no value, the result of <paramref name="none"/> is returned.
        /// </returns>
        public R Map<R>(Func<T, R> some, Func<R> none)
            => HasValue ? some(_value) : none();

        /// <summary>
        /// Tries to return the underlying value.
        /// </summary>
        /// <param name="orElse">Value to default to if <see langword="this"/> doesn't have an underlying value</param>
        /// <returns>
        /// If there is an underlying value, it is returned.
        /// Otherwise, <paramref name="orElse"/> is returned.
        /// </returns>
        public T ValueOrDefault(T orElse) => _value ?? orElse;

        /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
        public bool Equals(Option<T>? other)
            => other is not null && this.GetHashCode() == other.GetHashCode();
        /// <summary>
        /// Indicates whether the <see cref="Option{T}"/>s are equal
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if their underlying values are equal or are both <see langword="null"/>.
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(Option<T>? self, Option<T>? other)
            => self is null ? other is null : self.Equals(other);
        /// <summary>
        /// Indicates whether the <see cref="Option{T}"/>s are not equal
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if their underlying values are not equal or only one is <see langword="null"/>.
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(Option<T>? self, Option<T>? other) => !(self == other);

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        /// <inheritdoc cref="object.Equals(object?)"/>
        public override bool Equals(object? obj) => Equals(obj as Option<T>);
        internal const string _none = "None";
        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => $"{_value?.ToString() ?? _none}";
    }

    /// <summary>
    /// A shorthand to create an <see cref="Option{T}.Some(T)"/>
    /// </summary>
    /// <typeparam name="T">the desired underlying type</typeparam>
    /// <param name="value">value to store</param>
    public class Some<T>(T value) : Option<T>(value) where T : class;

    /// <summary>
    /// A shorthnd to create an <see cref="Option{T}.None"/>
    /// </summary>
    /// <typeparam name="T">the type of the value that is missing</typeparam>
    public class None<T>() : Option<T>() where T : class;

    /// <summary>
    /// The monad type used to represent that there might be a value, but doesn't have to be as well.
    /// </summary>
    /// <remarks>Also provides simple shorthands for ways of manipulating with them.</remarks>
    /// <typeparam name="T">the underlying value</typeparam>
    public class ValueOption<T> : IEquatable<ValueOption<T>>
        where T : struct
    {
        protected T? _value;

        /// <summary>
        /// Returns the underlying value
        /// </summary>
        /// <returns>the contained value</returns>
        /// <exception cref="InvalidOperationException">there is no underlying value</exception>
        public T Value => _value ?? throw new InvalidOperationException();

        /// <summary>
        /// Returns a value indicating whether the underlying value or is <see langword="null"/> or not.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the underlying value exists.
        /// <see langword="false"/> if there is no underlying value.
        /// </returns>
        //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(_value))]
        public bool HasValue => _value is not null;

        protected ValueOption(T? value = null) => _value = value;

        /// <summary>
        /// Creates an <see cref="ValueOption{T}"/> that contains a value of type <typeparamref name="T"/>
        /// </summary>
        /// <param name="value">value to store</param>
        /// <returns>An <see cref="ValueOption{T}"/> with some value of type <typeparamref name="T"/></returns>
        public static ValueOption<T> Some(T value) => new(value);

        /// <summary>
        /// Creates an <see cref="ValueOption{T}"/> with no underlying value
        /// </summary>
        /// <returns>An <see cref="ValueOption{T}"/> containing nothing</returns>
        public static ValueOption<T> None() => new(null);

        /// <summary>
        /// Tries to commit an operation with the possible underlying value.
        /// </summary>
        /// <typeparam name="R">Type of the result of the operation</typeparam>
        /// <param name="func">operation to perform</param>
        /// <returns>
        /// If there was an underlying value, returns an <see cref="ValueOption{R}.Some(R)"/>.
        /// If there was no value, returns an <see cref="ValueOption{R}.None"/>.
        /// </returns>
        public ValueOption<R> Bind<R>(Func<T, R> func) where R : struct
            => HasValue ? new(func(_value!.Value)) : new();

        /// <summary>
        /// Performs one of the actions,
        /// depending on if there exists an underlying value or not.
        /// </summary>
        /// <param name="some">action to perform if a value exists</param>
        /// <param name="none">action to perform if there is no value</param>
        public void Inspect(Action<T> some, Action none)
        {
            if (HasValue)
                some(_value.Value);
            else none();
        }

        /// <summary>
        /// Performs one of the operations,
        /// depending on if there exists an underlying value or not.
        /// </summary>
        /// <param name="some">operation to perform if the value exists</param>
        /// <param name="none">operation to perform if there is no value</param>
        /// <returns>
        /// If there is a value, it is sent to <paramref name="some"/> and its result is returned.
        /// If there is no value, the result of <paramref name="none"/> is returned.
        /// </returns>
        public R Map<R>(Func<T, R> some, Func<R> none)
            => HasValue ? some(_value!.Value) : none();

        /// <summary>
        /// Tries to return the underlying value.
        /// </summary>
        /// <param name="orElse">Value to default to if <see langword="this"/> doesn't have an underlying value</param>
        /// <returns>
        /// If there is an underlying value, it is returned.
        /// Otherwise, <paramref name="orElse"/> is returned.
        /// </returns>
        public T ValueOrDefault(T orElse) => _value ?? orElse;

        /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
        public bool Equals(ValueOption<T>? other)
            => other is not null && this.GetHashCode() == other.GetHashCode();
        /// <summary>
        /// Indicates whether the <see cref="ValueOption{T}"/>s are equal
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if their underlying values are equal or are both <see langword="null"/>.
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(ValueOption<T>? self, ValueOption<T>? other)
            => self is null ? other is null : self.Equals(other);
        /// <summary>
        /// Indicates whether the <see cref="ValueOption{T}"/>s are not equal
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if their underlying values are not equal or only one is <see langword="null"/>.
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(ValueOption<T>? self, ValueOption<T>? other) => !(self == other);

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        /// <inheritdoc cref="object.Equals(object?)"/>
        public override bool Equals(object? obj) => Equals(obj as ValueOption<T>);
        internal const string _none = "None";
        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => $"{_value?.ToString() ?? _none}";
    }

    /// <summary>
    /// A shorthand to create an <see cref="ValueOption{T}.Some(T)"/>
    /// </summary>
    /// <typeparam name="T">the desired underlying type</typeparam>
    /// <param name="value">value to store</param>
    public class ValueSome<T>(T value) : ValueOption<T>(value) where T : struct;

    /// <summary>
    /// A shorthnd to create an <see cref="ValueOption{T}.None"/>
    /// </summary>
    /// <typeparam name="T">the type of the value that is missing</typeparam>
    public class ValueNone<T>() : ValueOption<T>() where T : struct;
}
