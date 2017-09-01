using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackViewMisc_AttackViewUIElement : AttackViewUIElement
{
    public Button fireButton;
    RectTransform fireButtonTransform;
    public Vector2 retractedFireButtonPosition;
    public Vector2 extendedFireButtonPosition;
    Vector2 fireButtonVelocity;

    public override void Enable ()
    {
        base.Enable();
        fireButton.gameObject.SetActive( true );
        fireButtonTransform = fireButton.GetComponent<RectTransform>();
        fireButtonTransform.anchoredPosition = retractedFireButtonPosition;

        viewedPlayer.board.visualModules[2].Enable();
        viewedPlayer.board.visualModules[1].Enable();
    }

    public override void Disable ()
    {
        if (viewedPlayer != null)
        {
            viewedPlayer.board.visualModules[2].Disable();
        }
        fireButton.gameObject.SetActive( false );
        base.Disable();
    }

    protected override void OnFocusableTap ( Vector2 position )
    {
        base.OnFocusableTap( position );
        if (viewedPlayer.board.GetTileAtWorldPosition( InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y - 0.1f ) ) == null)
        {
            UserInterface.managedBattle.SelectPlayer( null );
        }
    }

    protected override void Update ()
    {
        base.Update();
        if (TargetingUIElement.activeTargetingElements != null)
        {
            fireButton.interactable = TargetingUIElement.activeTargetingElements.Count > 0;
        }
        else
        {
            fireButton.interactable = false;
        }

        fireButtonTransform.anchoredPosition = Vector2.SmoothDamp( fireButtonTransform.anchoredPosition, fireButton.interactable ? extendedFireButtonPosition : retractedFireButtonPosition, ref fireButtonVelocity, 0.3f, Mathf.Infinity, Time.deltaTime );
    }
}
