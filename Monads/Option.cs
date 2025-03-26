namespace Monads
{
    public class Option<T>
        where T : class
    {
        protected readonly T? _value;

        public T Value => _value ?? throw new InvalidOperationException();

        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(_value))]
        public bool HasValue => _value is not null;

        protected Option(T? value = null) => _value = value;

        public static Option<T> Some(T value) => new(value);

        public static Option<T> None() => new(null);
    }
}
