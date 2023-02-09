using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
