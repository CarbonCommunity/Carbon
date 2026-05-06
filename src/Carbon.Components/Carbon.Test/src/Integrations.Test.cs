using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using API.Logger;

namespace Carbon.Test;

public static partial class Integrations
{
	[AttributeUsage(AttributeTargets.Method)]
	public class Test : Attribute
	{
		/// <summary>
		/// The testing bed channel the test should run under (defaults to 1). Best used to execute tests selectively.
		/// </summary>
		public int Channel = DEFAULT_CHANNEL;

		/// <summary>
		/// The time ceiling constraint of what the test is expected to take. 0 to disable timeout.
		/// </summary>
		public float Timeout = 1000f;

		/// <summary>
		/// Cancel the test altogether at any failure
		/// </summary>
		public bool CancelOnFail = true;

		public enum StatusTypes
		{
			None,

			/// <summary>
			/// It's currently executing
			/// </summary>
			Running,

			/// <summary>
			/// Has been successfully completed
			/// </summary>
			Complete,

			/// <summary>
			/// Has been cancelled manually or programmatically
			/// </summary>
			Canceled,

			/// <summary>
			/// Has successfully failed
			/// </summary>
			Failed,

			/// <summary>
			/// Major error(s) occurred, which caused the test to end
			/// </summary>
			Fatal,

			/// <summary>
			/// The test took longer than expected
			/// </summary>
			Timeout
		}

		#region Internal

		private List<Exception> _exceptions = new();
		private Type _type;
		private MethodInfo _method;
		private object _target;
		private StatusTypes _statusType;
		private static int _prefixScale;
		private double _duration;
		private bool _isAsync;

		private static object[] _args = new object[1];

		#endregion

		#region Setup

		public void Setup(object target, Type type, MethodInfo info)
		{
			_type = type;
			_method = info;
			_target = target;
			_isAsync = _method.ReturnType?.GetMethod("GetAwaiter") != null ||
			           _method.GetCustomAttribute<AsyncStateMachineAttribute>() != null;
		}

		public void SetDuration(TimeSpan span)
		{
			_duration = span.TotalMilliseconds;
		}

		public void SetStatus(StatusTypes status)
		{
			_statusType = status;
		}

		#endregion

		#region Runtime

		public bool IsRunning => Status == StatusTypes.Running;

		public StatusTypes Status => _statusType;

		public IEnumerable<Exception> Exceptions => _exceptions.AsEnumerable();

		public void Run()
		{
			SetStatus(StatusTypes.Running);

			_args[0] = this;

			try
			{
				_method.Invoke(_target, _method.GetParameters().Length == 1 ? _args : Array.Empty<object>());

				if (!_isAsync)
				{
					Complete();
				}
			}
			catch (Exception exception)
			{
				_exceptions.Add(exception);
				Fatal("Runtime method failure", exception);
			}
		}

		public void RunCheck()
		{
			if (Timeout <= 0)
			{
				return;
			}

			if (_duration >= Timeout)
			{
				TimeOut();
			}
		}

		public void Reset()
		{
			SetStatus(StatusTypes.None);

			_exceptions.Clear();
			SetDuration(default);
		}

		public bool HasFailedFatally() => Status == StatusTypes.Fatal || (CancelOnFail && Status != StatusTypes.Complete);

		#endregion

		#region Finalizers

		public void Complete()
		{
			if (!IsRunning)
			{
				return;
			}

			SetStatus(StatusTypes.Complete);
			Log($"Complete - {_exceptions.Count:n0} excp.");
		}

		public void TimeOut()
		{
			if (!IsRunning)
			{
				return;
			}

			SetStatus(StatusTypes.Timeout);
			Warn($"Timeout >= {Timeout:0}ms");
		}

		public void Fail(string message, Exception exception = null)
		{
			if (!IsRunning)
			{
				return;
			}

			SetStatus(StatusTypes.Failed);
			Error($"Fail - {message}", exception);
		}

		public void Fatal(string message, Exception exception = null)
		{
			if (!IsRunning)
			{
				return;
			}

			SetStatus(StatusTypes.Fatal);
			Error($"Fatal - {message}", exception);
		}

		#endregion

		#region Logging

		public void Log(object message)
		{
			CalculatePrettyString(out var main, out var second);
			Logger.Console($"{second}{main}  {(message == null ? "no message" : message.ToString())}");
		}

		public void Warn(object message)
		{
			CalculatePrettyString(out var main, out var second);
			Logger.Console($"{second}{main}  {(message == null ? "no message" : message.ToString())}", Severity.Warning);
		}

		public void Error(object message, Exception exception)
		{
			CalculatePrettyString(out var main, out var second);
			Logger.Console($"{second}{main}  {(message == null ? "no message" : message.ToString())}", Severity.Error, exception);

			SetStatus(StatusTypes.Failed);
		}

		public void Fatal(object message, Exception exception)
		{
			CalculatePrettyString(out var main, out var second);
			Logger.Console($"{second}{main}  {(message == null ? "no message" : message.ToString())}", Severity.Error, exception);

			SetStatus(StatusTypes.Fatal);
		}

		public virtual string ToPrettyString() => $"{_type.Name}.{_method.Name}|{_duration:0}ms|".ToLower();

		public void CalculatePrettyString(out string mainString, out string spacing)
		{
			mainString = ToPrettyString();

			var currentStringLength = mainString.Length;

			if (currentStringLength > _prefixScale)
			{
				_prefixScale = currentStringLength;
			}

			spacing = new string(' ', _prefixScale - currentStringLength);
		}

		#endregion

		[AttributeUsage(AttributeTargets.Method)]
		public class Assert : Test
		{
			public override string ToPrettyString() => base.ToPrettyString() + "assert|";

			public bool IsTrue(bool condition, string info = null)
			{
				if (condition)
				{
					Warn($"IsTrue passed    - {(string.IsNullOrEmpty(info) ? "[bool condition]" : info)}");
;					return true;
				}

				Fail($"IsTrue failed    - {(string.IsNullOrEmpty(info) ? "[bool condition]" : info)}");
				return false;
			}

			public bool IsFalse(bool condition, string info = null)
			{
				if (!condition)
				{
					Warn($"IsFalse passed   - {(string.IsNullOrEmpty(info) ? "[bool condition]" : info)}");
					return true;
				}

				Fail($"IsFalse failed   - {(string.IsNullOrEmpty(info) ? "[bool condition]" : info)}");
				return false;
			}

			public bool IsNull(object value, string info = null)
			{
				if (value == null)
				{
					Warn($"IsNull passed    - {(string.IsNullOrEmpty(info) ? "[object value]" : info)} == null");
					return true;
				}

				Fail($"IsNull failed    - {(string.IsNullOrEmpty(info) ? "[object value]" : info)} == {value}");
				return false;
			}

			public bool IsNotNull(object value, string info = null)
			{
				if (value != null)
				{
					Warn($"IsNotNull passed - {(string.IsNullOrEmpty(info) ? "[object value]" : info)} == {value}");
					return true;
				}

				Fail($"IsNotNull failed - {(string.IsNullOrEmpty(info) ? "[object value]" : info)} = null");
				return false;
			}
		}
	}
}
