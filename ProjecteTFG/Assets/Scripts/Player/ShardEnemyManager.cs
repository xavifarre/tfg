using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ShardEnemyManager
{
    public static List<int> enemiesMarked;

    public static bool EnemyIsMarked(int enemy)
    {
        return enemiesMarked.Contains(enemy);
    }

    public static void ResetEnemies()
    {
        enemiesMarked = new List<int>();
    }

    public static void MarkEnemy(int enemy)
    {
        enemiesMarked.Add(enemy);
    }
}