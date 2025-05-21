using System;
using System.Collections.Generic;
using System.Text;

namespace SharpInterval;

/// <summary>
/// Provides factory methods for creating <see cref="Interval{C}"/> instances.
/// </summary>
/// <remarks>
/// This non-generic class only exposes these factories. The actual interval
/// representation is <see cref="Interval{C}"/>.
/// </remarks>
public static class Interval
{
    /// <summary>
    /// Returns an interval that contains all values strictly greater than <paramref name="lower"/> and strictly less
    /// than <paramref name="upper"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than or equal to <paramref name="upper"/>
    /// </exception>
    public static Interval<C> Open<C>(C lower, C upper) where C : IComparable<C>
    {
        return new Interval<C>(Cut.AboveValue(lower), Cut.BelowValue(upper));
    }

    /// <summary>
    /// Returns an interval that contains all values greater than or equal to <paramref name="lower"/> and less than
    /// or equal to <paramref name="upper"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than <paramref name="upper"/>
    /// </exception>
    public static Interval<C> Closed<C>(C lower, C upper) where C : IComparable<C>
    {
        return new Interval<C>(Cut.BelowValue(lower), Cut.AboveValue(upper));
    }

    /// <summary>
    /// Returns an interval that contains all values greater than or equal to <paramref name="lower"/> and strictly
    /// less than <paramref name="upper"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than <paramref name="upper"/>
    /// </exception>
    public static Interval<C> ClosedOpen<C>(C lower, C upper) where C : IComparable<C>
    {
        return new Interval<C>(Cut.BelowValue(lower), Cut.BelowValue(upper));
    }

    /// <summary>
    /// Returns an interval that contains all values strictly greater than <paramref name="lower"/> and less than or
    /// equal to <paramref name="upper"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than <paramref name="upper"/>
    /// </exception>
    public static Interval<C> OpenClosed<C>(C lower, C upper) where C : IComparable<C>
    {
        return new Interval<C>(Cut.AboveValue(lower), Cut.AboveValue(upper));
    }

    /// <summary>
    /// Returns an interval that contains any value from <paramref name="lower"/> to <paramref name="upper"/>, where each
    /// endpoint may be either inclusive (closed) or exclusive (open).
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than <paramref name="upper"/>
    /// </exception>
    public static Interval<C> Bounded<C>(C lower, BoundType lowerType, C upper, BoundType upperType) where C : IComparable<C>
    {
        Cut<C> lowerBound = (lowerType == BoundType.Open) ? Cut.AboveValue(lower) : Cut.BelowValue(lower);
        Cut<C> upperBound = (upperType == BoundType.Open) ? Cut.BelowValue(upper) : Cut.AboveValue(upper);
        return new Interval<C>(lowerBound, upperBound);
    }

    /// <summary>
    /// Returns an interval that contains all values strictly less than <paramref name="endpoint"/>.
    /// </summary>
    public static Interval<C> LessThan<C>(C endpoint) where C : IComparable<C>
    {
        return new Interval<C>(Cut.BelowAll<C>(), Cut.BelowValue(endpoint));
    }

    /// <summary>
    /// Returns an interval that contains all values less than or equal to <paramref name="endpoint"/>.
    /// </summary>
    public static Interval<C> AtMost<C>(C endpoint) where C : IComparable<C>
    {
        return new Interval<C>(Cut.BelowAll<C>(), Cut.AboveValue(endpoint));
    }

    /// <summary>
    /// Returns an interval with no lower bound up to the given endpoint, which may be either inclusive
    /// (closed) or exclusive (open).
    /// </summary>
    public static Interval<C> UpTo<C>(C endpoint, BoundType boundType) where C : IComparable<C>
    {
        switch (boundType)
        {
            case BoundType.Open:
                return LessThan(endpoint);
            case BoundType.Closed:
                return AtMost(endpoint);
            default:
                throw new ArgumentOutOfRangeException(nameof(boundType), boundType, "Invalid bound type");
        }
    }

    /// <summary>
    /// Returns an interval that contains all values strictly greater than <paramref name="endpoint"/>.
    /// </summary>
    public static Interval<C> GreaterThan<C>(C endpoint) where C : IComparable<C>
    {
        return new Interval<C>(Cut.AboveValue(endpoint), Cut.AboveAll<C>());
    }

    /// <summary>
    /// Returns an interval that contains all values greater than or equal to <paramref name="endpoint"/>.
    /// </summary>
    public static Interval<C> AtLeast<C>(C endpoint) where C : IComparable<C>
    {
        return new Interval<C>(Cut.BelowValue(endpoint), Cut.AboveAll<C>());
    }

    /// <summary>
    /// Returns an interval from the given endpoint, which may be either inclusive (closed) or exclusive
    /// (open), with no upper bound.
    /// </summary>
    public static Interval<C> DownTo<C>(C endpoint, BoundType boundType) where C : IComparable<C>
    {
        switch (boundType)
        {
            case BoundType.Open:
                return GreaterThan(endpoint);
            case BoundType.Closed:
                return AtLeast(endpoint);
            default:
                throw new ArgumentOutOfRangeException(nameof(boundType), boundType, "Invalid bound type");
        }
    }

    /// <summary>
    /// Returns an interval that contains every value of type <typeparamref name="C"/>.
    /// </summary>
    public static Interval<C> All<C>() where C : IComparable<C>
    {
        return new Interval<C>(Cut.BelowAll<C>(), Cut.AboveAll<C>());
    }

    /// <summary>
    /// Returns an interval that contains only the given value. The
    /// returned interval is closed on both ends.
    /// </summary>
    public static Interval<C> Singleton<C>(C value) where C : IComparable<C>
    {
        return Closed(value, value);
    }
}

/// <summary>
/// An interval (sometimes called a range) defines the boundaries around a contiguous span of values of some
/// <c>Comparable</c> type; for example, "integers from 1 to 100 inclusive." Note that it is not
/// possible to iterate over these contained values.
/// </summary>
/// <remarks>
/// <para>An interval is a contiguous range of comparable values. Each side can be
/// open, closed or unbounded, and factory methods exist for the common forms.</para>
/// <para>The upper endpoint may not be less than the lower one. They can coincide
/// only if at least one side is closed.</para>
/// <para>Use immutable value types whenever possible and ensure comparisons are
/// consistent with equality.</para>
/// <para>Intervals are convex: whenever two values are contained, all values in
/// between them are contained as well.</para>
/// </remarks>
[Serializable]
public sealed class Interval<C> where C : IComparable<C>
{
    internal readonly Cut<C> _lowerBound;
    internal readonly Cut<C> _upperBound;

    internal Interval(Cut<C> lowerBound, Cut<C> upperBound)
    {
        this._lowerBound = lowerBound ?? throw new ArgumentNullException(nameof(lowerBound));
        this._upperBound = upperBound ?? throw new ArgumentNullException(nameof(upperBound));

        if (lowerBound.CompareTo(upperBound) > 0 || lowerBound == Cut.AboveAll<C>() || upperBound == Cut.BelowAll<C>())
        {
            throw new ArgumentException("Invalid interval: " + ToString(lowerBound, upperBound));
        }
    }

    /// <summary>
    /// Returns <c>true</c> if this interval has a lower endpoint.
    /// </summary>
    public bool HasLowerBound()
    {
        return _lowerBound != Cut.BelowAll<C>();
    }

    /// <summary>
    /// Returns the lower endpoint of this interval.
    /// </summary>
    /// <exception cref="InvalidOperationException"> if this interval is unbounded below
    /// (that is, <seealso cref="Interval{C}.HasLowerBound"/> returns <c>false</c>)</exception>
    public C LowerEndpoint()
    {
        return _lowerBound.Endpoint();
    }

    /// <summary>
    /// Returns the type of this interval's lower bound: <seealso cref="BoundType.Closed"/> if the interval includes
    /// its lower endpoint, <seealso cref="BoundType.Open"/> if it does not.
    /// </summary>
    /// <exception cref="InvalidOperationException"> if this interval is unbounded below
    /// (that is, <seealso cref="Interval{C}.HasLowerBound"/> returns <c>false</c>)</exception>
    public BoundType LowerBoundType()
    {
        return _lowerBound.TypeAsLowerBound();
    }

    /// <summary>
    /// Returns <c>true</c> if this interval has an upper endpoint. </summary>
    public bool HasUpperBound()
    {
        return _upperBound != Cut.AboveAll<C>();
    }

    /// <summary>
    /// Returns the upper endpoint of this interval.
    /// </summary>
    /// <exception cref="InvalidOperationException"> if this interval is unbounded above
    /// (that is, <seealso cref="Interval{C}.HasUpperBound"/> returns <c>false</c>)</exception>
    public C UpperEndpoint()
    {
        return _upperBound.Endpoint();
    }

    /// <summary>
    /// Returns the type of this interval's upper bound: <seealso cref="BoundType.Closed"/> if the interval includes
    /// its upper endpoint, <seealso cref="BoundType.Open"/> if it does not.
    /// </summary>
    /// <exception cref="InvalidOperationException"> if this interval is unbounded above
    /// (that is, <seealso cref="Interval{C}.HasUpperBound"/> returns <c>false</c>)</exception>
    public BoundType UpperBoundType()
    {
        return _upperBound.TypeAsUpperBound();
    }

    /// <summary>
    /// Returns <c>true</c> if this interval is of the form <c>[v..v)</c> or <c>(v..v]</c>. (This does
    /// not encompass intervals of the form <c>(v..v)</c>, because such intervals are invalid and
    /// can't be constructed at all.)
    /// 
    /// <para>Note that certain discrete intervals such as the integer interval <c>(3..4)</c> are not
    /// considered empty, even though they contain no actual values.
    /// </para>
    /// </summary>
    public bool IsEmpty()
    {
        return _lowerBound.Equals(_upperBound);
    }

    /// <summary>
    /// Returns <c>true</c> if <c>value</c> is within the bounds of this interval. For example, on the
    /// interval <c>[0..2)</c>, <c>contains(1)</c> returns <c>true</c>, while <c>contains(2)</c>
    /// returns <c>false</c>.
    /// </summary>
    public bool Contains(C value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return _lowerBound.IsLessThan(value) && !_upperBound.IsLessThan(value);
    }

    /// <summary>
    /// Returns <c>true</c> if every element in <c>values</c> is contained in this interval.
    /// </summary>
    public bool ContainsAll<T1>(IEnumerable<T1> values) where T1 : C
    {
        foreach (C value in values)
        {
            if (!Contains(value))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns <c>true</c> if the bounds of <c>other</c> do not extend outside the bounds of this
    /// interval. Examples:
    /// 
    /// <list type="bullet">
    /// <item><description><c>[3..6]</c> encloses <c>[4..5]</c></description></item>
    /// <item><description><c>(3..6)</c> encloses <c>(3..6)</c></description></item>
    /// <item><description><c>[3..6]</c> encloses <c>[4..4)</c> (even though the latter is empty)</description></item>
    /// <item><description><c>(3..6]</c> does not enclose <c>[3..6]</c></description></item>
    /// <item><description><c>[4..5]</c> does not enclose <c>(3..6)</c> (even though it contains every value
    /// contained by the latter interval)</description></item>
    /// <item><description><c>[3..6]</c> does not enclose <c>(1..1]</c> (even though it contains every value
    /// contained by the latter interval)</description></item>
    /// </list>
    /// 
    /// <para>Note that if <c>a.encloses(b)</c>, then <c>b.contains(v)</c> implies <c>a.contains(v)</c>, but as the last two examples illustrate, the converse is not always true.
    /// 
    /// </para>
    /// <para>Being reflexive, antisymmetric and transitive, the <c>encloses</c> relation defines a
    /// partial order over intervals. There exists a unique maximal interval
    /// according to this relation, and also numerous minimal intervals. Enclosure
    /// also implies connectedness.
    /// </para>
    /// </summary>
    public bool Encloses(Interval<C> other)
    {
        return _lowerBound.CompareTo(other._lowerBound) <= 0 && _upperBound.CompareTo(other._upperBound) >= 0;
    }

    /// <summary>
    /// Returns <c>true</c> if there exists a (possibly empty) interval that is enclosed by both this interval and <paramref name="other"/>.
    /// 
    /// <para>For example,
    /// 
    /// <list type="bullet">
    /// <item><description><c>[2, 4)</c> and <c>[5, 7)</c> are not connected</description></item>
    /// <item><description><c>[2, 4)</c> and <c>[3, 5)</c> are connected, because both enclose <c>[3, 4)</c></description></item>
    /// <item><description><c>[2, 4)</c> and <c>[4, 6)</c> are connected, because both enclose the empty interval
    /// <c>[4, 4)</c></description></item>
    /// </list>
    /// 
    /// </para>
    /// <para>Note that this interval and <c>other</c> have a well-defined union and
    /// intersection (as a single, possibly-empty interval) if and only if this
    /// method returns <c>true</c>.
    /// 
    /// </para>
    /// <para>The connectedness relation is both reflexive and symmetric, but it is not an equivalence
    /// relation because it is not transitive.
    /// 
    /// </para>
    /// <para>Note that certain discrete intervals are not considered connected, even though there are no
    /// elements "between them." For example, <c>[3, 5]</c> is not considered connected to <c>[6,
    /// 10]</c>. In these cases, it may be desirable for both input intervals to be preprocessed with a canonical
    /// discrete domain before testing for connectedness.
    /// </para>
    /// </summary>
    public bool IsConnected(Interval<C> other)
    {
        return _lowerBound.CompareTo(other._upperBound) <= 0 && other._lowerBound.CompareTo(_upperBound) <= 0;
    }

    /// <summary>
    /// Returns the maximal interval enclosed by both this interval and <paramref name="connectedRange"/>, if such an interval exists.
    /// 
    /// <para>For example, the intersection of <c>[1..5]</c> and <c>(3..7)</c> is <c>(3..5]</c>. The
    /// resulting interval may be empty; for example, <c>[1..5)</c> intersected with <c>[5..7)</c>
    /// yields the empty interval <c>[5..5)</c>.
    /// 
    /// </para>
    /// <para>The intersection exists if and only if the two intervals are connected
    /// (<see cref="IsConnected"/>).</para>
    /// <para>The intersection operation is commutative, associative and idempotent, and its identity
    /// element is <seealso cref="Interval.All{C}"/>).
    /// 
    /// </para>
    /// </summary>
    /// <exception cref="ArgumentException"> if <c>isConnected(connectedRange)</c> is <c>false</c> </exception>
    public Interval<C> Intersection(Interval<C> connectedRange)
    {
        int lowerCmp = _lowerBound.CompareTo(connectedRange._lowerBound);
        int upperCmp = _upperBound.CompareTo(connectedRange._upperBound);
        if (lowerCmp >= 0 && upperCmp <= 0)
        {
            return this;
        }
        else if (lowerCmp <= 0 && upperCmp >= 0)
        {
            return connectedRange;
        }
        else
        {
            Cut<C> newLower = (lowerCmp >= 0) ? _lowerBound : connectedRange._lowerBound;
            Cut<C> newUpper = (upperCmp <= 0) ? _upperBound : connectedRange._upperBound;
            return new Interval<C>(newLower, newUpper);
        }
    }

    /// <summary>
    /// Returns the minimal interval that encloses both this interval and <paramref name="other"/>.
    /// For example, the span of <c>[1..3]</c> and <c>(5..7)</c> is <c>[1..7)</c>.
    /// 
    /// <para>If the input intervals are connected, the returned interval can
    /// also be called their union. If they are not, note that the span might contain values
    /// that are not contained in either input interval.
    /// 
    /// </para>
    /// <para>Like <seealso cref="Interval{C}.Intersection(Interval{C})"/>, this operation is commutative, associative
    /// and idempotent. Unlike it, it is always well-defined for any two input intervals.
    /// </para>
    /// </summary>
    public Interval<C> Span(Interval<C> other)
    {
        int lowerCmp = _lowerBound.CompareTo(other._lowerBound);
        int upperCmp = _upperBound.CompareTo(other._upperBound);
        if (lowerCmp <= 0 && upperCmp >= 0)
        {
            return this;
        }
        else if (lowerCmp >= 0 && upperCmp <= 0)
        {
            return other;
        }
        else
        {
            Cut<C> newLower = (lowerCmp <= 0) ? _lowerBound : other._lowerBound;
            Cut<C> newUpper = (upperCmp >= 0) ? _upperBound : other._upperBound;
            return new Interval<C>(newLower, newUpper);
        }
    }

    /// <summary>
    /// Returns <c>true</c> if <c>object</c> is an interval having the same endpoints and bound types as
    /// this interval. Note that discrete intervals such as <c>(1..4)</c> and <c>[2..3]</c> are not
    /// equal to one another, despite the fact that they each contain precisely the same set of values.
    /// Similarly, empty intervals are not equal unless they have exactly the same representation, so
    /// <c>[3..3)</c>, <c>(3..3]</c>, <c>(4..4]</c> are all unequal.
    /// </summary>
    public override bool Equals(object? other)
    {
        if (other is Interval<C> otherRange)
        {
            return _lowerBound.Equals(otherRange._lowerBound) && _upperBound.Equals(otherRange._upperBound);
        }
        return false;
    }

    /// <summary>
    /// Returns a hash code for this interval.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(_lowerBound, _upperBound);
    }

    /// <summary>
    /// Returns a string representation of this interval, such as <c>"[3..5)"</c> (other examples are
    /// listed in the class documentation).
    /// </summary>
    public override string ToString()
    {
        return ToString(_lowerBound, _upperBound);
    }

    private static string ToString<T1, T2>(Cut<T1> lowerBound, Cut<T2> upperBound)
        where T1 : IComparable<T1>
        where T2 : IComparable<T2>
    {
        StringBuilder sb = new StringBuilder(16);
        lowerBound.DescribeAsLowerBound(sb);
        sb.Append("..");
        upperBound.DescribeAsUpperBound(sb);
        return sb.ToString();
    }

}
