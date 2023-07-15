using PortalGun;
using UnityEngine;

public class Portals : MonoBehaviour
{
    public Portals exitPortal;
    static Plugin portalInfo;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        int portalEnterSoundRandom = Random.Range(1, 4);
        if (exitPortal)
        {
            TeleportPatch.TeleportPlayer(exitPortal.transform.TransformPoint(Vector3.forward * 1), 180f, false);
            if (portalEnterSoundRandom == 1)
            {
                portalInfo.portalEnter1.GetComponent<AudioSource>().Play();
            }
            else if (portalEnterSoundRandom == 2)
            {
                portalInfo.portalEnter2.GetComponent<AudioSource>().Play();
            }
            else if (portalEnterSoundRandom == 3)
            {
                portalInfo.portalEnter3.GetComponent<AudioSource>().Play();
            }
        }
    }
}

