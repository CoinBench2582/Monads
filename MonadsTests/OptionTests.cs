using Microsoft.VisualStudio.TestTools.UnitTesting;

using Monads;

using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Monads.Tests
{
    [TestClass]
    public class OptionTests
    {
        [TestMethod]
        public void ExistsTest() => IsTrue(true);
    }
}