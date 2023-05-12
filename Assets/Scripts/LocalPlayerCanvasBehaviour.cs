using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class LocalPlayerCanvasBehaviour : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private TextMeshProUGUI joinCode;
    void Start()
    {
        joinCode.text = "JoinCode: " + HostManager.Instance.joinCode;
        StartCoroutine(FindLocalPlayer(0.5f));
    }


    // MAKE SOME BUTTONS HERE TO SAVE PREFS AND CREATE A UNIQUE PLANE
    // COLOR CHANGE
    // PLANE MODEL CHANGE

    // ALSO CREATE PREFS

    private IEnumerator FindLocalPlayer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        {
            foreach (NetworkObject identity in FindObjectsOfType<NetworkObject>())
            {
                if (identity.IsLocalPlayer)
                {
                    player = identity.gameObject;
                    break;
                }
            }
        }
    }
}
