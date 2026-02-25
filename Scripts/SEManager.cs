using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SE再生
/// </summary>
public class SEManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource; //audioSaurceを入れる変数
    [SerializeField] AudioClip hitCharacter; //人に当たった時のSE
    [SerializeField] AudioClip hitVase; //花瓶に当たった時のSE

    public static SEManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// SEの再生関数
    /// </summary>
    /// <param name="a"></param>
    public void PlaySE(int a)
    {
        if (a == 1)
        {
            //花瓶ヒットSE
            audioSource.PlayOneShot(hitVase);
        }
        else if (a == 2)
        {
            //キャラクターヒットSE
            audioSource.PlayOneShot(hitCharacter);
        }
    }
}
