using UnityEngine;

public struct Vector2i 
{ 
    public static readonly Vector2i Empty = new Vector2i();
    private int x;
    private int y;
 
    public Vector2i(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int X 
    {
        get 
        {
            return x;
        }
        set 
        {
            x = value;
        }
    }
    public int Y 
    {
        get 
        {
            return y;
        }
        set 
        {
            y = value;
        }
    }
  
    public static implicit operator Vector2(Vector2i p) 
    {
        return new Vector2(p.X, p.Y);
    }
 
    public static bool operator ==(Vector2i left, Vector2i right) 
    {
        return left.X == right.X && left.Y == right.Y;
    }
  
    public static bool operator !=(Vector2i left, Vector2i right) 
    {
        return !(left == right);
    }
 
    public override bool Equals(object obj) 
    {
        if (!(obj is Vector2i))
        {
            return false;
        }
        Vector2i comp = (Vector2i)obj;
        return comp.X == this.X && comp.Y == this.Y;
    }
 
    public override int GetHashCode() 
    {
        return x ^ y;
    }
  
    public void Offset(int dx, int dy) 
    {
        X += dx;
        Y += dy;
    }

    public override string ToString() 
    {
        return "{X=" + X.ToString() + ",Y=" + Y.ToString() + "}";
    }
}
