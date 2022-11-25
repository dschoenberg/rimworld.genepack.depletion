using Verse;

namespace rimworld.genepack.depletion
{
    class GenepackDepletionSettings : ModSettings
    {
        public int minDepletionPerComplexity = 20;
        public int maxDepletionPerComplexity = 33;
        public bool applyEvenly = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref minDepletionPerComplexity, "minDepletionPerComplexity", 20);
            Scribe_Values.Look(ref maxDepletionPerComplexity, "maxDepletionPerComplexity", 33);
            Scribe_Values.Look(ref applyEvenly, "applyEvenly", false);
        }
    }
}