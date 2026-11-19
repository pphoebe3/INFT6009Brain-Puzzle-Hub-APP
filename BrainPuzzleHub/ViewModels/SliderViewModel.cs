using BrainPuzzleHub.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using Java.Security.Cert;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
//using static Android.InputMethodServices.Keyboard;


namespace BrainPuzzleHub.ViewModels;

public partial class SliderViewModel : ObservableObject
{
    private const int Size = 4;
    private const string GameKey = "15Puzzle";

    private readonly DatabaseService _db;
    private readonly Random _rng = new();

    [ObservableProperty] private int _moves;
    [ObservableProperty] private int _bestMoves;
    //0 indicates no records are available.
    [ObservableProperty] private bool _isWin;

    //Sixteen cells are bound to the UI /values ​​1-15, 0 represents a space
    public ObservableCollection<SliderTile> Cells { get; } = new();

    //Internal chessboard: 0 represents an empty space
    private readonly int[,] _board = new int[Size, Size];
    private int _emptyRow, _emptyCol;

    public SliderViewModel(DatabaseService db)
    {
        _db = db;
        StartNewGame();
    }

    // Called when the page appears, reads the optimal number of steps.
    public async Task LoadBestMovesAsync()
    {
        BestMoves = await _db.GetBestScoreAsync(GameKey);
    }

    [RelayCommand]
    public void StartNewGame()
    {
        Moves = 0;
        IsWin = false;

        // First, create an ordered chessboard (1-15 + spaces).
        int v = 1;
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                _board[r, c] = (r == Size - 1 && c == Size - 1) ? 0 : v++;

        _emptyRow = Size - 1;
        _emptyCol = Size - 1;

        Shuffle();
        SyncCells();
    }

    [RelayCommand]
    public async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    //Click the square to try moving it.
    [RelayCommand]
    public async Task TapTile(SliderTile tile)
    {
        if (tile is null || IsWin) return;

        //can only move when adjacent to a space up, down, left, or right.
        int dr = Math.Abs(tile.Row - _emptyRow);
        int dc = Math.Abs(tile.Col - _emptyCol);
        bool adjacent = (dr == 1 && dc == 0) || (dr == 0 && dc == 1);
        if (!adjacent) return;

        //Swap blocks and spaces
        _board[_emptyRow, _emptyCol] = tile.Value;
        _board[tile.Row, tile.Col] = 0;
        _emptyRow = tile.Row;
        _emptyCol = tile.Col;

        Moves++;
        SyncCells();

        if (CheckWin())
        {
            IsWin = true;
            await UpdateBestMovesAsync();
        }
    }

    //Starting with an ordered chessboard,
    //perform a large number of random legal moves to guarantee that a solution exists.
    private void Shuffle()
    {
        int steps = 200;
        int lastDr = 0, lastDc = 0; 
        //Avoid immediately undoing the previous step

        for (int i = 0; i < steps; i++)
        {
            var options = new List<(int dr, int dc)>();
            if (_emptyRow > 0) options.Add((-1, 0));
            if (_emptyRow < Size - 1) options.Add((1, 0));
            if (_emptyCol > 0) options.Add((0, -1));
            if (_emptyCol < Size - 1) options.Add((0, 1));

            //Exclude the direction that just cancels the previous step
            options.RemoveAll(o => o.dr == -lastDr && o.dc == -lastDc);

            var (dr, dc) = options[_rng.Next(options.Count)];
            int nr = _emptyRow + dr, nc = _emptyCol + dc;

            _board[_emptyRow, _emptyCol] = _board[nr, nc];
            _board[nr, nc] = 0;
            _emptyRow = nr;
            _emptyCol = nc;

            lastDr = dr;
            lastDc = dc;
        }
    }

    private bool CheckWin()
    {
        int expected = 1;
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
            {
                if (r == Size - 1 && c == Size - 1)
                {
                    if (_board[r, c] != 0) return false;
                }
                else
                {
                    if (_board[r, c] != expected) return false;
                    expected++;
                }
            }
        return true;
    }

    private async Task UpdateBestMovesAsync()
    {
        //BestMoves == 0 indicates no records yet,
        //or updates will only occur when the number of steps is lower.
        if (BestMoves == 0 || Moves < BestMoves)
            BestMoves = Moves;

        await _db.SaveBestMovesAsync(GameKey, Moves);
    }

    private void SyncCells()
    {
        int i = 0;
        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                int val = _board[r, c];
                if (i < Cells.Count)
                    Cells[i].Update(val, r, c);
                else
                    Cells.Add(new SliderTile(val, r, c));
                i++;
            }
        }
    }
}

//Single square data model
public partial class SliderTile : ObservableObject
{
    [ObservableProperty] private int _value; 
    // 0 = blank
    [ObservableProperty] private int _row;
    [ObservableProperty] private int _col;

    public bool IsEmpty => Value == 0;
    public string DisplayText => Value == 0 ? "" : Value.ToString();

    public SliderTile(int value, int row, int col) => Update(value, row, col);

    public void Update(int value, int row, int col)
    {
        Value = value;
        Row = row;
        Col = col;
        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(DisplayText));
    }
}