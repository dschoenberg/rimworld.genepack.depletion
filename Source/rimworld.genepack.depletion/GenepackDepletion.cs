using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;

namespace rimworld.genepack.depletion
{
    [StaticConstructorOnStartup]
    public static class GenepackDepletion
    {
        static GenepackDepletion()
        {
            Log.Message("GenepackDepletion loaded");
            var harmonyInstance = new Harmony("rimworld.genepack.depletion");
            harmonyInstance.Patch(AccessTools.Method(typeof(Building_GeneAssembler), "Finish"),
                   new HarmonyMethod(typeof(GenepackDepletion), "Finish_Postfix"));

            //harmonyInstance.Patch(AccessTools.Method(typeof(JobDriver_CreateXenogerm), "MakeNewToils"),
            //       new HarmonyMethod(typeof(GenepackDepletion), "MakeNewToils_Postfix"));
        }

        static void Finish_Postfix(Building_GeneAssembler __instance)
        {
            Log.Message("GeneAssembler Finished");
            List<Genepack> genepacks = __instance.GetGenepacks(true, true);
            int totalComplexity = genepacks.Sum(x => x.GeneSet.ComplexityTotal);
            Log.Message("Total complexity: " + totalComplexity);

            Pawn pawn = __instance.GetAssignedPawn();
            if (pawn != null)
            {
                Log.Message("Assigned Pawn" + pawn.Name.ToStringFull);
            }
            else
            {
                Log.Message("No Pawn");
            }

            genepacks.ForEach((x) =>
            {
                x.HitPoints -= 10;
                Log.Message(x.Label + ", " + x.HitPoints + " of " + x.MaxHitPoints + "; " + x.GeneSet.ComplexityTotal);
            });

        }

        static void MakeNewToils_Postfix(JobDriver_CreateXenogerm __instance, ref IEnumerable<Toil> __result)
        {
            Log.Message("MakeNewToils_Postfix");
            Log.Message("Assigned Pawn: " + __instance.pawn.Name.ToStringFull);
            Log.Message("Results: " +__result.GetType().FullName);
        }
    }
}
