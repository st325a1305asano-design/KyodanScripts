using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// 弾の進行と破壊のスクリプト
/// </summary>
public class Bullet : MonoBehaviour
{
    #region 変数の宣言
    //弾速
    [SerializeField] float speed = 10f;
    //最大射程
    [SerializeField] float maxDistance = 100f;
    //弾の進行方向
    private Vector3 direction;
    //移動した距離
    private float traveledDistance = 0f;

    //弾発射のフラグ
    private bool isLaunched = false; 
    //発射されたが、途中で止まった場合それは isLaunchedだけど移動中ではない状態になる　この状態をどう表すかは要検討

    BulletShooter bulletShooter;
    PlayerInput playerInput;
    #endregion

    /// <summary>
    /// BulletShooterの設定 --> 生成時に呼ばれるという書き方は誤解は招くかも
    /// </summary>
    /// <param name="s"></param>
    public void SetShooter(BulletShooter s)
    {
        bulletShooter = s;
    }

    void Start()
    {    
        direction = transform.forward;
        playerInput = FindAnyObjectByType<PlayerInput>();
    }

    void Update()
    {
        //弾が発射状態じゃない　または　プレイ状態じゃないなら処理を止める
        if (!isLaunched || !GameManager.Instance.IsPlaying()) return;

        MoveBullet();
    }

    #region 弾の状態管理
    /// <summary>
    /// 弾を静止状態にする
    /// </summary>
    public void Stop()
    {
        isLaunched = false; //要検討
    }

    /// <summary>
    /// 弾を発射状態にする
    /// </summary>
    public void Launch()
    {
        isLaunched = true; //要検討
    }
    #endregion

    /// <summary>
    /// BulletShooterに弾がDestroyされたことを通知する
    /// </summary>
    public void NotifyOnBulletDestroyed() 
    {
        //shooterがあるなら
        if (bulletShooter != null)
        {
            bulletShooter.OnBulletDestroyed();
        }
    }

    /// <summary>
    /// 弾を動かす処理
    /// </summary>
    public void MoveBullet()
    {
        //1フレームの移動距離
        float moveDistance = speed * Time.deltaTime;

        //移動した距離が最大距離を超えたら
        if (traveledDistance >= maxDistance)
        {
            //破壊して処理を止める
            Destroy(gameObject);

            //弾の破壊通知を送る
            NotifyOnBulletDestroyed();
            return;
        }

        //オブジェクトの判定を行う光線を作成
        Ray ray = new Ray(transform.position, direction);

        //一定距離まで伸びる光線が何かが当たったなら
        //ifの処理が長くて、読みづらいから、各タグに応じてメソッドを用意してください
        if (Physics.Raycast(ray, out RaycastHit hit, moveDistance))
        {
            //衝突点まで移動
            transform.position = hit.point;

            //ぶつかった相手の親にMirrorのコンポーネントがあるなら取得する
            Mirror mirror = hit.collider.GetComponentInParent<Mirror>();
            if (mirror != null)
            {
                //反射方向をMirrorスクリプトから取得
                direction = mirror.GetReflectedVector(direction, hit.normal);
                transform.forward = direction;
            }
            //当たったオブジェクトのタグが花瓶なら
            else if (hit.collider.gameObject.CompareTag("Vase"))
            {
                Debug.Log("Vaseに触れました");

                //BreakVaseAnimationを取得
                BreakVaseAnimation breakVaseAnimation = hit.collider.gameObject.GetComponentInParent<BreakVaseAnimation>();

                if (breakVaseAnimation != null)
                {
                    breakVaseAnimation.Break();
                }

                //破壊SEを再生
                SEManager.Instance.PlaySE(1); //破壊ＳＥのEnumなり、変数なり用意した方がいいよ

                //チュートリアルの時
                if (SceneManager.GetActiveScene().buildIndex == 2) //出た！マジックナンバー　Enumsでシーン管理した方がいい
                {
                    //ステージ進行
                    TutorialManager.Instance.PlusNumber(); //PlusNumber()じゃなくてNextStage()とかの方がわかりやすいかも
                }
                else
                {
                    //クリア画面表示
                    GameManager.Instance.ShowClearPanel();
                }
            }
            //人なら
            else if (hit.collider.gameObject.CompareTag("Character"))
            {
                //被弾音を再生
                SEManager.Instance.PlaySE(2); //上記と一緒

                //チュートリアルのとき
                if (SceneManager.GetActiveScene().buildIndex == 2) //上記と一緒
                {
                    GameManager.Instance.ShowGameOverPanel();

                    Destroy(gameObject);

                    //弾のDestroy通知を送る
                    NotifyOnBulletDestroyed();
                }
                //ここの説明をもう少し詳しく書いた方がいいかも　
                // 弾が当たったときの処理の流れがわかりにくい
                else if (bulletShooter.Ammo > 0) 
                {
                    //弾数をゼロにする --> ゲームオーバーになるならその処理を直接呼べば？
                    GameManager.Instance.GameOverProcess();
                    //ゲームオーバー時に弾数をゼロにする処理も呼ぶなら、
                    //GameOverProcessの中で中身とＵＩ両方0に設定する

                    //bulletShooter.ZeroAmmo(); //ＺｅｒｏＡｍｍｏのメソッド名は変更した方がいい。わかりにくい。
                }
            }
            else
            {
                //タグがBrokenVaseなら処理を止める
                if (hit.collider.gameObject.CompareTag("BrokenVase")) return;

                //このオブジェクトを壊す
                Destroy(gameObject);

                //弾の破壊通知を送る
                NotifyOnBulletDestroyed();
            }
        }
        //何にも当たらなければ
        else
        {
            //前進
            transform.position += direction * moveDistance;
            traveledDistance += moveDistance;
        }
    }
}
