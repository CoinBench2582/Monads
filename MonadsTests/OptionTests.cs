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
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void NoneTest()
        {
            //throw new NotImplementedException();
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