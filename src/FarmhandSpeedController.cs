using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Logic;
using UnityEngine;
using View.Farmhands;

[assembly: AssemblyCompany("JBluesword")]
[assembly: AssemblyProduct("FarmhandSpeedController")]
[assembly: AssemblyTitle("FarmhandSpeedController")]
[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]

namespace FarmhandSpeedController
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class FarmhandSpeedController : BasePlugin
    {
        private const string PluginGuid = "farmhandspeedcontroller";
        private const string PluginName = "FarmhandSpeedController";
        private const string PluginVersion = "0.1.0";
        private const float MinimumApproachDistance = 0.7f;

        private static ManualLogSource _log;
        private static ConfigEntry<bool> _enabled;
        private static ConfigEntry<float> _movementMultiplier;
        private static ConfigEntry<float> _workMultiplier;
        private static ConfigEntry<float> _approachSlowdownDistance;
        private static ConfigEntry<bool> _debugLogging;
        private static ConfigEntry<bool> _logEveryTask;
        private static ConfigEntry<int> _summaryIntervalSeconds;

        private static bool _movementPatchActive;
        private static bool _workPatchActive;
        private static int _movementErrors;
        private static int _workErrors;
        private static long _movementUpdates;
        private static long _movementScaled;
        private static long _workStarted;
        private static long _workInvoked;
        private static long _travelStarted;
        private static float _sumBaseMovement;
        private static float _sumTargetMovement;
        private static float _nextSummaryTime;
        private Harmony _harmony;

        public override void Load()
        {
            _log = Log;
            BindConfiguration();
            _log.LogInfo($"{PluginName} {PluginVersion} by JBluesword loading. Farmhand-only test build; no player/tractor speed or reward changes.");
            LogConfiguration();

            try
            {
                _harmony = new Harmony(PluginGuid);
                Patch("movement", typeof(LocalFarmhandView), "UpdateGoingToTile",
                    new[] { typeof(float) }, nameof(UpdateGoingToTilePostfix));
                Patch("travel", typeof(LocalFarmhandView), "StartGoingToTile",
                    new[] { typeof(FarmTileId) }, nameof(StartGoingToTilePostfix));
                Patch("work", typeof(FarmhandView), "StartWorking",
                    new[] { typeof(FarmTileId) }, nameof(StartWorkingPostfix));
                Patch("work observation", typeof(FarmhandView), "PerformWork",
                    new[] { typeof(FarmTileId) }, nameof(PerformWorkPostfix));
                _log.LogInfo($"Patch status: movement={_movementPatchActive}, work={_workPatchActive}. If either is false, send BepInEx/LogOutput.log.");
            }
            catch (Exception exception)
            {
                _log.LogError("Plugin initialization failed: " + exception);
            }
        }

        public override bool Unload()
        {
            _harmony?.UnpatchSelf();
            return true;
        }

        private void BindConfiguration()
        {
            _enabled = Config.Bind("General", "Enabled", true,
                "Enable farmhand movement and work speed changes. Does not affect players or tractors.");
            _movementMultiplier = Config.Bind("Speed", "MovementMultiplier", 2.0f,
                new ConfigDescription("Farmhand travel target speed. 1.0 is vanilla; 2.0 requests twice the target speed. Effective travel time also depends on pathing and acceleration.",
                    new AcceptableValueRange<float>(1.0f, 6.0f)));
            _workMultiplier = Config.Bind("Speed", "WorkMultiplier", 2.0f,
                new ConfigDescription("Farmhand tile-work and work-animation timers. 1.0 is vanilla; 2.0 halves the timers. Vanilla performs the actual work.",
                    new AcceptableValueRange<float>(1.0f, 10.0f)));
            _approachSlowdownDistance = Config.Bind("Speed", "ApproachSlowdownDistance", 2.5f,
                new ConfigDescription("Distance from a destination at which the movement boost begins tapering back to vanilla, reducing overshoot.",
                    new AcceptableValueRange<float>(1.0f, 5.0f)));
            _debugLogging = Config.Bind("Diagnostics", "EnableDebugLogging", true,
                "Log periodic patch counters and movement samples for this test build.");
            _logEveryTask = Config.Bind("Diagnostics", "LogEveryTask", true,
                "Log each farmhand destination, work start, and work invocation. Disable after testing to reduce console volume.");
            _summaryIntervalSeconds = Config.Bind("Diagnostics", "SummaryIntervalSeconds", 30,
                new ConfigDescription("Seconds between diagnostic summaries while farmhands are active.",
                    new AcceptableValueRange<int>(10, 300)));
        }

        private static void LogConfiguration()
        {
            _log.LogInfo($"Config: Enabled={_enabled.Value}, MovementMultiplier={_movementMultiplier.Value:0.00}, WorkMultiplier={_workMultiplier.Value:0.00}, ApproachSlowdownDistance={_approachSlowdownDistance.Value:0.00}, EnableDebugLogging={_debugLogging.Value}, LogEveryTask={_logEveryTask.Value}, SummaryIntervalSeconds={_summaryIntervalSeconds.Value}");
        }

        private void Patch(string label, Type targetType, string targetName, Type[] parameters, string callbackName)
        {
            try
            {
                MethodInfo target = AccessTools.Method(targetType, targetName, parameters);
                MethodInfo callback = AccessTools.Method(typeof(FarmhandSpeedController), callbackName);
                if (target == null || callback == null)
                {
                    _log.LogError($"Patch missing: {label}, target={targetType.FullName}.{targetName}, callback={callbackName}, targetFound={target != null}, callbackFound={callback != null}");
                    return;
                }

                _harmony.Patch(target, postfix: new HarmonyMethod(callback));
                if (label == "movement") _movementPatchActive = true;
                if (label == "work") _workPatchActive = true;
                _log.LogInfo($"Patch applied: {label} -> {targetType.FullName}.{targetName}({string.Join(", ", Array.ConvertAll(parameters, item => item.Name))})");
            }
            catch (Exception exception)
            {
                _log.LogError($"Patch failed: {label} -> {targetType.FullName}.{targetName}: {exception}");
            }
        }

        private static void StartGoingToTilePostfix(LocalFarmhandView __instance, FarmTileId __0)
        {
            if (!_enabled.Value || __instance == null) return;
            _travelStarted++;
            if (!_logEveryTask.Value) return;
            try
            {
                _log.LogInfo($"TRAVEL START worker={WorkerId(__instance)} tile={__0} state={__instance.State} position={Position(__instance)} target={__instance.targetPosition} expectedTravelTime={__instance.expectedTravelTime:0.000}s");
            }
            catch (Exception exception)
            {
                _log.LogWarning("Travel diagnostic failed (speed patch unaffected): " + exception);
            }
        }

        private static void UpdateGoingToTilePostfix(LocalFarmhandView __instance)
        {
            if (!_enabled.Value || !_movementPatchActive || __instance == null) return;
            try
            {
                if (__instance.State != FarmhandView.FarmhandState.GoingToTile) return;
                Vector2 vanillaTarget = __instance.targetVelocity;
                float baseMagnitude = vanillaTarget.magnitude;
                if (!Finite(baseMagnitude) || baseMagnitude < 0.01f) return;

                float multiplier = _movementMultiplier.Value;
                if (!Finite(multiplier) || multiplier < 1.0f) multiplier = 1.0f;
                if (multiplier > 6.0f) multiplier = 6.0f;

                Vector3 position = __instance.transform.position;
                Vector3 target = __instance.targetPosition;
                float dx = target.x - position.x;
                float dz = target.z - position.z;
                float distance = Mathf.Sqrt(dx * dx + dz * dz);
                if (!Finite(distance)) return;

                float approachDistance = _approachSlowdownDistance.Value;
                if (!Finite(approachDistance) || approachDistance <= MinimumApproachDistance)
                    approachDistance = 2.5f;
                float blend = Mathf.Clamp01((distance - MinimumApproachDistance) /
                    (approachDistance - MinimumApproachDistance));
                float appliedMultiplier = 1.0f + (multiplier - 1.0f) * blend;
                float targetMagnitude = baseMagnitude * appliedMultiplier;
                if (!Finite(targetMagnitude)) return;

                if (appliedMultiplier > 1.001f)
                {
                    __instance.targetVelocity = vanillaTarget * appliedMultiplier;
                    _movementScaled++;
                }

                _movementUpdates++;
                _sumBaseMovement += baseMagnitude;
                _sumTargetMovement += targetMagnitude;
                MaybeLogSummary(__instance, distance, baseMagnitude, targetMagnitude, appliedMultiplier);
            }
            catch (Exception exception)
            {
                RecordPatchError("movement", ref _movementErrors, ref _movementPatchActive, exception);
            }
        }

        private static void StartWorkingPostfix(FarmhandView __instance, FarmTileId __0)
        {
            if (!_enabled.Value || !_workPatchActive || __instance == null) return;
            try
            {
                if (__instance.IsRemote || __instance.State != FarmhandView.FarmhandState.Working)
                    return;

                float originalWork = __instance.workDuration;
                float originalAnimation = __instance.workAnimationDuration;
                if (!Finite(originalWork) || !Finite(originalAnimation) || originalWork < 0.0f || originalAnimation < 0.0f)
                {
                    _log.LogWarning($"WORK SKIP worker={WorkerId(__instance)} tile={__0} invalid timers work={originalWork}, animation={originalAnimation}");
                    return;
                }

                float multiplier = _workMultiplier.Value;
                if (!Finite(multiplier) || multiplier < 1.0f) multiplier = 1.0f;
                if (multiplier > 10.0f) multiplier = 10.0f;
                if (multiplier > 1.0f)
                {
                    if (originalWork > 0.0f)
                        __instance.workDuration = Mathf.Max(0.05f, originalWork / multiplier);
                    if (originalAnimation > 0.0f)
                        __instance.workAnimationDuration = Mathf.Max(0.05f, originalAnimation / multiplier);
                }

                _workStarted++;
                if (_logEveryTask.Value)
                    _log.LogInfo($"WORK START worker={WorkerId(__instance)} tile={__0} workType={__instance.WorkType} workTimer={originalWork:0.000}->{__instance.workDuration:0.000}s animationTimer={originalAnimation:0.000}->{__instance.workAnimationDuration:0.000}s multiplier={multiplier:0.00} performedWork={__instance.performedWork}");
            }
            catch (Exception exception)
            {
                RecordPatchError("work", ref _workErrors, ref _workPatchActive, exception);
            }
        }

        private static void PerformWorkPostfix(FarmhandView __instance, FarmTileId __0)
        {
            if (!_enabled.Value || __instance == null) return;
            try
            {
                if (__instance.IsRemote) return;
                _workInvoked++;
                if (_logEveryTask.Value)
                    _log.LogInfo($"WORK INVOKED worker={WorkerId(__instance)} tile={__0} workType={__instance.WorkType} state={__instance.State}; vanilla TryPerformFarmhandWork/network path returned (success must be verified in game)");
            }
            catch (Exception exception)
            {
                _log.LogWarning("Work diagnostic failed (vanilla work unaffected): " + exception);
            }
        }

        private static void MaybeLogSummary(LocalFarmhandView worker, float distance, float baseSpeed, float targetSpeed, float appliedMultiplier)
        {
            if (!_debugLogging.Value) return;
            float now = Time.realtimeSinceStartup;
            if (now < _nextSummaryTime) return;
            _nextSummaryTime = now + _summaryIntervalSeconds.Value;
            try
            {
                float averageBase = _movementUpdates > 0 ? _sumBaseMovement / _movementUpdates : 0.0f;
                float averageTarget = _movementUpdates > 0 ? _sumTargetMovement / _movementUpdates : 0.0f;
                _log.LogInfo($"DIAG worker={WorkerId(worker)} state={worker.State} targetTile={worker.targetTile} distance={distance:0.00}m vanillaTarget={baseSpeed:0.00}m/s adjustedTarget={targetSpeed:0.00}m/s appliedMultiplier={appliedMultiplier:0.00} rigidbodySpeed={worker.Speed:0.00}m/s movementUpdates={_movementUpdates} scaledUpdates={_movementScaled} avgVanillaTarget={averageBase:0.00} avgAdjustedTarget={averageTarget:0.00} travelStarted={_travelStarted} workStarted={_workStarted} workInvoked={_workInvoked} movementErrors={_movementErrors} workErrors={_workErrors}");
            }
            catch (Exception exception)
            {
                _log.LogWarning("Summary diagnostic failed (speed patch unaffected): " + exception);
            }
        }

        private static string WorkerId(FarmhandView worker)
        {
            try
            {
                return $"building={worker.BuildingId},ptr=0x{IL2CPP.Il2CppObjectBaseToPtr(worker).ToInt64():X}";
            }
            catch
            {
                return "building=?,ptr=?";
            }
        }

        private static string Position(FarmhandView worker)
        {
            try { return worker.transform.position.ToString(); }
            catch { return "?"; }
        }

        private static bool Finite(float number)
        {
            return !float.IsNaN(number) && !float.IsInfinity(number);
        }

        private static void RecordPatchError(string label, ref int count, ref bool active, Exception exception)
        {
            count++;
            _log.LogError($"{label} patch error #{count}: {exception}");
            if (count >= 3)
            {
                active = false;
                _log.LogError($"{label} speed changes disabled for this session after three errors; vanilla farmhand behavior remains available.");
            }
        }
    }
}
