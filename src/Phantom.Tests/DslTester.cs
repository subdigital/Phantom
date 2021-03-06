#region License

// Copyright 2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
//  
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
//  
// http://www.apache.org/licenses/LICENSE-2.0 
//  
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://github.com/JeremySkinner/Phantom

#endregion

namespace Phantom.Tests {
	using System;
	using System.IO;
	using System.Linq;
	using Core;
	using NUnit.Framework;

	[TestFixture]
	public class DslTester {
		StringWriter writer;
		BuildRunner runner;

		[SetUp]
		public void Setup() {
			writer = new StringWriter();
			Console.SetOut(writer);
			runner = new BuildRunner();
		}

		[Test]
		public void Loads_simple_script() {
			string path = "Scripts\\SingleTarget.boo";
			var script = runner.GenerateBuildScript(path);
			script.Count().ShouldEqual(1);
		}

		[Test]
		public void Retrieves_target_name() {
			string path = "Scripts\\SingleTarget.boo";
			var script = runner.GenerateBuildScript(path);
			script.GetTarget("default").Name.ShouldEqual("default");
		}

		[Test]
		public void Loads_multiple_targets() {
			string path = "Scripts\\Dependencies.boo";
			var script = runner.GenerateBuildScript(path);
			script.Count().ShouldEqual(4);
		}

		[Test]
		public void Loads_dependencies() {
			string path = "Scripts\\Dependencies.boo";
			var script = runner.GenerateBuildScript(path);
			var defaultTarget = script.GetTarget(ScriptModel.DefaultTargetName);
			var executionSequence = defaultTarget.GetExecutionSequence().ToList();
			executionSequence.Count.ShouldEqual(3);
			executionSequence[0].Name.ShouldEqual("compile");
			executionSequence[1].Name.ShouldEqual("test");
			executionSequence[2].Name.ShouldEqual("default");
		}

		[Test]
		public void Executes_target() {
			runner.Execute(new PhantomOptions()  { File = "Scripts\\PrintsText.boo"});
			writer.AssertOutput("default:", "executing", "");
		}

		[Test]
		public void Executes_multiple_targets() {
			var options = new PhantomOptions() { File = "Scripts\\PrintsText.boo"};
			options.AddTarget("default");
			options.AddTarget("hello");
			runner.Execute(options);
			writer.AssertOutput("default:", "executing", "", "hello:", "hello", "");
		}

        [Test]
        public void Executes_target_from_within_target() {
            var options = new PhantomOptions() { File = "Scripts\\PrintsText.boo" };
            options.AddTarget("helloWorld");
            runner.Execute(options);
            writer.AssertOutput("helloWorld:", "hello:", "hello", "", "world");
        }

		[Test]
		public void Calls_multiple_targets() {
			var options = new PhantomOptions {File = "Scripts\\PrintsText.boo"};
			options.AddTarget("helloWorldWithMultipleCalls");
			runner.Execute(options);
			writer.AssertOutput("helloWorldWithMultipleCalls:", "hello:", "hello", "", "world:", "world");
		}

		[Test]
		public void Loads_single_dependency() {
			string path = "Scripts\\Dependencies.boo";
			var script = runner.GenerateBuildScript(path);
			var target = script.GetTarget("oneDependency");
			var executionSequence = target.GetExecutionSequence().ToList();
			executionSequence.Count.ShouldEqual(2);
			executionSequence[0].Name.ShouldEqual("compile");
			executionSequence[1].Name.ShouldEqual("oneDependency");
		}
	}
}