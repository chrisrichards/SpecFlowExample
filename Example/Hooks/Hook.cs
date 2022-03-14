using NLog;

namespace Example.Hooks
{
    [Binding]
    public class Hooks
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [BeforeScenario]
        public void BeforeScenarioHook(ScenarioContext scenarioContext)
        {
            SetupLogger(scenarioContext);
            Logger.Info("---Scenario Started---");
            Logger.Info(scenarioContext.ScenarioInfo.Title);
        }

        [BeforeStep]
        public void LogStepName(ScenarioContext scenarioContext)
        {
            var keyWord = scenarioContext.CurrentScenarioBlock.ToString();
            Logger.Info($"{keyWord} {scenarioContext.StepContext.StepInfo.Text}");
        }

        [AfterStep]
        public void LogStepResult(ScenarioContext scenarioContext)
        {
            var step = scenarioContext.StepContext.StepInfo.Text;
            var status = scenarioContext.ScenarioExecutionStatus.ToString();
            Logger.Info($"{step} is {status}");
        }


        [AfterScenario]
        public void AfterScenarioHook(ScenarioContext scenarioContext)
        {
            if (scenarioContext.TestError != null)
            {
                Logger.Error(scenarioContext.TestError);
            }

            Logger.Info("---Scenario Ended---");
        }

        private void SetupLogger(ScenarioContext scenarioContext)
        {
            //logger initialization
        }
    }
}