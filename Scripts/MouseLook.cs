using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 一人称視点カメラ制御
/// </summary>
public class MouseLook : MonoBehaviour
{
    #region 変数の宣言
    [SerializeField] Transform playerBody; //プレイヤーの位置
    [SerializeField] float upperLimitView = 90f; //視点の上限
    [SerializeField] float lowerLimitView = -90f; //視点の下限

    [SerializeField] float xRotation = 0f; //カメラの縦回転角度
    [SerializeField] PlayerInput playerInput; //PlayerInputを入れる変数
    [SerializeField] PlayerController playerController;
    #endregion

    void Start()
    {
        //このスクリプトがアタッチされているオブジェクトについているPlayerInputを取得
        playerInput = GetComponentInParent<PlayerInput>();

        Cursor.lockState = CursorLockMode.Locked; //マウスカーソルを中央に固定
        Cursor.visible = false; //マウスカーソルを非表示

        xRotation = 0f; //初期回転を無しにする
        transform.localRotation = Quaternion.identity; //回転そのまま
    }

    void Update()
    {
        //PlayerInputがnullなら処理を止める
        if (playerInput == null) return;

        //Lookアクションからマウスの移動量を取得
        Vector2 lookInput = playerInput.actions["Look"].ReadValue<Vector2>();

        //1フレームで動く視点の量
        float mouseX = lookInput.x * Settings.Instance.sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * Settings.Instance.sensitivity * Time.deltaTime;

        //カメラ反転オンなら反転
        if (Settings.Instance.invertY)
        {
            mouseY = -mouseY;
        }

        //縦回転
        xRotation -= mouseY; //マウスと視点の動きを連動(下に動かすと視点も下に行く)
        xRotation = Mathf.Clamp(xRotation, lowerLimitView, upperLimitView); //上下の視点移動の上限

        //X軸を回転させる
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //横回転
        playerBody.Rotate(Vector3.up * mouseX); //プレイヤーの体を回す
    }
}
