using BepInEx;
using System;
using UnityEngine;
using Utilla;
using System.IO;
using System.Reflection;
using UnityEngine.XR;

namespace PortalGun
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]

    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        bool isPrimaryR;
        bool canPrimaryR;
        bool isPrimaryL;
        bool canPrimaryL;
        bool isSecondaryR;
        bool canSecondaryR;
        bool isTriggerL;
        bool canTriggerL;
        bool isSecondaryL;
        bool canSecondaryL;
        public bool isBlue = false;

        GameObject _portalGun;
        GameObject HandR;
        GameObject bluePortal;
        GameObject orangePortal;

        AudioSource activationAudioSource;
        AudioSource fireBlueAudioSource;
        AudioSource fireOrangeAudioSource;
        AudioSource resetAudioSource;
        AudioSource portalEnter1AudioSource;
        AudioSource portalEnter2AudioSource;
        AudioSource portalEnter3AudioSource;
        AudioSource songAudioSource;

        GameObject activation;
        GameObject fireOrange;
        GameObject fireBlue;
        GameObject reset;
        GameObject song;
        public GameObject portalEnter1;
        public GameObject portalEnter2;
        public GameObject portalEnter3;

        Portals orange, blue;
        private readonly XRNode rNode = XRNode.RightHand;
        private readonly XRNode lNode = XRNode.LeftHand;

        RaycastHit hit;

        Transform portalTransform;

        int portalCount = 0;
        void Start()
        {
            /* A lot of Gorilla Tag systems will not be set up when start is called /*
			/* Put code in OnGameInitialized to avoid null references */

            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled*/

            HarmonyPatches.ApplyHarmonyPatches();


            EnableThePortalGun();
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();

            DisableThePortalGun();
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
            SpawnPortalGun();
            DisableThePortalGun();
        }

        void ResetPortalGun()
        {
            blue.transform.position = new Vector3(0f, 0f, 0f);
            orange.transform.position = new Vector3(0f, 0f, 0f);
            reset.GetComponent<AudioSource>().Play();
        }

        void SpawnPortal(bool isBlue)
        {

            if (Physics.Raycast(portalTransform.position, portalTransform.forward, out hit))
            {
                if (isBlue)
                {
                    blue.transform.position = hit.point;
                    blue.transform.rotation = Quaternion.LookRotation(hit.normal);
                    fireBlue.GetComponent<AudioSource>().Play();
                    blue.exitPortal = orange;
                    orange.exitPortal = blue;
                }
                else if (!isBlue)
                {
                    orange.transform.position = hit.point;
                    orange.transform.rotation = Quaternion.LookRotation(hit.normal);
                    fireOrange.GetComponent<AudioSource>().Play();
                    orange.exitPortal = blue;
                    blue.exitPortal = orange;
                    
                }
            }
        }

        void SpawnPortalGun()
        {
            Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("PortalGun.Assets.prefabshit");
            AssetBundle asset = AssetBundle.LoadFromStream(str);
            GameObject portalGun = asset.LoadAsset<GameObject>("portal gun");
            _portalGun = Instantiate(portalGun);

            portalEnter3AudioSource = asset.LoadAsset<GameObject>("Teleport Audio Source 3").GetComponent<AudioSource>();
            portalEnter3 = Instantiate(portalEnter3AudioSource.gameObject);

            bluePortal = asset.LoadAsset<GameObject>("Blue Portal");
            blue = Instantiate(bluePortal).AddComponent<Portals>();
            blue.transform.position = new Vector3(0f, 0f, 0f);

            orangePortal = asset.LoadAsset<GameObject>("Orange Portal");
            orange = Instantiate(orangePortal).AddComponent<Portals>();
            orange.transform.position = new Vector3(0f, 0f, 0f);

            activationAudioSource = asset.LoadAsset<GameObject>("Activation Audio Source").GetComponent<AudioSource>();
            activation = Instantiate(activationAudioSource.gameObject);

            songAudioSource = asset.LoadAsset<GameObject>("Song Audio Source").GetComponent<AudioSource>();
            song = Instantiate(songAudioSource.gameObject);

            fireBlueAudioSource = asset.LoadAsset<GameObject>("Fire Blue Audio Source").GetComponent<AudioSource>();
            fireBlue = Instantiate(fireBlueAudioSource.gameObject);

            fireOrangeAudioSource = asset.LoadAsset<GameObject>("Fire Orange Audio Source").GetComponent<AudioSource>();
            fireOrange = Instantiate(fireOrangeAudioSource.gameObject);

            resetAudioSource = asset.LoadAsset<GameObject>("Reset Audio Source").GetComponent<AudioSource>();
            reset = Instantiate(resetAudioSource.gameObject);

            portalEnter1AudioSource = asset.LoadAsset<GameObject>("Teleport Audio Source 1").AddComponent<AudioSource>();
            portalEnter1 = Instantiate(portalEnter1AudioSource.gameObject);

            portalEnter2AudioSource = asset.LoadAsset<GameObject>("Teleport Audio Source 2").GetComponent<AudioSource>();
            portalEnter2 = Instantiate(portalEnter2AudioSource.gameObject);

            HandR = GameObject.Find("Global/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/palm.01.R/");
            _portalGun.transform.SetParent(HandR.transform, false);
            _portalGun.transform.position = new Vector3(-139.96f, 27.1682f, -102.0313f);
            _portalGun.transform.localPosition = new Vector3(-0.0562f, 0.2447f, -0.0767f);
            _portalGun.transform.localScale = new Vector3(7, 7, 7);
            _portalGun.transform.localRotation = Quaternion.Euler(341.5006f, 85.8224f, 268.9002f);

            portalTransform = new GameObject().transform;
            portalTransform.SetParent(_portalGun.transform, false);
            portalTransform.localPosition = new Vector3(-0.03049f, -0.00613f, 0f);
            portalTransform.localRotation = Quaternion.Euler(0f, -90.00001f, 0f);

            isBlue = true;

            DisableThePortalGun();
        }

        void DisableThePortalGun()
        {
            _portalGun.SetActive(false);
            orange.enabled = false;
            blue.enabled = false;
            orange.gameObject.SetActive(false);
            blue.gameObject.SetActive(false);
            activation.SetActive(false);
            reset.SetActive(false);
            fireBlue.SetActive(false);
            fireOrange.SetActive(false);
            song.SetActive(false);
        }

        void EnableThePortalGun()
        {
            _portalGun.SetActive(true);
            orange.enabled = true;
            blue.enabled = true;
            orange.gameObject.SetActive(true);
            blue.gameObject.SetActive(true);
            activation.SetActive(true);
            reset.SetActive(true);
            fireBlue.SetActive(true);
            fireOrange.SetActive(true);
            song.SetActive(true);
            activation.GetComponent<AudioSource>().Play();
        }
        void Update()
        {
            /* Code here runs every frame when the mod is enabled */
            if (inRoom)
            {
                InputDevices.GetDeviceAtXRNode(rNode).TryGetFeatureValue(CommonUsages.primaryButton, out isPrimaryR);
                InputDevices.GetDeviceAtXRNode(rNode).TryGetFeatureValue(CommonUsages.secondaryButton, out isSecondaryR);
                InputDevices.GetDeviceAtXRNode(lNode).TryGetFeatureValue(CommonUsages.primaryButton, out isPrimaryL);
                InputDevices.GetDeviceAtXRNode(lNode).TryGetFeatureValue(CommonUsages.secondaryButton, out isSecondaryL);
                InputDevices.GetDeviceAtXRNode(lNode).TryGetFeatureValue(CommonUsages.triggerButton, out isTriggerL);

                if (isPrimaryR)
                {
                    if (canPrimaryR)
                    {
                        SpawnPortal(true);
                        canPrimaryR = false;
                    }
                }
                else
                {
                    canPrimaryR = true;
                }

                if (isSecondaryR)
                {
                    if (canSecondaryR)
                    {
                        SpawnPortal(false);
                        canSecondaryR = false;
                    }
                }
                else
                {
                    canSecondaryR = true;
                }

                if (isPrimaryL)
                {
                    if (canPrimaryL)
                    {
                        ResetPortalGun();
                        canPrimaryL = false;
                    }
                }
                else
                {
                    canPrimaryL = true;
                }

                if (isSecondaryL)
                {
                    if (canSecondaryL)
                    {
                        song.GetComponent<AudioSource>().Play();
                        canSecondaryL = false;
                    }
                }
                else
                {
                    canSecondaryL = true;
                }

                if (isTriggerL)
                {
                    if (canTriggerL)
                    {
                        song.GetComponent<AudioSource>().Stop();
                        canTriggerL = false;
                    }
                }
                else
                {
                    canTriggerL = true;
                }
            }
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/
            EnableThePortalGun();

            inRoom = true;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/
            DisableThePortalGun();
            inRoom = false;
        }
    }
}
