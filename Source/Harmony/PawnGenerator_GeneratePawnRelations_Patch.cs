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

            var maturityPart = PawnHelper.MaturityPart(pawn);
            if (maturityPart == null)
                return;

            ChestManager.intialChest(pawn);
            
            SettingHelper.latest.update();
            
            var yearsOld = pawn.ageTracker.AgeBiologicalYears;

            if (yearsOld < SettingHelper.latest.PubertyOnset)
            {
                var dif = pawn.health.AddHediff(HediffDefOf.LifeStages_Youth, maturityPart, null);
                dif.Severity = 1f/(1+yearsOld);
            }
            else if (yearsOld < SettingHelper.latest.PubertyOnset+1)
            {
                pawn.health.AddHediff(HediffDefOf.LifeStages_Puberty, maturityPart, null);
            }
            else
            {
                for (var i = SplitsForPuberty; i > 0 ; i--)
                {
                    PubertyHelper.applyPubertyDay(pawn, 1f * i / SplitsForPuberty);
                }
                
                pawn.health.AddHediff(HediffDefOf.LifeStages_Adult, maturityPart);
            }
        }
    }
}