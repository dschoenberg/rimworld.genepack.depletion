using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;
using UnityEngine;
using System;

namespace rimworld.genepack.depletion
{
    //[StaticConstructorOnStartup]
    public class GenepackDepletion : Mod
    {
        private static GenepackDepletionSettings settings;

        public GenepackDepletion(ModContentPack content) : base(content)
        {

            settings = GetSettings<GenepackDepletionSettings>();
            Log.Message("GenepackDepletion loaded");
            var harmonyInstance = new Harmony("rimworld.genepack.depletion");
            harmonyInstance.Patch(AccessTools.Method(typeof(Building_GeneAssembler), "Finish"),
                   new HarmonyMethod(typeof(GenepackDepletion), "Finish_Postfix"));
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("rimworld.genepack.depletion.depletionPerComplexity".Translate());
            listingStandard.Label(settings.minDepletionPerComplexity + " - " + settings.maxDepletionPerComplexity);
            listingStandard.Label("rimworld.genepack.depletion.minValue".Translate());
            settings.minDepletionPerComplexity = (int)Math.Round(listingStandard.Slider(settings.minDepletionPerComplexity, 0f, 100f));
            listingStandard.Label("rimworld.genepack.depletion.maxValue".Translate());
            settings.maxDepletionPerComplexity = (int)Math.Round(listingStandard.Slider(settings.maxDepletionPerComplexity, 0f, 100f));
            listingStandard.CheckboxLabeled("applyEvenly", ref settings.applyEvenly, "applyEvenly");
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "rimworld.genepack.depletion.title".Translate();
        }

        static void Finish_Postfix(Building_GeneAssembler __instance)
        {
            Log.Message("GeneAssembler Finished");
            List<Genepack> genepacks = __instance.GetGenepacks(true, true);

            if (genepacks.NullOrEmpty()) return;

            int totalComplexity = genepacks.Sum(x => x.GeneSet.ComplexityTotal);
            Log.Message("Total complexity: " + totalComplexity);

            Log.Message("applyEvenly: " + settings.applyEvenly);

            Log.Message("minDepletionPerComplexity: " + settings.minDepletionPerComplexity);
            Log.Message("maxDepletionPerComplexity: " + settings.maxDepletionPerComplexity);
            int totalDepletion = randomDepletion();
            if(__instance.MaxHitPoints > 0)
            {
                totalDepletion = totalDepletion * (__instance.MaxHitPoints - __instance.HitPoints) / __instance.MaxHitPoints;
            }

            Log.Message("totalDepletion: " + totalDepletion);

            if (settings.applyEvenly)
            {

                int depeltionAmount = totalDepletion / totalComplexity;
                genepacks.ForEach((x) =>
                {
                    x.HitPoints -= depeltionAmount;
                    Log.Message(x.Label + ", " + x.HitPoints + " of " + x.MaxHitPoints + "; " + x.GeneSet.ComplexityTotal);
                });
            }
            else
            {
                Log.Message("randomizingDepeltion");
                List<Genepack> randomList = new List<Genepack>();

                genepacks.ForEach(genepack =>
                {
                    for (int i = 0; i < genepack.GeneSet.ComplexityTotal + 1; i++)
                    {
                        randomList.Add(genepack);
                    }
                });

                Log.Message("listLength: " + randomList.Count);
                for (; totalDepletion >= 0; totalDepletion--)
                {
                    int index = UnityEngine.Random.Range(0, randomList.Count);
                    randomList[index].HitPoints--;
                    Log.Message("Reducing: " + randomList[index].Label + " to " + randomList[index].HitPoints + " - " + totalDepletion);
                }

                genepacks.ForEach((x) =>
                {
                    Log.Message(x.Label + ", " + x.HitPoints + " of " + x.MaxHitPoints + "; " + x.GeneSet.ComplexityTotal);
                });
            }
        }

        private static int randomDepletion()
        {
            return UnityEngine.Random.Range(settings.minDepletionPerComplexity, settings.maxDepletionPerComplexity);
        }
    }
}
