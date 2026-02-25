using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 鏡のプレビュー機能
/// </summary>
public class MirrorPreview : MonoBehaviour
{
    #region 変数の宣言
    [SerializeField] GameObject previewPrefab; //プレビュー用鏡を入れる変数
    [SerializeField] GameObject wallPreviewPrefab; //プレビュー用壁掛け鏡を入れる変数
    [SerializeField] LayerMask groundLayer; //床レイヤー
    [SerializeField] LayerMask wallLayer; //壁レイヤー
    [SerializeField] Vector3 wallMirrorRotation; //壁掛け鏡の角度調整用
    [SerializeField] Vector2 offset; //プレビュー位置調整用

    PlayerInput playerInput; //PlayerInputを入れる変数
    Camera mainCamera; //MainCameraを入れる変数
    GameObject previewObj; //プレビューしている鏡を入れる変数

    [SerializeField] float rotateSpeed = 5f; //回転速度
    [SerializeField] float rayDistance = 50f; //光線距離

    CreateModeManager createModeManager; //CreateModeManagerを入れる変数

    Vector3 lastPos; //プレビューの最終位置を入れる関数
    Quaternion lastRot; //プレビューの最終回転を入れる関数
    bool isWall; //壁レイヤー判定フラグ
    #endregion

    #region ゲッター
    public Vector3 GetPlacePosition() => lastPos;
    public Quaternion GetPlaceRotation() => lastRot;
    public bool IsWall => isWall;
    #endregion

    void Start()
    {
        //このスクリプトがアタッチされているオブジェクトについているPlayerInputを取得
        playerInput = GetComponent<PlayerInput>();
        //mainCameraにMainCamera(一人称視点カメラ)を取得
        mainCamera = Camera.main;
        //CreateModeManagerを取得
        createModeManager = GetComponent<CreateModeManager>();
    }

    void Update()
    {
        //CreateModeじゃないなら処理を止める
        if (!IsCreateMode())
        {
            //プレビューオブジェクトがあるなら破壊
            if (previewObj != null) DestroyPreview();

            return;
        }

        //プレビュー表示をする関数を呼び出す
        UpdatePreviewPosition();
    }

    /// <summary>
    /// CreateModeフラグを返す関数
    /// </summary>
    /// <returns></returns>
    bool IsCreateMode()
    {
        return createModeManager.IsCreateMode;
    }

    #region プレビュー表示
    /// <summary>
    /// プレビュー表示をする関数
    /// </summary>
    void UpdatePreviewPosition()
    {
        //画面中央位置を取得
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        //画面中央から光線を出す
        Ray baseRay = mainCamera.ScreenPointToRay(screenCenter);

        //床＋壁をまとめる
        int placeLayerMask = groundLayer | wallLayer;

        //光線が床または壁レイヤーに当たったなら
        if (Physics.Raycast(baseRay, out RaycastHit hit, rayDistance, placeLayerMask))
        {
            //ビット演算による壁レイヤーの判定
            isWall = ((1 << hit.collider.gameObject.layer) & wallLayer) != 0;

            //地面レイヤー判定ならオフセット
            if (!isWall)
            {
                screenCenter += offset;
            }
        }
        else
        {
            if (previewObj != null) DestroyPreview();
            return;
        }

        //オフセットを考慮した位置から光線を出す
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);

        //光線が床または壁レイヤーに当たったなら
        if(Physics.Raycast(ray, out hit, rayDistance, placeLayerMask))
        {
            lastPos = hit.point; //lastPosに当たった地点を入れる

            //プレビューオブジェクトがあるなら
            if (previewObj != null)
            {
                //プレビューオブジェクトのタグを比較
                bool isWallPreview = previewObj.CompareTag("WallMirrorPreview");

                //壁レイヤーフラグと壁掛け鏡フラグが違うなら
                if (isWall != isWallPreview)
                {
                    //プレビューオブジェクトを破壊
                    DestroyPreview();
                }
            }

            //プレビューオブジェクトがnullなら
            if (previewObj == null)
            {
                //壁フラグに合わせてプレビューする
                previewObj = Instantiate(isWall ? wallPreviewPrefab : previewPrefab);
            }

            //previewObjの座標を当たった座標にする
            previewObj.transform.position = lastPos;

            //壁用は向きを合わせる
            if (isWall)
            {
                previewObj.transform.rotation = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(wallMirrorRotation);
            }
            //地面用なら
            else
            {
                //回転できるようにする
                RotationPreview();
            }

            //lastRotにプレビューの回転値を入れる
            lastRot = previewObj.transform.rotation;
        }
        else
        {
            if (previewObj != null) DestroyPreview();
            return;
        }
    }

    /// <summary>
    /// プレビューの回転関数
    /// </summary>
    void RotationPreview()
    {
        //回転量を取得
        float rotateInput = playerInput.actions["RotateMirror"].ReadValue<float>();

        //Debug.Log(rotateInput);

        //回転量の絶対値が0.01より大きいなら
        if (Mathf.Abs(rotateInput) > 0.01f)
        {
            //previewObjを回転
            previewObj.transform.Rotate(0, rotateInput * rotateSpeed, 0);
        }
    }
    #endregion

    /// <summary>
    /// 破壊処理
    /// </summary>
    void DestroyPreview()
    {
        if (previewObj == null) return;

        Destroy(previewObj);
        previewObj = null;
    }
}
