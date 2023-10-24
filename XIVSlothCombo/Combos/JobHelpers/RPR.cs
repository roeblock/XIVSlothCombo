using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class RPROpenerLogic : PvE.RPR
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(SoulSlice) < 2)
                return false;
            if (!CustomComboFunctions.ActionReady(ArcaneCircle))
                return false;
            if (!CustomComboFunctions.ActionReady(Gluttony))
                return false;

            return true;
        }

        private static uint OpenerLevel => 90;
        public uint PrePullStep = 1;
        public uint OpenerStep = 1;

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && LevelChecked;

        private OpenerState currentState = OpenerState.PrePull;

        public OpenerState CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                if (value != currentState)
                {
                    if (value == OpenerState.PrePull)
                    {
                        Svc.Log.Debug($"Entered PrePull Opener");
                    }
                    if (value == OpenerState.InOpener) OpenerStep = 1;
                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            Svc.Log.Information("Opener Failed");

                        ResetOpener();
                    }
                    if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                    currentState = value;
                }
            }
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (CanOpener && PrePullStep == 0)
            {
                PrePullStep = 1;
            }

            if (!HasCooldowns())
            {
                PrePullStep = 0;
            }

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (CustomComboFunctions.HasEffect(Buffs.Soulsow) && PrePullStep == 1) PrePullStep++;
                else if (PrePullStep == 1) actionID = Soulsow;

                if (CustomComboFunctions.LocalPlayer.CastActionId == Harpe && PrePullStep == 2) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 2) actionID = Harpe;

                if (PrePullStep == 2 && !CustomComboFunctions.HasEffect(Buffs.Soulsow))
                    CurrentState = OpenerState.FailedOpener;

                if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }

            PrePullStep = 0;
            return false;
        }

        private bool DoOpener(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (currentState == OpenerState.InOpener)
            {
                if (Config.RPR_OpenerChoice == 0)
                {
                    if (CustomComboFunctions.WasLastAction(ShadowOfDeath) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = ShadowOfDeath;

                    if (CustomComboFunctions.WasLastAction(ArcaneCircle) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = ArcaneCircle;

                    if (CustomComboFunctions.WasLastAction(SoulSlice) && CustomComboFunctions.GetRemainingCharges(SoulSlice) is 1 && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = SoulSlice;

                    if (CustomComboFunctions.WasLastAction(SoulSlice) && CustomComboFunctions.GetRemainingCharges(SoulSlice) is 0 && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = SoulSlice;

                    if (CustomComboFunctions.WasLastAction(PlentifulHarvest) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = PlentifulHarvest;

                    if (CustomComboFunctions.WasLastAction(Enshroud) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = Enshroud;

                    if (CustomComboFunctions.WasLastAction(VoidReaping) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = VoidReaping;

                    if (CustomComboFunctions.WasLastAction(CrossReaping) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = CrossReaping;

                    if (CustomComboFunctions.WasLastAction(LemuresSlice) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = LemuresSlice;

                    if (CustomComboFunctions.WasLastAction(VoidReaping) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = VoidReaping;

                    if (CustomComboFunctions.WasLastAction(CrossReaping) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = CrossReaping;

                    if (CustomComboFunctions.WasLastAction(LemuresSlice) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = LemuresSlice;

                    if (CustomComboFunctions.WasLastAction(Communio) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = Communio;

                    if (CustomComboFunctions.WasLastAction(Gluttony) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = Gluttony;

                    if (CustomComboFunctions.WasLastAction(Gallows) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = Gallows;

                    if (CustomComboFunctions.WasLastAction(Gibbet) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = Gibbet;

                    if (CustomComboFunctions.WasLastAction(UnveiledGallows) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = UnveiledGallows;

                    if (CustomComboFunctions.WasLastAction(Gallows) && OpenerStep == 18) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 18) actionID = Gallows;
                }

                else
                {
                    if (CustomComboFunctions.WasLastAction(ShadowOfDeath) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = ShadowOfDeath;

                    if (CustomComboFunctions.WasLastAction(SoulSlice) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = SoulSlice;

                    if (CustomComboFunctions.WasLastAction(ArcaneCircle) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = ArcaneCircle;

                    if (CustomComboFunctions.WasLastAction(Gluttony) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = Gluttony;

                    if (CustomComboFunctions.WasLastAction(Gallows) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = Gallows;

                    if (CustomComboFunctions.WasLastAction(Gibbet) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = Gibbet;

                    if (CustomComboFunctions.WasLastAction(PlentifulHarvest) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = PlentifulHarvest;

                    if (CustomComboFunctions.WasLastAction(Enshroud) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = Enshroud;

                    if (CustomComboFunctions.WasLastAction(VoidReaping) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = VoidReaping;

                    if (CustomComboFunctions.WasLastAction(CrossReaping) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = CrossReaping;

                    if (CustomComboFunctions.WasLastAction(LemuresSlice) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = LemuresSlice;

                    if (CustomComboFunctions.WasLastAction(VoidReaping) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = VoidReaping;

                    if (CustomComboFunctions.WasLastAction(CrossReaping) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = CrossReaping;

                    if (CustomComboFunctions.WasLastAction(LemuresSlice) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = LemuresSlice;

                    if (CustomComboFunctions.WasLastAction(Communio) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = Communio;

                    if (CustomComboFunctions.WasLastAction(SoulSlice) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = SoulSlice;

                    if (CustomComboFunctions.WasLastAction(UnveiledGallows) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = UnveiledGallows;

                    if (CustomComboFunctions.WasLastAction(Gallows) && OpenerStep == 18) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 18) actionID = Gallows;
                }

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }

            return false;
        }

        private bool DoOpenerSimple(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.WasLastAction(ShadowOfDeath) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = ShadowOfDeath;

                if (CustomComboFunctions.WasLastAction(ArcaneCircle) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = ArcaneCircle;

                if (CustomComboFunctions.WasLastAction(SoulSlice) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = SoulSlice;

                if (CustomComboFunctions.WasLastAction(SoulSlice) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = SoulSlice;

                if (CustomComboFunctions.WasLastAction(PlentifulHarvest) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = PlentifulHarvest;

                if (CustomComboFunctions.WasLastAction(Enshroud) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = Enshroud;

                if (CustomComboFunctions.WasLastAction(VoidReaping) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = VoidReaping;

                if (CustomComboFunctions.WasLastAction(CrossReaping) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = CrossReaping;

                if (CustomComboFunctions.WasLastAction(LemuresSlice) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = LemuresSlice;

                if (CustomComboFunctions.WasLastAction(VoidReaping) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = VoidReaping;

                if (CustomComboFunctions.WasLastAction(CrossReaping) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = CrossReaping;

                if (CustomComboFunctions.WasLastAction(LemuresSlice) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = LemuresSlice;

                if (CustomComboFunctions.WasLastAction(Communio) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = Communio;

                if (CustomComboFunctions.WasLastAction(Gluttony) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = Gluttony;

                if (CustomComboFunctions.WasLastAction(Gibbet) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = Gibbet;

                if (CustomComboFunctions.WasLastAction(Gallows) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = Gallows;

                if (CustomComboFunctions.WasLastAction(UnveiledGibbet) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = UnveiledGibbet;

                if (CustomComboFunctions.WasLastAction(Gibbet) && OpenerStep == 18) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 18) actionID = Gibbet;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }

            return false;
        }

        private void ResetOpener()
        {
            PrePullStep = 0;
            OpenerStep = 0;
        }

        public bool DoFullOpener(ref uint actionID, bool simpleMode)
        {
            if (!LevelChecked) return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID)) return true;

            if (CurrentState == OpenerState.InOpener)
            {
                if (simpleMode)
                {
                    if (DoOpenerSimple(ref actionID)) return true;
                }
                else
                {
                    if (DoOpener(ref actionID)) return true;
                }
            }

            if (!CustomComboFunctions.InCombat())
            {
                ResetOpener();
                CurrentState = OpenerState.PrePull;
            }

            return false;
        }
    }
}