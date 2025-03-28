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
            Ok<object, Exception> result = new(_testValue);
            IsTrue(result.IsOk);
        }

        [TestMethod]
        public void IsErrorTest()
        {
            Error<object, Exception> result = new(_testError);
            IsTrue(result.IsError);
        }

        [TestMethod]
        public void UnwrapTest()
        {
            Ok<object, Exception> result = new(_testValue);
            AreSame(_testValue, result.Unwrap);
        }

        [TestMethod]
        public void UnwrapErrorTest()
        {
            Error<object, Exception> result = new(_testError);
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
            Ok<object, Exception> result = new(_testValue);
            result.Expect("Error");
            AreSame(_testValue, result.Unwrap);
        }

        [TestMethod]
        public void ExpectErrorTest()
        {
            string msg = "Error";
            Error<object, Exception> result = new(_testError);
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
            Ok<object, Exception> result = new(_testValue);
            Func<object, bool> predicate = (object v) => v is not null;
            IsTrue(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndFalseTest()
        {
            Ok<object, Exception> result = new(_testValue);
            Func<object, bool> predicate = (object v) => v is null;
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsErrorAndTest()
        {
            Error<object, Exception> result = new(_testError);
            Func<Exception, bool> predicate = (Exception e) => e.Message == _testError.Message;
            IsTrue(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void IsErrorAndFalseTest()
        {
            Error<object, Exception> result = new(_testError);
            Func<Exception, bool> predicate = (Exception e) => e.Message == "Different Error";
            IsFalse(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void IsErrorAndNullTest()
        {
            Error<object, Exception> result = new(_testError);
            Func<Exception, bool> predicate = (Exception e) => e is null;
            IsFalse(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndNullTest()
        {
            Ok<object, Exception> result = new(_testValue);
            Func<object, bool> predicate = (object v) => v is null;
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndNullPredicateTest()
        {
            Ok<object, Exception> result = new(_testValue);
            Func<object, bool> predicate = (object v) => v is null;
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsErrorAndNullPredicateTest()
        {
            Error<object, Exception> result = new(_testError);
            Func<Exception, bool> predicate = (Exception e) => e is null;
            IsFalse(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void OkTest()
        {
            Ok<object, Exception> result = new(_testValue);
            IsTrue(result.Ok.Value == _testValue);
        }

        [TestMethod]
        public void OkNullTest()
        {
            Error<object, Exception> result = new(_testError);
            IsFalse(result.Ok.HasValue);
        }

        [TestMethod]
        public void ErrorTest()
        {
            Error<object, Exception> result = new(_testError);
            IsTrue(result.Error.Value == _testError);
        }

        [TestMethod]
        public void ErrorNullTest()
        {
            Ok<object, Exception> result = new(_testValue);
            IsFalse(result.Error.HasValue);
        }

        [TestMethod]
        public void MapTest()
        {
            Ok<object, Exception> result = new(_testValue);
            Result<string, Exception> mappedResult = result.Map(v => v.ToString()!);
            IsTrue(mappedResult.IsOk);
            AreNotSame(_testValue, mappedResult.Unwrap);
        }

        [TestMethod]
        public void MapErrorTest()
        {
            Error<object, Exception> result = new(_testError);
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
            Ok<object, Exception> result = new(_testValue);
            object mappedResult = result.MapOr(v => v.ToString()!, new object());
            AreSame(_testValue.ToString(), mappedResult);
        }

        [TestMethod]
        public void MapOrNullTest()
        {
            Error<object, Exception> result = new(_testError);
            object mappedResult = result.MapOr(v => v.ToString()!, _testValue);
            AreSame(_testValue, mappedResult);
        }

        [TestMethod]
        public void MapOrElseTest()
        {
            Ok<object, Exception> result = new(_testValue);
            string mappedResult = result.MapOrElse(v => v.ToString()!, e => e.Message);
            AreSame(_testValue.ToString(), mappedResult);
        }

        [TestMethod]
        public void MapOrElseNullTest()
        {
            Error<object, Exception> result = new(_testError);
            string mappedResult = result.MapOrElse(v => v.ToString()!, e => e.Message);
            AreSame(_testError.Message, mappedResult);
        }

        [TestMethod]
        public void MapErrorDiffTest()
        {
            Error<object, Exception> result = new(_testError);
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
            Ok<object, Exception> result = new(_testValue);
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
