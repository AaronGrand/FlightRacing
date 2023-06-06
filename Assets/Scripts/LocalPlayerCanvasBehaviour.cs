using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class LocalPlayerCanvasBehaviour : MonoBehaviour
{
    private FlightController3D player;
    private bool canChangeColor = false;

    [SerializeField] private TextMeshProUGUI joinCode;
    void Start()
    {
        joinCode.text = "JoinCode: " + HostManager.Instance.joinCode;
        StartCoroutine(FindLocalPlayer(0.5f));
    }

    public void OnColorChanged(Color newColor)
    {
        if (canChangeColor)
        {
            player.SetColor(newColor);
        }
    }

    public void OnModelChange()
    {
        player.NextModel();
    }

    private IEnumerator FindLocalPlayer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        {
            foreach (NetworkObject identity in FindObjectsOfType<NetworkObject>())
            {
                if (identity.IsLocalPlayer)
                {
                    player = identity.gameObject.GetComponent<FlightController3D>();
                    canChangeColor = true;
                    break;
                }
            }
        }
    }
}
