using System.Collections.Generic;
using System.Reflection.Emit;
using API.Hooks;
using Carbon.Components;
using HarmonyLib;
using JetBrains.Annotations;

namespace Carbon.Hooks;

public partial class Category_Static
{
	public partial class Static_BaseEntity
	{
		[HookAttribute.Patch("ICustomEntityDataSave", "ICustomEntityDataSave", typeof(BaseEntity), nameof(BaseEntity.Save), new[] { typeof(BaseNetworkable.SaveInfo) })]
		[HookAttribute.Identifier("558da1511c6d424aa520c146dd560fa7")]
		[HookAttribute.Options(HookFlags.Hidden | HookFlags.Static)]
		[UsedImplicitly(ImplicitUseTargetFlags.Members)]

		public class Static_BaseEntity_Save_558da1511c6d424aa520c146dd560fa7 : API.Hooks.Patch
		{
			private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				List<CodeInstruction> il = new List<CodeInstruction>(instructions);

				CodeInstruction retcode = il[^1];

				CodeInstruction ldcode = new CodeInstruction(OpCodes.Ldarg_1);

				retcode.MoveLabelsTo(ldcode);

				il.InsertRange(il.Count-1, new List<CodeInstruction>()
				{
					ldcode,
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(typeof(BaseEntity), "additional_data_cache")),
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(typeof(BaseEntity), "additional_data_raw")),
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomDataInternals), nameof(CustomDataInternals.SerializeAdditionalEntityData)))
				});

				return il;
			}
		}

		[HookAttribute.Patch("ICustomEntityDataLoad", "ICustomEntityDataLoad", typeof(BaseEntity), nameof(BaseEntity.Load), new[] { typeof(BaseNetworkable.LoadInfo) })]
		[HookAttribute.Identifier("8d82cdfbeea44f369dc51d7076e78fa6")]
		[HookAttribute.Options(HookFlags.Hidden | HookFlags.Static)]
		[UsedImplicitly(ImplicitUseTargetFlags.Members)]

		public class Static_BaseEntity_Load_8d82cdfbeea44f369dc51d7076e78fa6 : API.Hooks.Patch
		{
			private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				List<CodeInstruction> il = new List<CodeInstruction>(instructions);

				il.InsertRange(3, new List<CodeInstruction>()
				{
					new CodeInstruction(OpCodes.Ldarg_1),
					new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(BaseNetworkable.LoadInfo), nameof(BaseNetworkable.LoadInfo.msg))),
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(typeof(BaseEntity), "additional_data_cache")),
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(typeof(BaseEntity), "additional_data_raw")),
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomDataInternals), nameof(CustomDataInternals.DeserializeAdditionalEntityData)))
				});

				return il;
			}
		}
	}
}
