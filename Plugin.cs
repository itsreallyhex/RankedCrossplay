using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GorillaNetworking;
using HarmonyLib;

namespace RankedCrossplay
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "dev.itsreallyhex.rankedcrossplay";
        public const string NAME = "RankedCrossplay";
        public const string VERSION = "1.0.0";

        internal static ConfigEntry<bool> JoinQuestLobbies;
        internal static ManualLogSource Log;

        Harmony harmony;

        void Awake()
        {
            Log = Logger;
            JoinQuestLobbies = Config.Bind("General", "JoinQuestLobbies", true,
                "Search for ranked matches in the Quest player pool instead of the PC one. Your real rank is used either way.");

            if (RankedJoinPatch.JoinAsync == null)
                Log.LogWarning("The game's ranked join method was not found (the game may have updated). Ranked joins are left unchanged.");

            harmony = new Harmony(GUID);
            harmony.PatchAll(typeof(Plugin).Assembly);
            Log.LogInfo(NAME + " " + VERSION + " loaded. Quest lobbies: " + (JoinQuestLobbies.Value ? "on" : "off"));
        }

        // Removes the patches when the plugin is unloaded, so a reloaded copy does not patch the game twice.
        void OnDestroy()
        {
            if (harmony != null) harmony.UnpatchSelf();
        }
    }

    // When you walk into a ranked queue, the game searches for a room by game mode, your rank tier and your platform.
    // On PC the platform is always "PC", so you only meet other PC players. This runs the same search with your
    // real tier and "Quest", so you are matched with Quest players at your own level.
    [HarmonyPatch(typeof(PhotonNetworkController), nameof(PhotonNetworkController.AttemptToJoinRankedPublicRoom))]
    static class RankedJoinPatch
    {
        // The private method that does the actual search; the public one only picks the tier and platform for it.
        internal static readonly MethodInfo JoinAsync =
            AccessTools.Method(typeof(PhotonNetworkController), "AttemptToJoinRankedPublicRoomAsync");

        static bool Prefix(PhotonNetworkController __instance, GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType)
        {
            // Returning true lets the game run its normal PC search.
            if (!Plugin.JoinQuestLobbies.Value || JoinAsync == null || RankedProgressionManager.Instance == null)
                return true;

            string tier = RankedProgressionManager.Instance.GetRankedMatchmakingTier().ToString();
            Plugin.Log.LogInfo("Ranked search: tier " + tier + ", Quest pool");
            JoinAsync.Invoke(__instance, new object[] { triggeredTrigger, tier, "Quest", roomJoinType });
            return false;
        }
    }

    // After you join a ranked match, the game asks the ranked server whether the join is valid and leaves the room
    // if it is not. This only writes the answer to the log, so a refusal can be told apart from a normal disconnect.
    [HarmonyPatch(typeof(GorillaTagCompetitiveServerApi), nameof(GorillaTagCompetitiveServerApi.RequestValidateMatchJoin))]
    static class MatchJoinLog
    {
        static void Prefix(ref Action<bool> callback)
        {
            Action<bool> original = callback;
            callback = valid =>
            {
                Plugin.Log.LogInfo("Ranked match join " + (valid ? "accepted" : "refused") + " by the server");
                if (original != null) original(valid);
            };
        }
    }
}
