using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 破片が飛び散るスクリプト
/// </summary>
public class BreakVaseAnimation : MonoBehaviour
{
    #region 変数の宣言
    [SerializeField] GameObject vaseNormal; //通常状態の花瓶を入れる変数 //camelCaseに統一した方がいい
                           //normalVase, brokenVase の方が英語的に正しい
    [SerializeField] GameObject vaseBroken; //壊れた状態の花瓶を入れる変数
    [SerializeField] float explosionForce = 200f; 
    [SerializeField] float explosionRadius = 2f; 
    #endregion

    void Start()
    {
        Initialize();  
    }

    /// <summary>
    /// 初期化
    /// </summary>
    void Initialize()
    {
        //初期化
        vaseNormal.SetActive(true); //通常状態の花瓶を表示する
        vaseBroken.SetActive(false); //壊れた状態の花瓶を非表示にする
    }

    #region 爆散処理
    /// <summary>
    /// 破片を飛ばす関数
    /// </summary>

    public void Break()
    {
        vaseNormal.SetActive(false); //通常状態の花瓶を非表示にする
        vaseBroken.SetActive(true); //壊れた状態の花瓶を表示する

        foreach (Rigidbody rb in vaseBroken.GetComponentsInChildren<Rigidbody>())
        {
            //現在の座標から半径explosionRadiusの範囲にある変数rbをexplosionForceの強さで吹っ飛ばす
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        StartCoroutine(RemovePiece()); //破片の物理演算を止めるコルーチンを呼び出す
    }

    /// <summary>
    /// 破片の物理演算を止めるコルーチン
    /// </summary>
    /// <returns></returns>

    IEnumerator RemovePiece()
    {

        #if UNITY_EDITOR
        //デバッグ用
        Debug.Log("RemovePieceコルーチンが呼ばれました");
        #endif

        yield return new WaitForSeconds(3f); //3秒待つ

        foreach (Rigidbody rb in vaseBroken.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true; //物理演算停止
        }
    }
    #endregion
}
