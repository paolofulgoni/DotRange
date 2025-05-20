[![build](https://github.com/paolofulgoni/DotRange/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/paolofulgoni/DotRange/actions/workflows/dotnet.yml?query=branch%3Amaster)
[![NuGet](https://img.shields.io/nuget/v/DotRange.svg)](https://www.nuget.org/packages/DotRange)

# DotRange

The Range class for .Net Standard

## Introduction

A range, sometimes known as an interval, is a convex (informally, "contiguous"
or "unbroken") portion of a particular domain. Formally, convexity means that
for any `a <= b <= c`, `range.Contains(a) && range.Contains(c)` implies that
`range.Contains(b)`.

Ranges may "extend to infinity" -- for example, the range `x > 3` contains
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
DotRange's notion of `Range` requires that the upper endpoint may not be less than
the lower endpoint. The endpoints may be equal only if at least one of the
bounds is closed:

*   `[a..a]`: singleton range
*   `[a..a); (a..a]`: empty, but valid
*   `(a..a)`: invalid

A range in DotRange has the type `Range<C>`. All ranges are *immutable*.

## Building Ranges

Ranges can be obtained from the static methods on `Range`:

Range type | Method
:--------- | :-------------------
`(a..b)`   | `Open(C, C)`
`[a..b]`   | `Closed(C, C)`
`[a..b)`   | `ClosedOpen(C, C)`
`(a..b]`   | `OpenClosed(C, C)`
`(a..+∞)`  | `GreaterThan(C)`
`[a..+∞)`  | `AtLeast(C)`
`(-∞..b)`  | `LessThan(C)`
`(-∞..b]`  | `AtMost(C)`
`(-∞..+∞)` | `All()`

```cs
Range.Closed("left", "right"); // all strings lexicographically between "left" and "right" inclusive
Range.LessThan(4.0); // double values strictly less than 4
```

Additionally, Range instances can be constructed by passing the bound types
explicitly:

Range type                                   | Method
:------------------------------------------- | :-----
Bounded on both ends                         | `Bounded(C, BoundType, C, BoundType)`
Unbounded on top (`(a..+∞)` or `[a..+∞)`)    | `DownTo(C, BoundType)`
Unbounded on bottom (`(-∞..b)` or `(-∞..b]`) | `UpTo(C, BoundType)`

Here, `BoundType` is an enum containing the values `Closed` and `Open`.

```cs
Range.DownTo(4, boundType); // allows you to decide whether or not you want to include 4
Range.Bounded(1, BoundType.Closed, 4, BoundType.Open); // another way of writing Range.ClosedOpen(1, 4)
```

## Operations

The fundamental operation of a `Range` is its `Contains(C)` methods, which
behaves exactly as you might expect. Any `Range` also supports 
`ContainsAll(IEnumerable)`.

```cs
Range.Closed(1, 3).Contains(2); // returns true
Range.Closed(1, 3).Contains(4); // returns false
Range.LessThan(5).Contains(5); // returns false
Range.Closed(1, 4).ContainsAll(new int[] { 1, 2, 3 }); // returns true
```

### Query Operations

To look at the endpoints of a range, `Range` exposes the following methods:

*   `HasLowerBound()` and `HasUpperBound()`, which check if the range has
    the specified endpoints, or goes on "through infinity."
*   `LowerBoundType()` and `UpperBoundType()` return the `BoundType` for the
    corresponding endpoint, which can be either `Closed` or `Open`. If this
    range does not have the specified endpoint, the method throws an
    `InvalidOperationException`.
*   `LowerEndpoint()` and `UpperEndpoint()` return the endpoints on the
    specified end, or throw an `InvalidOperationException` if the range does not
    have the specified endpoint.
*   `IsEmpty()` tests if the range is empty, that is, it has the form `[a,a)`
    or `(a,a]`.

```cs
Range.ClosedOpen(4, 4).IsEmpty(); // returns true
Range.OpenClosed(4, 4).IsEmpty(); // returns true
Range.Closed(4, 4).IsEmpty(); // returns false
Range.Open(4, 4).IsEmpty(); // Range.Open throws ArgumentException

Range.Closed(3, 10).LowerEndpoint(); // returns 3
Range.Open(3, 10).LowerEndpoint(); // returns 3
Range.Closed(3, 10).LowerBoundType(); // returns Closed
Range.Open(3, 10).UpperBoundType(); // returns Open
```

### Interval Operations

#### `Encloses`

The most basic relation on ranges is `Encloses(Range)`, which is true if the
bounds of the inner range do not extend outside the bounds of the outer range.
This is solely dependent on comparisons between the endpoints!

*   `[3..6]` encloses `[4..5]`
*   `(3..6)` encloses `(3..6)`
*   `[3..6]` encloses `[4..4)` (even though the latter is empty)
*   `(3..6]` does not enclose `[3..6]`
*   `[4..5]` does not enclose `(3..6)` **even though it contains every value
    contained by the latter range**
*   `[3..6]` does not enclose `(1..1]` **even though it contains every value
    contained by the latter range**

Given this, `Range` provides the following operations:

#### `IsConnected`

`Range.IsConnected(Range)`, which tests if these ranges are *connected*.
Specifically, `isConnected` tests if there is some range enclosed by both of
these ranges, but this is equivalent to the mathematical definition that the
union of the ranges must form a connected set (except in the special case of
empty ranges).

`IsConnected` is a reflexive, symmetric relation.

```cs
Range.Closed(3, 5).IsConnected(Range.Open(5, 10)); // returns true
Range.Closed(0, 9).IsConnected(Range.Closed(3, 4)); // returns true
Range.Closed(0, 5).IsConnected(Range.Closed(3, 9)); // returns true
Range.Open(3, 5).IsConnected(Range.Open(5, 10)); // returns false
Range.Closed(1, 5).IsConnected(Range.Closed(6, 10)); // returns false
```

#### `Intersection`

`Range.Intersection(Range)` returns the maximal range enclosed by both this
range and other (which exists iff these ranges are connected), or if no such
range exists, throws an `ArgumentException`.

`Intersection` is a commutative, associative operation.

```cs
Range.Closed(3, 5).Intersection(Range.Open(5, 10)); // returns (5, 5]
Range.Closed(0, 9).Intersection(Range.Closed(3, 4)); // returns [3, 4]
Range.Closed(0, 5).Intersection(Range.Closed(3, 9)); // returns [3, 5]
Range.Open(3, 5).Intersection(Range.Open(5, 10)); // throws AE
Range.Closed(1, 5).Intersection(Range.Closed(6, 10)); // throws AE
```

#### `Span`

`Range.Span(Range)` returns the minimal range that encloses both this range
and other. If the ranges are both connected, this is their union.

`Span` is a commutative, associative and closed operation.

```cs
Range.Closed(3, 5).Span(Range.Open(5, 10)); // returns [3, 10)
Range.Closed(0, 9).Span(Range.Closed(3, 4)); // returns [0, 9]
Range.Closed(0, 5).Span(Range.Closed(3, 9)); // returns [0, 9]
Range.Open(3, 5).Span(Range.Open(5, 10)); // returns (3, 10)
Range.Closed(1, 5).Span(Range.Closed(6, 10)); // returns [1, 10]
```

## Credits

This small library is a porting to .Net of a few Java classes of the amazing
[Google Guava](https://github.com/google/guava) library.
