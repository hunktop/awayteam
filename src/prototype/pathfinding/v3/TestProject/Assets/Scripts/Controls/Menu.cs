using System.Collections.Generic;
class Menu : FContainer
{
    private string fontName;

    public List<FLabel> Entries
    {
        get;
        set;
    }

    public Menu(string font, List<string> entries)
    {
        this.fontName = font;

    }
}
