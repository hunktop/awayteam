using System.Collections.Generic;
using System;

class LineOfSight
{
    /// <summary>
    /// Returns the indices of the squares on a grid that intersect a line 
    /// beginning at vector "start" and ending at vector "end".  Right now 
    /// uses Bresenham's algorithm which may be a bad choice.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public IEnumerable<Vector2i> GetIntermediatePoints(Vector2i start, Vector2i end)
    {
        // http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
        var x0 = start.X;
        var x1 = end.X;
        var y0 = start.Y;
        var y1 = end.Y;
        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y0 - y1);
   
        var sx = (x0 < x1)? 1 : -1;
        var sy = (y0 < y1)? 1 : -1;
        var err = dx - dy;

        while (x0 != x1 || y0 != y1)
        {
            yield return new Vector2i(x0, y0);
            var e2 = err * 2;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}