using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Monads.Tests
{
    [TestClass]
    public class ResultTests
    {
        private static Object testValue = new Object();
        private static Exception testError = new Exception("Test exception");

        [TestMethod]
        public void IsOkTest() {
            var result = new Ok<Object,Exception>(testValue);
            IsTrue(result.IsOk);
        }

        [TestMethod]
        public void IsErrTest() {
            var result = new Err<Object,Exception>(testError);
            IsTrue(result.IsErr);
        }

        [TestMethod]
        public void UnwrapTest() {
            var result = new Ok<Object,Exception>(testValue);
            AreSame(testValue, result.Unwrap);
        }

        [TestMethod]
        public void UnwrapErrTest() {
            var result = new Err<Object,Exception>(testError);
            try {
                _ = result.Unwrap;
                Fail("Expected exception was not thrown");
            } catch (Exception ex) {
                AreSame(testError, ex);
            }
        }

        [TestMethod]
        public void ExpectTest() {
            var result = new Ok<Object,Exception>(testValue);
            result.Expect("Error");
            AreSame(testValue, result.Unwrap);
        }

        [TestMethod]
        public void ExpectErrTest() {
            var msg = "Error";
            var result = new Err<Object,Exception>(testError);
            try {
                result.Expect(msg);
                Fail("Expected exception was not thrown");
            } catch (Exception ex) {
                AreSame(ex.Message, msg);
            }
        }

        [TestMethod]
        public void IsOkAndTest() {
            var result = new Ok<Object,Exception>(testValue);
            var predicate = (object v) => v is not null;
            IsTrue(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndFalseTest() {
            var result = new Ok<Object,Exception>(testValue);
            var predicate = (object v) => v is null;
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsErrAndTest() {
            var result = new Err<Object,Exception>(testError);
            var predicate = (Exception e) => e.Message == testError.Message;
            IsTrue(result.IsErrAnd(predicate));
        }

        [TestMethod]
        public void IsErrAndFalseTest() {
            var result = new Err<Object,Exception>(testError);
            var predicate = (Exception e) => e.Message == "Different Error";
            IsFalse(result.IsErrAnd(predicate));
        }

        [TestMethod]
        public void IsErrAndNullTest() {
            var result = new Err<Object,Exception>(testError);
            var predicate = (Exception e) => e is null;
            IsFalse(result.IsErrAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndNullTest() {
            var result = new Ok<Object,Exception>(testValue);
            var predicate = (object v) => v is null;
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndNullPredicateTest() {
            var result = new Ok<Object,Exception>(testValue);
            var predicate = (object v) => v is null;
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsErrAndNullPredicateTest() {
            var result = new Err<Object,Exception>(testError);
            var predicate = (Exception e) => e is null;
            IsFalse(result.IsErrAnd(predicate));
        }

        [TestMethod]
        public void OkTest() {
            var result = new Ok<Object,Exception>(testValue);
            IsTrue(result.Ok.Value == testValue);
        }

        [TestMethod]
        public void OkTestNull() {
            var result = new Err<Object,Exception>(testError);
            IsFalse(result.Ok.HasValue);
        }

        [TestMethod]
        public void ErrTest() {
            var result = new Err<Object,Exception>(testError);
            IsTrue(result.Err.Value == testError);
        }

        [TestMethod]
        public void ErrTestNull() {
            var result = new Ok<Object,Exception>(testValue);
            IsFalse(result.Err.HasValue);
        }

        [TestMethod]
        public void MapTest() {
            var result = new Ok<Object,Exception>(testValue);
            var mappedResult = result.Map(v => v.ToString()!);
            IsTrue(mappedResult.IsOk);
            AreNotSame(testValue, mappedResult.Unwrap);
        }

        [TestMethod]
        public void MapErrorTest() {
            var result = new Err<Object,Exception>(testError);
            var mappedResult = result.Map(v => v.ToString()!);
            IsTrue(mappedResult.IsErr);
            try {
                _ = mappedResult.Unwrap;
                Fail("Expected exception was not thrown");
            } catch (Exception ex) {
                AreSame(testError, ex);
            }
        }

        [TestMethod]
        public void MapOrTest() {
            var result = new Ok<Object,Exception>(testValue);
            var mappedResult = result.MapOr(v => v.ToString()!, new Object());
            AreSame(testValue.ToString(), mappedResult);
        }

        [TestMethod]
        public void MapOrTestNull() {
            var result = new Err<Object,Exception>(testError);
            var mappedResult = result.MapOr(v => v.ToString()!, testValue);
            AreSame(testValue, mappedResult);
        }

        [TestMethod]
        public void MapOrElseTest() {
            var result = new Ok<Object,Exception>(testValue);
            var mappedResult = result.MapOrElse(v => v.ToString()!, e => e.Message);
            AreSame(testValue.ToString(), mappedResult);
        }

        [TestMethod]
        public void MapOrElseTestNull() {
            var result = new Err<Object,Exception>(testError);
            var mappedResult = result.MapOrElse(v => v.ToString()!, e => e.Message);
            AreSame(testError.Message, mappedResult);
        }

        [TestMethod]
        public void MapErrTest() {
            var result = new Err<Object,Exception>(testError);
            var mappedResult = result.MapErr(e => new Exception("Mapped Error"));
            IsTrue(mappedResult.IsErr);
            try {
                _ = mappedResult.Unwrap;
                Fail("Expected exception was not thrown");
            } catch (Exception ex) {
                AreNotSame(testError, ex);
                AreSame(ex.Message, "Mapped Error");
            }
        }

        [TestMethod]
        public void MapErrTestNull() {
            var result = new Ok<Object,Exception>(testValue);
            var mappedResult = result.MapErr(e => new Exception("Mapped Error"));
            IsTrue(mappedResult.IsOk);
            AreSame(testValue, mappedResult.Unwrap);
        }

        [TestMethod]
        public void TryExecuteSucceedTest()
        {
            var testFunc = static () => "ahoj";
            var result = testFunc.TryExecute<string, Exception>();
            IsTrue(result.IsOk);
        }

        [TestMethod]
        public void TryExecuteFailTest()
        {
            var testFunc = static () => false ? string.Empty : throw new InvalidOperationException();
            var result = testFunc.TryExecute<string, InvalidOperationException>();
            IsTrue(result.IsErr);
        }

        [TestMethod]
        public void TryExecuteUnexpectedTest()
        {
            ArgumentException exception = new(); 
            var testFunc = () => false ? string.Empty : throw exception;
            Result<string, InvalidOperationException> result = default!;
            try
            {
                result = testFunc.TryExecute<string, InvalidOperationException>();
                Fail("Didn't throw");
            }
            catch (Exception e)
            {
                AreEqual(exception, e.InnerException);
            }
            IsNull(result);
        }
    }
}
