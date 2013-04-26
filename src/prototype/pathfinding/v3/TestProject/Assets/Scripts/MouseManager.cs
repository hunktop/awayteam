using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MouseButton
{
    Left = 0,
    Right,
    Middle
}

public enum ButtonState 
{
    Pressed,
    Released
}

public struct MouseEvent
{
    public MouseButton MouseButton;
    public Vector2 GlobalCoordinates;
    public Vector2 ScreenCoordinates;

    // Only filled in if the handler is an FNode
    public Vector2 LocalCoordinates;
}

public interface IHandleMouseEvents
{
    void MouseClicked(MouseEvent e);
    void MousePressed(MouseEvent e);
    void MouseReleased(MouseEvent e);
}

public class MouseManager
{
    private class MouseButtonState
    {
        public ButtonState[] Buttons
        {
            get;
            private set;
        }

        public MouseButtonState()
        {
            this.Buttons = new ButtonState[3];
            for(int ii = 0; ii < 3; ii++)
            {
                this.Buttons[ii] = Input.GetMouseButton(ii) ? ButtonState.Pressed : ButtonState.Released;
            }
        }
    }

    private MouseButtonState previousMouseState = null;
    private IHandleMouseEvents mouseHandler;

    public MouseManager(IHandleMouseEvents handler)
    {
        this.mouseHandler = handler; 
    }

    public static Vector2 ScreenToGlobal(Vector2 screenCoord)
    {
        var scale = 1.0f / Futile.displayScale;
        var mouseX = Input.mousePosition.x;
        var mouseY = Input.mousePosition.y;
        var offsetX = -Futile.screen.originX * Futile.screen.pixelWidth;
        var offsetY = -Futile.screen.originY * Futile.screen.pixelHeight;
        var worldX = (mouseX + offsetX) * scale;
        var worldY = (mouseY + offsetY) * scale;
        return new Vector2(worldX, worldY);
    }

    public void Update()
    {
        // Determine the current mouse state. 
        var currentMouseState = new MouseButtonState();

        // Project the screen location of the mouse to world coordinates
        var mouseEvent = new MouseEvent();
        var mousePosition2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        mouseEvent.GlobalCoordinates = ScreenToGlobal(mousePosition2d);
        mouseEvent.ScreenCoordinates = mousePosition2d;

        // If the handler is an FNode, go ahead and fill it its local coordinates
        var fNode = this.mouseHandler as FNode;
        if(fNode != null)
        {
            mouseEvent.LocalCoordinates = fNode.GlobalToLocal(mouseEvent.GlobalCoordinates);
        }

        for(int ii = 0; ii < 3; ii++)
        {
            var curButtonState = currentMouseState.Buttons[ii];

            if (previousMouseState != null)
            {
                var prevButtonState = this.previousMouseState.Buttons[ii];
                mouseEvent.MouseButton = (MouseButton)ii;

                if (curButtonState == ButtonState.Pressed && prevButtonState == ButtonState.Released)
                {
                    this.mouseHandler.MousePressed(mouseEvent);
                }

                if (curButtonState == ButtonState.Released && prevButtonState == ButtonState.Pressed)
                {
                    this.mouseHandler.MouseReleased(mouseEvent);
                    this.mouseHandler.MouseClicked(mouseEvent);
                }
            }
        }

        previousMouseState = currentMouseState;
    }
}
