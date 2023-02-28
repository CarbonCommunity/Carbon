using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Carbon.Base.BaseProcessor;
using static Carbon.Contracts.IScriptProcessor;

namespace Carbon.Contracts
{
	public interface IBaseProcessor
	{
		void Start();

		T Get<T>(string fileName) where T : IInstance;

		Dictionary<string, IInstance> InstanceBuffer { get; set; }
		List<string> IgnoreList { get; set; }

		void Prepare(string path);
		void Prepare(string name, string path);
		void Ignore(string file);
		void Clear();
		void ClearIgnore(string file);

		public interface IInstance : IDisposable
		{
			bool IsRemoved { get; }
			bool IsDirty { get; }

			string File { get; set; }

			void Execute();
			void SetDirty();
			void MarkDeleted();
		}
		public interface IParser
		{
			void Process(string input, out string output);
		}
	}
}
