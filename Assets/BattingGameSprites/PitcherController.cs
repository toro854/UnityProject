using UnityEngine;
using System.Collections;
using TMPro;

public class PitcherController : MonoBehaviour
{
    private Animator animator;

    [Header("UI設定")]
    public TextMeshProUGUI readyText;

    [Header("設定")]
    public GameObject ballPrefab;    // Ball プレハブ

    // リリースポイント（出現位置）を2つ用意する
    public Transform spawnPointFrame3; // 早め（3枚目）用の位置
    public Transform spawnPointFrame5; // 遅め（5枚目）用の位置


    // 自動投球用の設定（インスペクターで調整可能）
    [Header("自動投球の設定")]
    public bool isAutoPitching = true;    // 自動で投げるかどうかのスイッチ
    public float minPitchInterval = 2.0f; // 投げるまでの最短時間（秒）
    public float maxPitchInterval = 4.0f; // 投げるまでの最長時間（秒）

    // 投げる球数と、今のカウント
    public int maxPitches = 10;
    private int throwCount = 0;


    private float nextBallSpeed;     // 投球速度

    private enum ReleaseTiming { Frame3, Frame5 }
    private ReleaseTiming currentTiming;

    void Start()
    {     
            // 👇いきなり AutoPitchRoutine を呼ぶのではなく、新しい開始用コルーチンを呼ぶように変更！
            StartCoroutine(GameStartAndPitchRoutine());
        }


        // ゲーム開始の演出をしてから、投球ループに繋ぐ新しいコルーチン
        private IEnumerator GameStartAndPitchRoutine()
        {
            // もしテキストが設定されていたら、READY演出を行う
            if (readyText != null)
            {
                readyText.gameObject.SetActive(true); // 文字を表示
                readyText.text = "READY...";

                // 1.0秒待つ（プレイヤーの心の準備時間）
                yield return new WaitForSeconds(1.0f);

                readyText.text = "PLAY !!"; // 文字を切り替え

                // 0.5秒だけ PLAY!! を見せる
                yield return new WaitForSeconds(0.5f);

                readyText.gameObject.SetActive(false); // 文字を消す
            }


        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator がアタッチされていません！");
        }
        if (isAutoPitching)
        {
            StartCoroutine(AutoPitchRoutine());
        }
    }

   


    // 自動で投げ続ける処理（コルーチン）
    private IEnumerator AutoPitchRoutine()
    {
        // isAutoPitching が true の間、ずっと繰り返し続ける
        while (isAutoPitching && throwCount < maxPitches)
        {
            // 最短時間〜最長時間のあいだで、ランダムな待機時間を決める
            float waitTime = Random.Range(minPitchInterval, maxPitchInterval);

            // 決めた時間だけ待つ
            yield return new WaitForSeconds(waitTime);

            // 待った後に投球処理を呼ぶ
            DecidePitch();

            // 投げたらカウントを1増やす
            throwCount++;
        }
        Debug.Log("規定の球数を投げ終えました！");
    }




    void DecidePitch()
    {
        // --- 1. 球のスピードをランダムに決定 ---
        int speedType = Random.Range(0, 5);

        switch (speedType)
        {
           
            case 0:
                nextBallSpeed = 12f;
                Debug.Log("12fの球");
                break;
            case 1:
                nextBallSpeed = 18f;
                Debug.Log("18fの球");
                break;
            case 2:
                nextBallSpeed = 20f;
                Debug.Log("20fの球");
                break;
            case 3:
                nextBallSpeed = 26f;
                Debug.Log("26fの球");
                break;
            case 4:
                nextBallSpeed = 28f;
                Debug.Log("28fの球");
                break;
           ;

        }
        Debug.Log($"球の速度: {nextBallSpeed}");

        // --- 2. アニメーションを確率で決定 ---
        int randomValue = Random.Range(0, 101); // 0〜100
        if (randomValue <= 70)
        {
            if (animator != null) animator.SetTrigger("doNormalPitch");
            Debug.Log("アニメーション: ノーマル");
        }
        else
        {
            if (animator != null) animator.SetTrigger("doFastPitch");
            Debug.Log("アニメーション: クイック");
        }
        int timingValue = Random.Range(0, 100);
        if (timingValue <= 20)
        {
            currentTiming = ReleaseTiming.Frame3; // 3枚目でのリリースを予約
            Debug.Log("リリース: 3枚目（早め）");
        }
        else
        {
            currentTiming = ReleaseTiming.Frame5; // 5枚目でのリリースを予約
            Debug.Log("リリース: 5枚目（遅め）");
        }
    }


    // --- 👇 アニメーションの【3枚目】のイベントから呼ばれる ---
    public void CreateBallAtFrame3()
    {
        if (currentTiming == ReleaseTiming.Frame3)
        {
            PerformThrow(spawnPointFrame3);
        }
    }

    // --- 👇 アニメーションの【5枚目】のイベントから呼ばれる ---
    public void CreateBallAtFrame5()
    {
        if (currentTiming == ReleaseTiming.Frame5)
        {
            PerformThrow(spawnPointFrame5);
        }
    }


    // Animation Event で呼ばれる
    // ボールを生成する共通処理
    private void PerformThrow(Transform currentSpawnPoint)
    {
        // プレハブか位置が設定されていなければエラーを出して止める
        if (ballPrefab == null || currentSpawnPoint == null)
        {
            Debug.LogError("プレハブかSpawnPointが設定されていません！");
            return;
        }

        // currentSpawnPoint.position を使ってボールを生成
        GameObject ball = Instantiate(ballPrefab, currentSpawnPoint.position, Quaternion.identity);
        BallController bc = ball.GetComponent<BallController>();

        if (bc != null) bc.SetSpeed(nextBallSpeed);

        Debug.Log($"ボール生成！ 位置: {currentSpawnPoint.name}");
    }
}

