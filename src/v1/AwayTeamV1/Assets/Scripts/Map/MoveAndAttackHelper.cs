//using System;
//using System.Collections.Generic;
//using System.Linq; 

//public class MoveAndAttackHelper
//{
//    #region Public Methods

//    public static PathfindResult Pathfind(Map map, Actor actor)
//    {
//        var start = actor.GridPosition;
//        var result = new PathfindResult(start);
//        var frontier = new PriorityQueue<Vector2i>();
//        //var potentialAttackablePoints = new HashSet<Vector2i>();

//        result.Distance.Add(new Vector2i(start.X, start.Y), 0);
//        frontier.Insert(start, 0);

//        while (frontier.Count > 0)
//        {
//            var min = frontier.ExtractMin();

//            //foreach (var point in GetAttackablePoints(map, min.Key, 1, 2))
//            //{
//            //    potentialAttackablePoints.Add(point);
//            //}

//            foreach (var adj in GetAdjacentCoordinates(map, min.Key))
//            {
//                Actor tempActor;
//                if (!map.TryGetActor(adj, out tempActor))
//                {
//                    var tile = map[adj];
//                    var cost = tile.Properties.MovementPenalty;
//                    var cur = result.Distance.ContainsKey(adj) ? result.Distance[adj] : int.MaxValue;
//                    var alt = min.Value + cost;

//                    if (alt < cur && alt <= actor.MovementPoints)
//                    {
//                        AddOrUpdate(result.Distance, adj, alt);
//                        AddOrUpdate(result.Previous, adj, min.Key);
//                        if (frontier.ContainsKey(adj))
//                        {
//                            frontier.DecreaseKey(adj, alt);
//                        }
//                        else
//                        {
//                            frontier.Insert(adj, alt);
//                        }
//                    }
//                }
//            }
//        }

//        //var attackablePoints = potentialAttackablePoints.Except(result.VisitablePoints);
//        //foreach (var point in attackablePoints)
//        //{
//        //    result.AttackablePoints.Add(point);
//        //}

//        return result;
//    }

//    public static IEnumerable<Vector2i> GetAttackablePoints(Map map, Vector2i start, int minRange, int maxRange)
//    {

//        HashSet<Vector2i> visible = new HashSet<Vector2i>();
//        ShadowCaster.ComputeFieldOfViewWithShadowCasting(start.X, start.Y, maxRange,
//            (x, y) => !(x == start.X && y == start.Y) && map.ContainsActorAtLocation(new Vector2i(x, y)),
//            (x, y) => visible.Add(new Vector2i(x, y)),
//            (x, y, z) => x + y <= z);

//        for (int ii = minRange; ii <= maxRange; ii++)
//        {
//            foreach (var point in GetPointsAtDistance(start, ii))
//            {
//                if (map.Contains(point) && visible.Contains(point))
//                {
//                    Actor inhabitant;
//                    if (map.TryGetActor(point, out inhabitant) && !inhabitant.IsComputer)
//                    {
//                        continue;
//                    }

//                    yield return point;
//                }
//            }
//        }
//    }

//    #endregion

//    #region Private Methods

//    private static void AddOrUpdate<K,V>(IDictionary<K,V> dict, K key, V value)
//    {
//        if (dict.ContainsKey(key))
//        {
//            dict[key] = value;
//        }
//        else
//        {
//            dict.Add(key, value);
//        }
//    }

//    private static IEnumerable<Vector2i> GetAdjacentCoordinates(Map map, Vector2i coord)
//    {
//        var x = coord.X;
//        var y = coord.Y;

//        if (x - 1 >= 0) yield return new Vector2i(x - 1, y);
//        if (x + 1 < map.Columns) yield return new Vector2i(x + 1, y);
//        if (y - 1 >= 0) yield return new Vector2i(x, y - 1);
//        if (y + 1 < map.Rows) yield return new Vector2i(x, y + 1);
//    }

//    private static IEnumerable<Vector2i> GetPointsAtDistance(Vector2i start, int dist)
//    {
//        int x = start.X;
//        int y = start.Y;

//        Vector2i p1 = new Vector2i(x - dist, y); yield return p1;
//        Vector2i p2 = new Vector2i(x + dist, y); yield return p2;
//        Vector2i p3 = new Vector2i(x, y - dist); yield return p3;
//        Vector2i p4 = new Vector2i(x, y + dist); yield return p4;

//        for (int ii = 0; ii < dist - 1; ii++)
//        {
//            p1.X++; p1.Y--; yield return p1;
//            p2.X--; p2.Y++; yield return p2;
//            p3.X++; p3.Y++; yield return p3;
//            p4.X--; p4.Y--; yield return p4;
//        }
//    }

//    #endregion
//}
