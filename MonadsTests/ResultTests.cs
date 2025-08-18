#pragma warning disable IDE0039 // Použít lokální funkci

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
            Result<object, Exception> result = Result<object, Exception>.Ok(_testValue);
            IsTrue(result.IsOk);
        }

        [TestMethod]
        public void IsErrorTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Failure(_testError);
            IsTrue(result.IsError);
        }

        [TestMethod]
        public void UnwrapTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Ok(_testValue);
            AreSame(_testValue, result.UnwrapValue);
        }

        [TestMethod]
        public void UnwrapErrorTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Failure(_testError);
            _ = ThrowsExactly<InvalidOperationException>(() => _ = result.UnwrapValue);
            AreSame(_testError, result.UnwrapError);
        }

        [TestMethod]
        public void ExpectTest()
        {
#pragma warning disable CS0618 // Typ nebo èlen je zastaralý.
            Result<object, Exception> result = Result<object, Exception>.Ok(_testValue);
            result.Expect("Error");
            AreSame(_testValue, result.UnwrapValue);
#pragma warning restore CS0618 // Typ nebo èlen je zastaralý.
        }

        [TestMethod]
        public void ExpectErrorTest()
        {
#pragma warning disable CS0618 // Typ nebo èlen je zastaralý.
            const string msg = "Error";
            Result<object, Exception> result = Result<object, Exception>.Failure(_testError);
            try
            {
                result.Expect(msg);
                Fail("Expected exception was not thrown");
            }
            catch (Exception ex)
            {
                AreSame(msg, ex.Message);
                AreSame(_testError, ex.InnerException);
            }
#pragma warning restore CS0618 // Typ nebo èlen je zastaralý.
        }

        [TestMethod]
        public void IsOkAndTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Ok(_testValue);
            Func<object, bool> predicate = static v => v is not null;
            IsTrue(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndFalseTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Ok(_testValue);
            object different = new();
            Func<object, bool> predicate = v => v == different;
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndNullTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Ok(_testValue);
            Func<object, bool> predicate = static v => v is null;
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsErrorAndTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Failure(_testError);
            Func<Exception, bool> predicate = static e => e.Message == _testError.Message;
            IsTrue(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void IsErrorAndFalseTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Failure(_testError);
            Func<Exception, bool> predicate = static e => e.Message == "Different Error";
            IsFalse(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void IsErrorAndNullTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Failure(_testError);
            Func<Exception, bool> predicate = static e => e is null;
            IsFalse(result.IsErrorAnd(predicate));
        }

        [TestMethod]
        public void OkTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Ok(_testValue);
            AreEqual(_testValue, result.Value.Value);
        }

        [TestMethod]
        public void OkNullTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Failure(_testError);
            IsFalse(result.Value.HasValue);
        }

        [TestMethod]
        public void ErrorTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Failure(_testError);
            AreEqual(_testError, result.Error.Value);
        }

        [TestMethod]
        public void ErrorNullTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Ok(_testValue);
            IsFalse(result.Error.HasValue);
        }

        [TestMethod]
        public void MapTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Ok(_testValue);
            Result<string, Exception> mappedResult = result.Map(v => v.ToString()!);
            IsTrue(mappedResult.IsOk);
            AreNotSame(_testValue, mappedResult.UnwrapValue);
        }

        [TestMethod]
        public void MapErrorTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Failure(_testError);
            Result<string, Exception> mappedResult = result.Map(v => v.ToString()!);
            IsTrue(mappedResult.IsError);
            _ = ThrowsExactly<InvalidOperationException>(() => _ = mappedResult.UnwrapValue);
            AreSame(_testError, mappedResult.UnwrapError);
        }

        [TestMethod]
        public void MapOrTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Ok(_testValue);
            object mappedResult = result.MapOr(v => v.ToString()!, new object());
            AreSame(_testValue.ToString(), mappedResult);
        }

        [TestMethod]
        public void MapOrNullTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Failure(_testError);
            object mappedResult = result.MapOr(v => v.ToString()!, _testValue);
            AreSame(_testValue, mappedResult);
        }

        [TestMethod]
        public void MapOrElseTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Ok(_testValue);
            string mappedResult = result.MapOrElse(v => v.ToString()!, e => e.Message);
            AreSame(_testValue.ToString(), mappedResult);
        }

        [TestMethod]
        public void MapOrElseNullTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Failure(_testError);
            string mappedResult = result.MapOrElse(v => v.ToString()!, e => e.Message);
            AreSame(_testError.Message, mappedResult);
        }

        [TestMethod]
        public void MapErrorDiffTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Failure(_testError);
            const string message = "Mapped Error";
            Result<object, Exception> mappedResult = result.MapError(e => new Exception(message));
            IsTrue(mappedResult.IsError);
            _ = ThrowsExactly<InvalidOperationException>(() => _ = mappedResult.UnwrapValue);
            AreNotSame(_testError, mappedResult.UnwrapError);
            AreSame(message, mappedResult.UnwrapError.Message);
        }

        [TestMethod]
        public void MapErrorNullTest()
        {
            Result<object, Exception> result = Result<object, Exception>.Ok(_testValue);
            Result<object, Exception> mappedResult = result.MapError(e => new Exception("Mapped Error"));
            IsTrue(mappedResult.IsOk);
            AreSame(_testValue, mappedResult.UnwrapValue);
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
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one
            // Intentional
            ArgumentException exception = new();
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
            Func<string> testFunc = () => false ? string.Empty : throw exception;
            Result<string, InvalidOperationException>? result = default;
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
#pragma warning restore IDE0039 // Použít lokální funkci