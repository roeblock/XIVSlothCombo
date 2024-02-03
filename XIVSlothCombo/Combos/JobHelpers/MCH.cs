using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System.Linq;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class MCHOpenerLogic : PvE.MCH
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(Ricochet) < 3)
                return false;
            if (CustomComboFunctions.GetRemainingCharges(GaussRound) < 3)
                return false;
            if (!CustomComboFunctions.ActionReady(Chainsaw))
                return false;
            if (!CustomComboFunctions.ActionReady(Wildfire))
                return false;
            if (!CustomComboFunctions.ActionReady(BarrelStabilizer))
                return false;

            return true;
        }

        public static bool HasPrePullCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(Reassemble) < 2 && Config.MCH_ST_RotationSelection == 2) return false;

            return true;
        }

        private static uint OpenerLevel => 90;

        public uint PrePullStep = 0;

        public uint OpenerStep = 0;

        private static uint[] DelayedToolsOpener = [
            GaussRound,
            Ricochet,
            Drill,
            BarrelStabilizer,
            HeatedSlugShot,
            Ricochet,
            HeatedCleanShot,
            Reassemble,
            GaussRound,
            AirAnchor,
            Reassemble,
            Wildfire,
            Chainsaw,
            AutomatonQueen,
            Hypercharge,
            Heatblast,
            Ricochet,
            Heatblast,
            GaussRound,
            Heatblast,
            Ricochet,
            Heatblast,
            GaussRound,
            Heatblast,
            Ricochet,
            Drill];

        private static uint[] OneTwoThreeToolsOpener = [
           GaussRound,
            Ricochet,
            HeatedSlugShot,
            BarrelStabilizer,
            HeatedCleanShot,
            AirAnchor,
            Reassemble,
            GaussRound,
            Drill,
            Reassemble,
            Wildfire,
            Chainsaw,
            AutomatonQueen,
            Hypercharge,
            Heatblast,
            Ricochet,
            Heatblast,
            GaussRound,
            Heatblast,
            Ricochet,
            Heatblast,
            GaussRound,
            Heatblast,
            Ricochet,
            HeatedSplitShot];

        private static uint[] EarlyToolsOpener = [
            AirAnchor,
            GaussRound,
            Ricochet,
            Drill,
            BarrelStabilizer,
            Reassemble,
            Chainsaw,
            GaussRound,
            Ricochet,
            HeatedSplitShot,
            GaussRound,
            HeatedSlugShot,
            Ricochet,
            Wildfire,
            HeatedCleanShot,
            AutomatonQueen,
            Hypercharge,
            Heatblast,
            GaussRound,
            Heatblast,
            Ricochet,
            Heatblast,
            GaussRound,
            Heatblast,
            Ricochet,
            Heatblast];

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && HasPrePullCooldowns() && LevelChecked;

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
                    if (value == OpenerState.InOpener) OpenerStep = 0;
                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            Svc.Log.Information($"Opener Failed at step {OpenerStep}");

                        ResetOpener();
                    }
                    if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                    currentState = value;
                }
            }
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CanOpener && PrePullStep == 0)
                PrePullStep = 1;

            if (!HasCooldowns())
                PrePullStep = 0;

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (Config.MCH_ST_RotationSelection == 0 || Config.MCH_ST_RotationSelection == 1)
                {
                    if (CustomComboFunctions.WasLastAction(HeatedSplitShot) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                    else if (PrePullStep == 1) actionID = HeatedSplitShot;

                    if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
                        CurrentState = OpenerState.FailedOpener;
                }

                if (Config.MCH_ST_RotationSelection == 2)
                {
                    if (CustomComboFunctions.HasEffect(Buffs.Reassembled) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                    else if (PrePullStep == 1) actionID = Reassemble;

                    if (PrePullStep == 2 && !CustomComboFunctions.HasEffect(Buffs.Reassembled))
                        CurrentState = OpenerState.FailedOpener;

                    if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
                        CurrentState = OpenerState.FailedOpener;

                }
                return true;
            }

            PrePullStep = 0;
            return false;
        }

        private bool DoOpener(uint[] OpenerActions, ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.WasLastAction(OpenerActions[OpenerStep]))
                    OpenerStep++;

                if (OpenerStep == OpenerActions.Length)
                    CurrentState = OpenerState.OpenerFinished;

                else actionID = OpenerActions[OpenerStep];

                if (CustomComboFunctions.InCombat() && ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == Ricochet && CustomComboFunctions.GetRemainingCharges(Ricochet) < 3) ||
                        (actionID == Chainsaw && CustomComboFunctions.IsOnCooldown(Chainsaw)) ||
                        (actionID == Wildfire && CustomComboFunctions.IsOnCooldown(Wildfire)) ||
                        (actionID == BarrelStabilizer && CustomComboFunctions.IsOnCooldown(BarrelStabilizer)) ||
                        (actionID == GaussRound && CustomComboFunctions.GetRemainingCharges(GaussRound) < 3)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                {
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }
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
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
            {
                if (simpleMode)
                {
                    if (DoOpener(DelayedToolsOpener, ref actionID))
                        return true;
                }
                else
                {
                    if (Config.MCH_ST_RotationSelection == 0)
                    {
                        if (DoOpener(DelayedToolsOpener, ref actionID))
                            return true;
                    }

                    if (Config.MCH_ST_RotationSelection == 1)
                    {
                        if (DoOpener(OneTwoThreeToolsOpener, ref actionID))
                            return true;
                    }

                    if (Config.MCH_ST_RotationSelection == 2)
                    {
                        if (DoOpener(EarlyToolsOpener, ref actionID))
                            return true;
                    }
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

    internal static class MCHExtensions
    {
        private static uint lastBattery = 0;
        internal static uint LastSummonBattery(this MCHGauge gauge)
        {
            if (!CustomComboFunctions.InCombat() || ActionWatching.CombatActions.Count(x => x == CustomComboFunctions.OriginalHook(MCH.RookAutoturret)) == 0)
                lastBattery = 0;

            if (ActionWatching.CombatActions.Count(x => x == CustomComboFunctions.OriginalHook(MCH.RookAutoturret)) > 0)
                lastBattery = gauge.LastSummonBatteryPower;

            return lastBattery;
        }
    }
}