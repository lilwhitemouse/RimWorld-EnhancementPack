﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using Verse;

namespace TD_Enhancement_Pack
{
	[HarmonyPatch(typeof(Zone_Growing))]
	[HarmonyPatch("GetInspectString")]
	static class ZoneGrowingSizeCount
	{
		public static void Postfix(Zone_Growing __instance, ref string __result)
		{
			//TODO save __instance.cells.Sum(cell => __instance.Map.fertilityGrid.FertilityAt(cell))

			//TODO: call Zone.GetInspectString (base)
			int count = __instance.cells.Count;
			__result += "\n" + 
				String.Format("TD.SizeAndFertile".Translate(), count, count + FertilityCount(__instance))
				+ "\n" +
				String.Format("To do: {0}", ToDoCount(__instance))
				+ "   " + 
				String.Format("Grown: {0}", GrownCount(__instance));
		}

		public static float FertilityCount(Zone_Growing zone)
		{
			return zone.GetPlantDefToGrow().plant.fertilitySensitivity
				* zone.cells.Sum(cell => zone.Map.fertilityGrid.FertilityAt(cell) - 1.0f);
		}

		public static int ToDoCount(Zone_Growing zone)
		{
			Map map = zone.Map;
			return zone.cells.FindAll(c => !map.thingGrid.ThingsAt(c).Any(t => t.def == zone.GetPlantDefToGrow())).Count;
		}

		public static int GrownCount(Zone_Growing zone)
		{
			Map map = zone.Map;
			int ret = 0;
			ThingDef plantDef = zone.GetPlantDefToGrow();
			foreach (IntVec3 cell in zone.cells)
			{
				if (map.thingGrid.ThingsAt(cell).FirstOrDefault(t => t is Plant) is Plant p
					&& p.def == plantDef && p.Growth == 1)
					ret++;
			}
			return ret;
		}
	}

	[HarmonyPatch(typeof(Zone))]
	[HarmonyPatch("GetInspectString")]
	static class ZoneSizeCount
	{
		public static void Postfix(Zone __instance, ref string __result)
		{
			string add = String.Format("TD.Size".Translate(), __instance.cells.Count);
			if (__result == String.Empty)
				__result = add;
			else
				__result += "\n" + add;
		}
	}
}