using BookNadPlay_API.Helpers;
using NUnit.Framework;
using System;

namespace API.tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void HoursOverlapping_RetursTrue()
        {
            var f1 = new TimeSpan(10, 0, 0);
            var t1 = new TimeSpan(11, 0, 0);
            var f2 = new TimeSpan(10, 45, 0);
            var t2 = new TimeSpan(12, 0, 0);
            Assert.IsTrue(DataHelper.IsTimeOverlapping(f1, t1, f2, t2));
        }

        [Test]
        public void HoursOverlapping_RetursTrue2()
        {
            var f1 = new TimeSpan(10, 0, 0);
            var t1 = new TimeSpan(11, 0, 0);
            var f2 = new TimeSpan(9, 45, 0);
            var t2 = new TimeSpan(10, 20, 0);
            Assert.IsTrue(DataHelper.IsTimeOverlapping(f1, t1, f2, t2));
        }

        [Test]
        public void HoursOverlapping_RetursTrue3()
        {
            var f1 = new TimeSpan(10, 0, 0);
            var t1 = new TimeSpan(12, 0, 0);
            var f2 = new TimeSpan(9, 45, 0);
            var t2 = new TimeSpan(13, 20, 0);
            Assert.IsTrue(DataHelper.IsTimeOverlapping(f1, t1, f2, t2));
        }

        [Test]
        public void HoursOverlapping_RetursFalse()
        {
            var f1 = new TimeSpan(10, 0, 0);
            var t1 = new TimeSpan(11, 0, 0);
            var f2 = new TimeSpan(11, 0, 0);
            var t2 = new TimeSpan(12, 0, 0);
            Assert.IsFalse(DataHelper.IsTimeOverlapping(f1, t1, f2, t2));
        }
    }
}