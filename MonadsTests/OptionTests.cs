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
#pragma warning restore CS8618 // Pole, které nemůže být null, musí při ukončování konstruktoru obsahovat hodnotu, která není null.

        // public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void Prepare(TestContext context)
        {
            _fine = new();
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
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void EqualsTest()
        {
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void GetHashCodeTest()
        {
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void ToStringTest()
        {
            //throw new NotImplementedException();
        }
    }
}