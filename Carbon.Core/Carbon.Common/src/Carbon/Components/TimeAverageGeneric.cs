/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using Google.Protobuf.WellKnownTypes;

namespace Carbon.Components;

public abstract class TimeAverageGeneric<T> : Dictionary<double, T>
{
	public TimeAverageGeneric (double time)
	{
		Time = time;
	}

	public double Time { get; set; }
	public T TotalValue { get; protected set; }
	public T AverageValue { get; protected set; }
	public T MinValue { get; protected set; }
	public T MaxValue { get; protected set; }

	public virtual void Increment(T value)
	{
		Calibrate();

		var time = Mathf.RoundToInt(UnityEngine.Time.realtimeSinceStartup);

		if (!ContainsKey(time))
		{
			Add(time, value);
		}
	}
	public virtual void Calibrate()
	{
		if (Count == 0)
		{
			return;
		}

		var instance = this.ElementAt(0);
		var timePassed = (UnityEngine.Time.realtimeSinceStartup - instance.Key);

		if (timePassed >= Time)
		{
			Remove(instance.Key);
		}
	}

	public abstract T CalculateTotal();
	public abstract T CalculateAverage();
	public abstract T CalculateMin();
	public abstract T CalculateMax();
}

public class HookTimeAverage : TimeAverageGeneric<double>
{
	public HookTimeAverage(double time) : base(time) { }

	public override double CalculateTotal()
	{
		using (TimeMeasure.New($"CalculateTotal<{typeof(double)}"))
		{
			TotalValue = 0f;

			foreach (var value in this)
			{
				TotalValue += value.Value;
			}
		}

		return TotalValue;
	}
	public override double CalculateAverage()
	{
		AverageValue = 0f;

		var total = CalculateTotal();

		if (total == 0)
		{
			return 0f;
		}

		return total / Count;
	}
	public override double CalculateMin()
	{
		MinValue = this.Min(x => x.Value);

		return MinValue;
	}
	public override double CalculateMax()
	{
		MaxValue = this.Max(x => x.Value);

		return MaxValue;
	}
}

public class MemoryAverage : TimeAverageGeneric<long>
{
	public MemoryAverage(double time) : base(time) { }

	public override long CalculateTotal()
	{
		Calibrate();

		using (TimeMeasure.New($"CalculateTotal<{typeof(long)}"))
		{
			TotalValue = 0;

			foreach (var value in this)
			{
				TotalValue += value.Value;
			}
		}

		return TotalValue;
	}
	public override long CalculateAverage()
	{
		Calibrate();

		AverageValue = 0;

		var total = CalculateTotal();

		if (total == 0)
		{
			return 0;
		}

		return total / Count;
	}
	public override long CalculateMin()
	{
		Calibrate();

		MinValue = this.Min(x => x.Value);

		return MinValue;
	}
	public override long CalculateMax()
	{
		Calibrate();

		MaxValue = this.Max(x => x.Value);

		return MaxValue;
	}
}
