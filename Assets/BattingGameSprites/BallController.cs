using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;














public class BallController : MonoBehaviour
{

    public Sprite hitBallSprite;

    private Sprite originalSprite; // 元の丸い画像を保存しておく用
    private SpriteRenderer spriteRenderer;

    public float speed = 10f;  // Inspectorで調整可能
    private Rigidbody2D rb;

    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 最初に元の画像を覚えておく
        originalSprite = spriteRenderer.sprite;

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D がアタッチされていません！");
        }
        else
        {
            rb.gravityScale = 0;
        }
    }

    void FixedUpdate()
    {
        // 左方向への移動を linearVelocity で制御
        if (rb != null && !hitDetected) rb.linearVelocity = Vector2.left * speed;
    }

    private bool hitDetected = false;



    void Update()
    {
        // まだ打たれていない状態で、ボールが「一定のX座標（画面の左側）」を通り過ぎたら「Miss」と判定する
 
        if (!hitDetected && transform.position.x < -10.0f)
        {
            hitDetected = true; // 何度も処理されないようにフラグを立てる
            Debug.Log("見逃し or 完全な空振り！");



            ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
            if (scoreManager != null)
            {
                // 👇 追加：スコア0、判定Missとして ScoreManager に報告し、コンボを強制リセットさせる！
                scoreManager.AddScoreWithCombo(0, HitType.Miss);

                GameObject batter = GameObject.Find("Batter");
                Vector3 targetPosition;



                if (batter != null)
                {
                    Vector3 batPos = batter.transform.position;
                    targetPosition = new Vector3(batPos.x + 0.9f, batPos.y - 1.7f, transform.position.z);
                }
                else
                {
                    // 万が一見つからなかった時のための保険
                    targetPosition = transform.position;
                }

                scoreManager.ShowFloatingText(targetPosition, "MISS", Color.white, HitType.Miss);
            }

            // ボールを消滅させる
            Destroy(gameObject);
        }
    }



    // ① 判定結果の定義
    public enum HitType
    {
        Miss,
        Late,
        Great,
        Perfect,
        Early
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hitDetected) return;



        if (other.CompareTag("Bat"))
        {
            hitDetected = true; // 衝突後は FixedUpdate で上書きしない

            Animator batterAnim = other.GetComponentInParent<Animator>();
            float progress = batterAnim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;

            // ② 判定メソッドを呼んで結果を受け取る
            HitType hitType = JudgeTiming(progress);

            HitEffectManager effectManager = FindFirstObjectByType<HitEffectManager>();

            Vector3 batPos = other.transform.position;
            Vector3 hitPoint = new Vector3(batPos.x + 0.9f, batPos.y - 1.7f, transform.position.z);


        
            SoundManager soundManager = FindFirstObjectByType<SoundManager>();
            if (soundManager != null)
            {
                soundManager.PlayHitSound(hitType);
            }




            ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
            // ③ 結果に応じて処理を分ける（ここで switch を使う！）
            switch (hitType)
            {
                case HitType.Late:
                    Debug.Log("振り遅れ！");
                    if (effectManager != null) effectManager.PlaySparks(hitPoint, 10);

                    if (effectManager != null) effectManager.TriggerLightHitStop();

                    // 0, 1, 2 のどれかがランダムで選ばれる
                    if (Random.Range(0, 3) == 0)
                    {
                        // 【1/3の確率】0が出た場合（前方向に飛ぶ）
                        Debug.Log("前に飛んだ（ボテボテ）");

                        int baseScore = Random.Range(200, 501);

                        if (scoreManager != null)
                        {
                            int finalScore = scoreManager.AddScoreWithCombo(baseScore, hitType);
                            scoreManager.ShowFloatingText(hitPoint, $"LATE\n+{finalScore}", Color.white, HitType.Late);
                        }



                        // 🚨 注意: ここの数値は「前方向に弱く飛ぶ」ための角度・強さに調整してください
                        ResultPitchRandom(15f, -10f, -30f);
                    }
                    else
                    {
                        int baseScore = Random.Range(100, 201);

                        if (scoreManager != null)
                        {
                            int finalScore = scoreManager.AddScoreWithCombo(baseScore, hitType);
                            scoreManager.ShowFloatingText(hitPoint, $"LATE\n+{finalScore}", Color.white, HitType.Late);
                        }

                        // 【2/3の確率】1か2が出た場合（今まで通り後ろに飛ぶ）
                        Debug.Log("後ろに飛んだ（ファウル）");
                        ResultPitchRandom(15f, 120f, 220f);
                    }
                    break;

                case HitType.Early:
                    Debug.Log("振り早い！");
                    if (effectManager != null) effectManager.PlaySparks(hitPoint, 10);

                    if (effectManager != null) effectManager.TriggerLightHitStop();

                    // 0, 1, 2 のどれかがランダムで選ばれる
                    if (Random.Range(0, 3) == 0)
                    {

                        // 【1/3の確率】前方向に飛ぶ
                        Debug.Log("前に飛んだ（ボテボテ）");

                        int baseScore = Random.Range(200, 501);

                        if (scoreManager != null)
                        {
                            int finalScore = scoreManager.AddScoreWithCombo(baseScore, hitType);
                            scoreManager.ShowFloatingText(hitPoint, $"EARLY\n+{finalScore}", Color.white, HitType.Early);
                        }

                        // 🚨 注意: こちらも「前方向に弱く飛ぶ」数値を設定してください
                        ResultPitchRandom(15f, 60f, 80f);
                    }
                    else
                    {

                        int baseScore = Random.Range(100, 201);

                        if (scoreManager != null)
                        {
                            int finalScore = scoreManager.AddScoreWithCombo(baseScore, hitType);
                            scoreManager.ShowFloatingText(hitPoint, $"EARLY\n+{finalScore}", Color.white, HitType.Early);
                        }

                        // 【2/3の確率】今まで通り後ろに飛ぶ
                        Debug.Log("後ろに飛んだ（ファウル）");
                        ResultPitchRandom(15f, 140f, 200f);
                    }
                    break;

                case HitType.Great:
                    Debug.Log("ジャストミート！");
                    effectManager.PlaySparks(hitPoint, 80);
                    int baseGreatScore = Random.Range(1000, 1401);

                    if (scoreManager != null)
                    {
                        int finalScore = scoreManager.AddScoreWithCombo(baseGreatScore, HitType.Great);
                        
                        scoreManager.ShowFloatingText(hitPoint, $"GREAT!\n+{finalScore}", Color.white, HitType.Great);
                    }

                    StartCoroutine(HitStop(other));
                    break;

                case HitType.Perfect:
                    Debug.Log("超ジャストミート！");
                    effectManager.PlaySparks(hitPoint, 100);
                    int baseSuperScore = Random.Range(1400, 1501);


                    if (scoreManager != null)
                    {
                        int finalScore = scoreManager.AddScoreWithCombo(baseSuperScore, HitType.Perfect);
                        
                        scoreManager.ShowFloatingText(hitPoint, $"PERFECT!\n+{finalScore}", Color.white, HitType.Perfect);
                    }
                    ;


                    StartCoroutine(HitStop2(other));
                    break;

                case HitType.Miss:
                default:
                    Debug.Log("当たり判定外（空振り or 無効）");
                    hitDetected = false; // 無効ならフラグを戻す
                    break;
            }

            // ミス以外ならボールを消す（元の仕様に合わせる場合）
            if (hitDetected)
            {
                Destroy(gameObject, 2f);
            }
        }
    }

    // ④ 判定ロジックのみを独立させる
    HitType JudgeTiming(float progress)
    {
        
        if (progress >= 0.18f && progress < 0.26f) return HitType.Late;
        if (progress >= 0.26f && progress < 0.28f) return HitType.Great;
        if (progress >= 0.28f && progress < 0.30f) return HitType.Perfect;
        if (progress >= 0.30f && progress < 0.32f) return HitType.Great;
        if (progress >= 0.32f && progress < 0.40f) return HitType.Early;

        return HitType.Miss;
    }


    




    IEnumerator HitStop(Collider2D batCollider)
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.enabled = false;

        // ボール自体はピタッと止める
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        //HitEffectManagerを呼んで、カメラ・スロー・フラッシュ・背景を一気に発動！
        HitEffectManager effectManager = FindFirstObjectByType<HitEffectManager>();
        if (effectManager != null) effectManager.HitStopEffect(false);

        // 打撃位置への移動と画像切り替え
        Vector3 batPos = batCollider.transform.position;
        Vector3 hitPoint = new Vector3(batPos.x + 1.65f, batPos.y - 1.6f, transform.position.z);
        transform.position = hitPoint;

        if (hitBallSprite != null)
        {
            spriteRenderer.sprite = hitBallSprite;
        }

        // 現実時間で0.3秒待つ（Great用）
        yield return new WaitForSecondsRealtime(0.3f);

        // 【修正】カメラ・時間・背景をすべて元に戻す！
        if (effectManager != null) effectManager.ReturnNormal();

        // ボールを再び動かす
        rb.bodyType = RigidbodyType2D.Dynamic;
        if (anim != null) anim.enabled = true;
        spriteRenderer.sprite = originalSprite;

        // ボールを飛ばす
        ResultPitchRandom(50f, 17f, 22f);
    }

    IEnumerator HitStop2(Collider2D batCollider)
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        HitEffectManager effectManager = FindFirstObjectByType<HitEffectManager>();
        if (effectManager != null) effectManager.HitStopEffect(true);

        Vector3 batPos = batCollider.transform.position;
        transform.position = new Vector3(batPos.x + 1.65f, batPos.y - 1.6f, transform.position.z);

        if (hitBallSprite != null)
        {
            spriteRenderer.sprite = hitBallSprite;
        }

        // 現実時間で0.5秒待つ（Perfect用は少し長い）
        yield return new WaitForSecondsRealtime(0.5f);

        // 【修正】元に戻す
        if (effectManager != null) effectManager.ReturnNormal();

        rb.bodyType = RigidbodyType2D.Dynamic;
        if (anim != null) anim.enabled = true;
        spriteRenderer.sprite = originalSprite;

        // より強く飛ばす
        ResultPitchRandom(60f, 25f, 25f);
    }











    // ボールを飛ばす処理（linearVelocity 対応）
    void ResultPitchRandom(float power, float minAngle, float maxAngle)
    {
        float angle = Random.Range(minAngle, maxAngle);
        float rad = angle * Mathf.Deg2Rad;

        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        rb.linearVelocity = dir * power;
    }

    // SetSpeed メソッドを追加
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}


