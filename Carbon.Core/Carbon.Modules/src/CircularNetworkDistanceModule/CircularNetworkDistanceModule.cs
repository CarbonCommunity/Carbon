using System;
using System.Collections.Generic;
using Carbon.Base;
using Network;
using Network.Visibility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Vice <https://codefling.com/vice>
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class CircularNetworkDistanceModule : CarbonModule<EmptyModuleConfig, EmptyModuleData>
{
	public override string Name => "CircularNetworkDistance";
	public override Type Type => typeof(CircularNetworkDistanceModule);

	public override void Init()
	{
		base.Init();
	}

#pragma warning disable IDE0051
	private void ae0577348a5140ea9aa861cd71c31e7c()
	{
		// FIXME:
		// We should be able to us Init() to subscribe to hooks like so
		// Subscribe("ae0577348a5140ea9aa861cd71c31e7c");
	}
#pragma warning restore IDE0051

	public override void OnEnabled(bool initialized)
	{
		if (ConVar.Net.visibilityRadiusFarOverride == -1)
			ConVar.Net.visibilityRadiusFarOverride = 6;

		if (ConVar.Net.visibilityRadiusNearOverride == -1)
			ConVar.Net.visibilityRadiusNearOverride = 4;

		Subscribe("ae0577348a5140ea9aa861cd71c31e7c");
	}

	public override void OnDisabled(bool initialized)
	{
		ConVar.Net.visibilityRadiusFarOverride = -1;
		ConVar.Net.visibilityRadiusNearOverride = -1;
	}

	// hardcoded values because this is probably fastest
	private static readonly List<int> EightCircle = new()
	{ 2, 4, 6, 6, 7, 7, 8, 8, 8, 8, 8, 7, 7, 6, 6, 4, 2 };

	private static readonly List<int> SevenCircle = new()
	{ 2, 4, 5, 6, 6, 7, 7, 7, 7, 7, 6, 6, 5, 4, 2 };

	private static readonly List<int> SixCircle = new()
	{ 2, 4, 5, 5, 6, 6, 6, 6, 6, 5, 5, 4, 2 };

	private static readonly List<int> FiveCircle = new()
	{ 2, 3, 4, 5, 5, 5, 5, 5, 4, 3, 2 };

	private static readonly List<int> FourCircle = new()
	{ 2, 3, 4, 4, 4, 4, 4, 3, 2 };

	private static readonly List<int> ThreeCircle = new()
	{ 1, 2, 3, 3, 3, 2, 1 };

	private static readonly List<int> TwoCircle = new()
	{ 1, 2, 2, 2, 1 };

	private static List<int> GetCircleSizeLookup(int radius)
	{
		return radius switch
		{
			8 => EightCircle,
			7 => SevenCircle,
			6 => SixCircle,
			5 => FiveCircle,
			4 => FourCircle,
			3 => ThreeCircle,
			2 => TwoCircle,
			_ => null,
		};
	}

	public static bool GetVisibleFromCircle(NetworkVisibilityGrid grid, Group group, List<Group> groups, int radius)
	{
		List<int> lookup = GetCircleSizeLookup(radius);
		if (lookup == null)
			return true; // this should be a NotImplementedException but y'all are retarded

		// Global netgroup
		groups.Add(Net.sv.visibility.Get(0U));

		if ((int)group.ID < grid.startID)
			return false;

		int layer = GetGroupLayer(grid, (int)group.ID);
		int num = (int)group.ID - grid.startID;

		int x = num / grid.cellCount;
		int y = num % grid.cellCount;

		for (int deltaY = -radius; deltaY <= radius; deltaY++)
		{
			int bounds = lookup[deltaY + radius];
			for (int deltaX = -bounds; deltaX <= bounds; deltaX++)
				AddLayers(grid, groups, x + deltaX, y + deltaY, GetGroupLayer(grid, layer));
		}

		return false;
	}

	private static void AddLayers(NetworkVisibilityGrid grid, List<Group> groups, int groupX, int groupY, int groupLayer)
	{
		Add(grid, groups, groupX, groupY, groupLayer);

		if (groupLayer == 0)
			Add(grid, groups, groupX, groupY, 1);

		if (groupLayer == 1)
		{
			Add(grid, groups, groupX, groupY, 2);
			Add(grid, groups, groupX, groupY, 0);
		}
	}

	private static void Add(NetworkVisibilityGrid grid, List<Group> groups, int groupX, int groupY, int groupLayer)
		=> groups.Add(Net.sv.visibility.Get(CoordToID(grid, groupX, groupY, groupLayer)));

	private static uint CoordToID(NetworkVisibilityGrid grid, int x, int y, int layer)
		=> (uint)(layer * (grid.cellCount * grid.cellCount) + (x * grid.cellCount + y) + grid.startID);

	private static int GetGroupLayer(NetworkVisibilityGrid grid, int groupId)
	{
		groupId -= grid.startID;
		return Math.DivRem(groupId, (grid.cellCount * grid.cellCount), out _);
	}
}
