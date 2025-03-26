namespace Monads
{
    public class Option<T> : IEquatable<Option<T>>
        where T : class
    {
        protected T? _value;

        public T Value => _value ?? throw new InvalidOperationException();

        //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(_value))]
        public bool HasValue => _value is not null;

        protected Option(T? value = null) => _value = value;

        public static Option<T> Some(T value) => new(value);

        public static Option<T> None() => new(null);

        public Option<R> Bind<R>(Func<T, R> func) where R : class
            => HasValue ? new(func(_value)) : new();

        public void Resolve(Action<T> some, Action none)
        {
            if (HasValue)
                some(_value!);
            else
                none();
        }

        public R Resolve<R>(Func<T, R> some, Func<R> none)
            => HasValue ? some(_value) : none();

        public bool Equals(Option<T>? other)
            => other is not null && this.GetHashCode() == other.GetHashCode();
        public static bool operator ==(Option<T>? self, Option<T>? other)
            => self is null ? other is null : self.Equals(other);
        public static bool operator !=(Option<T>? self, Option<T>? other) => !(self == other);

        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override bool Equals(object? obj) => Equals(obj as Option<T>);
        internal const string _none = "None";
        public override string ToString() => $"{_value?.ToString() ?? _none}";
    }

    public class Some<T>(T value) : Option<T>(value) where T : class;

    public class None<T>() : Option<T>() where T : class;
}
