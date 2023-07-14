using GorillaLocomotion;
using HarmonyLib;
using PlayFab.MultiplayerModels;
using PortalGun;
using System;
using System.Reflection;
using UnityEngine;

public class Portals : MonoBehaviour
{
    public Portals exitPortal;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (exitPortal)
        {
            TeleportPatch.TeleportPlayer(exitPortal.transform.TransformPoint(Vector3.forward * 1), 180f, false);
        }
    }
}

