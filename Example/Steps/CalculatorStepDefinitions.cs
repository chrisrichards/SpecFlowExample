using FluentAssertions;
using NLog;

namespace Example.Steps
{
    [Binding]
    public sealed class CalculatorStepDefinitions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ScenarioContext _scenarioContext;

        public CalculatorStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given("the first number is (.*)")]
        public void GivenTheFirstNumberIs(int number)
        {
            _scenarioContext["firstNumber"] = number;
            Logger.Info($"{number} saved into context");
        }

        [Given("the second number is (.*)")]
        public void GivenTheSecondNumberIs(int number)
        {
            _scenarioContext["secondNumber"] = number;
            Logger.Info($"{number} saved into context");
        }

        [When("the two numbers are added")]
        public void WhenTheTwoNumbersAreAdded()
        {
            int firstNumber = _scenarioContext.Get<int>("firstNumber");
            int secondNumber = (int) _scenarioContext["secondNumber"];
            var result = firstNumber + secondNumber;
            Logger.Info($"{firstNumber} + {secondNumber} = {result}");
            _scenarioContext.Add("result", result);
        }

        [Then("the result should be (.*)")]
        public void ThenTheResultShouldBe(int result)
        {
            var actualResult = (int) _scenarioContext["result"];
            actualResult.Should().Be(result);
            Logger.Info($"{actualResult} equals {result}");
        }
    }
}