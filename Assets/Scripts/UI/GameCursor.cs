using UnityEngine;
using UnityEngine.UI;

public enum CursorType { Dot, Hand, HandDrag }

/// <summary>The in-game cursor: a UI image on a screen-space canvas that either follows the mouse or sits at the screen center.</summary>
public class GameCursor : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite dot, hand, handDrag;
    [SerializeField] float dotSize = 24, handSize = 48; // In pixels

    CursorType? current; // Cached to avoid redundant sprite changes

    public void SetType(CursorType type)
    {
        if (type == current) return;
        current = type;

        bool isDot = type == CursorType.Dot;
        image.sprite = type switch
        {
            CursorType.Hand => hand,
            CursorType.HandDrag => handDrag,
            _ => dot
        };
        ((RectTransform)transform).sizeDelta = Vector2.one * (isDot ? dotSize : handSize);
    }

    public void FollowMouse() => transform.position = Input.mousePosition;

    public void CenterOnScreen() => transform.position = new Vector2(Screen.width, Screen.height) / 2;
}
