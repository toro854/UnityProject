using UnityEngine;
using TMPro;

public class FloatingTextController : MonoBehaviour
{
    public float moveSpeed = 2f;    // 上に浮き上がるスピード
    public float destroyTime = 1f;  // 消えるまでの時間

    [Header("Great時の設定")]
    public float fontSizeBoostGreat = 1.3f;
    public Material matGreat;
    public TMP_ColorGradient gradientGreat;

    [Header("Perfect時の設定")]
    public float fontSizeBoostPerfect = 1.4f; // Greatよりさらに大きく
    public Material matPerfect;
    public TMP_ColorGradient gradientPerfect;

    private Material matNormal;
    private TextMeshPro textMesh;
    private Color textColor;
    private Animator animator;



    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null) matNormal = textMesh.fontMaterial;

        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (textMesh != null) textColor = textMesh.color;
        // 指定時間後に自分自身を削除する
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // ⚠️重要：スローモーション（timeScale = 0.1）を無視するために unscaledDeltaTime を使う！
        transform.position += Vector3.up * moveSpeed * Time.unscaledDeltaTime;

        if (textMesh != null)
        {
            // 時間経過で徐々に透明（Alpha値を減らす）にする
            textColor.a -= (1f / destroyTime) * Time.unscaledDeltaTime;
            textMesh.color = textColor;
        }
    }


    public void Setup(string text, Color color, BallController.HitType hitType)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
            textMesh.color = color;

            // 👇 HitType に応じて 3 段階に分岐させる
            switch (hitType)
            {
                case BallController.HitType.Perfect:
                    textMesh.fontMaterial = matPerfect;
                    textMesh.fontSize *= fontSizeBoostPerfect;

                    textMesh.enableVertexGradient = true;
                    textMesh.colorGradientPreset = gradientPerfect;

                    if (animator != null) animator.Play("FloatingText_PopupPerfect"); // 必要なら別アニメーションに
                    break;

                case BallController.HitType.Great:
                    textMesh.fontMaterial = matGreat;
                    textMesh.fontSize *= fontSizeBoostGreat;

                    textMesh.enableVertexGradient = true;
                    textMesh.colorGradientPreset = gradientGreat;

                    if (animator != null) animator.Play("FloatingText_PopupGreat"); // 必要なら別アニメーションに
                    break;

                default: // Early, Late, Miss など
                    textMesh.fontMaterial = matNormal;

                    textMesh.enableVertexGradient = false;
                    textMesh.colorGradientPreset = null;
                    
                    if (animator != null) animator.Play("FloatingText_PopupNormal");
                    break;
            }
        }
    }
}