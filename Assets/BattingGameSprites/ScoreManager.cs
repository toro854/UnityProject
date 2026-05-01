
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [Header("UI設定")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    public TextMeshProUGUI ballCountText;
    public GameObject gameOverPanel;

    public GameObject pauseButton;

    public GameObject finishTextObject;

    [Header("フローティングテキスト設定")]
    public GameObject floatingTextPrefab;

    public float textDelay = 0.1f;

    //ゲーム進行用の変数
    [Header("ゲーム設定")]
    public int maxBalls = 10;
    private int processedBalls = 0; // 結果が出た球数
    private bool isGameOver = false;


    [Header("コンボ設定")]
    public float maxMultiplier = 2.0f;   // 倍率の最大値（2倍）
    public float minMultiplier = 1.0f;   // 倍率の最小値（1倍）



    // リザルト画面用のUIを登録する枠
    [Header("リザルト画面UI")]
    public TextMeshProUGUI resultScoreText;
    public TextMeshProUGUI resultMaxComboText;
    public TextMeshProUGUI resultPerfectText;
    public TextMeshProUGUI resultGreatText;
    public TextMeshProUGUI resultEarlyText;
    public TextMeshProUGUI resultLateText;
    public TextMeshProUGUI resultMissText;

    [Header("リザルト画面ボタン")]
    public GameObject retryButton;
    public GameObject rankingButton;
    public GameObject quitButton;

    // 回数をカウントするための変数
    private int perfectCount = 0;
    private int greatCount = 0;
    private int earlyCount = 0;
    private int lateCount = 0;
    private int missCount = 0;
    private int maxComboRecord = 0; // 記録した最大コンボ





    private int currentCombo = 0;        // 現在のコンボ数
    private float currentMultiplier = 1.0f; // 現在の倍率

    private BallController.HitType lastHitType = BallController.HitType.Miss;

    private int currentScore = 0;

    void Start()
    {
        UpdateScoreDisplay();
        UpdateComboDisplay();
        UpdateBallCountDisplay();
    }

    // 他のスクリプト（ボールなど）から「点数を足す時」に呼ばれるメソッド
    public int AddScoreWithCombo(int baseScore, BallController.HitType hitType)
    {

        if (isGameOver) return 0; // ゲームオーバー後はスコアを加算しない

        if (hitType == BallController.HitType.Perfect) perfectCount++;
        else if (hitType == BallController.HitType.Great) greatCount++;
        else if (hitType == BallController.HitType.Early) earlyCount++;
        else if (hitType == BallController.HitType.Late) lateCount++;
        else if (hitType == BallController.HitType.Miss) missCount++;


        // ① コンボの判定と倍率の計算
        if (hitType == BallController.HitType.Perfect || hitType == BallController.HitType.Great)
        {
            currentCombo++; // コンボ増加

            // 👇最大コンボの記録を更新する
            if (currentCombo > maxComboRecord)
            {
                maxComboRecord = currentCombo;
            }

            // 2コンボ目以降（前回のヒットがある場合）に倍率を変動させる
            if (currentCombo > 1)
            {
                if (lastHitType == BallController.HitType.Perfect && hitType == BallController.HitType.Perfect)
                {
                    currentMultiplier += 0.20f; // P -> P
                }
                else if (lastHitType == BallController.HitType.Great && hitType == BallController.HitType.Perfect)
                {
                    currentMultiplier += 0.10f; // G -> P
                }
                else if (lastHitType == BallController.HitType.Great && hitType == BallController.HitType.Great)
                {
                    currentMultiplier += 0.05f; // G -> G
                }
                else if (lastHitType == BallController.HitType.Perfect && hitType == BallController.HitType.Great)
                {
                    currentMultiplier += 0.07f; // P -> G
                }
            }

            // 👇 倍率が 1.0 ～ 2.0 の範囲をはみ出さないように制限する（Mathf.Clamp）
            currentMultiplier = Mathf.Clamp(currentMultiplier, minMultiplier, maxMultiplier);
        }
        else
        {
            // Early, Late, Miss の場合はコンボと倍率を初期化
            currentCombo = 0;
            currentMultiplier = 1.0f;
        }

        // ② 今回の判定を「前回の判定」として記憶しておく（次回の計算用）
        lastHitType = hitType;

        // ③ 最終スコアの計算（小数を四捨五入して整数にする）
        int finalScore = Mathf.RoundToInt(baseScore * currentMultiplier);
        currentScore += finalScore;

        // ④ UIの更新
        UpdateScoreDisplay();
        UpdateComboDisplay();

        //結果が出た球数をカウントアップする
        processedBalls++;
        UpdateBallCountDisplay();

        //もし処理した球数が最大数に達したら、終了処理を呼ぶ
        if (processedBalls >= maxBalls)
        {
            StartCoroutine(GameOverRoutine());
        }


        // ⑤ 最終スコアを返す
        return finalScore;
    }



    //球数表示を更新する
    private void UpdateBallCountDisplay()
    {
        if (ballCountText != null)
        {
            // 例: 「1 / 10」のように表示
            ballCountText.text = $"{processedBalls} / {maxBalls}";
        }
    }

    //追加：ゲームオーバー時の演出（少し待ってから画面を出す）
    private IEnumerator GameOverRoutine()
    {
        isGameOver = true;

        // 最後の1球のヒットストップ演出や、文字が消えるのを少し待つ（1.5秒）
        yield return new WaitForSecondsRealtime(1.5f);

        if (finishTextObject != null)
        {
            finishTextObject.SetActive(true);
        }

        yield return new WaitForSecondsRealtime(2.5f);

        if (finishTextObject != null)
        {
            finishTextObject.SetActive(false);
        }

        if (resultPerfectText != null) { resultPerfectText.text = "Perfect : " + perfectCount; resultPerfectText.gameObject.SetActive(false); }
        if (resultGreatText != null) { resultGreatText.text = "Great : " + greatCount; resultGreatText.gameObject.SetActive(false); }
        if (resultEarlyText != null) { resultEarlyText.text = "Early : " + earlyCount; resultEarlyText.gameObject.SetActive(false); }
        if (resultLateText != null) { resultLateText.text = "Late : " + lateCount; resultLateText.gameObject.SetActive(false); }
        if (resultMissText != null) { resultMissText.text = "Miss : " + missCount; resultMissText.gameObject.SetActive(false); }

        if (resultMaxComboText != null) { resultMaxComboText.text = "Max Combo : " + maxComboRecord; resultMaxComboText.gameObject.SetActive(false); }
        if (resultScoreText != null) { resultScoreText.text = "Total Score : " + currentScore; resultScoreText.gameObject.SetActive(false); }


        // パネルを出す前に、ボタン類もすべて非表示（隠す）にしておく！
        if (retryButton != null) retryButton.SetActive(false);
        if (rankingButton != null) rankingButton.SetActive(false);
        if (quitButton != null) quitButton.SetActive(false);


        // 背景の暗いパネルだけを先に出す
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);

        // パネルが出てから少しだけ「タメ」を作る
        yield return new WaitForSecondsRealtime(0.5f);

        // ----------------------------------------------------
        // 順番に表示していく！（0.4秒間隔）
        // ----------------------------------------------------
        if (resultMissText != null) { resultMissText.gameObject.SetActive(true); yield return new WaitForSecondsRealtime(0.4f); }
        if (resultLateText != null) { resultLateText.gameObject.SetActive(true); yield return new WaitForSecondsRealtime(0.4f); }
        if (resultEarlyText != null) { resultEarlyText.gameObject.SetActive(true); yield return new WaitForSecondsRealtime(0.4f); }
        if (resultGreatText != null) { resultGreatText.gameObject.SetActive(true); yield return new WaitForSecondsRealtime(0.4f); }
        if (resultPerfectText != null) { resultPerfectText.gameObject.SetActive(true); yield return new WaitForSecondsRealtime(0.4f); }

        if (resultMaxComboText != null) { resultMaxComboText.gameObject.SetActive(true); yield return new WaitForSecondsRealtime(0.6f); }

        // 少し長めに「タメ」を作る
        yield return new WaitForSecondsRealtime(0.4f);

        // 最後にトータルスコアをドーン！
        if (resultScoreText != null) { resultScoreText.gameObject.SetActive(true); }


        // スコアが出た後に少しだけタメて、ボタンを一気に表示する！
        yield return new WaitForSecondsRealtime(1.0f);

        if (retryButton != null) retryButton.SetActive(true);
        if (rankingButton != null) rankingButton.SetActive(true);
        if (quitButton != null) quitButton.SetActive(true);

        // ----------------------------------------------------
        // すべて表示し終わったらサーバーにスコア送信
        // ----------------------------------------------------
        LootLockerManager lootLocker = FindFirstObjectByType<LootLockerManager>();
        if (lootLocker != null)
        {
            lootLocker.SubmitHighScore(currentScore);
        }
    }

    //リトライボタンから呼ばれるメソッド
    public void RetryGame()
    {
        // 今のシーンを最初から読み込み直す（完全リセット）
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Quit（タイトルに戻る）ボタンから呼ばれるメソッド
    public void ReturnToTitle()
    {
        // ※ "TitleScene" の部分は、ご自身のタイトル画面のシーン名と完全に一致させてください
        SceneManager.LoadScene("TitleScene");
    }



    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + currentScore.ToString("D5");
        }
    }

    private void UpdateComboDisplay()
    {
        if (comboText != null)
        {
            if (currentCombo > 1)
            {
                // 👇 変更：TextMeshProのリッチテキスト機能（<size>タグ）を使って、
                // 「5 COMBO!」の下に、少し小さい文字で「x1.87」のように表示する！
                // "F2" は「小数点以下2桁まで必ず表示する（例: 2.00）」という命令です。
                comboText.text = $"{currentCombo} COMBO!\n<size=60%>x{currentMultiplier:F2}</size>";
            }
            else
            {
                comboText.text = "";
            }
        }
    }









    public void ShowFloatingText(Vector3 position, string message, Color color, BallController.HitType hitType)

    {
        StartCoroutine(ShowTextWithDelay(position, message, color, hitType));
    }


    private IEnumerator ShowTextWithDelay(Vector3 position, string message, Color color, BallController.HitType hitType)
    {
        // ⚠️ 超重要：ヒットストップ中（スロー）でも、現実の時間でカウントして待つ！
        if (textDelay > 0)
        {
            yield return new WaitForSecondsRealtime(textDelay);
        }

        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos;

            // 👇 HitType によって「文字を出す場所」を分ける！
            if (hitType == BallController.HitType.Perfect || hitType == BallController.HitType.Great)
            {
                // 【Great と Perfect の場合】画面の真ん中に表示する

                // Camera.main.transform.position でカメラの現在位置（画面中央）を取得
                Vector3 screenCenter = Camera.main.transform.position;

                // Z座標（奥行き）を0にしないと、カメラと同じ位置になり文字が見えなくなります
                screenCenter.z = 0f;

                // 画面中央から少し上（Yに+2fなど）にズラす
                spawnPos = screenCenter + new Vector3(6f, 2f, 0);
            }
            else
            {
                // 【それ以外（Early, Late）の場合】今まで通りバットの打点に表示する
                spawnPos = position + new Vector3(-1f, 5f, 0);
            }


            // 計算した spawnPos の位置にプレハブを生成
            GameObject popup = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            FloatingTextController ftc = popup.GetComponent<FloatingTextController>();

            if (ftc != null)
            {
                ftc.Setup(message, color, hitType);
            }
        }
    }
}