using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;

class Pathfinder
{
    public static PathfindResult Pathfind(Map map, Actor actor)
    {
        var result = new PathfindResult();
        var frontier = new PriorityQueue<Vector2i>();
        var actorLocation = map.GetLocation(actor);
        if (actorLocation == null)
        {
            throw new InvalidOperationException("Actor " + actor + " does not exist on map " + map);
        }

        var start = actorLocation.Value;
        result.Distance.Add(new Vector2i(start.X, start.Y), 0);
        frontier.Insert(start, 0);

        while (frontier.Count > 0)
        {
            var min = frontier.ExtractMin();
            foreach (var adj in GetAdjacentCoordinates(map, min.Key))
            {
                Actor tempActor;
                if (!map.TryGetActor(adj, out tempActor))
                {
                    var tile = map[adj];
                    var cost = tile.Properties.MovementPenalty;
                    var cur = result.Distance.ContainsKey(adj) ? result.Distance[adj] : int.MaxValue;
                    var alt = min.Value + cost;

                    if (alt < cur && alt <= actor.Properties.MovementPoints)
                    {
                        AddOrUpdate(result.Distance, adj, alt);
                        AddOrUpdate(result.Previous, adj, min.Key);
                        if (frontier.ContainsKey(adj))
                        {
                            frontier.DecreaseKey(adj, alt);
                        }
                        else
                        {
                            frontier.Insert(adj, alt);
                        }
                    }
                }
            }
        }

        return result;
    }

    private static void AddOrUpdate<K,V>(IDictionary<K,V> dict, K key, V value)
    {
        if (dict.ContainsKey(key))
        {
            dict[key] = value;
        }
        else
        {
            dict.Add(key, value);
        }
    }

    private static IEnumerable<Vector2i> GetAdjacentCoordinates(Map map, Vector2i coord)
    {
        var x = coord.X;
        var y = coord.Y;

        if (x - 1 >= 0) yield return new Vector2i(x - 1, y);
        if (x + 1 < map.Columns) yield return new Vector2i(x + 1, y);
        if (y - 1 >= 0) yield return new Vector2i(x, y - 1);
        if (y + 1 < map.Rows) yield return new Vector2i(x, y + 1);
    }
}
