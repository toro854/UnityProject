using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BatterController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Animatorコンポーネントを取得
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        // もしマウスのポインターが「UI（ボタンなど）」の上にあったら、
        // このフレームの処理をここで終了（return）して、下のスイング処理を読ませない！
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //左クリック「または (||)」スペースキーが押された瞬間
        if (Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Animatorのトリガーを引く
            animator.SetTrigger("doSwing");

        SoundManager soundManager = FindFirstObjectByType<SoundManager>();
        if (soundManager != null)
        {
            soundManager.PlaySwing();
        }
        }
    }
}