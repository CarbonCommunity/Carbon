using System;
using Carbon.Base;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public class ImageDatabaseModule : CarbonModule<ImageDatabaseConfig, ImageDatabaseData>
{
	public override string Name => "Image Database";
	public override Type Type => typeof(ImageDatabaseModule);
	public override bool EnabledByDefault => true;
}

public class ImageDatabaseConfig
{
}
public class ImageDatabaseData
{

}
