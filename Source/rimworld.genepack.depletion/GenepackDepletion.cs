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
        private static Harmony harmonyInstance = null;
        private static GenepackDepletionSettings settings;

        public GenepackDepletion(ModContentPack content) : base(content)
        {

            settings = GetSettings<GenepackDepletionSettings>();
            if (harmonyInstance == null)
            {
                var harmonyInstance = new Harmony("rimworld.genepack.depletion");
                harmonyInstance.Patch(AccessTools.Method(typeof(Building_GeneAssembler), "Finish"),
                       new HarmonyMethod(typeof(GenepackDepletion), "Finish_Postfix"));
            }
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
            listingStandard.CheckboxLabeled("applyEvenly", ref settings.applyEvenly, "rimworld.genepack.depletion.applyEvenly");
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "rimworld.genepack.depletion.title".Translate();
        }

        private static void Finish_Postfix(Building_GeneAssembler __instance)
        {
            List<Genepack> genepacks = __instance.GetGenepacks(true, true);
            if (genepacks.NullOrEmpty()) return;

            int totalComplexity = genepacks.Sum(x => x.GeneSet.ComplexityTotal);
            int totalDepletion = UnityEngine.Random.Range(settings.minDepletionPerComplexity, settings.maxDepletionPerComplexity) * totalComplexity;
            if (__instance.MaxHitPoints > 0)
            {
                totalDepletion = totalDepletion * (1 + (__instance.MaxHitPoints - __instance.HitPoints) / __instance.MaxHitPoints);
            }

            if (totalDepletion == 0) return;

            if (settings.applyEvenly)
            {

                int depeltionAmount = totalDepletion / totalComplexity;
                genepacks.ForEach((x) =>
                {
                    x.HitPoints -= depeltionAmount;
                });
            }
            else
            {
                List<Genepack> randomList = new List<Genepack>();

                genepacks.ForEach(genepack =>
                {
                    for (int i = 0; i < genepack.GeneSet.ComplexityTotal + 1; i++)
                    {
                        randomList.Add(genepack);
                    }
                });

                for (; totalDepletion > 0; totalDepletion--)
                {
                    int index = UnityEngine.Random.Range(0, randomList.Count);
                    randomList[index].HitPoints--;
                }
            }

            genepacks.ForEach((x) =>
            {
                if (!x.Destroyed && x.HitPoints <= 0)
                {
                    Messages.Message("rimworld.genepack.depletion.depleted".Translate(x.Label), MessageTypeDefOf.NeutralEvent);
                    x.Destroy();
                }
            });
        }
    }
}
