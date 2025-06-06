using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static int Score { get; set; } = 0;
    public static string PlayerName { get; set; } = string.Empty;
    public static int PlayerHealth { get; set; } = 100;
    public static string CurrentCheckpoint { get; set; } = string.Empty;
    public static List<string> DeadEnemiesId { get; set; } = new List<string>();
    public static List<Level> Levels { get; set; } = new List<Level>(3);
    public static Level CurrentLevel { get; set; } = Level.RestartLevel(1);
}
