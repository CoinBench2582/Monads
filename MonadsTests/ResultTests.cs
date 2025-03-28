using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Monads.Tests
{
    [TestClass]
    public class ResultTests
    {
        [TestMethod]
        public void IsOkTest() {
            var result = new Ok<Object,Exception>(new Object());
            IsTrue(result.IsOk);
        }

        [TestMethod]
        public void IsErrTest() {
            var result = new Err<Object,Exception>(new Exception("Error"));
            IsTrue(result.IsErr);
        }

        [TestMethod]
        public void UnwrapTest() {
            var value = new Object();
            var result = new Ok<Object,Exception>(value);
            AreSame(value, result.Unwrap);
        }

        [TestMethod]
        public void UnwrapErrTest() {
            var error = new Exception("Error");
            var result = new Err<Object,Exception>(error);
            try {
                var _ = result.Unwrap;
                Fail("Expected exception was not thrown");
            } catch (Exception ex) {
                AreSame(error, ex);
            }
        }

        [TestMethod]
        public void ExpectTest() {
            var value = new Object();
            var result = new Ok<Object,Exception>(value);
            result.Expect("Error");
            AreSame(value, result.Unwrap);
        }

        [TestMethod]
        public void ExpectErrTest() {
            var msg = "Error";
            var error = new Exception("Some error...");
            var result = new Err<Object,Exception>(error);
            try {
                result.Expect(msg);
                Fail("Expected exception was not thrown");
            } catch (Exception ex) {
                AreSame(ex.Message, msg);
            }
        }

        [TestMethod]
        public void IsOkAndTest() {
            var value = new Object();
            var result = new Ok<Object,Exception>(value);
            var predicate = new Func<Object,bool>(v => v is not null);
            IsTrue(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndFalseTest() {
            var value = new Object();
            var result = new Ok<Object,Exception>(value);
            var predicate = new Func<Object,bool>(v => v is null);
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsErrAndTest() {
            var error = new Exception("Error");
            var result = new Err<Object,Exception>(error);
            var predicate = new Func<Exception,bool>(e => e.Message == "Error");
            IsTrue(result.IsErrAnd(predicate));
        }

        [TestMethod]
        public void IsErrAndFalseTest() {
            var error = new Exception("Error");
            var result = new Err<Object,Exception>(error);
            var predicate = new Func<Exception,bool>(e => e.Message == "Different Error");
            IsFalse(result.IsErrAnd(predicate));
        }

        [TestMethod]
        public void IsErrAndNullTest() {
            var error = new Exception("Error");
            var result = new Err<Object,Exception>(error);
            var predicate = new Func<Exception,bool>(e => e is null);
            IsFalse(result.IsErrAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndNullTest() {
            var value = new Object();
            var result = new Ok<Object,Exception>(value);
            var predicate = new Func<Object,bool>(v => v is null);
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsOkAndNullPredicateTest() {
            var value = new Object();
            var result = new Ok<Object,Exception>(value);
            var predicate = new Func<Object,bool>(v => v is null);
            IsFalse(result.IsOkAnd(predicate));
        }

        [TestMethod]
        public void IsErrAndNullPredicateTest() {
            var error = new Exception("Error");
            var result = new Err<Object,Exception>(error);
            var predicate = new Func<Exception,bool>(e => e is null);
            IsFalse(result.IsErrAnd(predicate));
        }

        [TestMethod]
        public void OkTest() {
            var value = new Object();
            var result = new Ok<Object,Exception>(value);
            IsTrue(result.Ok.Value == value);
        }

        [TestMethod]
        public void OkTestNull() {
            var result = new Err<Object,Exception>(new Exception("Error"));
            IsFalse(result.Ok.HasValue);
        }

        [TestMethod]
        public void ErrTest() {
            var error = new Exception("Error");
            var result = new Err<Object,Exception>(error);
            IsTrue(result.Err.Value == error);
        }

        [TestMethod]
        public void ErrTestNull() {
            var result = new Ok<Object,Exception>(new Object());
            IsFalse(result.Err.HasValue);
        }

        [TestMethod]
        public void MapTest() {
            var value = new Object();
            var result = new Ok<Object,Exception>(value);
            var mappedResult = result.Map(v => v.ToString()!);
            IsTrue(mappedResult.IsOk);
            AreNotSame(value, mappedResult.Unwrap);
        }

        [TestMethod]
        public void MapErrorTest() {
            var error = new Exception("Error");
            var result = new Err<Object,Exception>(error);
            var mappedResult = result.Map(v => v.ToString()!);
            IsTrue(mappedResult.IsErr);
            try {
                var _ = mappedResult.Unwrap;
                Fail("Expected exception was not thrown");
            } catch (Exception ex) {
                AreSame(error, ex);
            }
        }

        [TestMethod]
        public void MapOrTest() {
            var value = new Object();
            var result = new Ok<Object,Exception>(value);
            var mappedResult = result.MapOr(v => v.ToString()!, new Object());
            AreSame(value.ToString(), mappedResult);
        }

        [TestMethod]
        public void MapOrTestNull() {
            var error = new Exception("Error");
            var defaultValue = new Object();
            var result = new Err<Object,Exception>(error);
            var mappedResult = result.MapOr(v => v.ToString()!, defaultValue);
            AreSame(defaultValue, mappedResult);
        }

        [TestMethod]
        public void MapOrElseTest() {
            var value = new Object();
            var result = new Ok<Object,Exception>(value);
            var mappedResult = result.MapOrElse(v => v.ToString()!, e => e.Message);
            AreSame(value.ToString(), mappedResult);
        }

        [TestMethod]
        public void MapOrElseTestNull() {
            var error = new Exception("Error");
            var result = new Err<Object,Exception>(error);
            var mappedResult = result.MapOrElse(v => v.ToString()!, e => e.Message);
            AreSame(error.Message, mappedResult);
        }

        [TestMethod]
        public void MapErrTest() {
            var error = new Exception("Error");
            var result = new Err<Object,Exception>(error);
            var mappedResult = result.MapErr(e => new Exception("Mapped Error"));
            IsTrue(mappedResult.IsErr);
            try {
                var _ = mappedResult.Unwrap;
                Fail("Expected exception was not thrown");
            } catch (Exception ex) {
                AreNotSame(error, ex);
                AreSame(ex.Message, "Mapped Error");
            }
        }

        [TestMethod]
        public void MapErrTestNull() {
            var value = new Object();
            var result = new Ok<Object,Exception>(value);
            var mappedResult = result.MapErr(e => new Exception("Mapped Error"));
            IsTrue(mappedResult.IsOk);
            AreSame(value, mappedResult.Unwrap);
        }
    }
}
