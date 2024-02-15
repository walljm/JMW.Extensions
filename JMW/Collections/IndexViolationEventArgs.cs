using System;

namespace JMW.Collections;

public class IndexViolationEventArgs<T> : EventArgs
{
    public T Item { get; }
    public string ErrorMessage { get; }

    public IndexViolationEventArgs(T item, string err)
    {
        Item = item;
        ErrorMessage = err;
    }
}