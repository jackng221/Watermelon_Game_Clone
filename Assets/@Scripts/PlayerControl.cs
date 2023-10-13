using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    GameManager gameManager;
    PlayerInput input;

    Vector2 dropPos;
    [SerializeField] Vector2 topLeftBoundary;
    [SerializeField] Vector2 bottomRightBoundary;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();

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
    private void Update()
    {
        dropPos = Camera.main.ScreenToWorldPoint(input.Player.PointerMove.ReadValue<Vector2>());
        dropPos.x = Mathf.Clamp(dropPos.x, topLeftBoundary.x, bottomRightBoundary.x);
        dropPos.y = Mathf.Clamp(dropPos.y, topLeftBoundary.y, bottomRightBoundary.y);
        gameManager.UpdateCurrentDropSpritePos(dropPos);
    }

    void DropObject()
    {
        gameManager.DropObject(dropPos);
    }
}
