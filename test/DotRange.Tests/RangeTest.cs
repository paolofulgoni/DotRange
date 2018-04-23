using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;

namespace DotRange
{
    /// <summary>
    /// Unit test for <seealso cref="Range"/>.
    /// </summary>
    [TestFixture]
    public class RangeTest
    {
        [Test]
        public void Open()
        {
            Range<int> range = Range.Open(4, 8);
            range.Contains(4).Should().BeFalse();
            range.Contains(5).Should().BeTrue();
            range.Contains(7).Should().BeTrue();
            range.Contains(8).Should().BeFalse();
            range.HasLowerBound().Should().BeTrue();
            range.LowerEndpoint().Should().Be(4);
            range.LowerBoundType().Should().Be(BoundType.OPEN);
            range.HasUpperBound().Should().BeTrue();
            range.UpperEndpoint().Should().Be(8);
            range.UpperBoundType().Should().Be(BoundType.OPEN);
            range.Empty.Should().BeFalse();
            range.ToString().Should().Be("(4..8)");
            range.Should().BeBinarySerializable();
        }

        [Test]
        public void Open_Invalid()
        {
            Action act1 = () => Range.Open(4, 3);
            act1.Should().Throw<ArgumentException>();

            Action act2 = () => Range.Open(3, 3);
            act2.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Closed()
        {
            Range<int> range = Range.Closed(5, 7);
            range.Contains(4).Should().BeFalse();
            range.Contains(5).Should().BeTrue();
            range.Contains(7).Should().BeTrue();
            range.Contains(8).Should().BeFalse();
            range.HasLowerBound().Should().BeTrue();
            range.LowerEndpoint().Should().Be(5);
            range.LowerBoundType().Should().Be(BoundType.CLOSED);
            range.HasUpperBound().Should().BeTrue();
            range.UpperEndpoint().Should().Be(7);
            range.UpperBoundType().Should().Be(BoundType.CLOSED);
            range.Empty.Should().BeFalse();
            range.ToString().Should().Be("[5..7]");
            range.Should().BeBinarySerializable();
        }

        [Test]
        public void Closed_Invalid()
        {
            Action act1 = () => Range.Closed(4, 3);
            act1.Should().Throw<ArgumentException>();
        }

        [Test]
        public void OpenClosed()
        {
            Range<int> range = Range.OpenClosed(4, 7);
            range.Contains(4).Should().BeFalse();
            range.Contains(5).Should().BeTrue();
            range.Contains(7).Should().BeTrue();
            range.Contains(8).Should().BeFalse();
            range.HasLowerBound().Should().BeTrue();
            range.LowerEndpoint().Should().Be(4);
            range.LowerBoundType().Should().Be(BoundType.OPEN);
            range.HasUpperBound().Should().BeTrue();
            range.UpperEndpoint().Should().Be(7);
            range.UpperBoundType().Should().Be(BoundType.CLOSED);
            range.Empty.Should().BeFalse();
            range.ToString().Should().Be("(4..7]");
            range.Should().BeBinarySerializable();
        }

        [Test]
        public void ClosedOpen()
        {
            Range<int> range = Range.ClosedOpen(5, 8);
            range.Contains(4).Should().BeFalse();
            range.Contains(5).Should().BeTrue();
            range.Contains(7).Should().BeTrue();
            range.Contains(8).Should().BeFalse();
            range.HasLowerBound().Should().BeTrue();
            range.LowerEndpoint().Should().Be(5);
            range.LowerBoundType().Should().Be(BoundType.CLOSED);
            range.HasUpperBound().Should().BeTrue();
            range.UpperEndpoint().Should().Be(8);
            range.UpperBoundType().Should().Be(BoundType.OPEN);
            range.Empty.Should().BeFalse();
            range.ToString().Should().Be("[5..8)");
            range.Should().BeBinarySerializable();
        }

        [Test]
        public void IsConnected()
        {
            Range.Closed(3, 5).IsConnected(Range.Open(5, 6)).Should().BeTrue();
            Range.Closed(3, 5).IsConnected(Range.OpenClosed(5, 5)).Should().BeTrue();
            Range.Open(3, 5).IsConnected(Range.Closed(5, 6)).Should().BeTrue();
            Range.Closed(3, 7).IsConnected(Range.Open(6, 8)).Should().BeTrue();
            Range.Open(3, 7).IsConnected(Range.Closed(5, 6)).Should().BeTrue();
            Range.Closed(3, 5).IsConnected(Range.Closed(7, 8)).Should().BeFalse();
            Range.Closed(3, 5).IsConnected(Range.ClosedOpen(7, 7)).Should().BeFalse();
        }

        [Test]
        public void Singleton()
        {
            Range<int> range = Range.Closed(4, 4);
            range.Contains(3).Should().BeFalse();
            range.Contains(4).Should().BeTrue();
            range.Contains(5).Should().BeFalse();
            range.HasLowerBound().Should().BeTrue();
            range.LowerEndpoint().Should().Be(4);
            range.LowerBoundType().Should().Be(BoundType.CLOSED);
            range.HasUpperBound().Should().BeTrue();
            range.UpperEndpoint().Should().Be(4);
            range.UpperBoundType().Should().Be(BoundType.CLOSED);
            range.Empty.Should().BeFalse();
            range.ToString().Should().Be("[4..4]");
            range.Should().BeBinarySerializable();
        }

        [Test]
        public void Empty1()
        {
            Range<int> range = Range.ClosedOpen(4, 4);
            range.Contains(3).Should().BeFalse();
            range.Contains(4).Should().BeFalse();
            range.Contains(5).Should().BeFalse();
            range.HasLowerBound().Should().BeTrue();
            range.LowerEndpoint().Should().Be(4);
            range.LowerBoundType().Should().Be(BoundType.CLOSED);
            range.HasUpperBound().Should().BeTrue();
            range.UpperEndpoint().Should().Be(4);
            range.UpperBoundType().Should().Be(BoundType.OPEN);
            range.Empty.Should().BeTrue();
            range.ToString().Should().Be("[4..4)");
            range.Should().BeBinarySerializable();
        }

        [Test]
        public void Empty2()
        {
            Range<int> range = Range.OpenClosed(4, 4);
            range.Contains(3).Should().BeFalse();
            range.Contains(4).Should().BeFalse();
            range.Contains(5).Should().BeFalse();
            range.HasLowerBound().Should().BeTrue();
            range.LowerEndpoint().Should().Be(4);
            range.LowerBoundType().Should().Be(BoundType.OPEN);
            range.HasUpperBound().Should().BeTrue();
            range.UpperEndpoint().Should().Be(4);
            range.UpperBoundType().Should().Be(BoundType.CLOSED);
            range.Empty.Should().BeTrue();
            range.ToString().Should().Be("(4..4]");
            range.Should().BeBinarySerializable();
        }

        [Test]
        public void LessThan()
        {
            Range<int> range = Range.LessThan(5);
            range.Contains(int.MinValue).Should().BeTrue();
            range.Contains(4).Should().BeTrue();
            range.Contains(5).Should().BeFalse();
            AssertUnboundedBelow(range);
            range.HasUpperBound().Should().BeTrue();
            range.UpperEndpoint().Should().Be(5);
            range.UpperBoundType().Should().Be(BoundType.OPEN);
            range.Empty.Should().BeFalse();
            range.ToString().Should().Be("(-\u221e..5)");
            range.Should().BeBinarySerializable();
        }

        [Test]
        public void GreaterThan()
        {
            Range<int> range = Range.GreaterThan(5);
            range.Contains(5).Should().BeFalse();
            range.Contains(6).Should().BeTrue();
            range.Contains(int.MaxValue).Should().BeTrue();
            range.HasLowerBound().Should().BeTrue();
            range.LowerEndpoint().Should().Be(5);
            range.LowerBoundType().Should().Be(BoundType.OPEN);
            AssertUnboundedAbove(range);
            range.Empty.Should().BeFalse();
            range.ToString().Should().Be("(5..+\u221e)");
            range.Should().BeBinarySerializable();
        }

        [Test]
        public void AtLeast()
        {
            Range<int> range = Range.AtLeast(6);
            range.Contains(5).Should().BeFalse();
            range.Contains(6).Should().BeTrue();
            range.Contains(int.MaxValue).Should().BeTrue();
            range.HasLowerBound().Should().BeTrue();
            range.LowerEndpoint().Should().Be(6);
            range.LowerBoundType().Should().Be(BoundType.CLOSED);
            AssertUnboundedAbove(range);
            range.Empty.Should().BeFalse();
            range.ToString().Should().Be("[6..+\u221e)");
            range.Should().BeBinarySerializable();
        }

        [Test]
        public void AtMost()
        {
            Range<int> range = Range.AtMost(4);
            range.Contains(int.MinValue).Should().BeTrue();
            range.Contains(4).Should().BeTrue();
            range.Contains(5).Should().BeFalse();
            AssertUnboundedBelow(range);
            range.HasUpperBound().Should().BeTrue();
            range.UpperEndpoint().Should().Be(4);
            range.UpperBoundType().Should().Be(BoundType.CLOSED);
            range.Empty.Should().BeFalse();
            range.ToString().Should().Be("(-\u221e..4]");
            range.Should().BeBinarySerializable();
        }

        [Test]
        public void All()
        {
            Range<int> range = Range.All<int>();
            range.Contains(int.MinValue).Should().BeTrue();
            range.Contains(int.MaxValue).Should().BeTrue();
            AssertUnboundedBelow(range);
            AssertUnboundedAbove(range);
            range.Empty.Should().BeFalse();
            range.ToString().Should().Be("(-\u221e..+\u221e)");
            range.Should().BeBinarySerializable();
        }

        private static void AssertUnboundedBelow(Range<int> range)
        {
            range.HasLowerBound().Should().BeFalse();
            Action act1 = () => range.LowerEndpoint();
            act1.Should().Throw<InvalidOperationException>();
            Action act2 = () => range.LowerBoundType();
            act2.Should().Throw<InvalidOperationException>();
        }

        private static void AssertUnboundedAbove(Range<int> range)
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
            Range<int> range = Range.Closed(3, 5);
            range.ContainsAll(new int[] { 3, 3, 4, 5 }).Should().BeTrue();
            range.ContainsAll(new int[] { 3, 3, 4, 5, 6 }).Should().BeFalse();

            Range.OpenClosed(3, 3).ContainsAll(Enumerable.Empty<int>()).Should().BeTrue();
        }

        [Test]
        public void Encloses_Open()
        {
            Range<int> range = Range.Open(2, 5);
            range.Encloses(range).Should().BeTrue();
            range.Encloses(Range.Open(2, 4)).Should().BeTrue();
            range.Encloses(Range.Open(3, 5)).Should().BeTrue();
            range.Encloses(Range.Closed(3, 4)).Should().BeTrue();

            range.Encloses(Range.OpenClosed(2, 5)).Should().BeFalse();
            range.Encloses(Range.ClosedOpen(2, 5)).Should().BeFalse();
            range.Encloses(Range.Closed(1, 4)).Should().BeFalse();
            range.Encloses(Range.Closed(3, 6)).Should().BeFalse();
            range.Encloses(Range.GreaterThan(3)).Should().BeFalse();
            range.Encloses(Range.LessThan(3)).Should().BeFalse();
            range.Encloses(Range.AtLeast(3)).Should().BeFalse();
            range.Encloses(Range.AtMost(3)).Should().BeFalse();
            range.Encloses(Range.All<int>()).Should().BeFalse();
        }

        [Test]
        public void Encloses_Closed()
        {
            Range<int> range = Range.Closed(2, 5);
            range.Encloses(range).Should().BeTrue();
            range.Encloses(Range.Open(2, 5)).Should().BeTrue();
            range.Encloses(Range.OpenClosed(2, 5)).Should().BeTrue();
            range.Encloses(Range.ClosedOpen(2, 5)).Should().BeTrue();
            range.Encloses(Range.Closed(3, 5)).Should().BeTrue();
            range.Encloses(Range.Closed(2, 4)).Should().BeTrue();

            range.Encloses(Range.Open(1, 6)).Should().BeFalse();
            range.Encloses(Range.GreaterThan(3)).Should().BeFalse();
            range.Encloses(Range.LessThan(3)).Should().BeFalse();
            range.Encloses(Range.AtLeast(3)).Should().BeFalse();
            range.Encloses(Range.AtMost(3)).Should().BeFalse();
            range.Encloses(Range.All<int>()).Should().BeFalse();
        }

        [Test]
        public void Intersection_Empty()
        {
            Range<int> range = Range.ClosedOpen(3, 3);
            range.Intersection(range).Should().Be(range);

            Action act1 = () => range.Intersection(Range.Open(3, 5));
            act1.Should().Throw<ArgumentException>();

            Action act2 = () => range.Intersection(Range.Closed(0, 2));
            act2.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Intersection_DeFactoEmpty()
        {
            Range<int> range = Range.Open(3, 4);
            range.Intersection(range).Should().Be(range);

            range.Intersection(Range.AtMost(3)).Should().Be(Range.OpenClosed(3, 3));
            range.Intersection(Range.AtLeast(4)).Should().Be(Range.ClosedOpen(4, 4));


            Action act1 = () => range.Intersection(Range.LessThan(3));
            act1.Should().Throw<ArgumentException>();

            Action act2 = () => range.Intersection(Range.GreaterThan(4));
            act2.Should().Throw<ArgumentException>();

            range = Range.Closed(3, 4);
            range.Intersection(Range.GreaterThan(4)).Should().Be(Range.OpenClosed(4, 4));
        }

        [Test]
        public void Intersection_Singleton()
        {
            Range<int> range = Range.Closed(3, 3);
            range.Intersection(range).Should().Be(range);

            range.Intersection(Range.AtMost(4)).Should().Be(range);
            range.Intersection(Range.AtMost(3)).Should().Be(range);
            range.Intersection(Range.AtLeast(3)).Should().Be(range);
            range.Intersection(Range.AtLeast(2)).Should().Be(range);

            range.Intersection(Range.LessThan(3)).Should().Be(Range.ClosedOpen(3, 3));
            range.Intersection(Range.GreaterThan(3)).Should().Be(Range.OpenClosed(3, 3));

            Action act1 = () => range.Intersection(Range.AtLeast(4));
            act1.Should().Throw<ArgumentException>();

            Action act2 = () => range.Intersection(Range.AtMost(2));
            act2.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Intersection_General()
        {
            Range<int> range = Range.Closed(4, 8);

            // separate below
            Action act1 = () => range.Intersection(Range.Closed(0, 2));
            act1.Should().Throw<ArgumentException>();

            // adjacent below
            range.Intersection(Range.ClosedOpen(2, 4)).Should().Be(Range.ClosedOpen(4, 4));

            // overlap below
            range.Intersection(Range.Closed(2, 6)).Should().Be(Range.Closed(4, 6));

            // enclosed with same start
            range.Intersection(Range.Closed(4, 6)).Should().Be(Range.Closed(4, 6));

            // enclosed, interior
            range.Intersection(Range.Closed(5, 7)).Should().Be(Range.Closed(5, 7));

            // enclosed with same end
            range.Intersection(Range.Closed(6, 8)).Should().Be(Range.Closed(6, 8));

            // equal
            range.Intersection(range).Should().Be(range);

            // enclosing with same start
            range.Intersection(Range.Closed(4, 10)).Should().Be(range);

            // enclosing with same end
            range.Intersection(Range.Closed(2, 8)).Should().Be(range);

            // enclosing, exterior
            range.Intersection(Range.Closed(2, 10)).Should().Be(range);

            // overlap above
            range.Intersection(Range.Closed(6, 10)).Should().Be(Range.Closed(6, 8));

            // adjacent above
            range.Intersection(Range.OpenClosed(8, 10)).Should().Be(Range.OpenClosed(8, 8));

            // separate above
            Action act2 = () => range.Intersection(Range.Closed(10, 12));
            act2.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Span_General()
        {
            Range<int> range = Range.Closed(4, 8);

            // separate below
            range.Span(Range.Closed(0, 2)).Should().Be(Range.Closed(0, 8));
            range.Span(Range.AtMost(2)).Should().Be(Range.AtMost(8));

            // adjacent below
            range.Span(Range.ClosedOpen(2, 4)).Should().Be(Range.Closed(2, 8));
            range.Span(Range.LessThan(4)).Should().Be(Range.AtMost(8));

            // overlap below
            range.Span(Range.Closed(2, 6)).Should().Be(Range.Closed(2, 8));
            range.Span(Range.AtMost(6)).Should().Be(Range.AtMost(8));

            // enclosed with same start
            range.Span(Range.Closed(4, 6)).Should().Be(range);

            // enclosed, interior
            range.Span(Range.Closed(5, 7)).Should().Be(range);

            // enclosed with same end
            range.Span(Range.Closed(6, 8)).Should().Be(range);

            // equal
            range.Span(range).Should().Be(range);

            // enclosing with same start
            range.Span(Range.Closed(4, 10)).Should().Be(Range.Closed(4, 10));
            range.Span(Range.AtLeast(4)).Should().Be(Range.AtLeast(4));

            // enclosing with same end
            range.Span(Range.Closed(2, 8)).Should().Be(Range.Closed(2, 8));
            range.Span(Range.AtMost(8)).Should().Be(Range.AtMost(8));

            // enclosing, exterior
            range.Span(Range.Closed(2, 10)).Should().Be(Range.Closed(2, 10));
            range.Span(Range.All<int>()).Should().Be(Range.All<int>());

            // overlap above
            range.Span(Range.Closed(6, 10)).Should().Be(Range.Closed(4, 10));
            range.Span(Range.AtLeast(6)).Should().Be(Range.AtLeast(4));

            // adjacent above
            range.Span(Range.OpenClosed(8, 10)).Should().Be(Range.Closed(4, 10));
            range.Span(Range.GreaterThan(8)).Should().Be(Range.AtLeast(4));

            // separate above
            range.Span(Range.Closed(10, 12)).Should().Be(Range.Closed(4, 12));
            range.Span(Range.AtLeast(10)).Should().Be(Range.AtLeast(4));
        }

        [Test]
        public void Equals()
        {
            Range<int> open1 = Range.Open(1, 5);
            Range<int> open2 = Range.Bounded(1, BoundType.OPEN, 5, BoundType.OPEN);

            open1.Should().Be(open1);
            open1.Should().Be(open2);
            open2.Should().Be(open1);

            Range<int> greater1 = Range.GreaterThan(2);
            Range<int> greater2 = Range.GreaterThan(2);

            greater1.Should().Be(greater1);
            greater1.Should().Be(greater2);

            Range<int> all1 = Range.All<int>();
            Range<int> all2 = Range.All<int>();

            all1.Should().Be(all1);
            all1.Should().Be(all2);
        }

        [Test]
        public void EquivalentFactories()
        {
            Range.AtLeast(1).Should().Be(Range.DownTo(1, BoundType.CLOSED));
            Range.GreaterThan(1).Should().Be(Range.DownTo(1, BoundType.OPEN));
            Range.AtMost(7).Should().Be(Range.UpTo(7, BoundType.CLOSED));
            Range.LessThan(7).Should().Be(Range.UpTo(7, BoundType.OPEN));
            Range.Open(1, 7).Should().Be(Range.Bounded(1, BoundType.OPEN, 7, BoundType.OPEN));
            Range.OpenClosed(1, 7).Should().Be(Range.Bounded(1, BoundType.OPEN, 7, BoundType.CLOSED));
            Range.Closed(1, 7).Should().Be(Range.Bounded(1, BoundType.CLOSED, 7, BoundType.CLOSED));
            Range.ClosedOpen(1, 7).Should().Be(Range.Bounded(1, BoundType.CLOSED, 7, BoundType.OPEN));
        }
    }

}