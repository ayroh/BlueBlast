using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private Camera cam;

    private int inputCount = 0;

    public void SetInputState(bool input)
    {
        if (input)
            inputCount++;
        else
            inputCount--;
    }

    void Update()
    {
        if (inputCount != 0 || GameManager.gameState != GameState.Started)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Index index = GridManager.instance.GetGridIndex(mousePos);

            if (index == Index.Default || !GridManager.instance.IsIndexActive(index))
                return;

            GameManager.instance.OnCellClickedAsync(index);
        }
    }

}
