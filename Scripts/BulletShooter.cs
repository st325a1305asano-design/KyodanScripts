using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 弾の発射と生成を管理するスクリプト
/// </summary>
public class BulletShooter : MonoBehaviour
{
    #region 変数の宣言
    [SerializeField] GameObject bulletPrefab; //弾丸のプレハブを入れる変数
    [SerializeField] Transform firePoint; //弾丸の生成位置
    [SerializeField] int ammo = 3; //残弾数

    PlayerInput playerInput; //PlayerInputを入れる変数
    GameObject currentBullet; //生成した弾丸を入れる変数
    #endregion
    public int Ammo { get; private set; }

    void Start()
    {
        //PlayerInputを入れる
        playerInput = FindAnyObjectByType<PlayerInput>();

        SpawnBullet(); //弾丸を生成する
        GameManager.Instance.UpdateAmmo(ammo); //残弾数表示を更新
    }

    void Update()
    {
        //Fireキーを押されたら
        if (playerInput.actions["Fire"].triggered)
        {
            Fire(); //弾丸を発射
        }
    }

    #region 生成と発射
    /// <summary>
    /// 弾丸の生成をする関数
    /// </summary>
    void SpawnBullet()
    {
        //残弾数が0なら
        if (ammo == 0)
        {
            //ゲームオーバー画面の表示
            GameManager.Instance.ShowGameOverPanel();
            GameManager.Instance.SwitchActionMaps("UI");

            return;
        }

        //弾丸を生成してcurrentBulletに入れる
        currentBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        //弾丸の状態をBulletスクリプトより"停止状態"にする
        currentBullet.GetComponent<Bullet>().Stop();
        //Bulletスクリプトにこのスクリプトを渡す
        currentBullet.GetComponent<Bullet>().SetShooter(this);
        Debug.Log("弾が生成されました");

    }

    /// <summary>
    /// 弾丸の発射をする関数
    /// </summary>
    void Fire()
    {
        //弾丸がない状態なら処理を止める
        if (currentBullet == null) return;

        //弾丸の状態を"発射状態"にする
        currentBullet.GetComponent<Bullet>().Launch();
        //currentBulletの中身をnullにする
        currentBullet = null;

        Debug.Log("弾が発射されました");

        if (ammo > 0)
        {
            //残弾数を減らす
            ammo--;

            //残弾数表示を更新
            GameManager.Instance.UpdateAmmo(ammo);
        }
    }
    #endregion

    /// <summary>
    /// 弾丸が壊れた時に呼び出される関数
    /// </summary>
    public void OnBulletDestroyed()
    {
        //弾丸を生成する
        SpawnBullet();
    }

    /// <summary>
    /// 残弾数を0にする
    /// </summary>
    public void ZeroAmmo()
    {
        ammo = 0;
        SpawnBullet();
        GameManager.Instance.UpdateAmmo(ammo);
    }
}
