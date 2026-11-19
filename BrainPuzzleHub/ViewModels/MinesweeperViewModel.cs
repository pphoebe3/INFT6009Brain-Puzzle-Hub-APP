using System;
using System.Collections.Generic;
using System.Text;
using BrainPuzzleHub.Models;
using BrainPuzzleHub.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;


namespace BrainPuzzleHub.ViewModels
{
    public partial class MinesweeperViewModel : ObservableObject
    {
        private const int Size = 9;
        private const int MineCount = 10;
        private const string GameKey = "Minesweeper";

        private readonly DatabaseService _db;
        private readonly Random _rng = new();

        [ObservableProperty] private bool _isFlagMode;
        //Is it in flag planting mode /false = uncover mode, true = flag planting mode
        [ObservableProperty] private int _elapsedSeconds;
        [ObservableProperty] private int _bestTime;
        //0 =No records available
        [ObservableProperty] private int _flagsLeft;
        [ObservableProperty] private bool _isGameOver;
        [ObservableProperty] private bool _isWin;

        public ObservableCollection<MinesweeperCell> Cells { get; } = new();

        private MinesweeperCell[,] _grid = new MinesweeperCell[Size, Size];
        private bool _minesPlaced;
        //Has a mine been laid
        //Lay a mine only after the first click to ensure
        //don't step on a mine on the first try.
        private System.Timers.Timer? _timer;

        public MinesweeperViewModel(DatabaseService db)
        {
            _db = db;
            StartNewGame();
        }

        //Optimal loading time when the page appears
        public async Task LoadBestTimeAsync()
        {
            BestTime = await _db.GetBestScoreAsync(GameKey);
        }

        [RelayCommand]
        public void StartNewGame()
        {
            StopTimer();
            ElapsedSeconds = 0;
            IsGameOver = false;
            IsWin = false;
            FlagsLeft = MineCount;
            _minesPlaced = false;

            Cells.Clear();
            _grid = new MinesweeperCell[Size, Size];
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    var cell = new MinesweeperCell(r, c);
                    _grid[r, c] = cell;
                    Cells.Add(cell);
                }
            }
            //Note
            //There are no mines yet; they will be laid when the player clicks for the first time.
        }

        [RelayCommand]
        public async Task GoBack()
        {
            StopTimer();
            await Shell.Current.GoToAsync("..");
        }

        //Click: Reveal the grid
        [RelayCommand]
        public async Task RevealCell(MinesweeperCell cell)
        {
            if (cell is null || IsGameOver || IsWin) return;


            //In flag planting mode, clicking on a square will plant a flag, without revealing it.
            if (IsFlagMode)
            {
                ToggleFlag(cell);
                return;
            }

            if (cell.IsRevealed || cell.IsFlagged) return;

            //The first click is when a mine is actually laid,
            //and it is guaranteed that the clicked square and its surrounding area are not mines.
            if (!_minesPlaced)
            {
                PlaceMines(cell.Row, cell.Col);
                _minesPlaced = true;
                StartTimer();
            }

            if (cell.IsMine)
            {
                RevealAllMines();
                IsGameOver = true;
                StopTimer();
                return;
            }

            FloodReveal(cell.Row, cell.Col);

            if (CheckWin())
            {
                IsWin = true;
                StopTimer();
                await UpdateBestTimeAsync();
            }
        }
        //Commands bound to the switch mode button
        [RelayCommand]
        private void ToggleMode()
        {
            IsFlagMode = !IsFlagMode;
        }
        // //Plant a flag / Cancel planting a flag
        [RelayCommand]
        public void ToggleFlag(MinesweeperCell cell)
        {
            if (cell is null || IsGameOver || IsWin) return;
            if (cell.IsRevealed) return;

            if (!cell.IsFlagged && FlagsLeft <= 0) return;
            //All the flags are used up

            cell.IsFlagged = !cell.IsFlagged;
            FlagsLeft += cell.IsFlagged ? -1 : 1;
            cell.Refresh();
        }

        // Lay mines/ Calculate the number of surrounding mines

        //safeRow/safeCol: The first tile the player clicks on,
        //ensuring that this tile and the surrounding 8 tiles are free of mines.
        private void PlaceMines(int safeRow, int safeCol)
        {
            int placed = 0;
            while (placed < MineCount)
            {
                int r = _rng.Next(Size);
                int c = _rng.Next(Size);


                bool tooCloseToSafeZone = Math.Abs(r - safeRow) <= 1 && Math.Abs(c - safeCol) <= 1;
                if (tooCloseToSafeZone || _grid[r, c].IsMine) continue;

                _grid[r, c].IsMine = true;
                placed++;
            }

            // Calculate the number of mines around each cell.
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    if (_grid[r, c].IsMine) continue;
                    _grid[r, c].AdjacentMines = CountAdjacentMines(r, c);
                }
            }
        }

        private int CountAdjacentMines(int row, int col)
        {
            int count = 0;
            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr, nc = col + dc;
                    if (nr < 0 || nr >= Size || nc < 0 || nc >= Size) continue;
                    if (_grid[nr, nc].IsMine) count++;
                }
            }
            return count;
        }

        // Flood Fill: Reveals blank connected areas
        //the effect of clicking on a large area in classic Minesweeper
        private void FloodReveal(int row, int col)
        {
            var stack = new Stack<(int r, int c)>();
            stack.Push((row, col));

            while (stack.Count > 0)
            {
                var (r, c) = stack.Pop();
                var cell = _grid[r, c];
                if (cell.IsRevealed || cell.IsFlagged) continue;

                cell.IsRevealed = true;
                cell.Refresh();

                //Only the empty squares with a lightning count of 0 continue to expand outwards.
                if (cell.AdjacentMines == 0)
                {
                    for (int dr = -1; dr <= 1; dr++)
                    {
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            if (dr == 0 && dc == 0) continue;
                            int nr = r + dr, nc = c + dc;
                            if (nr < 0 || nr >= Size || nc < 0 || nc >= Size) continue;
                            if (!_grid[nr, nc].IsRevealed)
                                stack.Push((nr, nc));
                        }
                    }
                }
            }
        }

        private void RevealAllMines()
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (_grid[r, c].IsMine)
                    {
                        _grid[r, c].IsRevealed = true;
                        _grid[r, c].Refresh();
                    }
        }

        //Victory condition: All non-Reggs have been revealed.
        private bool CheckWin()
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (!_grid[r, c].IsMine && !_grid[r, c].IsRevealed)
                        return false;
            return true;
        }

        // timer ────────────────────────

        private void StartTimer()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (s, e) =>
            {
                //The timer is triggered in a background thread, and UI property updates require switching back to the main thread.
                MainThread.BeginInvokeOnMainThread(() => ElapsedSeconds++);
            };
            _timer.Start();
        }

        private void StopTimer()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }

        private async Task UpdateBestTimeAsync()
        {
            if (BestTime == 0 || ElapsedSeconds < BestTime)
                BestTime = ElapsedSeconds;

            await _db.SaveBestMovesAsync(GameKey, ElapsedSeconds);
            //Reuse the "smaller is better" storage logic
        }
    }

}
