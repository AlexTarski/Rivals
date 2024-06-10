using Avalonia.Controls;
using Avalonia.Remote.Protocol.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rivals;

public class RivalsTask
{
	public static IEnumerable<OwnedLocation> AssignOwners(Map map)
	{
        HashSet<Point> ownedLocations = new();
        HashSet<Point> chests = map.Chests.ToHashSet();
        var queue = new Queue<ValueTuple<Point, int, int>>();
        for (int i = 0; i < map.Players.Length; i++)
        {
            if (chests.Contains(map.Players[i]))
            {
                ownedLocations.Add(map.Players[i]);
                yield return new OwnedLocation(i, map.Players[i], 0);
            }
            else
            {
                queue.Enqueue((map.Players[i], i, 0));
            }
        }
        while (queue.Count != 0)
        {
            var playersLocation = queue.Dequeue();
            if (!map.InBounds(playersLocation.Item1) || map.Maze[playersLocation.Item1.X, playersLocation.Item1.Y] != MapCell.Empty
                || ownedLocations.Contains(playersLocation.Item1)) continue;
                
            ownedLocations.Add(playersLocation.Item1);
            yield return new OwnedLocation(playersLocation.Item2, playersLocation.Item1, playersLocation.Item3);
            if (chests.Contains(playersLocation.Item1)) continue;
            for (var dy = -1; dy <= 1; dy++)
                for (var dx = -1; dx <= 1; dx++)
                    if ((dx != 0 && dy != 0) || (dx == 0 && dy == 0)) continue;
                    else queue.Enqueue(new(new Point { X = playersLocation.Item1.X + dx, Y = playersLocation.Item1.Y + dy }, playersLocation.Item2, playersLocation.Item3 + 1));
        }
        yield break;
	}
}