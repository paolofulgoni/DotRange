using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace SharpInterval;

internal class Cut
{
    internal static Cut<T> BelowAll<T>() where T : IComparable<T>
    {
        return Cut<T>.BelowAll.INSTANCE;
    }

    internal static Cut<T> AboveAll<T>() where T : IComparable<T>
    {
        return Cut<T>.AboveAll.INSTANCE;
    }

    internal static Cut<T> BelowValue<T>(T endpoint) where T : IComparable<T>
    {
        return new Cut<T>.BelowValue(endpoint);
    }

    internal static Cut<T> AboveValue<T>(T endpoint) where T : IComparable<T>
    {
        return new Cut<T>.AboveValue(endpoint);
    }
}

/// <summary>
/// Implementation detail for the internal structure of <seealso cref="Interval{T}"/> instances. Represents a unique
/// way of "cutting" a "number line" (actually of instances of type <typeparamref name="T"/>, not necessarily
/// "numbers") into two sections; this can be done below a certain value, above a certain value,
/// below all values or above all values. With this object defined in this way, an interval can
/// always be represented by a pair of <seealso cref="Cut{T}"/> instances.
/// </summary>
[Serializable]
internal abstract class Cut<T> : IComparable<Cut<T>> where T : IComparable<T>
{
    internal readonly T _endpoint;

    internal Cut(T endpoint)
    {
        _endpoint = endpoint;
    }

    internal abstract bool IsLessThan(T value);

    internal abstract BoundType TypeAsLowerBound();

    internal abstract BoundType TypeAsUpperBound();

    internal abstract void DescribeAsLowerBound(StringBuilder sb);

    internal abstract void DescribeAsUpperBound(StringBuilder sb);

    // note: overridden by {BELOW,ABOVE}_ALL
    public virtual int CompareTo(Cut<T>? that)
    {
        if (that is null)
        {
            return 1;
        }
        if (that == BelowAll.INSTANCE)
        {
            return 1;
        }
        if (that == AboveAll.INSTANCE)
        {
            return -1;
        }
        int result = _endpoint.CompareTo(that._endpoint);
        if (result != 0)
        {
            return result;
        }
        // same value. below comes before above
        return (this is AboveValue).CompareTo(that is AboveValue);
    }

    internal virtual T Endpoint()
    {
        return _endpoint;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Cut<T> that)
        {
            int compareResult = CompareTo(that);
            return compareResult == 0;
        }
        return false;
    }

    // Prevent "missing hashCode" warning by explicitly forcing subclasses implement it
    public override abstract int GetHashCode();


    [Serializable]
    public sealed class BelowAll : Cut<T>
    {
        public static readonly BelowAll INSTANCE = new BelowAll();

        internal BelowAll() : base(default!)
        {
        }
        internal override T Endpoint()
        {
            throw new InvalidOperationException("interval unbounded on this side");
        }
        internal override bool IsLessThan(T value)
        {
            return true;
        }
        internal override BoundType TypeAsLowerBound()
        {
            throw new InvalidOperationException();
        }
        internal override BoundType TypeAsUpperBound()
        {
            throw new InvalidOperationException("this statement should be unreachable");
        }
        internal override void DescribeAsLowerBound(StringBuilder sb)
        {
            sb.Append("(-\u221e");
        }
        internal override void DescribeAsUpperBound(StringBuilder sb)
        {
            throw new InvalidOperationException();
        }
        public override int CompareTo(Cut<T>? o)
        {
            return (o is BelowAll) ? 0 : -1;
        }
        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }
        public override string ToString()
        {
            return "-\u221e";
        }
    }

    [Serializable]
    public sealed class AboveAll : Cut<T>
    {
        internal static readonly AboveAll INSTANCE = new AboveAll();

        internal AboveAll() : base(default!)
        {
        }
        internal override T Endpoint()
        {
            throw new InvalidOperationException("interval unbounded on this side");
        }
        internal override bool IsLessThan(T value)
        {
            return false;
        }
        internal override BoundType TypeAsLowerBound()
        {
            throw new InvalidOperationException("this statement should be unreachable");
        }
        internal override BoundType TypeAsUpperBound()
        {
            throw new InvalidOperationException();
        }
        internal override void DescribeAsLowerBound(StringBuilder sb)
        {
            throw new InvalidOperationException();
        }
        internal override void DescribeAsUpperBound(StringBuilder sb)
        {
            sb.Append("+\u221e)");
        }
        public override int CompareTo(Cut<T>? o)
        {
            return (o is AboveAll) ? 0 : 1;
        }
        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }
        public override string ToString()
        {
            return "+\u221e";
        }
    }

    [Serializable]
    public sealed class BelowValue : Cut<T>
    {
        internal BelowValue(T endpoint) : base(endpoint)
        {
        }
        internal override bool IsLessThan(T value)
        {
            return _endpoint.CompareTo(value) <= 0;
        }
        internal override BoundType TypeAsLowerBound()
        {
            return BoundType.Closed;
        }
        internal override BoundType TypeAsUpperBound()
        {
            return BoundType.Open;
        }
        internal override void DescribeAsLowerBound(StringBuilder sb)
        {
            sb.Append('[').Append(_endpoint);
        }
        internal override void DescribeAsUpperBound(StringBuilder sb)
        {
            sb.Append(_endpoint).Append(')');
        }
        public override int GetHashCode()
        {
            return _endpoint.GetHashCode();
        }
        public override string ToString()
        {
            return "\\" + _endpoint + "/";
        }
    }

    [Serializable]
    public sealed class AboveValue : Cut<T>
    {
        internal AboveValue(T endpoint) : base(endpoint)
        {
        }
        internal override bool IsLessThan(T value)
        {
            return _endpoint.CompareTo(value) < 0;
        }
        internal override BoundType TypeAsLowerBound()
        {
            return BoundType.Open;
        }
        internal override BoundType TypeAsUpperBound()
        {
            return BoundType.Closed;
        }
        internal override void DescribeAsLowerBound(StringBuilder sb)
        {
            sb.Append('(').Append(_endpoint);
        }
        internal override void DescribeAsUpperBound(StringBuilder sb)
        {
            sb.Append(_endpoint).Append(']');
        }
        public override int GetHashCode()
        {
            return ~_endpoint.GetHashCode();
        }
        public override string ToString()
        {
            return "/" + _endpoint + "\\";
        }
    }
}
