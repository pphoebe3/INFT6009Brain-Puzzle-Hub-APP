using System;
using System.Collections.Generic;
using System.Text;
//
using BrainPuzzleHub.Models;
using SQLite;
//2048
//15puzzle

namespace BrainPuzzleHub.Services
{
    //Unified management of the local SQLite database, which can be shared by all game ViewModels.
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _connection;

        //Database file storage path / App-specific sandbox directory
        private static string DbPath =>
            Path.Combine(FileSystem.AppDataDirectory, "brainpuzzlehub.db3");

        //Ensure the database and tables have been created; initialization is required.
        //SQLitePCLRaw.bundle_green provides the native SQLite library,
        //but it does not initialize automatically in some MAUI+Android configurations.
        //need to manually call SQLitePCL.Batteries_V2.Init() once;
        //otherwise, using SQLite will throw an underlying exception, _JavaProxyThrowable.
        private async Task InitAsync()
        {
            if (_connection is not null) return;
            //On the Android platform, the SQLite native library needs to be manually initialized;
            //otherwise, CreateTableAsync will crash.
            SQLitePCL.Batteries_V2.Init();

            _connection = new SQLiteAsyncConnection(DbPath);
            await _connection.CreateTableAsync<GameScore>();
            await _connection.CreateTableAsync<User>();
            //Add User Table
        }
        //Score related
        //Read the highest score for a game
        //Return 0 if no record is found
        public async Task<int> GetBestScoreAsync(string gameName)
        {
            await InitAsync();

            var record = await _connection!.Table<GameScore>()
                .FirstOrDefaultAsync(x => x.GameName == gameName);

            return record?.BestScore ?? 0;
        }

        //Save your best number of steps: the fewer steps the better,
        //only update when it's "better" or "first time recording".
        //15puzzle
        public async Task SaveBestMovesAsync(string gameName, int moves)
        {
            await InitAsync();

            var existing = await _connection!.Table<GameScore>()
                .FirstOrDefaultAsync(x => x.GameName == gameName);

            if (existing is null)
            {
                //For the first record, insert directly.
                await _connection.InsertAsync(new GameScore
                {
                    GameName = gameName,
                    BestScore = moves,
                    LastPlayed = DateTime.Now
                });
            }
            else if (moves < existing.BestScore)
            {
                //Update only when the number of steps is less than before.
                existing.BestScore = moves;
                existing.LastPlayed = DateTime.Now;
                await _connection.UpdateAsync(existing);
            }
        }


        //Save//update the highest score for a game
        //Update if it exists, insert if it does not exist
        public async Task SaveBestScoreAsync(string gameName, int score)
        {
            await InitAsync();

            var existing = await _connection!.Table<GameScore>()
                .FirstOrDefaultAsync(x => x.GameName == gameName);

            if (existing is null)
            {
                //// 
                await _connection.InsertAsync(new GameScore
                {
                    GameName = gameName,
                    BestScore = score,
                    LastPlayed = DateTime.Now
                });
            }
            else
            {
                existing.BestScore = score;
                existing.LastPlayed = DateTime.Now;
                await _connection.UpdateAsync(existing);
            }
        }

        //The account page displays a summary of all game scores.
        public async Task<List<GameScore>> GetAllScoresAsync()
        {
            await InitAsync();
            return await _connection!.Table<GameScore>().ToListAsync();
        }

        //User related
        //Registration: Returns null if username already exists,
        //returns new user if successful
        public async Task<User?> RegisterAsync(string username, string password)
        {
            await InitAsync();

            var exists = await _connection!.Table<User>()
                .FirstOrDefaultAsync(u => u.Username == username);
            if (exists is not null) return null; 
            //Username is already taken

            var salt = PasswordHasher.GenerateSalt();
            var user = new User
            {
                Username = username,
                PasswordSalt = salt,
                PasswordHash = PasswordHasher.Hash(password, salt),
            };

            await _connection.InsertAsync(user);
            return user;
        }

        //Login: Returns the user name on successful verification, and returns null on failure.
        public async Task<User?> LoginAsync(string username, string password)
        {
            await InitAsync();

            var user = await _connection!.Table<User>()
                .FirstOrDefaultAsync(u => u.Username == username);
            if (user is null) return null;
            //User does not exist.

            bool valid = PasswordHasher.Verify(password, user.PasswordSalt, user.PasswordHash);
            return valid ? user : null;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            await InitAsync();
            return await _connection!.Table<User>().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateUserAsync(User user)
        {
            await InitAsync();
            await _connection!.UpdateAsync(user);
        }


    }

}
