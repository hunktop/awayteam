using System.Collections.Generic;
using System.Linq;

public enum ButtonStripOrientation 
{
    Horizontal,
    Vertical
}

/// <summary>
/// A simple class for displaying buttons in a horizontal or vertical strip.
/// </summary>
public class ButtonStrip : FContainer
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

    /// <summary>
    /// The pixel width of the control.
    /// </summary>
    public float Width
    {
        get
        {
            float result = 0f;
            
            if(this.Orientation == ButtonStripOrientation.Horizontal)
            {
                // For horizontal orientation, first sum the button widths
                result = this.buttons.Sum(b => b.sprite.width);

                // Then add in the spacing for every button but the last
                if(this.buttons.Count > 0)
                {
                    result += (this.buttons.Count - 1) * this.Spacing;
                }
            }
            else 
            {
                // For vertical orientation, return the width of the widest button
                result = this.buttons.Max(b => b.sprite.width);
            }

            return result;
        }
    }

    /// <summary>
    /// The pixel height of the control.
    /// </summary>
    public float Height
    {
        get
        {
            float result = 0f;

            if (this.Orientation == ButtonStripOrientation.Vertical)
            {
                // For horizontal orientation, first sum the button heights
                result = this.buttons.Sum(b => b.sprite.height);

                // Then add in the spacing for every button but the last
                if (this.buttons.Count > 0)
                {
                    result += (this.buttons.Count - 1) * this.Spacing;
                }
            }
            else
            {
                // For vertical orientation, return the height of the tallest button
                result = this.buttons.Max(b => b.sprite.height);
            }

            return result;
        }
    }

    #endregion

    #region Ctor

    public ButtonStrip()
    {
        this.buttons = new List<FButton>();
    }

    #endregion

    #region Public Methods

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

