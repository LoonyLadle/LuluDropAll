using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.DropAll
{
	[StaticConstructorOnStartup]
	static class HarmonyPatches
	{
		static HarmonyPatches()
		{
			Harmony harmony = new Harmony("rimworld.loonyladle.dropall");
			harmony.PatchAll();
		}
	}

	[HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
	public static class HarmonyPatch_Pawn_GetGizmos
	{
		public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> gizmos, Pawn __instance)
		{
			foreach (Gizmo gizmo in gizmos)
			{
				yield return gizmo;
			}

			if (__instance.Spawned && __instance.MentalStateDef == null && __instance.HostFaction == null && (__instance.Faction?.IsPlayer ?? false) && (__instance.inventory?.innerContainer?.Any ?? false))
			{
				yield return new Command_Action()
				{
					defaultLabel = "LuluDropAll_CommandDropAllLabel".Translate(),
					defaultDesc = "LuluDropAll_CommandDropAllDesc".Translate(),
					icon = ContentFinder<Texture2D>.Get("UI/Buttons/Drop"),
					action = delegate()
					{
						__instance.inventory.DropAllNearPawn(__instance.PositionHeld);
					}
				};
			}

			yield break;
		}
	}
}
