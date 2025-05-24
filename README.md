[![build](https://github.com/paolofulgoni/SharpInterval/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/paolofulgoni/SharpInterval/actions/workflows/dotnet.yml?query=branch%3Amain)
[![NuGet](https://img.shields.io/nuget/v/SharpInterval.svg)](https://www.nuget.org/packages/SharpInterval)

# SharpInterval

Define, combine, and test value intervals in .NET

## Introduction

An interval represents a continuous range of values. Think of it as a segment on a number line, or a portion of a sequence. For example, all numbers between 1 and 5, or all dates in a specific week.

The word "range" is a common synonym for "interval," but this library uses the name `Interval` to avoid confusion with other programming constructs like `System.Range`.

Intervals can be finite (e.g., numbers between 2 and 5) or they can extend indefinitely in one or both directions (e.g., all numbers greater than 3).

To describe intervals concisely, we use a common mathematical notation:

*   An interval like **`(a..b)`** represents all values **greater than 'a' and less than 'b'**. 'a' and 'b' themselves are *not* included. This is called an **open interval**. For example, `(1..5)` includes numbers like 1.1, 2, 4.9, but not 1 or 5.
*   An interval like **`[a..b]`** represents all values **greater than or equal to 'a' and less than or equal to 'b'**. 'a' and 'b' *are* included. This is called a **closed interval**. For example, `[1..5]` includes 1, 5, and all numbers in between.
*   An interval like **`[a..b)`** represents all values **greater than or equal to 'a' and less than 'b'**. 'a' *is* included, but 'b' is *not*. This is a **half-open interval** (closed at the start, open at the end). For example, `[1..5)` includes 1, 4.999..., but not 5.
*   An interval like **`(a..b]`** represents all values **greater than 'a' and less than or equal to 'b'**. 'a' is *not* included, but 'b' *is*. This is also a **half-open interval** (open at the start, closed at the end). For example, `(1..5]` includes 1.000...1, 5, but not 1.

Intervals can also be unbounded on one side, using `+∞` (positive infinity) or `-∞` (negative infinity):

*   **`(a..+∞)`**: All values **strictly greater than 'a'**. For example, `(5..+∞)` means all numbers from 5.000...1 upwards.
*   **`[a..+∞)`**: All values **greater than or equal to 'a'**. For example, `[5..+∞)` means all numbers from 5 upwards.
*   **`(-∞..b)`**: All values **strictly less than 'b'**. For example, `(-∞..5)` means all numbers up to 4.999....
*   **`(-∞..b]`**: All values **less than or equal to 'b'**. For example, `(-∞..5]` means all numbers up to 5.
*   **`(-∞..+∞)`**: Represents **all possible values** in the domain.

The values 'a' and 'b' in these notations are called the **endpoints** of the interval. They define the boundaries of the range.
Whether an endpoint is included or excluded determines if the interval's bound is **closed** (inclusive) or **open** (exclusive).
For example, `[10..20]` means both 10 and 20 are included (closed bounds). In `(10..20)`, neither 10 nor 20 are included (open bounds).

In SharpInterval, the upper endpoint must generally be greater than or equal to the lower endpoint.
What happens if the endpoints are equal?
*   `[a..a]`: This creates an interval containing a single value 'a' (e.g., `[5..5]` contains only 5). This is a **singleton interval**.
*   `[a..a)` or `(a..a]`: These define an **empty interval**, but are still considered valid. For example, `[5..5)` means "numbers greater than or equal to 5, but less than 5," which is impossible, hence empty. Similarly for `(5..5]`.
*   `(a..a)`: This is **invalid** because it means "numbers strictly greater than 'a' and strictly less than 'a'," which cannot be represented logically as an interval in this library.

An interval in SharpInterval has the type `Interval<T>`. All intervals are *immutable*.

## Building Intervals

You can create intervals using static methods on the `Interval` class.

Here's how the notation maps to the factory methods:

Interval type | Method                | Plain English Explanation
:-------------| :-------------------- | :--------------------------------------------------------------------------
`(a..b)`      | `Open(T, T)`          | Values strictly between 'a' and 'b' (a and b are not included).
`[a..b]`      | `Closed(T, T)`        | Values between 'a' and 'b', including 'a' and 'b'.
`[a..b)`      | `ClosedOpen(T, T)`    | Values from 'a' (included) up to 'b' (not included).
`(a..b]`      | `OpenClosed(T, T)`    | Values from 'a' (not included) up to 'b' (included).
`(a..+∞)`     | `GreaterThan(T)`      | Values strictly greater than 'a'.
`[a..+∞)`     | `AtLeast(T)`          | Values greater than or equal to 'a'.
`(-∞..b)`     | `LessThan(T)`         | Values strictly less than 'b'.
`(-∞..b]`     | `AtMost(T)`           | Values less than or equal to 'b'.
`(-∞..+∞)`    | `All()`               | All possible values.

```cs
Interval.Closed("left", "right"); // All strings alphabetically (or in dictionary order) between "left" and "right", inclusive.
Interval.LessThan(4.0); // Double values strictly less than 4.0.
```

You can also construct intervals by specifying the `BoundType` (which can be `Closed` or `Open`) for each endpoint:

Interval type Creation                                            | Method                                       | Plain English Explanation
:-----------------------------------------------------------------| :------------------------------------------- | :-----------------------------------------------------------------------------------------------------
Bounded on both ends                                              | `Bounded(T, BoundType, T, BoundType)`        | Values between two points, where you can specify if each point is included or excluded.
Unbounded on top (e.g., `(a..+∞)` or `[a..+∞)`)                   | `DownTo(T, BoundType)`                       | Values from a starting point (which can be included or excluded based on `BoundType`) extending indefinitely upwards.
Unbounded on bottom (e.g., `(-∞..b)` or `(-∞..b]`)                | `UpTo(T, BoundType)`                         | Values up to an ending point (which can be included or excluded based on `BoundType`) extending indefinitely downwards.

Here, `BoundType` is an enum with two values: `BoundType.Closed` (the endpoint is included) and `BoundType.Open` (the endpoint is excluded).

```cs
// Creates an interval like [4..+∞) if boundType is Closed, or (4..+∞) if boundType is Open
Interval.DownTo(4, boundType); 

// This is another way to write Interval.ClosedOpen(1, 4), which is [1..4)
Interval.Bounded(1, BoundType.Closed, 4, BoundType.Open); 
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

These methods let you inspect the properties of an interval:

*   `HasLowerBound()`: Checks if the interval has a defined starting point (i.e., it's not `(-∞..something)`).
*   `HasUpperBound()`: Checks if the interval has a defined ending point (i.e., it's not `(something..+∞)`).
*   `LowerBoundType()`: Returns the type of the lower bound (`Open` or `Closed`). Throws an exception if there's no lower bound (it extends to `-∞`).
*   `UpperBoundType()`: Returns the type of the upper bound (`Open` or `Closed`). Throws an exception if there's no upper bound (it extends to `+∞`).
*   `LowerEndpoint()`: Gets the starting value of the interval. Throws an exception if there's no lower bound.
*   `UpperEndpoint()`: Gets the ending value of the interval. Throws an exception if there's no upper bound.
*   `IsEmpty()`: Checks if the interval represents an empty range. An interval is empty if its definition results in no values being included (e.g., `[5..5)` which means "values greater than or equal to 5 AND less than 5", or `(5..5]` which means "values greater than 5 AND less than or equal to 5").

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

`Encloses(Interval otherInterval)` checks if this interval completely contains another interval. This means the `otherInterval`'s start and end points don't go beyond this one's start and end points. The type of bounds (open/closed) also matters.

*   `[3..6]` encloses `[4..5]` (True: 4 is after 3, 5 is before 6, and the bounds are compatible)
*   `(3..6)` encloses `(3..6)` (True: Identical intervals)
*   `[3..6]` encloses `[4..4)` (True: Even though `[4..4)` is empty, its conceptual bounds fit within `[3..6]`)
*   `(3..6)` does not enclose `[3..6]` (False: The outer interval `(3..6)` excludes 3 and 6, while the inner `[3..6]` includes them. So, the inner interval is not *entirely* within the outer one's boundary rules.)
*   `[4..5]` does not enclose `(3..6)` (False: The interval `(3..6)` starts before 4 and ends after 5).
    *   **Important Note:** `Encloses` is about whether the *boundaries* of one interval fit within another, including how endpoints are treated (open/closed). It's possible for all *values* of one interval to be contained in another, but for the second interval not to be *enclosed*. For example, `[4..5]` contains every value present in `(4..4.5)` (e.g., 4.2). However, if we consider `(3..6)`, its bounds 3 and 6 are outside of `[4..5]`, so `[4..5]` does not enclose `(3..6)`.
*   `[3..6]` does not enclose `(1..1]` (False: `(1..1]` is empty, but its conceptual lower bound 1 is outside `[3..6]`).

Given this, `Interval` provides the following operations:

#### `IsConnected`

`IsConnected(Interval otherInterval)` checks if two intervals touch or overlap, meaning there's no gap between them. If they are connected, their combined span forms a new, single continuous interval.

`IsConnected` is a reflexive (an interval is connected to itself) and symmetric (if A is connected to B, B is connected to A) relation.

```cs
Interval.Closed(3, 5).IsConnected(Interval.Open(5, 10)); // returns true
Interval.Closed(0, 9).IsConnected(Interval.Closed(3, 4)); // returns true
Interval.Closed(0, 5).IsConnected(Interval.Closed(3, 9)); // returns true
Interval.Open(3, 5).IsConnected(Interval.Open(5, 10)); // returns false
Interval.Closed(1, 5).IsConnected(Interval.Closed(6, 10)); // returns false
```

#### `Intersection`

`Intersection(Interval otherInterval)` finds the common part of two intervals—that is, the range of values that exists in *both* intervals. If the intervals don't overlap (i.e., they are not connected), this operation is not possible and will throw an `ArgumentException`.

`Intersection` is a commutative (A intersect B = B intersect A) and associative ( (A intersect B) intersect C = A intersect (B intersect C) ) operation.

```cs
Interval.Closed(3, 5).Intersection(Interval.Open(5, 10)); // returns (5, 5]
Interval.Closed(0, 9).Intersection(Interval.Closed(3, 4)); // returns [3, 4]
Interval.Closed(0, 5).Intersection(Interval.Closed(3, 9)); // returns [3, 5]
Interval.Open(3, 5).Intersection(Interval.Open(5, 10)); // throws AE
Interval.Closed(1, 5).Intersection(Interval.Closed(6, 10)); // throws AE
```

#### `Span`

`Span(Interval otherInterval)` returns the smallest single interval that covers *both* input intervals. If the intervals touch or overlap, their span is effectively their union. If there's a gap between them, the span will include that gap.

`Span` is a commutative (A span B = B span A), associative ( (A span B) span C = A span (B span C) ), and closed (the result is always a valid interval) operation.

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
