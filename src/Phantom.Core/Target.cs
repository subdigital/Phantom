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

namespace Phantom.Core {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Phantom.Core;

	public class Target {
		readonly Action block;
		readonly ScriptModel parentScript;
		readonly string[] dependencyNames;


		public Target(string name, Action block, string[] dependencies, string description, ScriptModel parentScript) {
			Name = name;
			Description = description;
			this.block = block;
			this.parentScript = parentScript;
			dependencyNames = dependencies ?? new string[0];
		}

		public string Name { get; private set; }
		public string Description { get; private set; }

		public IEnumerable<Target> GetExecutionSequence() {
			var executionSequence = new List<Target>() { this };
			PopulateExecutionSequence(executionSequence);
			return executionSequence.AsEnumerable().Reverse();
		}

		void PopulateExecutionSequence(ICollection<Target> sequence) {
			foreach (var dependencyName in dependencyNames.Reverse()) {
				var dependency = parentScript.GetTarget(dependencyName);

				if (dependency == null) {
					throw new PhantomException(string.Format("Target '{0}' depenends upon a target named '{1}' but it does not exist.",
															 Name, dependencyName));
				}

				if (sequence.Contains(dependency)) {
					throw new PhantomException(string.Format("Detected recursive dependency for target '{0}'", dependency.Name));
				}

				sequence.Add(dependency);
				dependency.PopulateExecutionSequence(sequence);
			}
		}


		public void Execute() {
			if(block != null) {
				block();
			}
		}
	}
}