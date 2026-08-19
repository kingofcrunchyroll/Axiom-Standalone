using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Axiom.Notifications;
using UnityEngine;

namespace Axiom.Patches
{
    [HarmonyPatch(typeof(MonoBehaviourPunCallbacks), "OnPlayerEnteredRoom")]
    public class JoinPatch : MonoBehaviour
    {
        private static void Prefix(Player newPlayer)
        {
            if (newPlayer != oldnewplayer)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=green>JOIN</color><color=grey>] </color><color=white>Name: " + newPlayer.NickName + "</color>");
                oldnewplayer = newPlayer;
            }
        }

        private static Player oldnewplayer;
    }
}