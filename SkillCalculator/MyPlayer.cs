using Moserware.Skills;

namespace SkillCalculator
{
    public class MyPlayer
    {
        public string Id { get; set; }
        public string Name { get; set; }

        private Player m_player;

        public Player ToPlayer()
        {
            if (m_player == null)
                m_player = new Player(Id);

            return m_player;
        }

        private Team m_team;

        public Team ToTeam()
        {
            if (m_team == null)
                m_team = new Team(ToPlayer(), GameInfo.DefaultGameInfo.DefaultRating);

            return m_team;
        }

        public override string ToString()
        {
            return string.Format("Id={0}, Name={1}", Id, Name);
        }
    }
}
