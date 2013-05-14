using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AttackHelper
{        
    public static IEnumerable<Vector2i> GetTargetablePoints(
        Map map, 
        Actor attacker, 
        int minRange, 
        int maxRange, 
        bool targetsEnemies,
        bool targetsAllies)
    {

        var visible = new HashSet<Vector2i>();
        var start = attacker.GridPosition;

        ShadowCaster.ComputeFieldOfViewWithShadowCasting(start.X, start.Y, maxRange,
            (x, y) => 
                map.Contains(x,y) &&
                !(x == start.X && y == start.Y) && 
                (map.ContainsActorAtLocation(x,y) || 
                map[x,y].Properties.BlocksVision),
            (x, y) => visible.Add(new Vector2i(x, y)),
            (x, y, z) => x + y <= z);

        for (int ii = minRange; ii <= maxRange; ii++)
        {
            foreach (var point in GetPointsAtDistance(start, ii))
            {
                if (map.Contains(point) && visible.Contains(point))
                {
                    Actor inhabitant;
                    if (map.TryGetActor(point, out inhabitant) && (inhabitant.Team == attacker.Team))
                    {
                        bool isAlly = inhabitant.Team == attacker.Team;
                        if (isAlly && targetsAllies || !isAlly && targetsEnemies)
                        {
                            yield return point;
                        }
                    }
                    else
                    {
                        yield return point;
                    }
                }
            }
        }
    }

    private static IEnumerable<Vector2i> GetPointsAtDistance(Vector2i start, int dist)
    {
        int x = start.X;
        int y = start.Y;

        Vector2i p1 = new Vector2i(x - dist, y); yield return p1;
        Vector2i p2 = new Vector2i(x + dist, y); yield return p2;
        Vector2i p3 = new Vector2i(x, y - dist); yield return p3;
        Vector2i p4 = new Vector2i(x, y + dist); yield return p4;

        for (int ii = 0; ii < dist - 1; ii++)
        {
            p1.X++; p1.Y--; yield return p1;
            p2.X--; p2.Y++; yield return p2;
            p3.X++; p3.Y++; yield return p3;
            p4.X--; p4.Y--; yield return p4;
        }
    }
}
