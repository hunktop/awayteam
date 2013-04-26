using System.Collections.Generic;

public enum ButtonStripOrientation 
{
    Horizontal,
    Vertical
}

public class ButtonStrip : FContainer, IHandleMouseEvents
{
    #region Private Fields

    private List<FButton> buttons;
    private ButtonStripOrientation orientation;
    private MouseManager mouseManager;
    private float spacing;

    #endregion

    #region Properties

    public ButtonStripOrientation Orientation
    {
        get
        {
            return this.orientation;
        }
        set
        {
            if (this.orientation != value)
            {
                this.orientation = value;
                this.layout();
            }
        }
    }

    public float Spacing
    {
        get
        {
            return this.spacing;
        }
        set
        {
            if (this.spacing != value)
            {
                this.spacing = value;
                this.layout();
            }
        }
    }

    #endregion

    public ButtonStrip()
    {
        this.buttons = new List<FButton>();
        this.mouseManager = new MouseManager(this);
    }

    public void Update()
    {

    }

    public void AddButton(FButton button)
    {
        this.buttons.Add(button);
        this.AddChild(button);
        this.layout();
    }

    public void RemoveButton(FButton button)
    {
        this.buttons.Remove(button);
        this.RemoveChild(button);
        this.layout();
    }

    #region Mouse Event Handlers

    public void MouseClicked(MouseEvent e)
    {
        throw new System.NotImplementedException();
    }

    public void MousePressed(MouseEvent e)
    {
        throw new System.NotImplementedException();
    }

    public void MouseReleased(MouseEvent e)
    {
        throw new System.NotImplementedException();
    }

    #endregion 

    #region Private Methods

    private void layout()
    {
        float curPosition = 0;
        for (int ii = 0; ii < this.buttons.Count; ii++)
        {
            var button = this.buttons[ii];
            if (this.Orientation == ButtonStripOrientation.Horizontal)
            {
                button.x = curPosition;
                button.y = 0;
                curPosition += button.sprite.width;
            }
            else
            {
                button.x = 0;
                button.y = curPosition;
                curPosition += button.sprite.height;
            }

            curPosition += this.Spacing;
        }
    }

    #endregion
}

