using Verse;

namespace rimworld.genepack.depletion
{
    class GenepackDepletionSettings : ModSettings
    {
        public int minDepletionPerComplexity = 15;
        public int maxDepletionPerComplexity = 25;
        public bool applyEvenly = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref minDepletionPerComplexity, "minDepletionPerComplexity", 15);
            Scribe_Values.Look(ref maxDepletionPerComplexity, "maxDepletionPerComplexity", 25);
            Scribe_Values.Look(ref applyEvenly, "applyEvenly", false);
        }
    }
}