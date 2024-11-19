/*
 *
 * Copyright (c) 2023 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

namespace Carbon.Components;

public partial class MonoProfiler
{
	/// <summary>
	/// Rust-side native protocol.
	/// </summary>
	public const int NATIVE_PROTOCOL = 4;

	/// <summary>
	/// Managed-side protocol. Primarily used by locally stored profiles.
	/// </summary>
	public const int MANAGED_PROTOCOL = NATIVE_PROTOCOL + 123;
}
