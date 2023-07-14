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
        bool isTrigger;
        bool canTrigger;
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

        GameObject activation;
        GameObject fireOrange;
        GameObject fireBlue;
        GameObject reset;
        public GameObject portalEnter1;
        public GameObject portalEnter2;
        public GameObject portalEnter3;

        Portals orange, blue;
        private readonly XRNode rNode = XRNode.RightHand;

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


        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
            Stream _str = Assembly.GetExecutingAssembly().GetManifestResourceStream("PortalGun.Assets.blueportal");
            AssetBundle _asset = AssetBundle.LoadFromStream(_str);
            bluePortal = _asset.LoadAsset<GameObject>("Blue Portal");

            Stream __str = Assembly.GetExecutingAssembly().GetManifestResourceStream("PortalGun.Assets.orangeportal");
            AssetBundle __asset = AssetBundle.LoadFromStream(__str);
            orangePortal = __asset.LoadAsset<GameObject>("Orange Portal");

            Stream ___str = Assembly.GetExecutingAssembly().GetManifestResourceStream("PortalGun.Assets.activation");
            AssetBundle ___asset = AssetBundle.LoadFromStream(___str);
            activationAudioSource = ___asset.LoadAsset<GameObject>("Activation Audio Source").GetComponent<AudioSource>();
            activation = Instantiate(activationAudioSource.gameObject);

            Stream _____str = Assembly.GetExecutingAssembly().GetManifestResourceStream("PortalGun.Assets.fireblue");
            AssetBundle _____asset = AssetBundle.LoadFromStream(_____str);
            fireBlueAudioSource = _____asset.LoadAsset<GameObject>("Fire Blue Audio Source").GetComponent<AudioSource>();
            fireBlue = Instantiate(fireBlueAudioSource.gameObject);

            Stream ______str = Assembly.GetExecutingAssembly().GetManifestResourceStream("PortalGun.Assets.fireorange");
            AssetBundle ______asset = AssetBundle.LoadFromStream(______str);
            fireOrangeAudioSource = ______asset.LoadAsset<GameObject>("Fire Orange Audio Source").GetComponent<AudioSource>();
            fireOrange = Instantiate(fireOrangeAudioSource.gameObject);

            Stream _______str = Assembly.GetExecutingAssembly().GetManifestResourceStream("PortalGun.Assets.portalgunreset");
            AssetBundle _______asset = AssetBundle.LoadFromStream(_______str);
            resetAudioSource = _______asset.LoadAsset<GameObject>("Reset Audio Source").GetComponent<AudioSource>();
            reset = Instantiate(resetAudioSource.gameObject);

            Stream ________str = Assembly.GetExecutingAssembly().GetManifestResourceStream("PortalGun.Assets.portalenter1");
            AssetBundle ________asset = AssetBundle.LoadFromStream(________str);
            portalEnter1AudioSource = ________asset.LoadAsset<GameObject>("Teleport Audio Source 1").AddComponent<AudioSource>();
            portalEnter1 = Instantiate(portalEnter1AudioSource.gameObject);

            Stream _________str = Assembly.GetExecutingAssembly().GetManifestResourceStream("PortalGun.Assets.portalenter2");
            AssetBundle _________asset = AssetBundle.LoadFromStream(_________str);
            portalEnter2AudioSource = _________asset.LoadAsset<GameObject>("Teleport Audio Source 2").GetComponent<AudioSource>();
            portalEnter2 = Instantiate(portalEnter2AudioSource.gameObject);

            Stream __________str = Assembly.GetExecutingAssembly().GetManifestResourceStream("PortalGun.Assets.portalenter3");
            AssetBundle __________asset = AssetBundle.LoadFromStream(__________str);
            portalEnter3AudioSource = __________asset.LoadAsset<GameObject>("Teleport Audio Source 3").GetComponent<AudioSource>();
            portalEnter3 = Instantiate(portalEnter3AudioSource.gameObject);

            Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("PortalGun.Assets.portalgun");
            AssetBundle asset = AssetBundle.LoadFromStream(str);
            GameObject portalGun = asset.LoadAsset<GameObject>("portal gun");
            _portalGun = Instantiate(portalGun);

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

            activation.GetComponent<AudioSource>().Play();

            isBlue = true;

        }
        void SpawnPortal()
        {

            if (Physics.Raycast(portalTransform.position, portalTransform.forward, out hit))
            {
                portalCount++;
                if (portalCount > 2)
                {
                    return;
                }
                if (portalCount == 1)
                {
                    isBlue = true;
                }
                else if (portalCount == 2)
                {
                    isBlue = false;
                }

                if (isBlue)
                {
                    if (blue.gameObject != null)
                    {
                        Destroy(blue.gameObject);
                        blue = Instantiate(bluePortal, hit.point, Quaternion.LookRotation(hit.normal)).AddComponent<Portals>();
                        fireBlue.GetComponent<AudioSource>().Play();
                        blue.exitPortal = orange;
                        orange.exitPortal = blue;
                    }
                    else if (blue.gameObject == null)
                    {
                        blue = Instantiate(bluePortal, hit.point, Quaternion.LookRotation(hit.normal)).AddComponent<Portals>();
                        fireBlue.GetComponent<AudioSource>().Play();
                        blue.exitPortal = orange;
                        orange.exitPortal = blue;
                    }
                }
                else if (!isBlue)
                {
                    if (orange.gameObject != null)
                    {
                        Destroy(orange.gameObject);
                        orange = Instantiate(orangePortal, hit.point, Quaternion.LookRotation(hit.normal)).AddComponent<Portals>();
                        fireOrange.GetComponent<AudioSource>().Play();
                        orange.exitPortal = blue;
                        blue.exitPortal = orange;
                    }
                    else if (orange.gameObject == null)
                    {
                        orange = Instantiate(orangePortal, hit.point, Quaternion.LookRotation(hit.normal)).AddComponent<Portals>();
                        fireOrange.GetComponent<AudioSource>().Play();
                        orange.exitPortal = blue;
                        blue.exitPortal = orange;
                    }
                }
            }

        }

        void SpawnPortulGun()
        {

        }

        void Update()
        {
            /* Code here runs every frame when the mod is enabled */
            if (portalCount > 2)
            {            
                portalCount = 1;
                isBlue = false;
                reset.GetComponent<AudioSource>().Play();
            }

            InputDevices.GetDeviceAtXRNode(rNode).TryGetFeatureValue(CommonUsages.triggerButton, out isTrigger);

            if (isTrigger)
            {
                if (canTrigger)
                {
                    SpawnPortal();
                    canTrigger = false;
                }
            }
            else
            {
                canTrigger = true;
            }


        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = true;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = false;
        }
    }
}
