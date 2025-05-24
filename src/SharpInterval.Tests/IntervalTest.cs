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
        var interval = Interval.Open(4, 8);
        interval.Contains(4).Should().BeFalse();
        interval.Contains(5).Should().BeTrue();
        interval.Contains(7).Should().BeTrue();
        interval.Contains(8).Should().BeFalse();
        interval.HasLowerBound().Should().BeTrue();
        interval.LowerEndpoint().Should().Be(4);
        interval.LowerBoundType().Should().Be(BoundType.Open);
        interval.HasUpperBound().Should().BeTrue();
        interval.UpperEndpoint().Should().Be(8);
        interval.UpperBoundType().Should().Be(BoundType.Open);
        interval.IsEmpty().Should().BeFalse();
        interval.ToString().Should().Be("(4..8)");
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
        var interval = Interval.Closed(5, 7);
        interval.Contains(4).Should().BeFalse();
        interval.Contains(5).Should().BeTrue();
        interval.Contains(7).Should().BeTrue();
        interval.Contains(8).Should().BeFalse();
        interval.HasLowerBound().Should().BeTrue();
        interval.LowerEndpoint().Should().Be(5);
        interval.LowerBoundType().Should().Be(BoundType.Closed);
        interval.HasUpperBound().Should().BeTrue();
        interval.UpperEndpoint().Should().Be(7);
        interval.UpperBoundType().Should().Be(BoundType.Closed);
        interval.IsEmpty().Should().BeFalse();
        interval.ToString().Should().Be("[5..7]");
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
        var interval = Interval.OpenClosed(4, 7);
        interval.Contains(4).Should().BeFalse();
        interval.Contains(5).Should().BeTrue();
        interval.Contains(7).Should().BeTrue();
        interval.Contains(8).Should().BeFalse();
        interval.HasLowerBound().Should().BeTrue();
        interval.LowerEndpoint().Should().Be(4);
        interval.LowerBoundType().Should().Be(BoundType.Open);
        interval.HasUpperBound().Should().BeTrue();
        interval.UpperEndpoint().Should().Be(7);
        interval.UpperBoundType().Should().Be(BoundType.Closed);
        interval.IsEmpty().Should().BeFalse();
        interval.ToString().Should().Be("(4..7]");
    }

    [Test]
    public void ClosedOpen()
    {
        var interval = Interval.ClosedOpen(5, 8);
        interval.Contains(4).Should().BeFalse();
        interval.Contains(5).Should().BeTrue();
        interval.Contains(7).Should().BeTrue();
        interval.Contains(8).Should().BeFalse();
        interval.HasLowerBound().Should().BeTrue();
        interval.LowerEndpoint().Should().Be(5);
        interval.LowerBoundType().Should().Be(BoundType.Closed);
        interval.HasUpperBound().Should().BeTrue();
        interval.UpperEndpoint().Should().Be(8);
        interval.UpperBoundType().Should().Be(BoundType.Open);
        interval.IsEmpty().Should().BeFalse();
        interval.ToString().Should().Be("[5..8)");
    }

    [Test]
    public void Bounded_Valid()
    {
        var openOpen = Interval.Bounded(4, BoundType.Open, 8, BoundType.Open);
        openOpen.Contains(4).Should().BeFalse();
        openOpen.Contains(8).Should().BeFalse();
        openOpen.HasLowerBound().Should().BeTrue();
        openOpen.LowerEndpoint().Should().Be(4);
        openOpen.LowerBoundType().Should().Be(BoundType.Open);
        openOpen.HasUpperBound().Should().BeTrue();
        openOpen.UpperEndpoint().Should().Be(8);
        openOpen.UpperBoundType().Should().Be(BoundType.Open);
        openOpen.Should().Be(Interval.Open(4, 8));

        var openClosed = Interval.Bounded(4, BoundType.Open, 8, BoundType.Closed);
        openClosed.Contains(4).Should().BeFalse();
        openClosed.Contains(8).Should().BeTrue();
        openClosed.HasLowerBound().Should().BeTrue();
        openClosed.LowerEndpoint().Should().Be(4);
        openClosed.LowerBoundType().Should().Be(BoundType.Open);
        openClosed.HasUpperBound().Should().BeTrue();
        openClosed.UpperEndpoint().Should().Be(8);
        openClosed.UpperBoundType().Should().Be(BoundType.Closed);
        openClosed.Should().Be(Interval.OpenClosed(4, 8));

        var closedOpen = Interval.Bounded(4, BoundType.Closed, 8, BoundType.Open);
        closedOpen.Contains(4).Should().BeTrue();
        closedOpen.Contains(8).Should().BeFalse();
        closedOpen.HasLowerBound().Should().BeTrue();
        closedOpen.LowerEndpoint().Should().Be(4);
        closedOpen.LowerBoundType().Should().Be(BoundType.Closed);
        closedOpen.HasUpperBound().Should().BeTrue();
        closedOpen.UpperEndpoint().Should().Be(8);
        closedOpen.UpperBoundType().Should().Be(BoundType.Open);
        closedOpen.Should().Be(Interval.ClosedOpen(4, 8));

        var closedClosed = Interval.Bounded(4, BoundType.Closed, 8, BoundType.Closed);
        closedClosed.Contains(4).Should().BeTrue();
        closedClosed.Contains(8).Should().BeTrue();
        closedClosed.HasLowerBound().Should().BeTrue();
        closedClosed.LowerEndpoint().Should().Be(4);
        closedClosed.LowerBoundType().Should().Be(BoundType.Closed);
        closedClosed.HasUpperBound().Should().BeTrue();
        closedClosed.UpperEndpoint().Should().Be(8);
        closedClosed.UpperBoundType().Should().Be(BoundType.Closed);
        closedClosed.Should().Be(Interval.Closed(4, 8));
    }

    [Test]
    public void Bounded_Invalid()
    {
        Action act1 = () => Interval.Bounded(4, BoundType.Closed, 3, BoundType.Open);
        act1.Should().Throw<ArgumentException>();

        Action act2 = () => Interval.Bounded(3, BoundType.Open, 3, BoundType.Open);
        act2.Should().Throw<ArgumentException>();
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
        var interval = Interval.Closed(4, 4);
        interval.Contains(3).Should().BeFalse();
        interval.Contains(4).Should().BeTrue();
        interval.Contains(5).Should().BeFalse();
        interval.HasLowerBound().Should().BeTrue();
        interval.LowerEndpoint().Should().Be(4);
        interval.LowerBoundType().Should().Be(BoundType.Closed);
        interval.HasUpperBound().Should().BeTrue();
        interval.UpperEndpoint().Should().Be(4);
        interval.UpperBoundType().Should().Be(BoundType.Closed);
        interval.IsEmpty().Should().BeFalse();
        interval.ToString().Should().Be("[4..4]");
    }

    [Test]
    public void Empty1()
    {
        var interval = Interval.ClosedOpen(4, 4);
        interval.Contains(3).Should().BeFalse();
        interval.Contains(4).Should().BeFalse();
        interval.Contains(5).Should().BeFalse();
        interval.HasLowerBound().Should().BeTrue();
        interval.LowerEndpoint().Should().Be(4);
        interval.LowerBoundType().Should().Be(BoundType.Closed);
        interval.HasUpperBound().Should().BeTrue();
        interval.UpperEndpoint().Should().Be(4);
        interval.UpperBoundType().Should().Be(BoundType.Open);
        interval.IsEmpty().Should().BeTrue();
        interval.ToString().Should().Be("[4..4)");
    }

    [Test]
    public void Empty2()
    {
        var interval = Interval.OpenClosed(4, 4);
        interval.Contains(3).Should().BeFalse();
        interval.Contains(4).Should().BeFalse();
        interval.Contains(5).Should().BeFalse();
        interval.HasLowerBound().Should().BeTrue();
        interval.LowerEndpoint().Should().Be(4);
        interval.LowerBoundType().Should().Be(BoundType.Open);
        interval.HasUpperBound().Should().BeTrue();
        interval.UpperEndpoint().Should().Be(4);
        interval.UpperBoundType().Should().Be(BoundType.Closed);
        interval.IsEmpty().Should().BeTrue();
        interval.ToString().Should().Be("(4..4]");
    }

    [Test]
    public void LessThan()
    {
        var interval = Interval.LessThan(5);
        interval.Contains(int.MinValue).Should().BeTrue();
        interval.Contains(4).Should().BeTrue();
        interval.Contains(5).Should().BeFalse();
        AssertUnboundedBelow(interval);
        interval.HasUpperBound().Should().BeTrue();
        interval.UpperEndpoint().Should().Be(5);
        interval.UpperBoundType().Should().Be(BoundType.Open);
        interval.IsEmpty().Should().BeFalse();
        interval.ToString().Should().Be("(-\u221e..5)");
    }

    [Test]
    public void GreaterThan()
    {
        var interval = Interval.GreaterThan(5);
        interval.Contains(5).Should().BeFalse();
        interval.Contains(6).Should().BeTrue();
        interval.Contains(int.MaxValue).Should().BeTrue();
        interval.HasLowerBound().Should().BeTrue();
        interval.LowerEndpoint().Should().Be(5);
        interval.LowerBoundType().Should().Be(BoundType.Open);
        AssertUnboundedAbove(interval);
        interval.IsEmpty().Should().BeFalse();
        interval.ToString().Should().Be("(5..+\u221e)");
    }

    [Test]
    public void AtLeast()
    {
        var interval = Interval.AtLeast(6);
        interval.Contains(5).Should().BeFalse();
        interval.Contains(6).Should().BeTrue();
        interval.Contains(int.MaxValue).Should().BeTrue();
        interval.HasLowerBound().Should().BeTrue();
        interval.LowerEndpoint().Should().Be(6);
        interval.LowerBoundType().Should().Be(BoundType.Closed);
        AssertUnboundedAbove(interval);
        interval.IsEmpty().Should().BeFalse();
        interval.ToString().Should().Be("[6..+\u221e)");
    }

    [Test]
    public void AtMost()
    {
        var interval = Interval.AtMost(4);
        interval.Contains(int.MinValue).Should().BeTrue();
        interval.Contains(4).Should().BeTrue();
        interval.Contains(5).Should().BeFalse();
        AssertUnboundedBelow(interval);
        interval.HasUpperBound().Should().BeTrue();
        interval.UpperEndpoint().Should().Be(4);
        interval.UpperBoundType().Should().Be(BoundType.Closed);
        interval.IsEmpty().Should().BeFalse();
        interval.ToString().Should().Be("(-\u221e..4]");
    }

    [Test]
    public void All()
    {
        var interval = Interval.All<int>();
        interval.Contains(int.MinValue).Should().BeTrue();
        interval.Contains(int.MaxValue).Should().BeTrue();
        AssertUnboundedBelow(interval);
        AssertUnboundedAbove(interval);
        interval.IsEmpty().Should().BeFalse();
        interval.ToString().Should().Be("(-\u221e..+\u221e)");
    }

    private static void AssertUnboundedBelow(Interval<int> interval)
    {
        interval.HasLowerBound().Should().BeFalse();
        Action act1 = () => interval.LowerEndpoint();
        act1.Should().Throw<InvalidOperationException>();
        Action act2 = () => interval.LowerBoundType();
        act2.Should().Throw<InvalidOperationException>();
    }

    private static void AssertUnboundedAbove(Interval<int> interval)
    {
        interval.HasUpperBound().Should().BeFalse();

        Action act1 = () => interval.UpperEndpoint();
        act1.Should().Throw<InvalidOperationException>();

        Action act2 = () => interval.UpperBoundType();
        act2.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void ContainsAll()
    {
        var interval = Interval.Closed(3, 5);
        interval.ContainsAll(new int[] { 3, 3, 4, 5 }).Should().BeTrue();
        interval.ContainsAll(new int[] { 3, 3, 4, 5, 6 }).Should().BeFalse();

        Interval.OpenClosed(3, 3).ContainsAll(Enumerable.Empty<int>()).Should().BeTrue();
    }

    [Test]
    public void Encloses_Open()
    {
        var interval = Interval.Open(2, 5);
        interval.Encloses(interval).Should().BeTrue();
        interval.Encloses(Interval.Open(2, 4)).Should().BeTrue();
        interval.Encloses(Interval.Open(3, 5)).Should().BeTrue();
        interval.Encloses(Interval.Closed(3, 4)).Should().BeTrue();

        interval.Encloses(Interval.OpenClosed(2, 5)).Should().BeFalse();
        interval.Encloses(Interval.ClosedOpen(2, 5)).Should().BeFalse();
        interval.Encloses(Interval.Closed(1, 4)).Should().BeFalse();
        interval.Encloses(Interval.Closed(3, 6)).Should().BeFalse();
        interval.Encloses(Interval.GreaterThan(3)).Should().BeFalse();
        interval.Encloses(Interval.LessThan(3)).Should().BeFalse();
        interval.Encloses(Interval.AtLeast(3)).Should().BeFalse();
        interval.Encloses(Interval.AtMost(3)).Should().BeFalse();
        interval.Encloses(Interval.All<int>()).Should().BeFalse();
    }

    [Test]
    public void Encloses_Closed()
    {
        var interval = Interval.Closed(2, 5);
        interval.Encloses(interval).Should().BeTrue();
        interval.Encloses(Interval.Open(2, 5)).Should().BeTrue();
        interval.Encloses(Interval.OpenClosed(2, 5)).Should().BeTrue();
        interval.Encloses(Interval.ClosedOpen(2, 5)).Should().BeTrue();
        interval.Encloses(Interval.Closed(3, 5)).Should().BeTrue();
        interval.Encloses(Interval.Closed(2, 4)).Should().BeTrue();

        interval.Encloses(Interval.Open(1, 6)).Should().BeFalse();
        interval.Encloses(Interval.GreaterThan(3)).Should().BeFalse();
        interval.Encloses(Interval.LessThan(3)).Should().BeFalse();
        interval.Encloses(Interval.AtLeast(3)).Should().BeFalse();
        interval.Encloses(Interval.AtMost(3)).Should().BeFalse();
        interval.Encloses(Interval.All<int>()).Should().BeFalse();
    }

    [Test]
    public void Intersection_Empty()
    {
        var interval = Interval.ClosedOpen(3, 3);
        interval.Intersection(interval).Should().Be(interval);

        Action act1 = () => interval.Intersection(Interval.Open(3, 5));
        act1.Should().Throw<ArgumentException>();

        Action act2 = () => interval.Intersection(Interval.Closed(0, 2));
        act2.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Intersection_DeFactoEmpty()
    {
        var interval = Interval.Open(3, 4);
        interval.Intersection(interval).Should().Be(interval);

        interval.Intersection(Interval.AtMost(3)).Should().Be(Interval.OpenClosed(3, 3));
        interval.Intersection(Interval.AtLeast(4)).Should().Be(Interval.ClosedOpen(4, 4));

        Action act1 = () => interval.Intersection(Interval.LessThan(3));
        act1.Should().Throw<ArgumentException>();

        Action act2 = () => interval.Intersection(Interval.GreaterThan(4));
        act2.Should().Throw<ArgumentException>();

        interval = Interval.Closed(3, 4);
        interval.Intersection(Interval.GreaterThan(4)).Should().Be(Interval.OpenClosed(4, 4));
    }

    [Test]
    public void Intersection_Singleton()
    {
        var interval = Interval.Closed(3, 3);
        interval.Intersection(interval).Should().Be(interval);

        interval.Intersection(Interval.AtMost(4)).Should().Be(interval);
        interval.Intersection(Interval.AtMost(3)).Should().Be(interval);
        interval.Intersection(Interval.AtLeast(3)).Should().Be(interval);
        interval.Intersection(Interval.AtLeast(2)).Should().Be(interval);

        interval.Intersection(Interval.LessThan(3)).Should().Be(Interval.ClosedOpen(3, 3));
        interval.Intersection(Interval.GreaterThan(3)).Should().Be(Interval.OpenClosed(3, 3));

        Action act1 = () => interval.Intersection(Interval.AtLeast(4));
        act1.Should().Throw<ArgumentException>();

        Action act2 = () => interval.Intersection(Interval.AtMost(2));
        act2.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Intersection_General()
    {
        var interval = Interval.Closed(4, 8);

        // separate below
        Action act1 = () => interval.Intersection(Interval.Closed(0, 2));
        act1.Should().Throw<ArgumentException>();

        // adjacent below
        interval.Intersection(Interval.ClosedOpen(2, 4)).Should().Be(Interval.ClosedOpen(4, 4));

        // overlap below
        interval.Intersection(Interval.Closed(2, 6)).Should().Be(Interval.Closed(4, 6));

        // enclosed with same start
        interval.Intersection(Interval.Closed(4, 6)).Should().Be(Interval.Closed(4, 6));

        // enclosed, interior
        interval.Intersection(Interval.Closed(5, 7)).Should().Be(Interval.Closed(5, 7));

        // enclosed with same end
        interval.Intersection(Interval.Closed(6, 8)).Should().Be(Interval.Closed(6, 8));

        // equal
        interval.Intersection(interval).Should().Be(interval);

        // enclosing with same start
        interval.Intersection(Interval.Closed(4, 10)).Should().Be(interval);

        // enclosing with same end
        interval.Intersection(Interval.Closed(2, 8)).Should().Be(interval);

        // enclosing, exterior
        interval.Intersection(Interval.Closed(2, 10)).Should().Be(interval);

        // overlap above
        interval.Intersection(Interval.Closed(6, 10)).Should().Be(Interval.Closed(6, 8));

        // adjacent above
        interval.Intersection(Interval.OpenClosed(8, 10)).Should().Be(Interval.OpenClosed(8, 8));

        // separate above
        Action act2 = () => interval.Intersection(Interval.Closed(10, 12));
        act2.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Span_General()
    {
        var interval = Interval.Closed(4, 8);

        // separate below
        interval.Span(Interval.Closed(0, 2)).Should().Be(Interval.Closed(0, 8));
        interval.Span(Interval.AtMost(2)).Should().Be(Interval.AtMost(8));

        // adjacent below
        interval.Span(Interval.ClosedOpen(2, 4)).Should().Be(Interval.Closed(2, 8));
        interval.Span(Interval.LessThan(4)).Should().Be(Interval.AtMost(8));

        // overlap below
        interval.Span(Interval.Closed(2, 6)).Should().Be(Interval.Closed(2, 8));
        interval.Span(Interval.AtMost(6)).Should().Be(Interval.AtMost(8));

        // enclosed with same start
        interval.Span(Interval.Closed(4, 6)).Should().Be(interval);

        // enclosed, interior
        interval.Span(Interval.Closed(5, 7)).Should().Be(interval);

        // enclosed with same end
        interval.Span(Interval.Closed(6, 8)).Should().Be(interval);

        // equal
        interval.Span(interval).Should().Be(interval);

        // enclosing with same start
        interval.Span(Interval.Closed(4, 10)).Should().Be(Interval.Closed(4, 10));
        interval.Span(Interval.AtLeast(4)).Should().Be(Interval.AtLeast(4));

        // enclosing with same end
        interval.Span(Interval.Closed(2, 8)).Should().Be(Interval.Closed(2, 8));
        interval.Span(Interval.AtMost(8)).Should().Be(Interval.AtMost(8));

        // enclosing, exterior
        interval.Span(Interval.Closed(2, 10)).Should().Be(Interval.Closed(2, 10));
        interval.Span(Interval.All<int>()).Should().Be(Interval.All<int>());

        // overlap above
        interval.Span(Interval.Closed(6, 10)).Should().Be(Interval.Closed(4, 10));
        interval.Span(Interval.AtLeast(6)).Should().Be(Interval.AtLeast(4));

        // adjacent above
        interval.Span(Interval.OpenClosed(8, 10)).Should().Be(Interval.Closed(4, 10));
        interval.Span(Interval.GreaterThan(8)).Should().Be(Interval.AtLeast(4));

        // separate above
        interval.Span(Interval.Closed(10, 12)).Should().Be(Interval.Closed(4, 12));
        interval.Span(Interval.AtLeast(10)).Should().Be(Interval.AtLeast(4));
    }

    [Test]
    public void Equals()
    {
        var open1 = Interval.Open(1, 5);
        var open2 = Interval.Bounded(1, BoundType.Open, 5, BoundType.Open);

        open1.Should().Be(open1);
        open1.Should().Be(open2);
        open2.Should().Be(open1);

        var greater1 = Interval.GreaterThan(2);
        var greater2 = Interval.GreaterThan(2);

        greater1.Should().Be(greater1);
        greater1.Should().Be(greater2);

        var all1 = Interval.All<int>();
        var all2 = Interval.All<int>();

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
