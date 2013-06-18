using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Patterns.SpecificationPattern
{
    public class PoorMansDiExample
    {
        [TestClass]
        public class TaxStrategyTests
        {
            [TestMethod]
            public void PoorMansDiTest()
            {
                IOrderProcessor orderProcessor = new OrderProcessor(
                    new TaxStrategyFactory(
                        new ITaxStrategy[] {new CanadaTaxStrategy(), new USTaxStrategy()}));

                var order = new Order { Country = "US" };
                
                orderProcessor.Process(order);

                Assert.AreEqual(10, order.Tax);
            }
        }
    }
}