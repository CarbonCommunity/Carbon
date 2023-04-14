using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Carbon.Contracts;

public interface IScriptProcessor : IBaseProcessor, IDisposable
{
	void InvokeRepeating(Action action, float delay, float repeat);

	GameObject gameObject { get; }

	bool AllPendingScriptsComplete();
	bool AllNonRequiresScriptsComplete();
	bool AllExtensionsComplete();

	void StartCoroutine(IEnumerator coroutine);
	void StopCoroutine(IEnumerator coroutine);

	void Remove(string name);

	public interface IScript : IInstance
	{
		IScriptLoader Loader { get; set; }
	}
}
