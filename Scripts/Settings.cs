using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ê›íËï€éù
/// </summary>
public class Settings : MonoBehaviour
{
    public static Settings Instance;

    [Header("ä¥ìx")]
    [SerializeField]public float sensitivity = 100f;

    [Header("îΩì]")]
    [SerializeField]public bool invertY = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
