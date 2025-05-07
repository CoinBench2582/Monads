
namespace Monads
{
    public interface IOption<T>
    {
        T Value { get; }
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(Value))]
        bool HasValue { get; }

        static abstract IOption<T> Some(T value);
        static abstract IOption<T> None();

        IOption<R> Bind<R>(Func<T, R> func);
        void Inspect(Action<T> some, Action none);
        R Map<R>(Func<T, R> some, Func<R> none);
        T ValueOrDefault(T orElse);
    }
}