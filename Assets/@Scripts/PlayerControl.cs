using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    GameManager gameManager;
    PlayerInput input;

    private void Awake()
    {
        input = new PlayerInput();
        input.Player.PointerClick.performed += (context) => DropObject();
        //input.Player.PointerMove.performed += (context) => Debug.Log("Moved");
        this.enabled = false;
    }

    private void OnEnable()
    {
        input.Enable();
    }
    private void OnDisable()
    {
        input.Disable();
    }

    void DropObject()
    {
        Vector2 dropPos = Camera.main.ScreenToWorldPoint( input.Player.PointerMove.ReadValue<Vector2>() );
        gameManager.DropObject(dropPos);
    }
}
