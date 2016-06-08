using NUnit.Framework;

namespace JMW.AlgebraicTypes.Tests
{
    [TestFixture]
    public class EitherTests
    {
        [Test]
        public void Test1()
        {
            Either<string, string> e = MonthName(2);

            // Match with no return value.
            e.Match(
                Left: null,
                Right: name => { Assert.That("Feb" == name); }
            );

            // Match with a return value.
            string monthName = e.Match(
                    Left: null,
                    Right: name => name
                );
            Assert.That(monthName == "Feb");

            e = MonthName(20);

            // Match with no return value.
            e.Match(
                Left: err => { Assert.That("" == err); },
                Right: null
            );

            // Match with a return value.
            monthName = e.Match(
                    Left: err => string.Empty,
                    Right: null
                );
            Assert.That(monthName == string.Empty);
        }

        private Either<string, string> MonthName(int i)
        {
            if (i < 1 || i > 12) return new Left<string, string>("");
            var months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            return new Right<string, string>(months[i - 1]);
        }
    }
}