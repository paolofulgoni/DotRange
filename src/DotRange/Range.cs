using System;
using System.Collections.Generic;
using System.Text;

namespace DotRange;

public static class Range
{
    /// <summary>
    /// Returns a range that contains all values strictly greater than <paramref name="lower"/> and strictly less
    /// than <paramref name="upper"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than or equal to <paramref name="upper"/>
    /// </exception>
    public static Range<C> Open<C>(C lower, C upper) where C : IComparable<C>
    {
        return new Range<C>(Cut.AboveValue(lower), Cut.BelowValue(upper));
    }

    /// <summary>
    /// Returns a range that contains all values greater than or equal to <paramref name="lower"/> and less than
    /// or equal to <paramref name="upper"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than <paramref name="upper"/>
    /// </exception>
    public static Range<C> Closed<C>(C lower, C upper) where C : IComparable<C>
    {
        return new Range<C>(Cut.BelowValue(lower), Cut.AboveValue(upper));
    }

    /// <summary>
    /// Returns a range that contains all values greater than or equal to <paramref name="lower"/> and strictly
    /// less than <paramref name="upper"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than <paramref name="upper"/>
    /// </exception>
    public static Range<C> ClosedOpen<C>(C lower, C upper) where C : IComparable<C>
    {
        return new Range<C>(Cut.BelowValue(lower), Cut.BelowValue(upper));
    }

    /// <summary>
    /// Returns a range that contains all values strictly greater than <paramref name="lower"/> and less than or
    /// equal to <paramref name="upper"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than <paramref name="upper"/>
    /// </exception>
    public static Range<C> OpenClosed<C>(C lower, C upper) where C : IComparable<C>
    {
        return new Range<C>(Cut.AboveValue(lower), Cut.AboveValue(upper));
    }

    /// <summary>
    /// Returns a range that contains any value from <paramref name="lower"/> to <paramref name="upper"/>, where each
    /// endpoint may be either inclusive (closed) or exclusive (open).
    /// </summary>
    /// <exception cref="ArgumentException"> if <paramref name="lower"/> is greater than <paramref name="upper"/>
    /// </exception>
    public static Range<C> Bounded<C>(C lower, BoundType lowerType, C upper, BoundType upperType) where C : IComparable<C>
    {
        Cut<C> lowerBound = (lowerType == BoundType.Open) ? Cut.AboveValue(lower) : Cut.BelowValue(lower);
        Cut<C> upperBound = (upperType == BoundType.Open) ? Cut.BelowValue(upper) : Cut.AboveValue(upper);
        return new Range<C>(lowerBound, upperBound);
    }

    /// <summary>
    /// Returns a range that contains all values strictly less than <paramref name="endpoint"/>.
    /// </summary>
    public static Range<C> LessThan<C>(C endpoint) where C : IComparable<C>
    {
        return new Range<C>(Cut.BelowAll<C>(), Cut.BelowValue(endpoint));
    }

    /// <summary>
    /// Returns a range that contains all values less than or equal to <paramref name="endpoint"/>.
    /// </summary>
    public static Range<C> AtMost<C>(C endpoint) where C : IComparable<C>
    {
        return new Range<C>(Cut.BelowAll<C>(), Cut.AboveValue(endpoint));
    }

    /// <summary>
    /// Returns a range with no lower bound up to the given endpoint, which may be either inclusive
    /// (closed) or exclusive (open).
    /// </summary>
    public static Range<C> UpTo<C>(C endpoint, BoundType boundType) where C : IComparable<C>
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
    /// Returns a range that contains all values strictly greater than <paramref name="endpoint"/>.
    /// </summary>
    public static Range<C> GreaterThan<C>(C endpoint) where C : IComparable<C>
    {
        return new Range<C>(Cut.AboveValue(endpoint), Cut.AboveAll<C>());
    }

    /// <summary>
    /// Returns a range that contains all values greater than or equal to <paramref name="endpoint"/>.
    /// </summary>
    public static Range<C> AtLeast<C>(C endpoint) where C : IComparable<C>
    {
        return new Range<C>(Cut.BelowValue(endpoint), Cut.AboveAll<C>());
    }

    /// <summary>
    /// Returns a range from the given endpoint, which may be either inclusive (closed) or exclusive
    /// (open), with no upper bound.
    /// </summary>
    public static Range<C> DownTo<C>(C endpoint, BoundType boundType) where C : IComparable<C>
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
    /// Returns a range that contains every value of type <typeparamref name="C"/>.
    /// </summary>
    public static Range<C> All<C>() where C : IComparable<C>
    {
        return new Range<C>(Cut.BelowAll<C>(), Cut.AboveAll<C>());
    }

    /// <summary>
    /// Returns a range that contains only the given value. The
    /// returned range is closed on both ends.
    /// </summary>
    public static Range<C> Singleton<C>(C value) where C : IComparable<C>
    {
        return Closed(value, value);
    }
}

/// <summary>
/// A range (or "interval") defines the boundaries around a contiguous span of values of some
/// <c>Comparable</c> type; for example, "integers from 1 to 100 inclusive." Note that it is not
/// possible to iterate over these contained values.
/// </summary>
/// <remarks>
/// <para>Types of ranges</para>
/// 
/// <para>Each end of the range may be bounded or unbounded. If bounded, there is an associated
/// endpoint value, and the range is considered to be either open (does not include the
/// endpoint) or closed (includes the endpoint) on that side. With three possibilities on each
/// side, this yields nine basic types of ranges, enumerated below. (Notation: a square bracket
/// (<c>[ ]</c>) indicates that the range is closed on that side; a parenthesis (<c>( )</c>) means
/// it is either open or unbounded. The construct <c>{x | statement</c>} is read "the set of all
/// x such that statement.")
/// </para>
/// <para>
/// Range Types
/// <list type="table">
/// <listheader><term>Notation</term><term>Definition</term><term>Factory method</term></listheader>
/// <item><term><c>(a..b)</c>  </term><term><c>x | a < x < b</c>  </term><term><seealso cref="Range.Open{C}(C, C)"/></term></item>
/// <item><term><c>[a..b]</c>  </term><term><c>x | a <= x <= b</c></term><term><seealso cref="Range.Closed{C}(C, C)"/></term></item>
/// <item><term><c>(a..b]</c>  </term><term><c>x | a < x <= b</c> </term><term><seealso cref="Range.OpenClosed{C}(C, C)"/></term></item>
/// <item><term><c>[a..b)</c>  </term><term><c>x | a <= x < b</c> </term><term><seealso cref="Range.ClosedOpen{C}(C, C)"/></term></item>
/// <item><term><c>(a..+∞)</c> </term><term><c>x | x > a</c>      </term><term><seealso cref="Range.GreaterThan{C}(C)"/></term></item>
/// <item><term><c>[a..+∞)</c> </term><term><c>x | x >= a</c>     </term><term><seealso cref="Range.AtLeast{C}(C)"/></term></item>
/// <item><term><c>(-∞..b)</c> </term><term><c>x | x < b</c>      </term><term><seealso cref="Range.LessThan{C}(C)"/></term></item>
/// <item><term><c>(-∞..b]</c> </term><term><c>x | x <= b</c>     </term><term><seealso cref="Range.AtMost{C}(C)"/></term></item>
/// <item><term><c>(-∞..+∞)</c></term><term><c>x</c>}             </term><term><seealso cref="Range.All{C}"/></term></item>
/// </list>
/// </para>
/// 
/// </para>
/// <para>When both endpoints exist, the upper endpoint may not be less than the lower. The endpoints
/// may be equal only if at least one of the bounds is closed:
/// 
/// <list type="bullet">
/// <item><description><c>[a..a]</c> : a singleton range</description></item>
/// <item><description><c>[a..a); (a..a]</c> : empty ranges; also valid</description></item>
/// <item><description><c>(a..a)</c> : invalid; an exception will be thrown</description></item>
/// </list>
/// 
/// <para>Warnings</para>
/// 
/// <list type="bullet">
/// <item><description>Use immutable value types only, if at all possible. If you must use a mutable type, do
/// not allow the endpoint instances to mutate after the range is created!</description></item>
/// <item><description>Your value type's comparison method should be consistent with
/// <see cref="object.Equals(object)"/> if at all possible. Otherwise, be aware that concepts used throughout this
/// documentation such as "equal", "same", "unique" and so on actually refer to whether
/// <seealso cref="IComparable{T}.CompareTo(T)"/> returns zero, not whether <seealso cref="object.Equals(object)"/>
/// returns <c>true</c>.</description></item>
/// </list>
/// 
/// <para>Other notes</para>
///
/// <list type="bullet">
/// <item><description>Instances of this type are obtained using the static factory methods in this class.</description></item>
/// <item><description>Ranges are convex: whenever two values are contained, all values in between them
/// must also be contained. More formally, for any <c>c1 <= c2 <= c3</c> of type <typeparamref name="C"/>,
/// <c>r.contains(c1) && r.contains(c3)</c> implies <c>r.contains(c2)</c>. This means that a
/// <c>Range&lt;Integer&gt;</c> can never be used to represent, say, "all prime numbers from
/// 1 to 100."</description></item>
/// <item><description>Terminology note: a range <c>a</c> is said to be the maximal range having property
/// <c>P</c> if, for all ranges <c>b</c> also having property <c>P</c>, <c>a.encloses(b)</c>.
/// Likewise, <c>a</c> is minimal when <c>b.encloses(a)</c> for all <c>b</c> having
/// property <c>P</c>. See, for example, the definition of <seealso cref="Range{C}.Intersection(Range{C})"/>.</description></item>
/// </list>
/// 
/// </para>
/// </remarks>
[Serializable]
public sealed class Range<C> where C : IComparable<C>
{
    internal readonly Cut<C> _lowerBound;
    internal readonly Cut<C> _upperBound;

    internal Range(Cut<C> lowerBound, Cut<C> upperBound)
    {
        this._lowerBound = lowerBound ?? throw new ArgumentNullException(nameof(lowerBound));
        this._upperBound = upperBound ?? throw new ArgumentNullException(nameof(upperBound));

        if (lowerBound.CompareTo(upperBound) > 0 || lowerBound == Cut.AboveAll<C>() || upperBound == Cut.BelowAll<C>())
        {
            throw new ArgumentException("Invalid range: " + ToString(lowerBound, upperBound));
        }
    }

    /// <summary>
    /// Returns <c>true</c> if this range has a lower endpoint.
    /// </summary>
    public bool HasLowerBound()
    {
        return _lowerBound != Cut.BelowAll<C>();
    }

    /// <summary>
    /// Returns the lower endpoint of this range.
    /// </summary>
    /// <exception cref="InvalidOperationException"> if this range is unbounded below
    /// (that is, <seealso cref="Range{C}.HasLowerBound"/> returns <c>false</c>)</exception>
    public C LowerEndpoint()
    {
        return _lowerBound.Endpoint();
    }

    /// <summary>
    /// Returns the type of this range's lower bound: <seealso cref="BoundType.Closed"/> if the range includes
    /// its lower endpoint, <seealso cref="BoundType.Open"/> if it does not.
    /// </summary>
    /// <exception cref="InvalidOperationException"> if this range is unbounded below
    /// (that is, <seealso cref="Range{C}.HasLowerBound"/> returns <c>false</c>)</exception>
    public BoundType LowerBoundType()
    {
        return _lowerBound.TypeAsLowerBound();
    }

    /// <summary>
    /// Returns <c>true</c> if this range has an upper endpoint. </summary>
    public bool HasUpperBound()
    {
        return _upperBound != Cut.AboveAll<C>();
    }

    /// <summary>
    /// Returns the upper endpoint of this range.
    /// </summary>
    /// <exception cref="InvalidOperationException"> if this range is unbounded above 
    /// (that is, <seealso cref="Range{C}.HasUpperBound"/> returns <c>false</c>)</exception>
    public C UpperEndpoint()
    {
        return _upperBound.Endpoint();
    }

    /// <summary>
    /// Returns the type of this range's upper bound: <seealso cref="BoundType.Closed"/> if the range includes
    /// its upper endpoint, <seealso cref="BoundType.Open"/> if it does not.
    /// </summary>
    /// <exception cref="InvalidOperationException"> if this range is unbounded above 
    /// (that is, <seealso cref="Range{C}.HasUpperBound"/> returns <c>false</c>)</exception>
    public BoundType UpperBoundType()
    {
        return _upperBound.TypeAsUpperBound();
    }

    /// <summary>
    /// Returns <c>true</c> if this range is of the form <c>[v..v)</c> or <c>(v..v]</c>. (This does
    /// not encompass ranges of the form <c>(v..v)</c>, because such ranges are invalid and
    /// can't be constructed at all.)
    /// 
    /// <para>Note that certain discrete ranges such as the integer range <c>(3..4)</c> are not
    /// considered empty, even though they contain no actual values.
    /// </para>
    /// </summary>
    public bool IsEmpty()
    {
        return _lowerBound.Equals(_upperBound);
    }

    /// <summary>
    /// Returns <c>true</c> if <c>value</c> is within the bounds of this range. For example, on the
    /// range <c>[0..2)</c>, <c>contains(1)</c> returns <c>true</c>, while <c>contains(2)</c>
    /// returns <c>false</c>.
    /// </summary>
    public bool Contains(C value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return _lowerBound.IsLessThan(value) && !_upperBound.IsLessThan(value);
    }

    /// <summary>
    /// Returns <c>true</c> if every element in <c>values</c> is contained in this range.
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
    /// range. Examples:
    /// 
    /// <list type="bullet">
    /// <item><description><c>[3..6]</c> encloses <c>[4..5]</c></description></item>
    /// <item><description><c>(3..6)</c> encloses <c>(3..6)</c></description></item>
    /// <item><description><c>[3..6]</c> encloses <c>[4..4)</c> (even though the latter is empty)</description></item>
    /// <item><description><c>(3..6]</c> does not enclose <c>[3..6]</c></description></item>
    /// <item><description><c>[4..5]</c> does not enclose <c>(3..6)</c> (even though it contains every value
    /// contained by the latter range)</description></item>
    /// <item><description><c>[3..6]</c> does not enclose <c>(1..1]</c> (even though it contains every value
    /// contained by the latter range)</description></item>
    /// </list>
    /// 
    /// <para>Note that if <c>a.encloses(b)</c>, then <c>b.contains(v)</c> implies <c>a.contains(v)</c>, but as the last two examples illustrate, the converse is not always true.
    /// 
    /// </para>
    /// <para>Being reflexive, antisymmetric and transitive, the <c>encloses</c> relation defines a
    /// partial order over ranges. There exists a unique maximal range
    /// according to this relation, and also numerous minimal ranges. Enclosure
    /// also implies connectedness.
    /// </para>
    /// </summary>
    public bool Encloses(Range<C> other)
    {
        return _lowerBound.CompareTo(other._lowerBound) <= 0 && _upperBound.CompareTo(other._upperBound) >= 0;
    }

    /// <summary>
    /// Returns <c>true</c> if there exists a (possibly empty) range that is enclosed by both this range and <paramref name="other"/>.
    /// 
    /// <para>For example,
    /// 
    /// <list type="bullet">
    /// <item><description><c>[2, 4)</c> and <c>[5, 7)</c> are not connected</description></item>
    /// <item><description><c>[2, 4)</c> and <c>[3, 5)</c> are connected, because both enclose <c>[3, 4)</c></description></item>
    /// <item><description><c>[2, 4)</c> and <c>[4, 6)</c> are connected, because both enclose the empty range
    /// <c>[4, 4)</c></description></item>
    /// </list>
    /// 
    /// </para>
    /// <para>Note that this range and <c>other</c> have a well-defined union and
    /// intersection (as a single, possibly-empty range) if and only if this
    /// method returns <c>true</c>.
    /// 
    /// </para>
    /// <para>The connectedness relation is both reflexive and symmetric, but it is not an equivalence
    /// relation because it is not transitive.
    /// 
    /// </para>
    /// <para>Note that certain discrete ranges are not considered connected, even though there are no
    /// elements "between them." For example, <c>[3, 5]</c> is not considered connected to <c>[6,
    /// 10]</c>. In these cases, it may be desirable for both input ranges to be preprocessed with a canonical
    /// discrete domain before testing for connectedness.
    /// </para>
    /// </summary>
    public bool IsConnected(Range<C> other)
    {
        return _lowerBound.CompareTo(other._upperBound) <= 0 && other._lowerBound.CompareTo(_upperBound) <= 0;
    }

    /// <summary>
    /// Returns the maximal range enclosed by both this range and <paramref name="connectedRange"/>, if such a range exists.
    /// 
    /// <para>For example, the intersection of <c>[1..5]</c> and <c>(3..7)</c> is <c>(3..5]</c>. The
    /// resulting range may be empty; for example, <c>[1..5)</c> intersected with <c>[5..7)</c>
    /// yields the empty range <c>[5..5)</c>.
    /// 
    /// </para>
    /// <para>The intersection exists if and only if the two ranges are connected
    /// (<see cref="IsConnected"/>).</para>
    /// <para>The intersection operation is commutative, associative and idempotent, and its identity
    /// element is <seealso cref="Range.All{C}"/>).
    /// 
    /// </para>
    /// </summary>
    /// <exception cref="ArgumentException"> if <c>isConnected(connectedRange)</c> is <c>false</c> </exception>
    public Range<C> Intersection(Range<C> connectedRange)
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
            return new Range<C>(newLower, newUpper);
        }
    }

    /// <summary>
    /// Returns the minimal range that encloses both this range and <paramref name="other"/>.
    /// For example, the span of <c>[1..3]</c> and <c>(5..7)</c> is <c>[1..7)</c>.
    /// 
    /// <para>If the input ranges are connected, the returned range can
    /// also be called their union. If they are not, note that the span might contain values
    /// that are not contained in either input range.
    /// 
    /// </para>
    /// <para>Like <seealso cref="Range{C}.Intersection(Range{C})"/>, this operation is commutative, associative
    /// and idempotent. Unlike it, it is always well-defined for any two input ranges.
    /// </para>
    /// </summary>
    public Range<C> Span(Range<C> other)
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
            return new Range<C>(newLower, newUpper);
        }
    }

    /// <summary>
    /// Returns <c>true</c> if <c>object</c> is a range having the same endpoints and bound types as
    /// this range. Note that discrete ranges such as <c>(1..4)</c> and <c>[2..3]</c> are not
    /// equal to one another, despite the fact that they each contain precisely the same set of values.
    /// Similarly, empty ranges are not equal unless they have exactly the same representation, so
    /// <c>[3..3)</c>, <c>(3..3]</c>, <c>(4..4]</c> are all unequal.
    /// </summary>
    public override bool Equals(object other)
    {
        if (other is Range<C> otherRange)
        {
            return _lowerBound.Equals(otherRange._lowerBound) && _upperBound.Equals(otherRange._upperBound);
        }
        return false;
    }

    /// <summary>
    /// Returns a hash code for this range.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(_lowerBound, _upperBound);
    }

    /// <summary>
    /// Returns a string representation of this range, such as <c>"[3..5)"</c> (other examples are
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
