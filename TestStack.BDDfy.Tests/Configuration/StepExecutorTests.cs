﻿using System;
using System.Text;
using NUnit.Framework;
using TestStack.BDDfy.Configuration;

namespace TestStack.BDDfy.Tests.Configuration
{
    [TestFixture]
    public class StepExecutorTests
    {
        private class TestStepExecutor : StepExecutor
        {
            readonly StringBuilder _builder = new StringBuilder();

            public string Results
            {
                get { return _builder.ToString(); }
            }

            public override object Execute(Step step, object testObject)
            {
                try
                {
                    _builder.AppendLine(string.Format("About to run step '{0}'", step.Title));
                    return base.Execute(step, testObject);
                }
                finally
                {
                    _builder.AppendLine(string.Format("Finished running step '{0}'", step.Title));
                }
            }
        }

        [Test]
        public void CustomizingStepExecutionByOverridingStepExecutor()
        {
            try
            {
                var testStepExecutor = new TestStepExecutor();
                
                Configurator.StepExecutor = testStepExecutor;

                new EmptyScenario()
                    .Given(s => s.GivenSomething())
                    .When(s => s.WhenSomething())
                    .Then(s => s.ThenSomething())
                    .BDDfy();
                
                string expected =
@"About to run step 'Given something'
Finished running step 'Given something'
About to run step 'When something'
Finished running step 'When something'
About to run step 'Then something'
Finished running step 'Then something'
".Replace("\r", string.Empty).Trim();

                string actual = testStepExecutor.Results.Replace("\r", string.Empty).Trim();
                Assert.AreEqual(expected, actual);

            }
            finally
            {
                Configurator.StepExecutor = new StepExecutor();
            }
        }
    }
}