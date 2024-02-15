using System;

namespace JMW.Functional;

/// <summary>
/// Immutable <see cref="Optional{T}"/> type. This is used in cases where you might have a null reference
/// It forces the user to be aware of this fact and includes some Monadic tools to help
/// you deal with non existent values. This has the pleasant side effect of making null references
/// a first class case in your code.
/// </summary>
/// <remarks>
/// Since <see cref="Optional{T}"/> is a struct it is impossible to construct null references to it.
/// this means that to construct the non existent default case you should use the Static factory None()
/// method.
/// </remarks>
public readonly struct Optional<T> where T : class
{
    /// <summary>
    /// The base case of an <see cref="Optional{T}"/>. The Value will be unassigned and Exists
    /// will be false.
    /// </summary>
    public static Optional<T> None()
    {
        return new Optional<T>(null);
    }

    private readonly T? internalValue;

    public Optional(T? val)
    {
        internalValue = val;
    }

    public readonly bool Exists => internalValue is not null;

    /// <summary>
    /// Returns the value. This property may be null. Check Exists first to ensure that
    /// there is something to retrieve here. Better yet use the Monadic Do method
    /// to run your computation safely.
    /// </summary>
    /// <value>The value.</value>
    public readonly T? Value => internalValue;

    /// <summary>
    /// Monadic Do for this Optional Type.
    ///
    /// If the value exists then runs the function with the value as an argument.
    /// otherwise it is a noop.
    /// </summary>
    /// <param name="fn">Fn.</param>
    public readonly void Do(Action<T>? fn)
    {
        if (Exists && Value is not null && fn is not null) fn(Value);
    }

    ///  <summary>
    ///  Monadic Do for this Optional Type.
    ///
    ///  If the value exists then runs the function with the value as an argument.
    ///  otherwise it is a noop.
    ///  </summary>
    ///  <param name="fn">Fn.</param>
    /// <param name="if_null">The action to run if the value is null</param>
    public readonly void Do(Action<T?> fn, Action if_null)
    {
        if (Exists) fn(Value);
        else if_null();
    }

    /// <summary>
    /// Monadic Do for this <see cref="Optional{T}"/> that can result in an Optional
    /// return. We keep the results wrapped in <see cref="Optional{R}"/> which makes
    /// chaining Do calls completely safe even if your <see cref="Func{T, R}"/> returns
    /// a null.
    /// </summary>
    /// <param name="fn">Fn.</param>
    /// <typeparam name="R">The 1st type parameter.</typeparam>
    public readonly Optional<R> Do<R>(Func<T?, R> fn) where R : class
    {
        if (Exists)
            return new Optional<R>(fn(Value));
        // No Op so return the None base case for this Optional<R> return type.
        return Optional<R>.None();
    }

    /// <summary>
    /// Monadic Do for this <see cref="Optional{T}"/>. The user is responsible for handling the null case.
    /// </summary>
    /// <param name="fn">Fn.</param>
    /// <param name="if_null"></param>
    /// <typeparam name="R">The 1st type parameter.</typeparam>
    public readonly R Do<R>(Func<T?, R> fn, Func<R> if_null) where R : class
    {
        if (Exists)
            return fn(Value);

        // No Op so return the None base case for this Optional<R> return type.
        return if_null();
    }
}