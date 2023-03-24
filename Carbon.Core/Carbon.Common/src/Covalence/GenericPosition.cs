/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core.Libraries.Covalence;

public struct GenericPosition
{
	public float X;
	public float Y;
	public float Z;

	public GenericPosition(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	#region Compatibility

	public static bool operator ==(GenericPosition source, GenericPosition target)
	{
		return source.X == target.X && source.Y == target.Y && source.Z == target.Z;
	}
	public static bool operator !=(GenericPosition source, GenericPosition target)
	{
		return !(source == target);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}
	public override int GetHashCode() { return base.GetHashCode(); }

	#endregion
}
