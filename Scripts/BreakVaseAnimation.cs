using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 破片が飛び散るスクリプト
/// </summary>
public class BreakVaseAnimation : MonoBehaviour
{
    #region 変数の宣言
    [SerializeField] GameObject Vase_Normal; //通常状態の花瓶を入れる変数
    [SerializeField] GameObject Vase_Break; //壊れた状態の花瓶を入れる変数
    [SerializeField] float explosionForce = 200f; //飛ばす強さ
    [SerializeField] float explosionRadius = 2f; //効果範囲(半径)
    #endregion

    void Start()
    {
        Vase_Normal.SetActive(true); //通常状態の花瓶を表示する
        Vase_Break.SetActive(false); //壊れた状態の花瓶を非表示にする
    }

    #region 爆散処理
    /// <summary>
    /// 破片を飛ばす関数
    /// </summary>

    public void Break()
    {
        Vase_Normal.SetActive(false); //通常状態の花瓶を非表示にする
        Vase_Break.SetActive(true); //壊れた状態の花瓶を表示する

        //壊れた花瓶の子にあるRigidBodyをすべて変数rbに入れる
        foreach (Rigidbody rb in Vase_Break.GetComponentsInChildren<Rigidbody>())
        {
            //現在の座標から半径explosionRadiusの範囲にある変数rbをexplosionForceの強さで吹っ飛ばす
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        StartCoroutine(RemovePiece()); //コルーチンを呼び出す
    }

    /// <summary>
    /// 破片の物理演算を止めるコルーチン
    /// </summary>
    /// <returns></returns>

    IEnumerator RemovePiece()
    {
        //デバッグ用
        Debug.Log("コルーチンが呼ばれました");

        yield return new WaitForSeconds(3f); //3秒待つ

        //壊れた花瓶の子にあるRigidBodyをすべて変数rbに入れる
        foreach (Rigidbody rb in Vase_Break.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true; //物理演算停止
        }
    }
    #endregion
}
