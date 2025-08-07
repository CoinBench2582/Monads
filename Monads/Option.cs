namespace Monads
{
    /// <summary>
    /// The monad type used to represent that there might be a value, but doesn't have to be as well.
    /// </summary>
    /// <remarks>Also provides simple shorthands for ways of manipulating with them.</remarks>
    /// <typeparam name="T">the underlying type</typeparam>
    public class Option<T> : IOption<T>, IOptionFactory<T>
        where T : class
    {
        /// <summary>
        /// the underlying value
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Nedeklarujte viditelná pole instance.", Justification = "Protected field is not externally visible.")]
        protected readonly T? _value;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="Option{T}"/> class with an optional value.
        /// </summary>
        /// <param name="value">
        /// The initial value of the option.
        /// Can be <see langword="null"/> to represent no value.
        /// </param>
        protected Option(T? value = null) => _value = value;

        /// <summary>
        /// Shorthand for <see cref="System.Reflection.BindingFlags"/> to make reflection more performant.
        /// </summary>
        private protected const System.Reflection.BindingFlags _hereFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly;

        /// <summary>
        /// Creates an <see cref="Option{T}"/> that contains a value of type <typeparamref name="T"/>
        /// </summary>
        /// <param name="value">value to store</param>
        /// <exception cref="ArgumentNullException">The provided value was <see langword="null"/></exception>
        /// <returns>An <see cref="Option{T}"/> with some value of type <typeparamref name="T"/></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Nedeklarujte statické členy u obecných typů.", Justification = "Fulfilling IOptionFactory<T> interface.")]
        public static Option<T> Some(T value) => new(value ?? throw new ArgumentNullException(nameof(value)));
        
        static IOption<T> IOptionFactory<T>.Some(T value)
            => (IOption<T>)
                typeof(Option<>).MakeGenericType(typeof(T)).GetMethod(nameof(Some), _hereFlags)!
                .Invoke(null, [value
                    ?? throw new ArgumentNullException(nameof(value))])!;

        /// <summary>
        /// Creates an <see cref="Option{T}"/> with no underlying value
        /// </summary>
        /// <returns>An <see cref="Option{T}"/> containing nothing</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Nedeklarujte statické členy u obecných typů.", Justification = "Plnění rozhraní IOptionFactory<T>")]
        public static Option<T> None() => new();

        static IOption<T> IOptionFactory<T>.None()
            => (IOption<T>)
                typeof(Option<>).MakeGenericType(typeof(T)).GetMethod(nameof(None), _hereFlags)!
                .Invoke(null, null)!;

        /// <summary>
        /// Tries to commit an operation with the possible underlying value.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the operation</typeparam>
        /// <param name="func">operation to perform</param>
        /// <returns>
        /// If there was an underlying value, returns an <see cref="Option{R}.Some(R)"/>.
        /// If there was no value, returns an <see cref="Option{R}.None"/>.
        /// </returns>
        public Option<TResult> Bind<TResult>(Func<T, TResult> func) where TResult : class
            => this.HasValue
                ? new((func ?? throw new ArgumentNullException(nameof(func))).Invoke(_value))
                : new();

        IOption<R> IOption<T>.Bind<R>(Func<T, R> func)
        {
            Type typeR = typeof(R);
            // Check won't throw once ValueOption is implemented - will determine between used Type for @base instead.
            if (!(typeR.IsClass || typeR.IsInterface))
                throw new TypeArgumentException($"{nameof(R)} is not compliant with the generic type constraints.", nameof(R));
            Type @base = typeof(Option<>).MakeGenericType(typeR);
            return (IOption<R>)(this.HasValue
                ? @base.GetMethod(nameof(IOptionFactory<R>.Some), _hereFlags)!
                    .Invoke(null, [(func ?? throw new ArgumentNullException(nameof(func))).Invoke(this._value)])!
                : @base.GetMethod(nameof(IOptionFactory<R>.None), _hereFlags)!.Invoke(null, null)!);
        }

        /// <summary>
        /// Performs one of the actions,
        /// depending on if there exists an underlying value or not.
        /// </summary>
        /// <param name="some">action to perform if a value exists</param>
        /// <param name="none">action to perform if there is no value</param>
        public void Inspect(Action<T> some, Action none)
        {
            if (this.HasValue)
                (some ?? throw new ArgumentNullException(nameof(some))).Invoke(_value);
            else
                (none ?? throw new ArgumentNullException(nameof(none))).Invoke();
        }
        /// <summary>
        /// Performs the action <paramref name="some"/> only if there exists an underlying value.
        /// </summary>
        /// <param name="some">the action to perform if a value exists</param>
        public void Inspect(Action<T> some)
        {
            if (this.HasValue)
                (some ?? throw new ArgumentNullException(nameof(some))).Invoke(_value);
        }
        /// <summary>
        /// Performs the action <paramref name="none"/> only if there doesn't exist any value.
        /// </summary>
        /// <param name="none">the action to perform if there is no value</param>
        public void Inspect(Action none)
        {
            if (!this.HasValue)
                (none ?? throw new ArgumentNullException(nameof(none))).Invoke();
        }

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
        public TResult Map<TResult>(Func<T, TResult> some, Func<TResult> none)
            => this.HasValue
                ? (some ?? throw new ArgumentNullException(nameof(some))).Invoke(_value)
                : (none ?? throw new ArgumentNullException(nameof(none))).Invoke();

        /// <summary>
        /// Tries to return the underlying value.
        /// </summary>
        /// <param name="orElse">Value to default to if <see langword="this"/> doesn't have an underlying value</param>
        /// <returns>
        /// If there is an underlying value, it is returned.
        /// Otherwise, <paramref name="orElse"/> is returned.
        /// </returns>
        public T ValueOrDefault(T orElse) => _value ?? orElse;

        /// <summary>
        /// Creates a new <see cref="Option{T}"/> object, wrapping the provided <paramref name="value"/>.
        /// </summary>
        /// <param name="value">the value to wrap</param>
        /// <returns>
        /// An <see cref="Option{T}.Some(T)"/> if the <paramref name="value"/> is not <see langword="null"/>.
        /// Otherwise a <see cref="Option{T}.None"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Přetížení operátorů mají pojmenované alternativy.", Justification = "Would result in an invalid public method.")]
        public static implicit operator Option<T>(T? value) => new(value);

        /// <summary>
        /// Returns the underlying wrapped value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The instance to convert.</param>
        /// <returns>
        /// If the instance has an underlying value, it is returned;
        /// otherwise, <see langword="null"/> is returned.
        /// </returns>
        /// <exception cref="InvalidCastException">the supplied <paramref name="value"/> was <see langword="null"/></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Přetížení operátorů mají pojmenované alternativy.", Justification = "Would result in an invalid public method.")]
        public static explicit operator T?(Option<T> value)
            => value is not null ? value._value
                : throw new InvalidCastException($"The converted {nameof(value)} was null.");

#pragma warning disable S3875 // "operator==" should not be overloaded on reference types
        // used for correctly comparing internal values
        /// <summary>
        /// Indicates whether the <see cref="Option{T}"/>s' values are <c>==</c> equal
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if their values are <c>==</c> equal or both are <see cref="Option{T}.None"/>;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(Option<T>? self, Option<T>? other)
            => self is null ? other is null : other is not null && self._value == other._value;

        /// <summary>
        /// Indicates whether the <see cref="Option{T}"/>s' values are <c>!=</c> not equal
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if their values are <c>!=</c> not equal or only one is <see cref="Option{T}.None"/>;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(Option<T>? self, Option<T>? other)
            => self is null ? other is not null : other is null || self._value != other._value;
#pragma warning restore S3875 // "operator==" should not be overloaded on reference types

        /// <inheritdoc cref="object.Equals(object?)"/>
        public override bool Equals(object? obj)
            => obj is Option<T> other
                && (this._value is null ? other._value is null : this._value.Equals(other._value));

        /// <returns>
        /// A hash code for the current underlying value,
        /// or <c>0</c> if the value is <see langword="null"/>.
        /// </returns>
        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;

        /// <summary>
        /// Returns a string representation of the underlying value.
        /// </summary>
        /// <returns>
        /// The string representation of the underlying value,
        /// or <see langword="null"/> if the value is <see langword="null"/>.
        /// </returns>
        public override string? ToString() => _value?.ToString();
    }

    /// <summary>
    /// A shorthand to create an <see cref="Option{T}.Some(T)"/>
    /// </summary>
    /// <remarks>
    /// This class carries no additional behaviour.
    /// It cannot be inherited.
    /// </remarks>
    /// <typeparam name="T">the underlying type</typeparam>
    /// <param name="value">value to store</param>
    /// <exception cref="ArgumentNullException">The provided value was <see langword="null"/></exception>
    public sealed class Some<T>(T value)
        : Option<T>(value ?? throw new ArgumentNullException(nameof(value)))
        where T : class;

    /// <summary>
    /// A shorthand to create an <see cref="Option{T}.None"/>
    /// </summary>
    /// <remarks>
    /// This class carries no additional behaviour.
    /// It cannot be inherited.
    /// </remarks>
    /// <typeparam name="T">the type of the value that is missing</typeparam>
    public sealed class None<T>() : Option<T>() where T : class;
}
