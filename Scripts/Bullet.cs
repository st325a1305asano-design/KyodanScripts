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
    //BulletShooterを入れる変数
    BulletShooter shooter;
    PlayerInput playerInput;
    #endregion

    /// <summary>
    /// 弾生成時に呼ばれる。shooterにBulletShooterを入れる
    /// </summary>
    /// <param name="s"></param>
    public void SetShooter(BulletShooter s)
    {
        shooter = s;
    }

    void Start()
    {
        //進行方向の設定
        direction = transform.forward;

        playerInput = FindAnyObjectByType<PlayerInput>();
    }

    void Update()
    {
        //弾が発射状態じゃない　または　プレイ状態じゃないなら処理を止める
        if (!isLaunched || !GameManager.Instance.IsPlaying()) return;

        //弾丸の進行の関数を呼び出す
        MoveBullet();
    }

    #region 弾の状態管理
    /// <summary>
    /// 弾を静止状態にする
    /// </summary>
    public void Stop()
    {
        isLaunched = false;
    }

    /// <summary>
    /// 弾を発射状態にする
    /// </summary>
    public void Launch()
    {
        isLaunched = true;
    }
    #endregion

    /// <summary>
    /// BulletShooterに破壊を通知する
    /// </summary>
    public void NoticeBreak()
    {
        //shooterがあるなら
        if (shooter != null)
        {
            //shooterのOnBulletDestroyedを呼び出す
            shooter.OnBulletDestroyed();
        }
    }

    /// <summary>
    /// 弾丸の進行関数
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
            NoticeBreak();
            return;
        }

        //オブジェクトの判定を行う光線を作成
        Ray ray = new Ray(transform.position, direction);

        //一定距離まで伸びる光線が何かが当たったなら
        if (Physics.Raycast(ray, out RaycastHit hit, moveDistance))
        {
            //衝突点まで移動
            transform.position = hit.point;

            //ぶつかった相手の親にMirrorのコンポーネントがあるならmirrorに入れる
            Mirror mirror = hit.collider.GetComponentInParent<Mirror>();
            if (mirror != null)
            {
                //反射方向をMirrorスクリプトから取得
                direction = mirror.GetRflectVectol(direction, hit.normal);
                transform.forward = direction;
            }
            //当たったオブジェクトのタグが花瓶なら
            else if (hit.collider.gameObject.CompareTag("Vase"))
            {
                Debug.Log("Vaseに触れました");
                //BreakVaseAnimationを取得
                BreakVaseAnimation breakVaseAnimation = hit.collider.gameObject.GetComponentInParent<BreakVaseAnimation>();
                //breakVaseAnimationがnullじゃないなら
                if (breakVaseAnimation != null)
                {
                    //breakVaseAnimationの中のBreak関数を呼び出す
                    breakVaseAnimation.Break();
                }

                //破壊SEを再生
                SEManager.Instance.PlaySE(1);

                //チュートリアルの時
                if (SceneManager.GetActiveScene().buildIndex == 2)
                {
                    //ステージ進行
                    TutorialManager.Instance.PlusNumber();
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
                SEManager.Instance.PlaySE(2);

                //チュートリアルのとき
                if (SceneManager.GetActiveScene().buildIndex == 2)
                {
                    //ゲームオーバー画面表示
                    GameManager.Instance.ShowGameOverPanel();

                    //このオブジェクトを壊す
                    Destroy(gameObject);

                    //弾の破壊通知を送る
                    NoticeBreak();
                }
                else if (shooter.Ammo > 0)
                {
                    //弾数をゼロにする
                    shooter.ZeroAmmo();
                }
            }
            else
            {
                //タグがBrokenVaseなら処理を止める
                if (hit.collider.gameObject.CompareTag("BrokenVase")) return;

                //このオブジェクトを壊す
                Destroy(gameObject);

                //弾の破壊通知を送る
                NoticeBreak();
            }
        }
        //何にも当たらなけらば
        else
        {
            //前進
            transform.position += direction * moveDistance;
            traveledDistance += moveDistance;
        }
    }
}
