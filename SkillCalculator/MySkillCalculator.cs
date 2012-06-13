using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Moserware.Skills.TrueSkill;
using Moserware.Skills;

namespace SkillCalculator
{
    public sealed class MySkillCalculator
    {
        private const string NoOneId = "-";

        private Dictionary<string,MyPlayer> Players = new Dictionary<string,MyPlayer>();
        private Queue<MyGame> Games = new Queue<MyGame>();

        public void LoadPlayers()
        {
            using (var sr = File.OpenText("Players.txt"))
            {
                var line = sr.ReadLine();
                int lineNumber = 0;
                while (line != null)
                {
                    if (lineNumber == 0)
                    {
                        line = sr.ReadLine();
                        lineNumber++;
                        continue;
                    }

                    var parts = line.Split(',');
                    string name = parts[0];
                    string id = parts[1];

                    var player = new MyPlayer() { Name = name, Id = id };
                    Players.Add(player.Id,player);

                    line = sr.ReadLine();
                }
            }
        }

        public void LoadGames()
        {
            using (var sr = File.OpenText("Games.txt"))
            {
                var line = sr.ReadLine();
                int lineNumber = 0;
                while (line != null)
                {
                    if (lineNumber == 0)
                    {
                        line = sr.ReadLine();
                        lineNumber++;
                        continue;
                    }

                    var parts = line.Split(',');
                    string playerId0 = parts[0];
                    string playerId1 = parts[1];
                    string playerId2 = parts[2];
                    string playerId3 = parts[3];
                    string playerId4 = parts[4];
                    string playerId5 = parts[5];

                    string playerId0Rank = parts[6];
                    string playerId1Rank = parts[7];
                    string playerId2Rank = parts[8];
                    string playerId3Rank = parts[9];
                    string playerId4Rank = parts[10];
                    string playerId5Rank = parts[11];

                    List<MyRankedPlayer> rankedParticipants = new List<MyRankedPlayer>();

                    AddPlayer(playerId0, playerId0Rank, rankedParticipants);
                    AddPlayer(playerId1, playerId1Rank, rankedParticipants);
                    AddPlayer(playerId2, playerId2Rank, rankedParticipants);
                    AddPlayer(playerId3, playerId3Rank, rankedParticipants);
                    AddPlayer(playerId4, playerId4Rank, rankedParticipants);
                    AddPlayer(playerId5, playerId5Rank, rankedParticipants);

                    MyGame game = new MyGame()
                    {
                        RankedParticipants = rankedParticipants,
                    };

                    rankedParticipants.Sort(new Comparison<MyRankedPlayer>((a, b) => a.Rank - b.Rank));
                    Games.Enqueue(game);

                    line = sr.ReadLine();
                }
            }
        }

        private void AddPlayer(string playerId, string playerIdRank, List<MyRankedPlayer> rankedParticipants)
        {
            if (playerId == NoOneId)
                return;

            var player = Players[playerId];
            rankedParticipants.Add(new MyRankedPlayer()
            {
                Id = playerId,
                Name = player.Name,
                Rank = int.Parse(playerIdRank),
            });
        }

        private Dictionary<Player, Rating> m_currentRatings;

        public void CalculateInitalGameSkills()
        {
            var initialGame = Games.Dequeue();

            var teams = Teams.Concat(initialGame.RankedParticipants.Select(p => p.ToTeam()).ToArray());
            int [] ranks = initialGame.RankedParticipants.Select(p => p.Rank).ToArray();
            
            var newRatings = TrueSkillCalculator.CalculateNewRatings(
                GameInfo.DefaultGameInfo, teams, ranks);
            double matchQuality = TrueSkillCalculator.CalculateMatchQuality(
                GameInfo.DefaultGameInfo, teams);

            System.Diagnostics.Debug.Assert(m_currentRatings == null);

            m_currentRatings = new Dictionary<Player, Rating>();

            foreach (var kvp in newRatings)
                m_currentRatings.Add(kvp.Key, kvp.Value);
        }

        public bool GetNextGameResults()
        {
            if (!Games.Any())
                return false;

            var nextGame = Games.Dequeue();

            var playerIds = nextGame.RankedParticipants.Select(mp => mp.Id);
            var players = playerIds
                .Select(pId => m_currentRatings.Keys.First(p => ((string)p.Id) == pId));
            var playerRanks = nextGame.RankedParticipants.Select(p => p.Rank);
            var gameRankings = players.Select(p => Tuple.Create(p, nextGame.RankedParticipants.First(rp => ((string)rp.Id) == (string)p.Id).Rank));

            var teams = Teams.Concat(gameRankings.Select(t => new Team(t.Item1, m_currentRatings[t.Item1])).ToArray());
            int[] ranks = gameRankings.Select(t => t.Item2).ToArray();

            var newRatings = TrueSkillCalculator.CalculateNewRatings(
                GameInfo.DefaultGameInfo, teams, ranks);

            UpdateNewRatings(newRatings);
            return true;
        }

        private void UpdateNewRatings(IDictionary<Player, Rating> newRatings)
        {
            foreach (var kvp in newRatings)
            {
                if (m_currentRatings.ContainsKey(kvp.Key))
                    m_currentRatings[kvp.Key] = kvp.Value;
                else
                    ;
            }
        }
    }
}
