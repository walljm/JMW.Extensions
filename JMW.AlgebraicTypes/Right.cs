using System;

namespace JMW.AlgebraicTypes
{
    public class Right<L, R> : Either<L, R>
    {
        private R Value { get; set; }

        public Right(R Value)
        {
            this.Value = Value;
        }

        public override T Match<T>(Func<L, T> Left, Func<R, T> Right)
        {
            return Right(Value);
        }
    }
}