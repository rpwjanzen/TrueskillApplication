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
        public Dictionary<string,MyPlayer> Players = new Dictionary<string,MyPlayer>();
        private Queue<MyGame> Games = new Queue<MyGame>();

        public void LoadPlayers()
        {
            var players = SevenWondersParser.GetPlayers("Games.csv");
            foreach (var player in players)
            {
                Players.Add(player.Id, player);
            }
        }

        public void LoadGames()
        {
            // Seed the engine with dummy values for all players so we do not fail a dictionary lookup later.
            Games.Enqueue(new MyGame()
            {
                RankedParticipants = SevenWondersParser.ReadPlayers("Games.csv").Select(p => new MyRankedPlayer()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Rank = 0,
                }).ToList(),
            });

            foreach (var game in SevenWondersParser.ReadGames("Games.csv"))
            {
                Games.Enqueue(game);
            }
        }

        private Dictionary<Player, Rating> m_currentRatings;
        public Dictionary<Player, Rating> CurrentRatings
        {
            get { return m_currentRatings; }
        }

        public void CalculateInitalGameSkills()
        {
            var initialGame = Games.Dequeue();

            var teams = Teams.Concat(initialGame.RankedParticipants.Select(p => p.ToTeam()).ToArray());
            int [] ranks = initialGame.RankedParticipants.Select(p => p.Rank).ToArray();
            
            var newRatings = TrueSkillCalculator.CalculateNewRatings(
                GameInfo.DefaultGameInfo, teams, ranks);
            //double matchQuality = TrueSkillCalculator.CalculateMatchQuality(
            //    GameInfo.DefaultGameInfo, teams);

            System.Diagnostics.Debug.Assert(m_currentRatings == null);

            m_currentRatings = new Dictionary<Player, Rating>();

            foreach (var kvp in newRatings)
            {
                m_currentRatings.Add(kvp.Key, kvp.Value);
            }
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
                    //m_currentRatings[kvp.Key] = kvp.Value;
            }
        }
    }
}
