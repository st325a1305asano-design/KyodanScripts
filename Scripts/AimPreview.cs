using System.Collections;
using System.Collections.Generic;
using UniGLTF.Extensions.VRMC_materials_mtoon;
using UnityEngine;

/// <summary>
/// 発射前に弾道を予測するスクリプト
/// </summary>
public class AimPreview : MonoBehaviour
{
    #region 変数の宣言
    //予測線の最大距離
    [SerializeField] float maxDistance = 100f;
    //反射するオブジェクトのレイヤー
    [SerializeField] LayerMask reflectLayer;
    //無限ループ防止用の反射回数上限
    [SerializeField] int safetyLimit = 100;
    //LineRendererを入れる変数line
    private LineRenderer line;
    //線の太さ
    [SerializeField] float lineWidth = 0.05f;
    //線のマテリアルを入れる変数
    [SerializeField] Material lineMaterial;

    Bullet bullet; //Bulletを入れる変数
    #endregion

    void Start()
    {
        //lineにLineRendererを入れる
        line = GetComponent<LineRenderer>();
        //lineがnullならこのスクリプトを無効化して処理を止める
        if (line == null)
        {
            Debug.LogError("LineRendererがアタッチされていません。");
            enabled = false;
            return;
        }
        line.positionCount = 1; // 頂点の数
        line.startWidth = lineWidth; //中心の線の太さ
        line.endWidth = lineWidth; //外側の線の太さ
        line.material = lineMaterial; //線を表示するマテリアルの設定
        line.startColor = Color.yellow; //線の中心の色
        line.endColor = Color.yellow; //線の外側の色

        bullet = GetComponent<Bullet>(); //このオブジェクトについているBulletを入れる
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward); //現在地から前方向に向かってRay（光線）を飛ばす
        DrawReflectionPath(ray.origin, ray.direction); //弾道線を引く処理を呼び出す
    }

    /// <summary>
    /// 弾道予測線を描く
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="direction"></param>
    void DrawReflectionPath(Vector3 startPos, Vector3 direction)
    {
        line.positionCount = 1; //頂点の個数
        line.SetPosition(0, startPos); //始点の位置を設定
        int reflectionCount = 0; //反射回数を記録

        Vector3 currentPos = startPos; //位置の引数をcurrentPosに入れる
        Vector3 currentDir = direction; //方向の引数をcurrentDirに入れる

        //ループ防止上限より反射数が少ない間繰り返す
        while (reflectionCount < safetyLimit)
        {
            //直線を飛ばして、反射対象に当たった場所をhitに入れる
            if (Physics.Raycast(currentPos, currentDir, out RaycastHit hit, maxDistance, reflectLayer))
            {
                //当たったオブジェクトの親からMirrorスクリプトを取得
                Mirror mirror = hit.collider.GetComponentInParent<Mirror>();
                //頂点数を増やす
                line.positionCount++;
                //最後の点に直線が当たった場所を入れ、そこまで線を引く
                line.SetPosition(line.positionCount - 1, hit.point);

                currentDir = mirror.GetRflectVectol(currentDir, hit.normal); //currentDirに反射ベクトルを入れる
                currentPos = hit.point; //currentPosに当たった場所を入れる

                //反射した回数を増やす
                reflectionCount++;
            }
            //なににも当たらなかったら
            else
            {
                //頂点を増やして
                line.positionCount++;
                //最後の点(最大距離)まで線を伸ばす
                line.SetPosition(line.positionCount - 1, currentPos + currentDir * maxDistance);

                break;
            }
        }

        if (reflectionCount >= safetyLimit)
        {
                Debug.Log("上限です");
        }
    }
}
