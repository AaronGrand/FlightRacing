using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameDebugging : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI joinCode;

    private void Start()
    {
        joinCode.text = HostManager.Instance.joinCode;
    }
}
