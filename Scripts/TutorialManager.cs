using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// チュートリアル進行
/// </summary>
public class TutorialManager : MonoBehaviour
{
    #region 変数の宣言
    [Header("各ステージに昇べきに対応")]
    [SerializeField] List<Transform> playerSpawn;
    [SerializeField] List<Transform> bulletSpwan;
    [SerializeField] List<Transform> camPos;
    [SerializeField] List<GameObject> panel;

    [Header("移動物")]
    [SerializeField] Camera topView;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject player;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject gameOverRetry;

    [SerializeField] int wait = 3;

    //PlayerInputを入れる変数
    PlayerInput playerInput;

    //ステージ番号
    int index = 0;
    #endregion

    //インスタンス化する変数
    public static TutorialManager Instance { get; private set; }

    private void Awake()
    {
        //インスタンス化
        Instance = this;

        UpdateStage();
    }

    void Start()
    {
        //プレイヤーについているPlayerInputを取得
        playerInput = player.GetComponent<PlayerInput>();

        //各パネル非表示
        foreach(GameObject go in panel)
        {
            go.SetActive(false);
        }
    }

    /// <summary>
    /// ステージ番号更新
    /// </summary>
    public void PlusNumber()
    {
        panel[index].SetActive(false);

        index++;
        UpdateStage();
    }

    /// <summary>
    /// ステージ更新関数
    /// </summary>
    public void UpdateStage()
    {
        Debug.Log("呼ばれました");

        //ステージ番号がリストの要素数以下なら
        if (index < camPos.Count)
        {
            MovePos();

            StartCoroutine(MovePlayerPos());
            StartCoroutine(ShowSubject());
        }
        else
        {
            GameManager.Instance.ShowClearPanel();
        }
    }

    /// <summary>
    /// プレイヤーのステージ移動
    /// </summary>
    /// <returns></returns>
    IEnumerator MovePlayerPos()
    {  
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = playerSpawn[index].position;

        yield return new WaitForSeconds(1f);

        player.GetComponent<CharacterController>().enabled = true;
    }

    /// <summary>
    /// ステージ目標表示関数
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowSubject()
    {
        //パネルの表示非表示
        panel[index].SetActive (true);
        yield return new WaitForSeconds (wait);
        panel[index].SetActive(false);
    }

    /// <summary>
    /// ステージに対応した場所に移動
    /// </summary>
    void MovePos()
    {
        topView.transform.position = camPos[index].position;
        bullet.transform.position = bulletSpwan[index].position;
        bullet.transform.rotation = bulletSpwan[index].rotation;
    }

    /// <summary>
    /// ゲームオーバー画面を表示する関数
    /// </summary>
    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        GameManager.Instance.SwitchActionMaps("UI");
        GameManager.Instance.SetCurrentButton(gameOverRetry);
    }
}
