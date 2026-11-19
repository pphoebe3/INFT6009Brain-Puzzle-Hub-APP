//sqlite-net-pcl
//SQLitePCLRaw.bundle_green
//!!!!!!add!NuGet
//数据模型
using System;
//using System.Collections.Generic;
//using System.Text;
using SQLite;
//data

namespace BrainPuzzleHub.Models;
// 对应SQLite里的一张表，存每个游戏的最高分
[Table("GameScores")]
//
public class GameScore
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // 游戏标识，比如 "2048" / "Minesweeper" / "Sudoku"
    [Unique]
    public string GameName { get; set; } = string.Empty;

    public int BestScore { get; set; }

    public DateTime LastPlayed { get; set; }
}
