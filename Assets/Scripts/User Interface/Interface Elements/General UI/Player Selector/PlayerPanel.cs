using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    /// <summary>
    /// The input field containing the name of the player.
    /// </summary>
    public InputField name;
    /// <summary>
    /// The AI on/off toggle.
    /// </summary>
    public Toggle AI;
    /// <summary>
    /// The slider determining the size of this player's board.
    /// </summary>
    public Slider boardSizeSlider;
    /// <summary>
    /// The text used to display current board size setting.
    /// </summary>
    public Text boardSizeText;
    public RectTransform rectTransform;
    public float targetX;
    Vector2 velocity;

    void Start()
    {
        rectTransform.localScale = Vector3.one;
    }
    void Update()
    {
        rectTransform.anchoredPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition, Vector2.right * targetX, ref velocity, 1f, Mathf.Infinity, Time.deltaTime);
        int boardSide = (int)boardSizeSlider.value;
        boardSizeText.text = boardSide + "x" + boardSide;
    }

    public void Remove()
    {
        ((PlayerSelection_UIElement)UserInterface.elements[2]).RemovePlayerPanel(this);
    }
}
