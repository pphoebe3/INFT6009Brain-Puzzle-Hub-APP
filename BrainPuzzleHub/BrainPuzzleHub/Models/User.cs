using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace BrainPuzzleHub.Models;
    [Table("Users")]
public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique]
    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;

    public string Avatar { get; set; } = "🙂"; 
    // emoji占位头像

    public bool SoundEnabled { get; set; } = true;
    public bool VibrationEnabled { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
