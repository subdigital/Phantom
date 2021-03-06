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
	using System.Linq;
	using Core;
	using NUnit.Framework;

	[TestFixture]
	public class PhantomOptionsTester {
		PhantomOptions args;

		[SetUp]
		public void Setup() {
			args = new PhantomOptions();
		}

		[Test]
		public void Sensible_Defaults() {
		
			args.Parse(new string[0]);
			args.File.ShouldEqual("build.boo");
			args.Help.ShouldEqual(false);
			args.TargetNames.Single().ShouldEqual("default");
		}

		[Test]
		public void Parses_help() {
			args.Parse(new[] { "-h" });
			args.Help.ShouldBeTrue();
		}

		[Test]
		public void Parses_file() {
			args.Parse(new[] { "-f:test.boo" });
			args.File.ShouldEqual("test.boo");
		}

		[Test]
		public void Custom_target_names() {
			args.Parse(new[] { "foo", "bar" });
			args.TargetNames.Count().ShouldEqual(2);
			args.TargetNames.First().ShouldEqual("foo");
			args.TargetNames.Last().ShouldEqual("bar");
		}

		[Test]
		public void Parses_targets() {
			args.Parse(new[] { "-t" });
			args.ShowTargets.ShouldBeTrue();
		}

		[Test]
		public void Adds_arguments_to_environment() {
			args.Parse(new[] { "-a:foo=bar", "-a:boo=baz" });
			Environment.GetEnvironmentVariable("foo").ShouldEqual("bar");
			Environment.GetEnvironmentVariable("boo").ShouldEqual("baz");
		}
	}
}