using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BrainPuzzleHub.Models
{

    public partial class MinesweeperCell : ObservableObject
    {
        public int Row { get; }
        public int Col { get; }

        [ObservableProperty] private bool _isMine;
        [ObservableProperty] private bool _isRevealed;
        [ObservableProperty] private bool _isFlagged;
        [ObservableProperty] private int _adjacentMines; // 周围雷数，0表示空白格

        // 显示文本：未揭开=空，插旗=🚩，雷=💣，数字=对应adjacentMines，0=空白
        public string DisplayText
        {
            get
            {
                if (IsFlagged && !IsRevealed) return "🚩";
                if (!IsRevealed) return "";
                if (IsMine) return "💣";
                return AdjacentMines == 0 ? "" : AdjacentMines.ToString();
            }
        }

        // 背景色：未揭开=灰，已揭开=浅色，踩雷=红
        public Color Background
        {
            get
            {
                if (!IsRevealed) return Color.FromArgb("#9E9E9E");
                return IsMine ? Color.FromArgb("#E53935") : Color.FromArgb("#ECECEC");
            }
        }

        // 数字颜色，经典扫雷配色：1蓝 2绿 3红 4紫...
        public Color TextColor => AdjacentMines switch
        {
            1 => Color.FromArgb("#1976D2"),
            2 => Color.FromArgb("#388E3C"),
            3 => Color.FromArgb("#D32F2F"),
            4 => Color.FromArgb("#7B1FA2"),
            5 => Color.FromArgb("#F57C00"),
            _ => Color.FromArgb("#333333"),
        };

        public MinesweeperCell(int row, int col)
        {
            Row = row;
            Col = col;
        }

        // 状态变化后统一刷新界面显示相关字段
        public void Refresh()
        {
            OnPropertyChanged(nameof(DisplayText));
            OnPropertyChanged(nameof(Background));
            OnPropertyChanged(nameof(TextColor));
        }
    }


}
