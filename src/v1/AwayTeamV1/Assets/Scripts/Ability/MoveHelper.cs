using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class MoveHelper
{
    public static PathfindResult Djikstra(
        Func<Vector2i, int> cost,
        Func<Vector2i, IEnumerable<Vector2i>> adjacent,
        Vector2i start,
        int range)
    {
        var result = new PathfindResult(start);
        var frontier = new PriorityQueue<Vector2i>();

        result.Distance.Add(start, 0);
        frontier.Insert(start, 0);

        while (frontier.Count > 0)
        {
            var min = frontier.ExtractMin();

            foreach (var adj in adjacent(min.Key))
            {
                var d = cost(adj);
                var cur = result.Distance.ContainsKey(adj) ? result.Distance[adj] : int.MaxValue;
                var alt = min.Value + d;

                // Wrap around!?
                if (alt < 0 && d > 0)
                {
                    alt = int.MaxValue;
                }

                if (alt < cur && alt <= range)
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

        return result;
    }

    private static void AddOrUpdate<K, V>(IDictionary<K, V> dict, K key, V value)
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
}
