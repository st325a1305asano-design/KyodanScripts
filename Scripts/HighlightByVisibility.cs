using UnityEngine;

/// <summary>
/// 花瓶の強調表示管理
/// </summary>
public class HighlightByVisibility : MonoBehaviour
{
    [SerializeField] private Camera fpsCamera; //一人称カメラ
    [SerializeField] private LayerMask wallLayer; //壁レイヤー
    [SerializeField] private Material normalMat; //通常マテリアル
    [SerializeField] private Material highlightMat; //強調表示マテリアル

    private Renderer rend; //Rendererを入れる変数

    void Awake()
    {
        rend = GetComponent<Renderer>();
        rend.material = highlightMat; // 最初は強調
    }

    void Update()
    {
        if (IsDirectlyVisible())
        {
            Debug.Log("見えてる");
            rend.material = normalMat;
        }
        else
        {
            Debug.Log("見えてない");
            rend.material = highlightMat;
        }
    }

    /// <summary>
    /// カメラに直接見えているか判定する関数
    /// </summary>
    /// <returns></returns>
    bool IsDirectlyVisible()
    {
        Vector3 from = fpsCamera.ViewportToWorldPoint(
            new Vector3(0.5f, 0.5f, fpsCamera.nearClipPlane)
        );

        Vector3 to = rend.bounds.center;
        Vector3 dir = to - from;
        float dist = dir.magnitude;

        LayerMask mask = wallLayer | (1 << gameObject.layer);

        Debug.DrawRay(from, dir, Color.red);

        if (Physics.Raycast(from, dir.normalized, out RaycastHit hit, dist, mask))
        {
            Debug.Log("hit object : " + hit.collider.gameObject.name);
            Debug.Log("this gameobject:" + gameObject.name);

            return hit.collider.gameObject == gameObject.transform.parent.transform.gameObject;
        }

        return false;
    }

}
