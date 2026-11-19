using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using Google.Android.Material.Color.Utilities;
//using Microsoft.Maui.Controls.Shapes;


//error//using Google.Android.Material.Color.Utilities;
//using Microsoft.Maui.Graphics.Text;
using System.Collections.ObjectModel;
//using static Android.Telecom.Call;
//using static Xamarin.Google.Crypto.Tink.Shaded.Protobuf.DescriptorProtos;
using BrainPuzzleHub.Services;
//Replace Preferences with SQLite


namespace BrainPuzzleHub.ViewModels;

public partial class Game2048ViewModel : ObservableObject
{
    //constant
    private const int Size = 4;
    private const string GameKey = "2048";
    //Game identifiers in the database
    private readonly DatabaseService _db;
    //dependency

    //Observable properties
    [ObservableProperty] private int _score;
    [ObservableProperty] private int _bestScore;
    [ObservableProperty] private bool _isGameOver;
    [ObservableProperty] private bool _isWin;

    //The 16 squares of the chessboard are bound to a UI4*4.
    public ObservableCollection<TileCell> Cells { get; } = new();

    //Internal state
    private readonly int[,] _board = new int[Size, Size];
    private readonly Random _rng = new();


    //Injecting DatabaseService into constructor
    public Game2048ViewModel(DatabaseService db)
    {
        _db = db;
        StartNewGame();
    }
    //Called when the page appears, asynchronous reading gets the highest score
    //Constructor cannot be async
    public async Task LoadBestScoreAsync()
    {
        BestScore = await _db.GetBestScoreAsync(GameKey);
    }






    //The logic for updating the highest score has been changed to asynchronous write to SQLite.
    private async Task UpdateBestScoreAsync()
    {
        if (Score > BestScore)
        {
            BestScore = Score;
            await _db.SaveBestScoreAsync(GameKey, BestScore);
        }
    }






    //command bu
    [RelayCommand]
    public void StartNewGame()
    {
        Score = 0;
        IsGameOver = false;
        IsWin = false;

        //Clear the chessboard
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                _board[r, c] = 0;

        SpawnTile();
        SpawnTile();
        SyncCells();
    }

    [RelayCommand]
    public async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    //The OnSwiped slider is invoked using a XAML gesture.
    public async void OnSwiped(SwipeDirection direction)
        //add async!to await UpdateBestScoreAsync();
    {
        if (IsGameOver || IsWin) return;
        int scoreBefore = Score;
        //Record the score before the move to determine whether a merge occurred during the move.

        bool moved = direction switch
        {
            SwipeDirection.Left => SlideLeft(),
            SwipeDirection.Right => SlideRight(),
            SwipeDirection.Up => SlideUp(),
            SwipeDirection.Down => SlideDown(),
            _ => false
        };

        if (!moved) return;

        // If this move results in a merge (score increase), then vibrate!!!!!!!!
        //!!!!
        if (Score > scoreBefore)
        {
            try { Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(40)); }
            catch { /* Some simulators do not support vibration. */ }
        }

        SpawnTile();
        SyncCells();
        //UpdateBestScore();
        //The call in OnSwiped should be changed accordingly to await.
        await UpdateBestScoreAsync();
        CheckGameState();
    }

    //Core mobile logic
    //Coremo move logic
    //
    //
    //Merge a row to the left, return a new row and the score increment + mark
    private (int[] newRow, int gained) MergeLeft(int[] row)//
                                                           //It takes an array of integers and returns the processed result.
    {
        var tiles = row.Where(v => v != 0).ToArray();
        //
        //LINQ only retains numbers that are not equal to 0.
        var result = new int[Size];//
                                   //Prepare the result array
        int gained = 0;
        int wi = 0;//Write location
        //write index

        for (int i = 0; i < tiles.Length; i++)//
                                              //Start iterating and checking to obtain numbers!
        {
            if (i + 1 < tiles.Length && tiles[i] == tiles[i + 1])
            //
            //Check if merging is possible
            {
                result[wi++] = tiles[i] * 2; //result
                gained += tiles[i] * 2;//Bonus points
                i++;
                //
                //Skip merged numbers!!!!!!
                //The `tiles[]` function returns the original value before addition.
            }
            else
            {
                result[wi++] = tiles[i];
            }
        }
        return (result, gained);
    }

    //SlideLeft
    private bool SlideLeft()
    {
        bool moved = false;
        for (int r = 0; r < Size; r++)
        //Traverse each row
        {
            var row = GetRow(r);
            //row
            var (newRow, gained) = MergeLeft(row);
            //
            //Extract the current row and merge it to the left.
            if (!row.SequenceEqual(newRow)) moved = true;
            //
            //Determine if there is movement
            SetRow(r, newRow);
            //
            //Write back to the chessboard
            Score += gained;
            //add gained Bonus points
        }
        return moved;
    }

    private bool SlideRight()
    {
        bool moved = false;
        for (int r = 0; r < Size; r++)
        {
            var row = GetRow(r).Reverse().ToArray();
            var (newRow, gained) = MergeLeft(row);
            newRow = newRow.Reverse().ToArray();
            var original = GetRow(r);
            if (!original.SequenceEqual(newRow)) moved = true;
            SetRow(r, newRow);
            Score += gained;
        }
        return moved;
    }

    private bool SlideUp()
    {
        bool moved = false;
        for (int c = 0; c < Size; c++)
        {
            var col = GetCol(c);
            var (newCol, gained) = MergeLeft(col);
            if (!col.SequenceEqual(newCol)) moved = true;
            SetCol(c, newCol);
            Score += gained;
        }
        return moved;
    }

    private bool SlideDown()
    {
        bool moved = false;
        for (int c = 0; c < Size; c++)
        {
            var col = GetCol(c).Reverse().ToArray();
            var (newCol, gained) = MergeLeft(col);
            newCol = newCol.Reverse().ToArray();
            var original = GetCol(c);
            if (!original.SequenceEqual(newCol)) moved = true;
            SetCol(c, newCol);
            Score += gained;
        }
        return moved;
    }

    //Auxiliary methods

    private int[] GetRow(int r)
    {
        var row = new int[Size];
        for (int c = 0; c < Size; c++) row[c] = _board[r, c];
        return row;
    }

    private void SetRow(int r, int[] row)
    {
        for (int c = 0; c < Size; c++) _board[r, c] = row[c];
    }

    private int[] GetCol(int c)
    {
        var col = new int[Size];
        for (int r = 0; r < Size; r++) col[r] = _board[r, c];
        return col;
    }

    private void SetCol(int c, int[] col)
    {
        for (int r = 0; r < Size; r++) _board[r, c] = col[r];
    }

    //
    //Randomly generate 2_90% or 4_10% in the blank space.
    private void SpawnTile()
    {
        var empty = new List<(int r, int c)>();
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                if (_board[r, c] == 0) empty.Add((r, c));

        if (empty.Count == 0) return;
        var (row, col) = empty[_rng.Next(empty.Count)];
        _board[row, col] = _rng.Next(10) == 0 ? 4 : 2;
    }

    //Sync _board to ObservableCollection
    private void SyncCells()
    {
        int i = 0;
        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                int val = _board[r, c];
                if (i < Cells.Count)
                    Cells[i].Update(val);
                else
                    Cells.Add(new TileCell(val));
                i++;
            }
        }
    }

    //private void UpdateBestScore()
    //{
    //    if (Score > BestScore)
    //    {
    //        BestScore = Score;
    //        Preferences.Set("2048_best", BestScore);
    //    }
    //}

    private void CheckGameState()
    {
        //Check if 2048 has been reached.
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                if (_board[r, c] == 2048) { IsWin = true; return; }

        //Check for any remaining spaces or spaces that can be merged.
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
            {
                if (_board[r, c] == 0) return;
                if (c + 1 < Size && _board[r, c] == _board[r, c + 1]) return;
                if (r + 1 < Size && _board[r, c] == _board[r + 1, c]) return;
            }

        IsGameOver = true;
    }
}

//Single cell model
public partial class TileCell : ObservableObject
{
    [ObservableProperty] private int _value;
    [ObservableProperty] private Color _background;
    [ObservableProperty] private Color _textColor;

    public string DisplayText => Value == 0 ? "" : Value.ToString();

    public TileCell(int value) => Update(value);

    public void Update(int value)
    {
        Value = value;
        OnPropertyChanged(nameof(DisplayText));
        (Background, TextColor) = value switch
        {
            0 => (Color.FromArgb("#CDC1B4"), Color.FromArgb("#CDC1B4")),
            2 => (Color.FromArgb("#EEE4DA"), Color.FromArgb("#776E65")),
            4 => (Color.FromArgb("#EDE0C8"), Color.FromArgb("#776E65")),
            8 => (Color.FromArgb("#F2B179"), Color.FromArgb("#FFFFFF")),
            16 => (Color.FromArgb("#F59563"), Color.FromArgb("#FFFFFF")),
            32 => (Color.FromArgb("#F67C5F"), Color.FromArgb("#FFFFFF")),
            64 => (Color.FromArgb("#F65E3B"), Color.FromArgb("#FFFFFF")),
            128 => (Color.FromArgb("#EDCF72"), Color.FromArgb("#FFFFFF")),
            256 => (Color.FromArgb("#EDCC61"), Color.FromArgb("#FFFFFF")),
            512 => (Color.FromArgb("#EDC850"), Color.FromArgb("#FFFFFF")),
            1024 => (Color.FromArgb("#EDC53F"), Color.FromArgb("#FFFFFF")),
            2048 => (Color.FromArgb("#EDC22E"), Color.FromArgb("#FFFFFF")),
            _ => (Color.FromArgb("#3C3A32"), Color.FromArgb("#FFFFFF")),
        };
    }
}