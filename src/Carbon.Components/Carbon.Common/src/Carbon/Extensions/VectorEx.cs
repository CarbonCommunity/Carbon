namespace Carbon.Extensions;

public static class VectorEx
{
	public static string ToParsableString(this Vector3 vector)
	{
		return $"{vector.x} {vector.y} {vector.z}";
	}
	public static string ToParsableString(this Vector2 vector)
	{
		return $"{vector.x} {vector.y}";
	}
}
