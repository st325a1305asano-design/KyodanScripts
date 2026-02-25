using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 鏡の設置と回収のスクリプト(CreateModeの切り替え)
/// </summary>
public class CreateModeManager : MonoBehaviour
{
    #region 変数の宣言
    [SerializeField] GameObject mirrorPrefab; //鏡のプレハブを入れる変数
    [SerializeField] GameObject wallMirrorPrefab; //壁掛け鏡のプレハブを入れる変数

    [Header("レイヤー設定")]
    [SerializeField] LayerMask groundLayer; //床レイヤー
    [SerializeField] LayerMask wallLayer; //壁レイヤー
    [SerializeField] LayerMask mirrorLayer; //鏡レイヤー

    [SerializeField] float rayDistance = 100; //光線距離
    [SerializeField] int maxMirrorNumber = 10; //鏡の最大設置可能数
    [SerializeField] MirrorPreview mirrorPreview; //MirrorPreviewを入れる変数

    int canSetMirrorNumber; //鏡の設置可能数
    PlayerInput playerInput; //PlayerInputを入れる変数
    Camera mainCamera; //MainCameraを入れる変数

    bool isCreateMode = false; //CreateMode管理用フラグ
    #endregion

    //プロパティ
    public bool IsCreateMode { get => isCreateMode; set => isCreateMode = value; }

    void Start()
    {
        //鏡の設置可能数の設定
        canSetMirrorNumber = maxMirrorNumber;

        //PlayerInputを入れる
        playerInput = FindAnyObjectByType<PlayerInput>();

        //mainCameraにMainCamera(一人称視点カメラ)を取得
        mainCamera = Camera.main;

        //鏡設置可能数表示を更新
        GameManager.Instance.UpdateSetMirrorNumber(canSetMirrorNumber);

        //操作画面表示
        GameManager.Instance.ShowModeText(isCreateMode);
    }

    void Update()
    {
        HandleModeSwitch(); //モード切替関数を呼び出す

        //CreateModeがオンなら
        if (isCreateMode)
        {
            HandleCreateModeInput(); //設置と回収の入力をする関数を呼び出す
        }
    }

    #region 入力読み取り
    /// <summary>
    /// NormalModeとCreateModeを切り替える関数
    /// </summary>
    void HandleModeSwitch()
    {
        //モード切替が押されたら
        if (playerInput.actions["ToggleCreateMode"].triggered)
        {
            //フラグを反転させる
            isCreateMode = !isCreateMode;

            GameManager.Instance.ShowModeText(isCreateMode);

            //CreateModeなら
            if (isCreateMode)
            {
                //ActionMapsをCreateに変更
                GameManager.Instance.SwitchActionMaps("Create");
            }
            else
            {
                //ActionMapsをPlayerActionに変更
                GameManager.Instance.SwitchActionMaps("PlayerAction");
            }
        }
    }

    /// <summary>
    /// 設置と回収を読み取る関数
    /// </summary>
    void HandleCreateModeInput()
    {
        //設置が押されたら
        if (playerInput.actions["Put"].triggered)
        {
            //鏡設置関数を呼び出す
            TryPlaceMirror();
        }
        //回収が押されたら
        if (playerInput.actions["Collect"].triggered)
        {
            //鏡回収関数を呼び出す
            TryRemoveMirror();
        }
        //全回収が押されたら
        if (playerInput.actions["AllCollect"].triggered)
        {
            //鏡全回収関数を呼び出す
            CollectAllMirrors();
        }
    }
    #endregion

    #region 設置と回収
    /// <summary>
    /// 鏡を設置する関数
    /// </summary>
    void TryPlaceMirror()
    {
        if (!isCreateMode) return;

        if (canSetMirrorNumber != 0)
        {

            // プレビューが示す位置と角度を取得
            Vector3 placePos = mirrorPreview.GetPlacePosition(); //←ここ
            Quaternion placeRot = mirrorPreview.GetPlaceRotation();

            // 鏡を設置
            if (mirrorPreview.IsWall == true)
            {
                Instantiate(wallMirrorPrefab, placePos, placeRot);
            }
            else if (mirrorPreview.IsWall == false)
            {
                Instantiate(mirrorPrefab, placePos, placeRot);
            }

            if (canSetMirrorNumber > 0)
            {
                //鏡設置可能数表示を減らす
                canSetMirrorNumber--;
                GameManager.Instance.UpdateSetMirrorNumber(canSetMirrorNumber);
            }
        }
        return;
    }


    /// <summary>
    /// 鏡を回収する関数
    /// </summary>
    void TryRemoveMirror()
    {
        //マウスの位置を取得
        Vector2 cursorPos = playerInput.actions["CursorPosition"].ReadValue<Vector2>();

        //マウス位置から光線を出す
        Ray ray = mainCamera.ScreenPointToRay(cursorPos);

        //光線が鏡レイヤーに当たったなら
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, mirrorLayer))
        {
                //mirrorRootに当たったオブジェクトの親を入れる
                GameObject mirrorRoot = hit.collider.transform.root.gameObject;

                //当たったオブジェクトを壊す
                Destroy(mirrorRoot);

            //当たったものが設置した鏡なら
            if (hit.collider.CompareTag("PlacedMirror"))
            {
                if (canSetMirrorNumber >= 0)
                {
                    //鏡設置可能数を増やす
                    canSetMirrorNumber++;
                    GameManager.Instance.UpdateSetMirrorNumber(canSetMirrorNumber);
                }
            }
        }
    }

    /// <summary>
    /// 鏡の全回収
    /// </summary>
    public void CollectAllMirrors()
    {
        //シーン内のある設置した鏡を見つけて入れる
        GameObject[] mirrors = GameObject.FindGameObjectsWithTag("PlacedMirror");

        //鏡を全部破壊
        foreach (GameObject mirror in mirrors)
        {
            Destroy(mirror);
        }

        //設置可能数を初期化
        canSetMirrorNumber = maxMirrorNumber;
        GameManager.Instance.UpdateSetMirrorNumber(canSetMirrorNumber);

        Debug.Log("すべての鏡を回収しました");
    }
    #endregion
}
