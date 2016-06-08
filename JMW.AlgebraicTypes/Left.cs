using System;

namespace JMW.AlgebraicTypes
{
    public class Left<L, R> : Either<L, R>
    {
        private L Value { get; set; }

        public Left(L Value)
        {
            this.Value = Value;
        }

        public override T Match<T>(Func<L, T> Left, Func<R, T> Right)
        {
            return Left(Value);
        }
    }
}