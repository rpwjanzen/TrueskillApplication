using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SkillCalculator
{
    public static class SevenWondersParser
    {
        public static IEnumerable<MyPlayer> ReadPlayers(string filename)
        {
            var headersLine = File.ReadLines(filename).FirstOrDefault();
            return GetPlayers(headersLine);
        }

        public static IEnumerable<MyPlayer> GetPlayers(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                yield break;
            }

            var parts = text.Split(',');
            foreach (var part in parts)
            {
                yield return new MyPlayer()
                {
                    Id = part,
                    Name = part
                };
            }
        }

        public static IEnumerable<MyGame> ReadGames(string filename)
        {
            var players = ReadPlayers(filename);
            if (!players.Any())
            {
                yield break;
            }

            int i = 0;
            var playersByPosition = players.ToArray();

            var lines = File.ReadLines(filename)
                // skip header lines
                .Skip(1);

            foreach (var line in lines)
            {
                yield return ReadGame(line, playersByPosition);
            }
        }

        private static MyGame ReadGame(string line, IEnumerable<MyPlayer> playersByPosition)
        {
            var gameParts = line.Split(',');
            var rankings = GetRankings(gameParts, playersByPosition);

            var rankedPlayers = rankings
                .SelectMany(kvp => kvp.Value
                    .Select(p => new MyRankedPlayer()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Rank = kvp.Key,
                    }));

            var game = new MyGame()
            {
                RankedParticipants = rankedPlayers.ToList()
            };
            game.RankedParticipants.Sort(new Comparison<MyRankedPlayer>((a, b) => a.Rank - b.Rank));

            return game;
        }

        public static IDictionary<int, IEnumerable<MyPlayer>> GetRankings(IEnumerable<string> gameParts, IEnumerable<MyPlayer> playersByPosition)
        {
            var scores = GetScores(gameParts);
            var playerWithScores = playersByPosition
                .Zip(scores, (p, s) => new { Player = p, Score = s })
                .Where(a => a.Score != null)
                .GroupBy(a => a.Score)
                .OrderByDescending(a => a.Key);

            var playersByRanking =
                playerWithScores.Zip(Enumerable.Range(0, int.MaxValue), (g, i) => new { Rank = i, Players = g })
                .ToDictionary(a => a.Rank, a1 => a1.Players.Select(a2 => a2.Player));
            return playersByRanking;
        }

        public static IEnumerable<int?> GetScores(IEnumerable<string> scoresText)
        {
            return scoresText.Select(t =>
            {
                int result;
                if (int.TryParse(t, out result))
                {
                    return new Nullable<int>(result);
                }
                else
                {
                    return null;
                }
            });
        }
    }
}
