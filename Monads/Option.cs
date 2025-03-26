namespace Monads
{
    public class Option<T>
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
    public class Some<T> : Option<T>
        where T : class
    {
        public Some(T value) : base(value) { }
    }

    public class None<T> : Option<T>
        where T : class
    {
        public None() : base(null) { }
    }
}
