namespace Monads.Tests
{
    [TestClass]
    public class OptionTests
    {
#pragma warning disable CS8618 // Pole, které nemůže být null, musí při ukončování konstruktoru obsahovat hodnotu, která není null.
        // Set in Prepare
        private static object _faulty;
        private static object _fine;
        private static object _nulling;
        private static object _empty;
        private const string _testString = "Ahoj";
#pragma warning restore CS8618 // Pole, které nemůže být null, musí při ukončování konstruktoru obsahovat hodnotu, která není null.

        [ClassInitialize]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Odebrat nepoužívaný parametr", Justification = "Součástí API")]
        public static void Prepare(TestContext context)
        {
            _fine = _testString;
            _faulty = null!;
            _nulling = new NullingClass();
            _empty = new EmptyClass();
            Console.WriteLine($"Started {nameof(OptionTests)}");
        }

        [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
        public static void Cleanup()
        {
            _fine = null!;
            _nulling = null!;
            _empty = null!;
            Console.WriteLine($"Ended {nameof(OptionTests)}");
        }

        [TestMethod]
        public void SomeTest()
        {
            Option<object> someO = Option<object>.Some(_fine);
            Option<object> someC = new Some<object>(_fine);
            IsTrue(someO.HasValue); IsTrue(someC.HasValue);
            _ = someO.Value;
            _ = someC.Value;

            _ = ThrowsException<ArgumentNullException>(() => Option<object>.Some(_faulty));
            _ = ThrowsException<ArgumentNullException>(() => _ = new Some<object>(_faulty));
        }

        [TestMethod]
        public void NoneTest()
        {
            Option<object> noneO = Option<object>.None();
            Option<object> noneC = new None<object>();
            IsFalse(noneO.HasValue); IsFalse(noneC.HasValue);
            _ = ThrowsException<InvalidOperationException>(() => _ = noneO.Value);
            _ = ThrowsException<InvalidOperationException>(() => _ = noneC.Value);
        }

        [TestMethod]
        public void BindTest()
        {
            Option<string> someInit = Option<string>.Some((string)_fine);
            Option<string> someNext = someInit.Bind(s  => string.Concat(s, " světe"));
            Option<string> someLast = someNext.Bind(s => string.Concat(s, "!"));
            const string mid = _testString + " světe";
            const string end = mid + "!";
            IsTrue(someNext.HasValue && someLast.HasValue);
            AreEqual(mid, someNext.Value);
            AreEqual(end, someLast.Value);

            Option<string> noneInit = Option<string>.None();
            Option<string> noneNext = noneInit.Bind(s => string.Concat(s, " světe"));
            Option<string> noneLast = noneNext.Bind(s => string.Concat(s, "!"));
            IsFalse(noneNext.HasValue); IsFalse(noneLast.HasValue);
            _ = ThrowsException<InvalidOperationException>(() => _ = noneNext.Value);
            _ = ThrowsException<InvalidOperationException>(() => _ = noneLast.Value);
        }

        [TestMethod]
        public void InspectTest()
        {
            bool? outsideVar = null;
            void ResetVar() => outsideVar = null;
            void Some(object _) => outsideVar = true;
            void None() => outsideVar = false;

            Option<string> some = Option<string>.Some((string)_fine);
            some.Inspect(Some, None);
            IsTrue(outsideVar);
            ResetVar();

            Option<object> none = Option<object>.None();
            none.Inspect(Some, None);
            IsFalse(outsideVar);
            ResetVar();
        }

        [TestMethod]
        public void CastTest()
        {
            Option<object> some = _fine;
            IsTrue(some.HasValue);
            _ = some.Value;

            Option<object> none = _faulty;
            IsFalse(none.HasValue);
            _ = ThrowsException<InvalidOperationException>(() => none.Value);
        }

        [TestMethod]
        public void MapTest()
        {
            const string doesnt = "doesn\'t exist";
            const string exists = _testString + " exists";

            Option<string> some = (string)_fine;
            int len = some.Map(
                s => s.Length,
                () => 0
            );
            AreEqual(_testString.Length, len);
            string mod = some.Map(
                s => $"{s} exists",
                () => doesnt
            );
            AreEqual(exists, mod);

            Option<string> none = (string?)null;
            len = none.Map(
                s => s.Length,
                () => 0
            );
            AreEqual(0, len);
            mod = none.Map(
                s => $"{s} exists",
                () => doesnt
            );
            AreEqual(doesnt, mod);
        }

        [TestMethod]
        public void ValueOrDefaultTest()
        {
            object the = new();
            Option<object> obj = Option<object>.Some(the);
            Option<string> str = Option<string>.Some((string)_fine);
            AreEqual(the, obj.ValueOrDefault(_testString));
            AreEqual(_testString, str.ValueOrDefault(the.ToString()!));

            Option<object> noObj = Option<object>.None();
            Option<string> noStr = Option<string>.None();
            AreEqual(the, noObj.ValueOrDefault(the));
            AreEqual(_testString, noObj.ValueOrDefault(_testString));
            AreEqual(_testString, noStr.ValueOrDefault(_testString));
        }

        [TestMethod]
        public void EqualsTest()
        {
            // Reference equality
            object obj = new();
            Option<object> objO = Option<object>.Some(obj);
            Option<object> objC = new Some<object>(obj);
            IsTrue(objO.Equals((object)objC)); IsTrue(objC.Equals((object)objO));
            IsTrue(objO.Equals(objC)); IsTrue(objC.Equals(objO));
            IsFalse(objO != objC); IsFalse(objC != objO);
            // Value equality
            Option<object> strO = Option<object>.Some(_fine);
            Option<object> strC = new Some<object>(_testString);
            IsTrue(strO.Equals((object)strC)); IsTrue(strC.Equals((object)strO));
            IsTrue(strO.Equals(strC)); IsTrue(strC.Equals(strO));
            IsFalse(strO != strC); IsFalse(strC != strO);
            // None equality
            Option<object> noneO = Option<object>.None();
            Option<object> noneC = new None<object>();
            IsTrue(noneO.Equals((object)noneC)); IsTrue(noneC.Equals((object)noneO));
            IsTrue(noneO.Equals(noneC)); IsTrue(noneC.Equals(noneO));
            IsFalse(noneO != noneC); IsFalse(noneC != noneO);
        }

        [TestMethod]
        public void ToStringTest()
        {
            Option<object> some = Option<object>.Some(_fine);
            AreEqual(_fine.ToString(), some.ToString());

            Option<object> none = Option<object>.None();
            AreEqual(null, none.ToString());

            Option<object> nulling = Option<object>.Some(_nulling);
            AreEqual(null, nulling.ToString());

            Option<object> empty = Option<object>.Some(_empty);
            AreEqual(_empty.ToString(), empty.ToString());
        }
    }

    file sealed class NullingClass
    {
        public sealed override string? ToString() => null;
    }

#pragma warning disable S2094 // Classes should not be empty
    file sealed class EmptyClass;
#pragma warning restore S2094 // Classes should not be empty
}