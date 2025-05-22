[![build](https://github.com/paolofulgoni/SharpInterval/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/paolofulgoni/SharpInterval/actions/workflows/dotnet.yml?query=branch%3Amain)
[![NuGet](https://img.shields.io/nuget/v/SharpInterval.svg)](https://www.nuget.org/packages/SharpInterval)

# SharpInterval

The Interval class for .Net Standard

## Introduction

An interval, sometimes known as a range, is a convex (informally, "contiguous"
or "unbroken") portion of a particular domain. Formally, convexity means that
for any `a <= b <= c`, `interval.Contains(a) && interval.Contains(c)` implies that
`interval.Contains(b)`.

The word "range" is a common synonym for "interval", but the name `Interval` is used here to avoid clashes with `System.Range`.

Intervals may "extend to infinity" -- for example, the interval `x > 3` contains
arbitrarily large values -- or may be finitely constrained, for example `2 <= x
< 5`. We will use the more compact notation, familiar to programmers with a math
background:

*   `(a..b) = {x | a < x < b}`
*   `[a..b] = {x | a <= x <= b}`
*   `[a..b) = {x | a <= x < b}`
*   `(a..b] = {x | a < x <= b}`
*   `(a..+∞) = {x | x > a}`
*   `[a..+∞) = {x | x >= a}`
*   `(-∞..b) = {x | x < b}`
*   `(-∞..b] = {x | x <= b}`
*   `(-∞..+∞) = all values`

The values a and b used above are called endpoints. To improve consistency,
SharpInterval's notion of `Interval` requires that the upper endpoint may not be less than
the lower endpoint. The endpoints may be equal only if at least one of the
bounds is closed:

*   `[a..a]`: singleton interval
*   `[a..a); (a..a]`: empty, but valid
*   `(a..a)`: invalid

An interval in SharpInterval has the type `Interval<T>`. All intervals are *immutable*.

## Building Intervals

Intervals can be obtained from the static methods on `Interval`:

Interval type | Method
:--------- | :-------------------
`(a..b)`   | `Open(T, T)`
`[a..b]`   | `Closed(T, T)`
`[a..b)`   | `ClosedOpen(T, T)`
`(a..b]`   | `OpenClosed(T, T)`
`(a..+∞)`  | `GreaterThan(T)`
`[a..+∞)`  | `AtLeast(T)`
`(-∞..b)`  | `LessThan(T)`
`(-∞..b]`  | `AtMost(T)`
`(-∞..+∞)` | `All()`

```cs
Interval.Closed("left", "right"); // all strings lexicographically between "left" and "right" inclusive
Interval.LessThan(4.0); // double values strictly less than 4
```

Additionally, Interval instances can be constructed by passing the bound types
explicitly:

Interval type                                   | Method
:------------------------------------------- | :-----
Bounded on both ends                         | `Bounded(T, BoundType, T, BoundType)`
Unbounded on top (`(a..+∞)` or `[a..+∞)`)    | `DownTo(T, BoundType)`
Unbounded on bottom (`(-∞..b)` or `(-∞..b]`) | `UpTo(T, BoundType)`

Here, `BoundType` is an enum containing the values `Closed` and `Open`.

```cs
Interval.DownTo(4, boundType); // allows you to decide whether or not you want to include 4
Interval.Bounded(1, BoundType.Closed, 4, BoundType.Open); // another way of writing Interval.ClosedOpen(1, 4)
```

## Operations

The fundamental operation of an `Interval` is its `Contains(T)` methods, which
behaves exactly as you might expect. Any `Interval` also supports 
`ContainsAll(IEnumerable)`.

```cs
Interval.Closed(1, 3).Contains(2); // returns true
Interval.Closed(1, 3).Contains(4); // returns false
Interval.LessThan(5).Contains(5); // returns false
Interval.Closed(1, 4).ContainsAll(new int[] { 1, 2, 3 }); // returns true
```

### Query Operations

To look at the endpoints of an interval, `Interval` exposes the following methods:

*   `HasLowerBound()` and `HasUpperBound()`, which check if the interval has
    the specified endpoints, or goes on "through infinity."
*   `LowerBoundType()` and `UpperBoundType()` return the `BoundType` for the
    corresponding endpoint, which can be either `Closed` or `Open`. If this
    interval does not have the specified endpoint, the method throws an
    `InvalidOperationException`.
*   `LowerEndpoint()` and `UpperEndpoint()` return the endpoints on the
    specified end, or throw an `InvalidOperationException` if the interval does not
    have the specified endpoint.
*   `IsEmpty()` tests if the interval is empty, that is, it has the form `[a,a)`
    or `(a,a]`.

```cs
Interval.ClosedOpen(4, 4).IsEmpty(); // returns true
Interval.OpenClosed(4, 4).IsEmpty(); // returns true
Interval.Closed(4, 4).IsEmpty(); // returns false
Interval.Open(4, 4).IsEmpty(); // Interval.Open throws ArgumentException

Interval.Closed(3, 10).LowerEndpoint(); // returns 3
Interval.Open(3, 10).LowerEndpoint(); // returns 3
Interval.Closed(3, 10).LowerBoundType(); // returns Closed
Interval.Open(3, 10).UpperBoundType(); // returns Open
```

### Interval Operations

#### `Encloses`

The most basic relation on intervals is `Encloses(Interval)`, which is true if the
bounds of the inner interval do not extend outside the bounds of the outer interval.
This is solely dependent on comparisons between the endpoints!

*   `[3..6]` encloses `[4..5]`
*   `(3..6)` encloses `(3..6)`
*   `[3..6]` encloses `[4..4)` (even though the latter is empty)
*   `(3..6]` does not enclose `[3..6]`
*   `[4..5]` does not enclose `(3..6)` **even though it contains every value
    contained by the latter interval**
*   `[3..6]` does not enclose `(1..1]` **even though it contains every value
    contained by the latter interval**

Given this, `Interval` provides the following operations:

#### `IsConnected`

`Interval.IsConnected(Interval)`, which tests if these intervals are *connected*.
Specifically, `isConnected` tests if there is some interval enclosed by both of
these intervals, but this is equivalent to the mathematical definition that the
union of the intervals must form a connected set (except in the special case of
empty intervals).

`IsConnected` is a reflexive, symmetric relation.

```cs
Interval.Closed(3, 5).IsConnected(Interval.Open(5, 10)); // returns true
Interval.Closed(0, 9).IsConnected(Interval.Closed(3, 4)); // returns true
Interval.Closed(0, 5).IsConnected(Interval.Closed(3, 9)); // returns true
Interval.Open(3, 5).IsConnected(Interval.Open(5, 10)); // returns false
Interval.Closed(1, 5).IsConnected(Interval.Closed(6, 10)); // returns false
```

#### `Intersection`

`Interval.Intersection(Interval)` returns the maximal interval enclosed by both this
interval and other (which exists iff these intervals are connected), or if no such
interval exists, throws an `ArgumentException`.

`Intersection` is a commutative, associative operation.

```cs
Interval.Closed(3, 5).Intersection(Interval.Open(5, 10)); // returns (5, 5]
Interval.Closed(0, 9).Intersection(Interval.Closed(3, 4)); // returns [3, 4]
Interval.Closed(0, 5).Intersection(Interval.Closed(3, 9)); // returns [3, 5]
Interval.Open(3, 5).Intersection(Interval.Open(5, 10)); // throws AE
Interval.Closed(1, 5).Intersection(Interval.Closed(6, 10)); // throws AE
```

#### `Span`

`Interval.Span(Interval)` returns the minimal interval that encloses both this interval
and other. If the intervals are both connected, this is their union.

`Span` is a commutative, associative and closed operation.

```cs
Interval.Closed(3, 5).Span(Interval.Open(5, 10)); // returns [3, 10)
Interval.Closed(0, 9).Span(Interval.Closed(3, 4)); // returns [0, 9]
Interval.Closed(0, 5).Span(Interval.Closed(3, 9)); // returns [0, 9]
Interval.Open(3, 5).Span(Interval.Open(5, 10)); // returns (3, 10)
Interval.Closed(1, 5).Span(Interval.Closed(6, 10)); // returns [1, 10]
```

## Credits

This small library is a porting to .Net of a few Java classes of the amazing
[Google Guava](https://github.com/google/guava) library.
