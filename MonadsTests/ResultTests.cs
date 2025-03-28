using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Monads.Tests
{
    [TestClass]
    public class ResultTests
    {
        private static object _testValue = new();
        private static Exception _testError = new("Test exception");

        [TestMethod]
        public void IsOkTest()
        {
            Result<object, Exception> result = new Ok<object, Exception>(_testValue);
            IsTrue(result.IsOk);
        }

        [TestMethod]
        public void IsErrorTest()
        {
            Result<object, Exception> result = new Error<object, Exception>(_testError);
            IsTrue(result.IsError);
        }

        [TestMethod]
        public void UnwrapTest()
        {
            Result<object, Exception> result = new Ok<object, Exception>(_testValue);
            AreSame(_testValue, result.Unwrap);
        }

        [TestMethod]
        public void UnwrapErrorTest()
        {
            Result<object, Exception> result = new Error<object, Exception>(_testError);
            try
            {
                _ = result.Unwrap;
                Fail("Expected exception was not thrown");
            }
            catch (Exception ex)
            {
                AreSame(_testError, ex);
            }
        }

        [TestMethod]
        public void ExpectTest()
        {
            Result<object, Exception> result = new Ok<object, Exception>(_testValue);
            result.Expect("Error");
            AreSame(_testValue, result.Unwrap);
        }

        [TestMethod]
        public void ExpectErrorTest()
        {
            string msg = "Error";
            Result<object, Exception> result = new Error<object, Exception>(_testError);
            try
            {
                result.Expect(msg);
                Fail("Expected exception was not thrown");
            }
            catch (Exception ex)
            {
                AreSame(ex.Message, msg);
            }
        }

        [TestMethod]
        public void IsOkAndTest()
        {
            Result<object, Exception> result = new Ok<object, Exception>(_testValue);
            Func<object, bool> predicate = (object v) => v is not null;
            IsTrue(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndFalseTest()
        {
            Result<object, Exception> result = new Ok<object, Exception>(_testValue);
            Func<object, bool> predicate = (object v) => v is null;
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsErrorAndTest()
        {
            Result<object, Exception> result = new Error<object, Exception>(_testError);
            Func<Exception, bool> predicate = (Exception e) => e.Message == _testError.Message;
            IsTrue(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void IsErrorAndFalseTest()
        {
            Result<object, Exception> result = new Error<object, Exception>(_testError);
            Func<Exception, bool> predicate = (Exception e) => e.Message == "Different Error";
            IsFalse(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void IsErrorAndNullTest()
        {
            Result<object, Exception> result = new Error<object, Exception>(_testError);
            Func<Exception, bool> predicate = (Exception e) => e is null;
            IsFalse(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndNullTest()
        {
            Result<object, Exception> result = new Ok<object, Exception>(_testValue);
            Func<object, bool> predicate = (object v) => v is null;
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndNullPredicateTest()
        {
            Result<object, Exception> result = new Ok<object, Exception>(_testValue);
            Func<object, bool> predicate = (object v) => v is null;
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsErrorAndNullPredicateTest()
        {
            Result<object, Exception> result = new Error<object, Exception>(_testError);
            Func<Exception, bool> predicate = (Exception e) => e is null;
            IsFalse(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void OkTest()
        {
            Result<object, Exception> result = new Ok<object, Exception>(_testValue);
            IsTrue(result.Ok.Value == _testValue);
        }

        [TestMethod]
        public void OkNullTest()
        {
            Result<object, Exception> result = new Error<object, Exception>(_testError);
            IsFalse(result.Ok.HasValue);
        }

        [TestMethod]
        public void ErrorTest()
        {
            Result<object, Exception> result = new Error<object, Exception>(_testError);
            IsTrue(result.Error.Value == _testError);
        }

        [TestMethod]
        public void ErrorNullTest()
        {
            Result<object, Exception> result = new Ok<object, Exception>(_testValue);
            IsFalse(result.Error.HasValue);
        }

        [TestMethod]
        public void MapTest()
        {
            Result<object, Exception> result = new Ok<object, Exception>(_testValue);
            Result<string, Exception> mappedResult = result.Map(v => v.ToString()!);
            IsTrue(mappedResult.IsOk);
            AreNotSame(_testValue, mappedResult.Unwrap);
        }

        [TestMethod]
        public void MapErrorTest()
        {
            Result<object, Exception> result = new Error<object, Exception>(_testError);
            Result<string, Exception> mappedResult = result.Map(v => v.ToString()!);
            IsTrue(mappedResult.IsError);
            try
            {
                _ = mappedResult.Unwrap;
                Fail("Expected exception was not thrown");
            }
            catch (Exception ex)
            {
                AreSame(_testError, ex);
            }
        }

        [TestMethod]
        public void MapOrTest()
        {
            Result<object, Exception> result = new Ok<object, Exception>(_testValue);
            object mappedResult = result.MapOr(v => v.ToString()!, new object());
            AreSame(_testValue.ToString(), mappedResult);
        }

        [TestMethod]
        public void MapOrNullTest()
        {
            Result<object, Exception> result = new Error<object, Exception>(_testError);
            object mappedResult = result.MapOr(v => v.ToString()!, _testValue);
            AreSame(_testValue, mappedResult);
        }

        [TestMethod]
        public void MapOrElseTest()
        {
            Result<object, Exception> result = new Ok<object, Exception>(_testValue);
            string mappedResult = result.MapOrElse(v => v.ToString()!, e => e.Message);
            AreSame(_testValue.ToString(), mappedResult);
        }

        [TestMethod]
        public void MapOrElseNullTest()
        {
            Result<object, Exception> result = new Error<object, Exception>(_testError);
            string mappedResult = result.MapOrElse(v => v.ToString()!, e => e.Message);
            AreSame(_testError.Message, mappedResult);
        }

        [TestMethod]
        public void MapErrorDiffTest()
        {
            Result<object, Exception> result = new Error<object, Exception>(_testError);
            const string message = "Mapped Error";
            Result<object, Exception> mappedResult = result.MapError(e => new Exception(message));
            IsTrue(mappedResult.IsError);
            try
            {
                _ = mappedResult.Unwrap;
                Fail("Expected exception was not thrown");
            }
            catch (Exception ex)
            {
                AreNotSame(_testError, ex);
                AreSame(ex.Message, message);
            }
        }

        [TestMethod]
        public void MapErrorNullTest()
        {
            Result<object, Exception> result = new Ok<object, Exception>(_testValue);
            Result<object, Exception> mappedResult = result.MapError(e => new Exception("Mapped Error"));
            IsTrue(mappedResult.IsOk);
            AreSame(_testValue, mappedResult.Unwrap);
        }

        [TestMethod]
        public void TryExecuteSucceedTest()
        {
            Func<string> testFunc = static () => "ahoj";
            Result<string, Exception> result = testFunc.TryExecute<string, Exception>();
            IsTrue(result.IsOk);
        }

        [TestMethod]
        public void TryExecuteFailTest()
        {
            Func<string> testFunc = static () => false ? string.Empty : throw new InvalidOperationException();
            Result<string, InvalidOperationException> result = testFunc.TryExecute<string, InvalidOperationException>();
            IsTrue(result.IsError);
        }

        [TestMethod]
        public void TryExecuteUnexpectedTest()
        {
            ArgumentException exception = new();
            Func<string> testFunc = () => false ? string.Empty : throw exception;
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
