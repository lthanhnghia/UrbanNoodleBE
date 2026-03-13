using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanNoodle.Services;

namespace UrbanNoodleTest
{
    public class CalculatorServiceTests
    { 
        private readonly CalculatorService _calculatorService;
        public CalculatorServiceTests() {
            _calculatorService = new CalculatorService();
        }
        [Fact]
        public void Add_ReturnsCorrectResult()
        {
          
            var result = _calculatorService.Add(2, 3);

            Assert.Equal(5, result);
        }
    }
}
