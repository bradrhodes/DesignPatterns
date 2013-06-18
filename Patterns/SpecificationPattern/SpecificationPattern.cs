using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;

namespace Patterns.SpecificationPattern
{
    public interface IOrderProcessor
    {
        void Process(Order order);
    }

    public class OrderProcessor2 : IOrderProcessor
    {
        public ITaxStrategy Strategy { get; set; }

        public virtual void Process(Order order)
        {
            order.Tax = Strategy.CalcTax(order);
        }
    }

    public class OrderProcessor : IOrderProcessor
    {
        private readonly ITaxStrategyFactory strategyFactory;

        public OrderProcessor(ITaxStrategyFactory strategyFactory)
        {
            this.strategyFactory = strategyFactory;
        }

        public void Process(Order order)
        {
            var strategy = strategyFactory.GetTaxStrategy(order);
            order.Tax = strategy.CalcTax(order);
        }
    }

    /// <summary>
    /// The factory returns the strategy that should be used
    /// </summary>
    public interface ITaxStrategyFactory
    {
        ITaxStrategy GetTaxStrategy(Order o);
    }
    
    public class TaxStrategyFactory : ITaxStrategyFactory
    {
        private ITaxStrategy[] _taxStrategies;

        public TaxStrategyFactory(params ITaxStrategy[] taxStrategies)
        {
            _taxStrategies = taxStrategies;
        }

        public ITaxStrategy GetTaxStrategy(Order o)
        {
            try
            {
                return _taxStrategies.First(taxStrategy => taxStrategy.IsApplicable(o));
            }
            catch (Exception ex)
            {
                throw new Exception("No suitable tax strategy was found.", ex);
            }
        }
    }

    /// <summary>
    /// A particular strategy to apply.  In this example, the strategies implement the Tester-Doer 
    /// pattern where they determine if they are applicable or not.  This determination could be
    /// done in the factory itself using a variety of methods.
    /// </summary>
    public interface ITaxStrategy
    {
        bool IsApplicable(Order order);
        decimal CalcTax(Order order);
    }

    public class USTaxStrategy : ITaxStrategy
    {
        public bool IsApplicable(Order order)
        {
            return order.Country == "US";
        }

        public decimal CalcTax(Order order)
        {
            return 10;
        }
    }

    public class CanadaTaxStrategy : ITaxStrategy
    {
        public bool IsApplicable(Order order)
        {
            return order.Country == "CA";
        }

        public decimal CalcTax(Order order)
        {
            return 5;
        }
    }

    public class Order
    {
        public string Country { get; set; }
        public decimal Tax { get; set; }
    }
}