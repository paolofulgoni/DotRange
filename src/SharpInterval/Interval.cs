using System;
using System.Collections.Generic;
using System.Text;

namespace SharpInterval;

/// <summary>
/// Provides factory methods for creating <see cref="Interval{T}"/> instances.
/// </summary>
/// <remarks>
/// This non-generic class only exposes these factories. The actual interval
/// representation is <see cref="Interval{T}"/>.
/// </remarks>
public static class Interval
{
    /// <summary>
    /// Creates an interval representing all values that are strictly greater than <paramref name="lower"/> and strictly less
    /// than <paramref name="upper"/>. Neither <paramref name="lower"/> nor <paramref name="upper"/> are included in the interval.
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than or equal to <paramref name="upper"/>
    /// </exception>
    public static Interval<T> Open<T>(T lower, T upper) where T : IComparable<T>
    {
        return new Interval<T>(Cut.AboveValue(lower), Cut.BelowValue(upper));
    }

    /// <summary>
    /// Creates an interval representing all values from <paramref name="lower"/> to <paramref name="upper"/>,
    /// including both <paramref name="lower"/> and <paramref name="upper"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than <paramref name="upper"/>
    /// </exception>
    public static Interval<T> Closed<T>(T lower, T upper) where T : IComparable<T>
    {
        return new Interval<T>(Cut.BelowValue(lower), Cut.AboveValue(upper));
    }

    /// <summary>
    /// Creates an interval representing all values from <paramref name="lower"/> (inclusive) up to, but not including,
    /// <paramref name="upper"/> (exclusive).
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than <paramref name="upper"/>
    /// </exception>
    public static Interval<T> ClosedOpen<T>(T lower, T upper) where T : IComparable<T>
    {
        return new Interval<T>(Cut.BelowValue(lower), Cut.BelowValue(upper));
    }

    /// <summary>
    /// Creates an interval representing all values strictly greater than <paramref name="lower"/> (exclusive) up to,
    /// and including, <paramref name="upper"/> (inclusive).
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than <paramref name="upper"/>
    /// </exception>
    public static Interval<T> OpenClosed<T>(T lower, T upper) where T : IComparable<T>
    {
        return new Interval<T>(Cut.AboveValue(lower), Cut.AboveValue(upper));
    }

    /// <summary>
    /// Creates an interval from <paramref name="lower"/> to <paramref name="upper"/>. You can specify whether
    /// <paramref name="lower"/> and <paramref name="upper"/> are included or excluded using <paramref name="lowerType"/>
    /// and <paramref name="upperType"/> respectively.
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than <paramref name="upper"/>
    /// </exception>
    public static Interval<T> Bounded<T>(T lower, BoundType lowerType, T upper, BoundType upperType) where T : IComparable<T>
    {
        Cut<T> lowerBound = (lowerType == BoundType.Open) ? Cut.AboveValue(lower) : Cut.BelowValue(lower);
        Cut<T> upperBound = (upperType == BoundType.Open) ? Cut.BelowValue(upper) : Cut.AboveValue(upper);
        return new Interval<T>(lowerBound, upperBound);
    }

    /// <summary>
    /// Creates an interval representing all values strictly less than the <paramref name="endpoint"/>.
    /// The <paramref name="endpoint"/> itself is not included. The interval has no lower bound (extends to negative infinity).
    /// </summary>
    public static Interval<T> LessThan<T>(T endpoint) where T : IComparable<T>
    {
        return new Interval<T>(Cut.BelowAll<T>(), Cut.BelowValue(endpoint));
    }

    /// <summary>
    /// Creates an interval representing all values less than or equal to the <paramref name="endpoint"/>.
    /// The <paramref name="endpoint"/> itself is included. The interval has no lower bound (extends to negative infinity).
    /// </summary>
    public static Interval<T> AtMost<T>(T endpoint) where T : IComparable<T>
    {
        return new Interval<T>(Cut.BelowAll<T>(), Cut.AboveValue(endpoint));
    }

    /// <summary>
    /// Creates an interval that extends indefinitely from negative infinity up to the specified <paramref name="endpoint"/>.
    /// Whether the <paramref name="endpoint"/> is included or excluded is determined by <paramref name="boundType"/>.
    /// </summary>
    public static Interval<T> UpTo<T>(T endpoint, BoundType boundType) where T : IComparable<T>
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
    /// Creates an interval representing all values strictly greater than the <paramref name="endpoint"/>.
    /// The <paramref name="endpoint"/> itself is not included. The interval has no upper bound (extends to positive infinity).
    /// </summary>
    public static Interval<T> GreaterThan<T>(T endpoint) where T : IComparable<T>
    {
        return new Interval<T>(Cut.AboveValue(endpoint), Cut.AboveAll<T>());
    }

    /// <summary>
    /// Creates an interval representing all values greater than or equal to the <paramref name="endpoint"/>.
    /// The <paramref name="endpoint"/> itself is included. The interval has no upper bound (extends to positive infinity).
    /// </summary>
    public static Interval<T> AtLeast<T>(T endpoint) where T : IComparable<T>
    {
        return new Interval<T>(Cut.BelowValue(endpoint), Cut.AboveAll<T>());
    }

    /// <summary>
    /// Creates an interval that starts from the specified <paramref name="endpoint"/> and extends indefinitely to positive infinity.
    /// Whether the <paramref name="endpoint"/> is included or excluded is determined by <paramref name="boundType"/>.
    /// </summary>
    public static Interval<T> DownTo<T>(T endpoint, BoundType boundType) where T : IComparable<T>
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
    /// Creates an interval that represents all possible values of type <typeparamref name="T"/>.
    /// It has no lower or upper bounds (extends from negative to positive infinity).
    /// </summary>
    public static Interval<T> All<T>() where T : IComparable<T>
    {
        return new Interval<T>(Cut.BelowAll<T>(), Cut.AboveAll<T>());
    }

    /// <summary>
    /// Creates an interval that contains only a single <paramref name="value"/>.
    /// This is equivalent to an interval where the start and end points are both <paramref name="value"/>, and both are included.
    /// </summary>
    public static Interval<T> Singleton<T>(T value) where T : IComparable<T>
    {
        return Closed(value, value);
    }
}

/// <summary>
/// Represents an interval, which is a continuous range of values of a comparable type (e.g., numbers from 1 to 100).
/// This class defines the boundaries of such a range but doesn't allow iterating through the individual values within it.
/// </summary>
/// <remarks>
/// <para>An interval represents a continuous set of values. Its start and end points can be 'open' (value excluded),
/// 'closed' (value included), or 'unbounded' (extending to infinity). Helper methods are available to create
/// common types of intervals easily.</para>
/// <para>The end point of an interval cannot be before its start point. The start and end points can be the same value
/// if the interval includes that value (i.e., at least one bound is 'closed').</para>
/// <para>Use immutable value types whenever possible and ensure comparisons are
/// consistent with equality.</para>
/// <para>A key property of intervals is that they are 'convex', meaning if two values are in the interval,
/// all values between them are also in the interval.</para>
/// </remarks>
[Serializable]
public sealed class Interval<T> where T : IComparable<T>
{
    internal readonly Cut<T> _lowerBound;
    internal readonly Cut<T> _upperBound;

    internal Interval(Cut<T> lowerBound, Cut<T> upperBound)
    {
        this._lowerBound = lowerBound ?? throw new ArgumentNullException(nameof(lowerBound));
        this._upperBound = upperBound ?? throw new ArgumentNullException(nameof(upperBound));

        if (lowerBound.CompareTo(upperBound) > 0 || lowerBound == Cut.AboveAll<T>() || upperBound == Cut.BelowAll<T>())
        {
            throw new ArgumentException("Invalid interval: " + ToString(lowerBound, upperBound));
        }
    }

    /// <summary>
    /// Checks if this interval has a defined starting point (it's not unbounded towards negative infinity).
    /// </summary>
    public bool HasLowerBound()
    {
        return _lowerBound != Cut.BelowAll<T>();
    }

    /// <summary>
    /// Gets the starting value of this interval.
    /// </summary>
    /// <exception cref="InvalidOperationException"> if this interval has no defined starting point (i.e., it extends to negative infinity)
    /// (that is, <seealso cref="Interval{T}.HasLowerBound"/> returns <c>false</c>)</exception>
    public T LowerEndpoint()
    {
        return _lowerBound.Endpoint();
    }

    /// <summary>
    /// Determines if the interval's starting point is 'Open' (start value not included) or 'Closed' (start value included).
    /// </summary>
    /// <exception cref="InvalidOperationException"> if this interval has no defined starting point
    /// (that is, <seealso cref="Interval{T}.HasLowerBound"/> returns <c>false</c>)</exception>
    public BoundType LowerBoundType()
    {
        return _lowerBound.TypeAsLowerBound();
    }

    /// <summary>
    /// Checks if this interval has a defined ending point (it's not unbounded towards positive infinity).
    /// </summary>
    public bool HasUpperBound()
    {
        return _upperBound != Cut.AboveAll<T>();
    }

    /// <summary>
    /// Gets the ending value of this interval.
    /// </summary>
    /// <exception cref="InvalidOperationException"> if this interval has no defined ending point (i.e., it extends to positive infinity)
    /// (that is, <seealso cref="Interval{T}.HasUpperBound"/> returns <c>false</c>)</exception>
    public T UpperEndpoint()
    {
        return _upperBound.Endpoint();
    }

    /// <summary>
    /// Determines if the interval's ending point is 'Open' (end value not included) or 'Closed' (end value included).
    /// </summary>
    /// <exception cref="InvalidOperationException"> if this interval has no defined ending point
    /// (that is, <seealso cref="Interval{T}.HasUpperBound"/> returns <c>false</c>)</exception>
    public BoundType UpperBoundType()
    {
        return _upperBound.TypeAsUpperBound();
    }

    /// <summary>
    /// Checks if the interval contains no values. For example, an interval like `[5..5)` (from 5, including 5, up to 5, excluding 5) is empty.
    /// (This does not encompass intervals of the form <c>(v..v)</c>, because such intervals are invalid and
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
    /// Checks if the given <paramref name="value"/> is within this interval. For example, on the
    /// interval <c>[0..2)</c>, <c>contains(1)</c> returns <c>true</c>, while <c>contains(2)</c>
    /// returns <c>false</c>.
    /// </summary>
    public bool Contains(T value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return _lowerBound.IsLessThan(value) && !_upperBound.IsLessThan(value);
    }

    /// <summary>
    /// Checks if every element in the provided <paramref name="values"/> collection is contained in this interval.
    /// </summary>
    public bool ContainsAll<T1>(IEnumerable<T1> values) where T1 : T
    {
        foreach (T value in values)
        {
            if (!Contains(value))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Checks if this interval completely contains the <paramref name="other"/> interval.
    /// This means the <paramref name="other"/> interval's start and end points must not extend beyond this interval's own start and end points.
    /// Examples:
    /// <list type="bullet">
    /// <item><description><c>[3..6]</c> encloses <c>[4..5]</c> (True)</description></item>
    /// <item><description><c>(3..6)</c> encloses <c>(3..6)</c> (True)</description></item>
    /// <item><description><c>[3..6]</c> encloses <c>[4..4)</c> (True, even though <c>[4..4)</c> is empty)</description></item>
    /// <item><description><c>(3..6]</c> does not enclose <c>[3..6]</c> (False, because the start of <c>[3..6]</c> is '3 inclusive', which is not in <c>(3..6]</c>)</description></item>
    /// <item><description><c>[4..5]</c> does not enclose <c>(3..6)</c> (False, because <c>(3..6)</c> extends beyond <c>[4..5]</c>)</description></item>
    /// </list>
    /// 
    /// <para>If <c>A.Encloses(B)</c> is true, then any value contained in B is also contained in A.
    /// However, the reverse is not always true. For example, <c>[4..5]</c> contains all values of <c>(4.0..4.5)</c>, but <c>[4..5]</c> does not enclose <c>(3..6)</c>.
    /// 
    /// </para>
    /// <para>Being reflexive, antisymmetric and transitive, the <c>encloses</c> relation defines a
    /// partial order over intervals. There exists a unique maximal interval
    /// according to this relation, and also numerous minimal intervals. Enclosure
    /// also implies connectedness.
    /// </para>
    /// </summary>
    public bool Encloses(Interval<T> other)
    {
        return _lowerBound.CompareTo(other._lowerBound) <= 0 && _upperBound.CompareTo(other._upperBound) >= 0;
    }

    /// <summary>
    /// Checks if this interval and the <paramref name="other"/> interval touch or overlap, meaning there is no gap between them.
    /// 
    /// <para>For example:
    /// <list type="bullet">
    /// <item><description><c>[2..4)</c> and <c>[5..7)</c> are not connected (gap between 4 and 5).</description></item>
    /// <item><description><c>[2..4)</c> and <c>[3..5)</c> are connected (overlap is <c>[3..4)</c>).</description></item>
    /// <item><description><c>[2..4)</c> and <c>[4..6)</c> are connected (touch at 4, the empty interval <c>[4..4)</c> is common).</description></item>
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
    public bool IsConnected(Interval<T> other)
    {
        return _lowerBound.CompareTo(other._upperBound) <= 0 && other._lowerBound.CompareTo(_upperBound) <= 0;
    }

    /// <summary>
    /// Calculates the common part (overlap) of this interval and <paramref name="connectedRange"/>.
    /// 
    /// <para>For example, the intersection of <c>[1..5]</c> and <c>(3..7)</c> is <c>(3..5]</c>.
    /// The result can be an empty interval, e.g., <c>[1..5)</c> intersected with <c>[5..7)</c> results in <c>[5..5)</c>.
    /// 
    /// </para>
    /// <para>This operation is only valid if the two intervals are connected (i.e., they touch or overlap, as determined by <see cref="IsConnected"/>).</para>
    /// <para>The intersection operation is commutative, associative and idempotent, and its identity
    /// element is <seealso cref="Interval.All{T}"/>.
    /// </para>
    /// </summary>
    /// <exception cref="ArgumentException"> if the intervals are not connected (<c>IsConnected(connectedRange)</c> is <c>false</c>). </exception>
    public Interval<T> Intersection(Interval<T> connectedRange)
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
            Cut<T> newLower = (lowerCmp >= 0) ? _lowerBound : connectedRange._lowerBound;
            Cut<T> newUpper = (upperCmp <= 0) ? _upperBound : connectedRange._upperBound;
            return new Interval<T>(newLower, newUpper);
        }
    }

    /// <summary>
    /// Calculates the smallest single interval that encompasses both this interval and the <paramref name="other"/> interval.
    /// For example, the span of <c>[1..3]</c> and <c>(5..7)</c> is <c>[1..7)</c>.
    /// 
    /// <para>If the two input intervals are connected (touch or overlap), their span is effectively their union.
    /// If they are not connected (there's a gap between them), the span will include that gap as well.
    /// For instance, the span of <c>[1..3]</c> and <c>[5..7]</c> is <c>[1..7]</c>.
    /// 
    /// </para>
    /// <para>Like <seealso cref="Interval{T}.Intersection(Interval{T})"/>, this operation is commutative, associative
    /// and idempotent. Unlike it, it is always well-defined for any two input intervals.
    /// </para>
    /// </summary>
    public Interval<T> Span(Interval<T> other)
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
            Cut<T> newLower = (lowerCmp <= 0) ? _lowerBound : other._lowerBound;
            Cut<T> newUpper = (upperCmp >= 0) ? _upperBound : other._upperBound;
            return new Interval<T>(newLower, newUpper);
        }
    }

    /// <summary>
    /// Checks if this interval is identical to another interval, meaning they have the same start and end points and the same bound types (Open/Closed).
    /// Note that intervals like <c>(1..4)</c> (integers 2, 3) and <c>[2..3]</c> (integers 2, 3) are not
    /// considered equal by this method, because their boundary definitions differ, even if they happen to contain the same set of discrete values.
    /// Similarly, empty intervals like <c>[3..3)</c> and <c>(3..3]</c> are not equal.
    /// </summary>
    public override bool Equals(object? other)
    {
        if (other is Interval<T> otherRange)
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
