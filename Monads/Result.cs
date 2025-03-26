namespace Monads
{
    class Result<T,E> where E:Exception where T:class {
        T? value;
        E? error;

        private protected Result(T v) => value = v;
        private protected Result(E e) => error = e;

        public bool IsOk => error is null;
        public bool IsErr => !IsOk;

        public T Unwrap => error is not null ? throw error : value!;

        public void Expect(string msg) {
            if (error is not null) {
                throw new Exception(msg);
            }
        }

        public bool IsOkAnd(Func<T,bool> f) => IsOk && value is not null ? f(value) : false;
        public bool IsErrAnd(Func<E,bool> f) => IsErr && error is not null ? f(error) : false;

        public Option<T> Ok => value is not null ? new Some<T>(value) : new None<T>();
        public Option<E> Err => error is not null ? new Some<E>(error) : new None<E>();

        public Result<U,E> Map<U>(Func<T,U> op) where U:class => IsOk ? new Ok<U,E>(op(value!)) : new Err<U,E>(error!);
        public U MapOr<U>(Func<T,U> op, U def) => IsOk ? op(value!) : def;
        public U MapOrElse<U>(Func<T,U> op, Func<E,U> cb) => IsOk ? op(value!) : cb(error!);
        public Result<T,F> MapErr<F>(Func<E,F> op) where F:Exception => IsOk ? new Ok<T,F>(value!) : new Err<T,F>(op(error!));

        public Result<T,E> Inspect(Action<T> op) {
            if (IsOk) {
                op(value!);
            }
            return this;
        }

        public Result<T,E> InspectErr(Action<E> op) {
            if (!IsOk) {
                op(error!);
            }
            return this;
        }

        public static Result<T,E> Exec(Func<T> f) {
            try {
                return new Ok<T,E>(f());
            } catch (E e) {
                return new Err<T,E>(e);
            }
        }
    }

    class Ok<T,E> : Result<T,E> where E:Exception where T:class {
        public Ok(T v) : base(v) {}
    }

    class Err<T,E> : Result<T,E> where E:Exception where T:class {
        public Err(E e) : base(e) {}
    }
}
