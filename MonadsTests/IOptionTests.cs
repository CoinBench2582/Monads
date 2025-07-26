namespace Monads.Tests;

[TestClass]
public partial class IOptionTests
{
#pragma warning disable CS8618 // Pole, které nemůže být null, musí při ukončování konstruktoru obsahovat hodnotu, která není null.
    // Set in Prepare
    private static object _faulty;
    private static object _fine;
    private const string _testString = "Ahoj";
#pragma warning restore CS8618 // Pole, které nemůže být null, musí při ukončování konstruktoru obsahovat hodnotu, která není null.

    [ClassInitialize]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Odebrat nepoužívaný parametr", Justification = "Součástí API")]
    public static void Prepare(TestContext context)
    {
        _fine = _testString;
        _faulty = null!;
        Console.WriteLine($"Started {nameof(IOptionTests)}");
    }

    [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
    public static void Cleanup()
    {
        _fine = null!;
        Console.WriteLine($"Ended {nameof(IOptionTests)}");
    }

    [TestMethod]
    public void SomeTest()
    {
        Do<Option<object>, object>();

        static void Do<O, T>() where O : IOption<T> where T : notnull
        {
            IOption<T> someO = O.Some((T)_fine);
            IsTrue(someO.HasValue);
            _ = someO.Value;
            _ = ThrowsException<ArgumentNullException>(() => O.Some((T?)_faulty!));
        }
    }

    [TestMethod]
    public void NoneTest()
    {
        Do<Option<object>, object>();

        static void Do<O, T>() where O : IOption<T> where T : notnull
        {
            IOption<T> noneO = O.None();
            IsFalse(noneO.HasValue);
            _ = ThrowsException<InvalidOperationException>(() => _ = noneO.Value);
        }
    }

    [TestMethod]
    public void BindTest()
    {
        const string first = " světe";
        const string bang = "!";
        const string mid = _testString + first;
        const string end = mid + bang;
        static string ToStr<T>(T val) => val?.ToString()! ?? string.Empty;

        Some<Option<string>, string>();
        None<Option<string>, string>();
        Fail<Option<string>>();

        static void Some<O, T>() where O : IOption<T> where T : notnull
        {
            IOption<string> someInit = O.Some((T)_fine).Bind(ToStr);
            IOption<string> someNext = someInit.Bind(s => string.Concat(s, first));
            IOption<string> someLast = someNext.Bind(s => string.Concat(s, bang));
            IsTrue(someNext.HasValue && someLast.HasValue);
            AreEqual(mid, someNext.Value);
            AreEqual(end, someLast.Value);
        }

        static void None<O, T>() where O : IOption<T> where T : notnull
        {
            IOption<string> noneInit = O.None().Bind(ToStr);
            IOption<string> noneNext = noneInit.Bind(s => string.Concat(s, first));
            IOption<string> noneLast = noneNext.Bind(s => string.Concat(s, bang));
            IsFalse(noneNext.HasValue);
            IsFalse(noneLast.HasValue);
            _ = ThrowsException<InvalidOperationException>(() => _ = noneNext.Value);
            _ = ThrowsException<InvalidOperationException>(() => _ = noneLast.Value);
        }

        // Zatím by nemělo fachat.
        // Po implementaci ValueOption by mělo fungovat, takže přepsat na checkování, že funguje.
        static void Fail<O>() where O : IOption<string>
        {
            IOption<string> some = O.Some((string)_fine);
            IOption<string> none = O.None();
            IOption<int> @out;
            _ = ThrowsException<TypeArgumentException>(() => @out = some.Bind(s => s.Length));
            _ = ThrowsException<TypeArgumentException>(() => @out = none.Bind(s => s.Length));
        }
    }
}