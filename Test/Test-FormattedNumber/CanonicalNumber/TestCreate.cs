namespace TestCanonicalNumber
{
    using FormattedNumber;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestCreate
    {
        [TestMethod]
        public void TestCreate0()
        {
            FormattedNumber NaN = FormattedNumber.Parse("NaN");
            Assert.IsTrue(NaN.Value.IsNaN);
        }
    }
}
