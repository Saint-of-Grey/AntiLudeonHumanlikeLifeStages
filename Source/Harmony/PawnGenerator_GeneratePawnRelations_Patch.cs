using System;
using Harmony;
using Verse;

namespace HumanlikeLifeStages
{
    [HarmonyPatch(typeof(PawnGenerator), "GenerateSkills")]
    public static class PawnGenerator_GeneratePawnRelations_Patch
    {
        private const float SplitsForPuberty = 100f;

        [HarmonyPostfix]
        public static void Postfix(Pawn pawn)
        {
            if (!PawnHelper.is_human(pawn))
                return;

            if (BodyPartDefOf.Maturity == null)
                throw new Exception("BodyPartDefOf.Maturity missing!");

            BodyPartRecord maturityPart = pawn.RaceProps.body.AllParts.Find(b => (b.def == BodyPartDefOf.Maturity));
            if (maturityPart == null)
                return;

            ChestManager.intialChest(pawn);

            var yearsOld = pawn.ageTracker.AgeBiologicalYears;

            if (yearsOld < 3)
            {
                Log.Message("XXX - Child");
                var dif = pawn.health.AddHediff(HediffDefOf.LifeStages_Youth, maturityPart, null);
                dif.Severity = 1f;
                
                Log.Message("XXX - Child ["+pawn+"] added ["+dif+"]");
            }
            else if (yearsOld < 5)
            {
                Log.Message("XXX - Puberty");
                pawn.health.AddHediff(HediffDefOf.LifeStages_Puberty, maturityPart, null);
            }
            else
            {
                Log.Message("XXX - Roll dice");
                for (var i = SplitsForPuberty; i > 0 ; i--)
                {
                    PubertyHelper.applyPubertyDay(pawn, 1f * i / SplitsForPuberty);
                }
            }
        }
    }
}