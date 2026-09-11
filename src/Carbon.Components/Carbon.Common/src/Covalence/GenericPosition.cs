namespace Oxide.Core.Libraries.Covalence;

public class GenericPosition
{
	public float X;
	public float Y;
	public float Z;

	public static GenericPosition Blank = new();

	public GenericPosition()
	{
	}

	public GenericPosition(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	#region Compatibility

    public static bool operator ==(GenericPosition a, GenericPosition b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if ((object)a == null || (object)b == null)
        {
            return false;
        }

        return a.X.Equals(b.X) && a.Y.Equals(b.Y) && a.Z.Equals(b.Z);
    }

    public static bool operator !=(GenericPosition a, GenericPosition b)
    {
        return !(a == b);
    }

    public static GenericPosition operator +(GenericPosition a, GenericPosition b)
    {
        return new GenericPosition(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static GenericPosition operator -(GenericPosition a, GenericPosition b)
    {
        return new GenericPosition(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static GenericPosition operator *(float mult, GenericPosition a)
    {
        return new GenericPosition(a.X * mult, a.Y * mult, a.Z * mult);
    }

    public static GenericPosition operator *(GenericPosition a, float mult)
    {
        return new GenericPosition(a.X * mult, a.Y * mult, a.Z * mult);
    }

    public static GenericPosition operator /(GenericPosition a, float div)
    {
        return new GenericPosition(a.X / div, a.Y / div, a.Z / div);
    }

    public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() << 2 ^ Z.GetHashCode() >> 2;

    public override bool Equals(object obj)
    {
	    if (obj is not GenericPosition pos)
	    {
		    return false;
	    }

	    return X.Equals(pos.X) && Y.Equals(pos.Y) && Z.Equals(pos.Z);
    }

    public override string ToString()
    {
	    return $"({X}, {Y}, {Z})";
    }

	#endregion
}
