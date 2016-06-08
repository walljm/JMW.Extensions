using System;

namespace JMW.AlgebraicTypes
{
    public abstract class Either<L, R>
    {
        // Subclass implementation calls the appropriate continuation.
        public abstract T Match<T>(Func<L, T> Left, Func<R, T> Right);

        // Convenience wrapper for when the caller doesn't want to return a value
        // from the match expression.
        public void Match(Action<L> Left, Action<R> Right)
        {
            this.Match<int>(
                Left: x => { Left(x); return 0; },
                Right: x => { Right(x); return 0; }
            );
        }
    }
}