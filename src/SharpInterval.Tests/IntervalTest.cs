using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace SharpInterval.Tests;

/// <summary>
/// Unit test for <seealso cref="Interval"/>.
/// </summary>
[TestFixture]
public class IntervalTest
{
    [Test]
    public void Open()
    {
        Interval<int> range = Interval.Open(4, 8);
        range.Contains(4).Should().BeFalse();
        range.Contains(5).Should().BeTrue();
        range.Contains(7).Should().BeTrue();
        range.Contains(8).Should().BeFalse();
        range.HasLowerBound().Should().BeTrue();
        range.LowerEndpoint().Should().Be(4);
        range.LowerBoundType().Should().Be(BoundType.Open);
        range.HasUpperBound().Should().BeTrue();
        range.UpperEndpoint().Should().Be(8);
        range.UpperBoundType().Should().Be(BoundType.Open);
        range.IsEmpty().Should().BeFalse();
        range.ToString().Should().Be("(4..8)");
    }

    [Test]
    public void Open_Invalid()
    {
        Action act1 = () => Interval.Open(4, 3);
        act1.Should().Throw<ArgumentException>();

        Action act2 = () => Interval.Open(3, 3);
        act2.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Closed()
    {
        Interval<int> range = Interval.Closed(5, 7);
        range.Contains(4).Should().BeFalse();
        range.Contains(5).Should().BeTrue();
        range.Contains(7).Should().BeTrue();
        range.Contains(8).Should().BeFalse();
        range.HasLowerBound().Should().BeTrue();
        range.LowerEndpoint().Should().Be(5);
        range.LowerBoundType().Should().Be(BoundType.Closed);
        range.HasUpperBound().Should().BeTrue();
        range.UpperEndpoint().Should().Be(7);
        range.UpperBoundType().Should().Be(BoundType.Closed);
        range.IsEmpty().Should().BeFalse();
        range.ToString().Should().Be("[5..7]");
    }

    [Test]
    public void Closed_Invalid()
    {
        Action act1 = () => Interval.Closed(4, 3);
        act1.Should().Throw<ArgumentException>();
    }

    [Test]
    public void OpenClosed()
    {
        Interval<int> range = Interval.OpenClosed(4, 7);
        range.Contains(4).Should().BeFalse();
        range.Contains(5).Should().BeTrue();
        range.Contains(7).Should().BeTrue();
        range.Contains(8).Should().BeFalse();
        range.HasLowerBound().Should().BeTrue();
        range.LowerEndpoint().Should().Be(4);
        range.LowerBoundType().Should().Be(BoundType.Open);
        range.HasUpperBound().Should().BeTrue();
        range.UpperEndpoint().Should().Be(7);
        range.UpperBoundType().Should().Be(BoundType.Closed);
        range.IsEmpty().Should().BeFalse();
        range.ToString().Should().Be("(4..7]");
    }

    [Test]
    public void ClosedOpen()
    {
        Interval<int> range = Interval.ClosedOpen(5, 8);
        range.Contains(4).Should().BeFalse();
        range.Contains(5).Should().BeTrue();
        range.Contains(7).Should().BeTrue();
        range.Contains(8).Should().BeFalse();
        range.HasLowerBound().Should().BeTrue();
        range.LowerEndpoint().Should().Be(5);
        range.LowerBoundType().Should().Be(BoundType.Closed);
        range.HasUpperBound().Should().BeTrue();
        range.UpperEndpoint().Should().Be(8);
        range.UpperBoundType().Should().Be(BoundType.Open);
        range.IsEmpty().Should().BeFalse();
        range.ToString().Should().Be("[5..8)");
    }

    [Test]
    public void IsConnected()
    {
        Interval.Closed(3, 5).IsConnected(Interval.Open(5, 6)).Should().BeTrue();
        Interval.Closed(3, 5).IsConnected(Interval.OpenClosed(5, 5)).Should().BeTrue();
        Interval.Open(3, 5).IsConnected(Interval.Closed(5, 6)).Should().BeTrue();
        Interval.Closed(3, 7).IsConnected(Interval.Open(6, 8)).Should().BeTrue();
        Interval.Open(3, 7).IsConnected(Interval.Closed(5, 6)).Should().BeTrue();
        Interval.Closed(3, 5).IsConnected(Interval.Closed(7, 8)).Should().BeFalse();
        Interval.Closed(3, 5).IsConnected(Interval.ClosedOpen(7, 7)).Should().BeFalse();
    }

    [Test]
    public void Singleton()
    {
        Interval<int> range = Interval.Closed(4, 4);
        range.Contains(3).Should().BeFalse();
        range.Contains(4).Should().BeTrue();
        range.Contains(5).Should().BeFalse();
        range.HasLowerBound().Should().BeTrue();
        range.LowerEndpoint().Should().Be(4);
        range.LowerBoundType().Should().Be(BoundType.Closed);
        range.HasUpperBound().Should().BeTrue();
        range.UpperEndpoint().Should().Be(4);
        range.UpperBoundType().Should().Be(BoundType.Closed);
        range.IsEmpty().Should().BeFalse();
        range.ToString().Should().Be("[4..4]");
    }

    [Test]
    public void Empty1()
    {
        Interval<int> range = Interval.ClosedOpen(4, 4);
        range.Contains(3).Should().BeFalse();
        range.Contains(4).Should().BeFalse();
        range.Contains(5).Should().BeFalse();
        range.HasLowerBound().Should().BeTrue();
        range.LowerEndpoint().Should().Be(4);
        range.LowerBoundType().Should().Be(BoundType.Closed);
        range.HasUpperBound().Should().BeTrue();
        range.UpperEndpoint().Should().Be(4);
        range.UpperBoundType().Should().Be(BoundType.Open);
        range.IsEmpty().Should().BeTrue();
        range.ToString().Should().Be("[4..4)");
    }

    [Test]
    public void Empty2()
    {
        Interval<int> range = Interval.OpenClosed(4, 4);
        range.Contains(3).Should().BeFalse();
        range.Contains(4).Should().BeFalse();
        range.Contains(5).Should().BeFalse();
        range.HasLowerBound().Should().BeTrue();
        range.LowerEndpoint().Should().Be(4);
        range.LowerBoundType().Should().Be(BoundType.Open);
        range.HasUpperBound().Should().BeTrue();
        range.UpperEndpoint().Should().Be(4);
        range.UpperBoundType().Should().Be(BoundType.Closed);
        range.IsEmpty().Should().BeTrue();
        range.ToString().Should().Be("(4..4]");
    }

    [Test]
    public void LessThan()
    {
        Interval<int> range = Interval.LessThan(5);
        range.Contains(int.MinValue).Should().BeTrue();
        range.Contains(4).Should().BeTrue();
        range.Contains(5).Should().BeFalse();
        AssertUnboundedBelow(range);
        range.HasUpperBound().Should().BeTrue();
        range.UpperEndpoint().Should().Be(5);
        range.UpperBoundType().Should().Be(BoundType.Open);
        range.IsEmpty().Should().BeFalse();
        range.ToString().Should().Be("(-\u221e..5)");
    }

    [Test]
    public void GreaterThan()
    {
        Interval<int> range = Interval.GreaterThan(5);
        range.Contains(5).Should().BeFalse();
        range.Contains(6).Should().BeTrue();
        range.Contains(int.MaxValue).Should().BeTrue();
        range.HasLowerBound().Should().BeTrue();
        range.LowerEndpoint().Should().Be(5);
        range.LowerBoundType().Should().Be(BoundType.Open);
        AssertUnboundedAbove(range);
        range.IsEmpty().Should().BeFalse();
        range.ToString().Should().Be("(5..+\u221e)");
    }

    [Test]
    public void AtLeast()
    {
        Interval<int> range = Interval.AtLeast(6);
        range.Contains(5).Should().BeFalse();
        range.Contains(6).Should().BeTrue();
        range.Contains(int.MaxValue).Should().BeTrue();
        range.HasLowerBound().Should().BeTrue();
        range.LowerEndpoint().Should().Be(6);
        range.LowerBoundType().Should().Be(BoundType.Closed);
        AssertUnboundedAbove(range);
        range.IsEmpty().Should().BeFalse();
        range.ToString().Should().Be("[6..+\u221e)");
    }

    [Test]
    public void AtMost()
    {
        Interval<int> range = Interval.AtMost(4);
        range.Contains(int.MinValue).Should().BeTrue();
        range.Contains(4).Should().BeTrue();
        range.Contains(5).Should().BeFalse();
        AssertUnboundedBelow(range);
        range.HasUpperBound().Should().BeTrue();
        range.UpperEndpoint().Should().Be(4);
        range.UpperBoundType().Should().Be(BoundType.Closed);
        range.IsEmpty().Should().BeFalse();
        range.ToString().Should().Be("(-\u221e..4]");
    }

    [Test]
    public void All()
    {
        Interval<int> range = Interval.All<int>();
        range.Contains(int.MinValue).Should().BeTrue();
        range.Contains(int.MaxValue).Should().BeTrue();
        AssertUnboundedBelow(range);
        AssertUnboundedAbove(range);
        range.IsEmpty().Should().BeFalse();
        range.ToString().Should().Be("(-\u221e..+\u221e)");
    }

    private static void AssertUnboundedBelow(Interval<int> range)
    {
        range.HasLowerBound().Should().BeFalse();
        Action act1 = () => range.LowerEndpoint();
        act1.Should().Throw<InvalidOperationException>();
        Action act2 = () => range.LowerBoundType();
        act2.Should().Throw<InvalidOperationException>();
    }

    private static void AssertUnboundedAbove(Interval<int> range)
    {
        range.HasUpperBound().Should().BeFalse();

        Action act1 = () => range.UpperEndpoint();
        act1.Should().Throw<InvalidOperationException>();
            
        Action act2 = () => range.UpperBoundType();
        act2.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void ContainsAll()
    {
        Interval<int> range = Interval.Closed(3, 5);
        range.ContainsAll(new int[] { 3, 3, 4, 5 }).Should().BeTrue();
        range.ContainsAll(new int[] { 3, 3, 4, 5, 6 }).Should().BeFalse();

        Interval.OpenClosed(3, 3).ContainsAll(Enumerable.Empty<int>()).Should().BeTrue();
    }

    [Test]
    public void Encloses_Open()
    {
        Interval<int> range = Interval.Open(2, 5);
        range.Encloses(range).Should().BeTrue();
        range.Encloses(Interval.Open(2, 4)).Should().BeTrue();
        range.Encloses(Interval.Open(3, 5)).Should().BeTrue();
        range.Encloses(Interval.Closed(3, 4)).Should().BeTrue();

        range.Encloses(Interval.OpenClosed(2, 5)).Should().BeFalse();
        range.Encloses(Interval.ClosedOpen(2, 5)).Should().BeFalse();
        range.Encloses(Interval.Closed(1, 4)).Should().BeFalse();
        range.Encloses(Interval.Closed(3, 6)).Should().BeFalse();
        range.Encloses(Interval.GreaterThan(3)).Should().BeFalse();
        range.Encloses(Interval.LessThan(3)).Should().BeFalse();
        range.Encloses(Interval.AtLeast(3)).Should().BeFalse();
        range.Encloses(Interval.AtMost(3)).Should().BeFalse();
        range.Encloses(Interval.All<int>()).Should().BeFalse();
    }

    [Test]
    public void Encloses_Closed()
    {
        Interval<int> range = Interval.Closed(2, 5);
        range.Encloses(range).Should().BeTrue();
        range.Encloses(Interval.Open(2, 5)).Should().BeTrue();
        range.Encloses(Interval.OpenClosed(2, 5)).Should().BeTrue();
        range.Encloses(Interval.ClosedOpen(2, 5)).Should().BeTrue();
        range.Encloses(Interval.Closed(3, 5)).Should().BeTrue();
        range.Encloses(Interval.Closed(2, 4)).Should().BeTrue();

        range.Encloses(Interval.Open(1, 6)).Should().BeFalse();
        range.Encloses(Interval.GreaterThan(3)).Should().BeFalse();
        range.Encloses(Interval.LessThan(3)).Should().BeFalse();
        range.Encloses(Interval.AtLeast(3)).Should().BeFalse();
        range.Encloses(Interval.AtMost(3)).Should().BeFalse();
        range.Encloses(Interval.All<int>()).Should().BeFalse();
    }

    [Test]
    public void Intersection_Empty()
    {
        Interval<int> range = Interval.ClosedOpen(3, 3);
        range.Intersection(range).Should().Be(range);

        Action act1 = () => range.Intersection(Interval.Open(3, 5));
        act1.Should().Throw<ArgumentException>();

        Action act2 = () => range.Intersection(Interval.Closed(0, 2));
        act2.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Intersection_DeFactoEmpty()
    {
        Interval<int> range = Interval.Open(3, 4);
        range.Intersection(range).Should().Be(range);

        range.Intersection(Interval.AtMost(3)).Should().Be(Interval.OpenClosed(3, 3));
        range.Intersection(Interval.AtLeast(4)).Should().Be(Interval.ClosedOpen(4, 4));


        Action act1 = () => range.Intersection(Interval.LessThan(3));
        act1.Should().Throw<ArgumentException>();

        Action act2 = () => range.Intersection(Interval.GreaterThan(4));
        act2.Should().Throw<ArgumentException>();

        range = Interval.Closed(3, 4);
        range.Intersection(Interval.GreaterThan(4)).Should().Be(Interval.OpenClosed(4, 4));
    }

    [Test]
    public void Intersection_Singleton()
    {
        Interval<int> range = Interval.Closed(3, 3);
        range.Intersection(range).Should().Be(range);

        range.Intersection(Interval.AtMost(4)).Should().Be(range);
        range.Intersection(Interval.AtMost(3)).Should().Be(range);
        range.Intersection(Interval.AtLeast(3)).Should().Be(range);
        range.Intersection(Interval.AtLeast(2)).Should().Be(range);

        range.Intersection(Interval.LessThan(3)).Should().Be(Interval.ClosedOpen(3, 3));
        range.Intersection(Interval.GreaterThan(3)).Should().Be(Interval.OpenClosed(3, 3));

        Action act1 = () => range.Intersection(Interval.AtLeast(4));
        act1.Should().Throw<ArgumentException>();

        Action act2 = () => range.Intersection(Interval.AtMost(2));
        act2.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Intersection_General()
    {
        Interval<int> range = Interval.Closed(4, 8);

        // separate below
        Action act1 = () => range.Intersection(Interval.Closed(0, 2));
        act1.Should().Throw<ArgumentException>();

        // adjacent below
        range.Intersection(Interval.ClosedOpen(2, 4)).Should().Be(Interval.ClosedOpen(4, 4));

        // overlap below
        range.Intersection(Interval.Closed(2, 6)).Should().Be(Interval.Closed(4, 6));

        // enclosed with same start
        range.Intersection(Interval.Closed(4, 6)).Should().Be(Interval.Closed(4, 6));

        // enclosed, interior
        range.Intersection(Interval.Closed(5, 7)).Should().Be(Interval.Closed(5, 7));

        // enclosed with same end
        range.Intersection(Interval.Closed(6, 8)).Should().Be(Interval.Closed(6, 8));

        // equal
        range.Intersection(range).Should().Be(range);

        // enclosing with same start
        range.Intersection(Interval.Closed(4, 10)).Should().Be(range);

        // enclosing with same end
        range.Intersection(Interval.Closed(2, 8)).Should().Be(range);

        // enclosing, exterior
        range.Intersection(Interval.Closed(2, 10)).Should().Be(range);

        // overlap above
        range.Intersection(Interval.Closed(6, 10)).Should().Be(Interval.Closed(6, 8));

        // adjacent above
        range.Intersection(Interval.OpenClosed(8, 10)).Should().Be(Interval.OpenClosed(8, 8));

        // separate above
        Action act2 = () => range.Intersection(Interval.Closed(10, 12));
        act2.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Span_General()
    {
        Interval<int> range = Interval.Closed(4, 8);

        // separate below
        range.Span(Interval.Closed(0, 2)).Should().Be(Interval.Closed(0, 8));
        range.Span(Interval.AtMost(2)).Should().Be(Interval.AtMost(8));

        // adjacent below
        range.Span(Interval.ClosedOpen(2, 4)).Should().Be(Interval.Closed(2, 8));
        range.Span(Interval.LessThan(4)).Should().Be(Interval.AtMost(8));

        // overlap below
        range.Span(Interval.Closed(2, 6)).Should().Be(Interval.Closed(2, 8));
        range.Span(Interval.AtMost(6)).Should().Be(Interval.AtMost(8));

        // enclosed with same start
        range.Span(Interval.Closed(4, 6)).Should().Be(range);

        // enclosed, interior
        range.Span(Interval.Closed(5, 7)).Should().Be(range);

        // enclosed with same end
        range.Span(Interval.Closed(6, 8)).Should().Be(range);

        // equal
        range.Span(range).Should().Be(range);

        // enclosing with same start
        range.Span(Interval.Closed(4, 10)).Should().Be(Interval.Closed(4, 10));
        range.Span(Interval.AtLeast(4)).Should().Be(Interval.AtLeast(4));

        // enclosing with same end
        range.Span(Interval.Closed(2, 8)).Should().Be(Interval.Closed(2, 8));
        range.Span(Interval.AtMost(8)).Should().Be(Interval.AtMost(8));

        // enclosing, exterior
        range.Span(Interval.Closed(2, 10)).Should().Be(Interval.Closed(2, 10));
        range.Span(Interval.All<int>()).Should().Be(Interval.All<int>());

        // overlap above
        range.Span(Interval.Closed(6, 10)).Should().Be(Interval.Closed(4, 10));
        range.Span(Interval.AtLeast(6)).Should().Be(Interval.AtLeast(4));

        // adjacent above
        range.Span(Interval.OpenClosed(8, 10)).Should().Be(Interval.Closed(4, 10));
        range.Span(Interval.GreaterThan(8)).Should().Be(Interval.AtLeast(4));

        // separate above
        range.Span(Interval.Closed(10, 12)).Should().Be(Interval.Closed(4, 12));
        range.Span(Interval.AtLeast(10)).Should().Be(Interval.AtLeast(4));
    }

    [Test]
    public void Equals()
    {
        Interval<int> open1 = Interval.Open(1, 5);
        Interval<int> open2 = Interval.Bounded(1, BoundType.Open, 5, BoundType.Open);

        open1.Should().Be(open1);
        open1.Should().Be(open2);
        open2.Should().Be(open1);

        Interval<int> greater1 = Interval.GreaterThan(2);
        Interval<int> greater2 = Interval.GreaterThan(2);

        greater1.Should().Be(greater1);
        greater1.Should().Be(greater2);

        Interval<int> all1 = Interval.All<int>();
        Interval<int> all2 = Interval.All<int>();

        all1.Should().Be(all1);
        all1.Should().Be(all2);
    }

    [Test]
    public void EquivalentFactories()
    {
        Interval.AtLeast(1).Should().Be(Interval.DownTo(1, BoundType.Closed));
        Interval.GreaterThan(1).Should().Be(Interval.DownTo(1, BoundType.Open));
        Interval.AtMost(7).Should().Be(Interval.UpTo(7, BoundType.Closed));
        Interval.LessThan(7).Should().Be(Interval.UpTo(7, BoundType.Open));
        Interval.Open(1, 7).Should().Be(Interval.Bounded(1, BoundType.Open, 7, BoundType.Open));
        Interval.OpenClosed(1, 7).Should().Be(Interval.Bounded(1, BoundType.Open, 7, BoundType.Closed));
        Interval.Closed(1, 7).Should().Be(Interval.Bounded(1, BoundType.Closed, 7, BoundType.Closed));
        Interval.ClosedOpen(1, 7).Should().Be(Interval.Bounded(1, BoundType.Closed, 7, BoundType.Open));
    }

    [Test]
    public void ReadmeExamples()
    {
        var lexiInterval = Interval.Closed("left", "right");
        lexiInterval.Contains("lift").Should().BeTrue();
        Interval.LessThan(4.0);

        var boundType = BoundType.Closed;
        Interval.DownTo(4, boundType);
        Interval.Bounded(1, BoundType.Closed, 4, BoundType.Open);

        Interval.Closed(1, 3).Contains(2).Should().BeTrue();
        Interval.Closed(1, 3).Contains(4).Should().BeFalse();
        Interval.LessThan(5).Contains(5).Should().BeFalse();
        Interval.Closed(1, 4).ContainsAll(new int[] { 1, 2, 3 }).Should().BeTrue();

        Interval.ClosedOpen(4, 4).IsEmpty().Should().BeTrue();
        Interval.OpenClosed(4, 4).IsEmpty().Should().BeTrue();
        Interval.Closed(4, 4).IsEmpty().Should().BeFalse();
        Action act1 = () => Interval.Open(4, 4).IsEmpty();
        act1.Should().Throw<ArgumentException>();

        Interval.Closed(3, 10).LowerEndpoint().Should().Be(3);
        Interval.Open(3, 10).LowerEndpoint().Should().Be(3);
        Interval.Closed(3, 10).LowerBoundType().Should().Be(BoundType.Closed);
        Interval.Open(3, 10).UpperBoundType().Should().Be(BoundType.Open);

        Interval.Closed(3, 6).Encloses(Interval.Closed(4, 5)).Should().BeTrue();
        Interval.Open(3, 6).Encloses(Interval.Open(3, 6)).Should().BeTrue();
        Interval.Closed(4, 5).Encloses(Interval.Open(3, 6)).Should().BeFalse();

        Interval.Closed(3, 5).IsConnected(Interval.Open(5, 10)).Should().BeTrue();
        Interval.Closed(0, 9).IsConnected(Interval.Closed(3, 4)).Should().BeTrue();
        Interval.Closed(0, 5).IsConnected(Interval.Closed(3, 9)).Should().BeTrue();
        Interval.Open(3, 5).IsConnected(Interval.Open(5, 10)).Should().BeFalse();
        Interval.Closed(1, 5).IsConnected(Interval.Closed(6, 10)).Should().BeFalse();

        Interval.Closed(3, 5).Intersection(Interval.Open(5, 10)).Should().Be(Interval.OpenClosed(5, 5));
        Interval.Closed(0, 9).Intersection(Interval.Closed(3, 4)).Should().Be(Interval.Closed(3, 4));
        Interval.Closed(0, 5).Intersection(Interval.Closed(3, 9)).Should().Be(Interval.Closed(3, 5));
        Action act2 = () => Interval.Open(3, 5).Intersection(Interval.Open(5, 10));
        act2.Should().Throw<ArgumentException>();
        Action act3 = () => Interval.Closed(1, 5).Intersection(Interval.Closed(6, 10));
        act3.Should().Throw<ArgumentException>();

        Interval.Closed(3, 5).Span(Interval.Open(5, 10)).Should().Be(Interval.ClosedOpen(3, 10));
        Interval.Closed(0, 9).Span(Interval.Closed(3, 4)).Should().Be(Interval.Closed(0, 9));
        Interval.Closed(0, 5).Span(Interval.Closed(3, 9)).Should().Be(Interval.Closed(0, 9));
        Interval.Open(3, 5).Span(Interval.Open(5, 10)).Should().Be(Interval.Open(3, 10));
        Interval.Closed(1, 5).Span(Interval.Closed(6, 10)).Should().Be(Interval.Closed(1, 10));
    }
}
