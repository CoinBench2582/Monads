using System.Diagnostics.CodeAnalysis;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Monads.Tests
{
    [TestClass]
    public class OptionTests
    {
#pragma warning disable CS8618 // Pole, které nemůže být null, musí při ukončování konstruktoru obsahovat hodnotu, která není null.
        // Set in Prepare
        private static object _faulty;
        private static object _fine;
        private const string _testString = "Ahoj";
        internal const string _none = "None";
#pragma warning restore CS8618 // Pole, které nemůže být null, musí při ukončování konstruktoru obsahovat hodnotu, která není null.

        // public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void Prepare(TestContext context)
        {
            _fine = _testString;
            _faulty = null!;
            Console.WriteLine($"Started {nameof(OptionTests)}");
        }

        [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
        public static void Cleanup()
        {
            _fine = null!;
            Console.WriteLine($"Ended {nameof(OptionTests)}");
        }

        [TestMethod]
        public void SomeTest()
        {
            Option<object> someO = Option<object>.Some(_fine);
            Option<object> someC = new Some<object>(_fine);
            IsNotNull(someO); IsNotNull(someC);
            IsTrue(someO.HasValue); IsTrue(someC.HasValue);

            _ = ThrowsException<ArgumentNullException>(() => Option<object>.Some(_faulty));
            _ = ThrowsException<ArgumentNullException>(() => _ = new Some<object>(_faulty));
        }

        [TestMethod]
        public void NoneTest()
        {
            Option<object> noneO = Option<object>.None();
            Option<object> noneC = new None<object>();
            IsFalse(noneO.HasValue); IsFalse(noneC.HasValue);
        }

        [TestMethod]
        public void BindTest()
        {
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void InspectTest()
        {
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void MapTest()
        {
            //throw new NotImplementedException();
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
            Option<object> someO = Option<object>.Some(_fine);
            Option<object> someC = new Some<object>(_fine);
            IsTrue(someO.Equals((object)someC)); IsTrue(someC.Equals((object)someO));
            IsTrue(someO.Equals(someC)); IsTrue(someC.Equals(someO));
            IsFalse(someO != someC); IsFalse(someC != someO);

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
            AreEqual(_none, none.ToString());
        }
    }
}