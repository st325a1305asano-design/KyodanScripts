using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 一人称視点と俯瞰視点を変更するスクリプト
/// </summary>
public class CameraSwitcher : MonoBehaviour
{
    #region 変数の宣言
    [SerializeField] Camera firstPersonCamera; //一人称視点カメラを入れる変数
    [SerializeField] Camera mapTopViewCamera; //俯瞰視点カメラを入れる変数

    PlayerInput playerInput; //PlayerInputを入れる変数

    bool isTopView = false; //俯瞰視点フラグ
    #endregion

    void Start()
    {
        //このスクリプトがアタッチされているオブジェクトについているPlayerInputを取得
        playerInput = GetComponent<PlayerInput>();

        //一人称にする
        firstPersonCamera.enabled = true;
        mapTopViewCamera.enabled = false;
    }
    
    void Update()
    {
        //ボタン入力を取得
        if (playerInput.actions["SwitchCamera"].triggered == true)
        {
            //視点変更関数を呼び出す
            ToggleCamera();
        }
    }

    /// <summary>
    /// 視点を切り替える関数
    /// </summary>
    void ToggleCamera()
    {
        //俯瞰視点フラグを反対の状態にする
        isTopView = !isTopView;

        //一人称→俯瞰視点
        if (isTopView)//SelectCamera.Top;
        {
            //俯瞰にする
            firstPersonCamera.enabled = false;
            mapTopViewCamera.enabled = true;

            //Lookの入力を停止
            playerInput.actions["Look"].Disable();
        }
        //俯瞰視点→一人称
        else //SelectCamera.First;
        {
            //一人称にする
            firstPersonCamera.enabled= true;
            mapTopViewCamera.enabled= false;

            //Lookの入力を再開
            playerInput.actions["Look"].Enable();
        }
    }
}
