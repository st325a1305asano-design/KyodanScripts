using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 設定UI連携
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [SerializeField] Slider slider; //スライダーを入れる変数
    [SerializeField] Toggle toggle; //トグルを入れる変数

    void Start()
    {
        //感度設定の連携
        slider.value = Settings.Instance.sensitivity;
        slider.onValueChanged.AddListener(OnValueChanged);

        //カメラ反転の連携
        toggle.isOn = Settings.Instance.invertY;
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    /// <summary>
    /// 感度設定の変更
    /// </summary>
    /// <param name="value"></param>
    void OnValueChanged(float value)
    {
        Settings.Instance.sensitivity = value;
    }

    /// <summary>
    /// カメラ反転の変更
    /// </summary>
    /// <param name="value"></param>
    void OnToggleChanged(bool value)
    {
        Settings.Instance.invertY = value;
    }
}
