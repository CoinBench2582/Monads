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
        public void IsErrorTest() {
            var result = new Error<Object,Exception>(testError);
            IsTrue(result.IsError);
        }

        [TestMethod]
        public void UnwrapTest() {
            var result = new Ok<Object,Exception>(testValue);
            AreSame(testValue, result.Unwrap);
        }

        [TestMethod]
        public void UnwrapErrorTest() {
            var result = new Error<Object,Exception>(testError);
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
        public void ExpectErrorTest() {
            var msg = "Error";
            var result = new Error<Object,Exception>(testError);
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
        public void IsErrorAndTest() {
            var result = new Error<Object,Exception>(testError);
            var predicate = (Exception e) => e.Message == testError.Message;
            IsTrue(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void IsErrorAndFalseTest() {
            var result = new Error<Object,Exception>(testError);
            var predicate = (Exception e) => e.Message == "Different Error";
            IsFalse(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void IsErrorAndNullTest() {
            var result = new Error<Object,Exception>(testError);
            var predicate = (Exception e) => e is null;
            IsFalse(result.IsErrorAnd(predicate));
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
        public void IsErrorAndNullPredicateTest() {
            var result = new Error<Object,Exception>(testError);
            var predicate = (Exception e) => e is null;
            IsFalse(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void OkTest() {
            var result = new Ok<Object,Exception>(testValue);
            IsTrue(result.Ok.Value == testValue);
        }

        [TestMethod]
        public void OkTestNull() {
            var result = new Error<Object,Exception>(testError);
            IsFalse(result.Ok.HasValue);
        }

        [TestMethod]
        public void ErrorTest() {
            var result = new Error<Object,Exception>(testError);
            IsTrue(result.Error.Value == testError);
        }

        [TestMethod]
        public void ErrorTestNull() {
            var result = new Ok<Object,Exception>(testValue);
            IsFalse(result.Error.HasValue);
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
            var result = new Error<Object,Exception>(testError);
            var mappedResult = result.Map(v => v.ToString()!);
            IsTrue(mappedResult.IsError);
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
            var result = new Error<Object,Exception>(testError);
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
            var result = new Error<Object,Exception>(testError);
            var mappedResult = result.MapOrElse(v => v.ToString()!, e => e.Message);
            AreSame(testError.Message, mappedResult);
        }

        [TestMethod]
        public void MapErrorDiffTest() {
            var result = new Error<Object,Exception>(testError);
            const string message = "Mapped Error";
            var mappedResult = result.MapError(e => new Exception(message));
            IsTrue(mappedResult.IsError);
            try {
                _ = mappedResult.Unwrap;
                Fail("Expected exception was not thrown");
            } catch (Exception ex) {
                AreNotSame(testError, ex);
                AreSame(ex.Message, message);
            }
        }

        [TestMethod]
        public void MapErrorTestNull() {
            var result = new Ok<Object,Exception>(testValue);
            var mappedResult = result.MapError(e => new Exception("Mapped Error"));
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
            IsTrue(result.IsError);
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
