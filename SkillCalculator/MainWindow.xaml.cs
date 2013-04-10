using System;
using System.Windows;

namespace SkillCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MySkillCalculator s = new MySkillCalculator();
            s.LoadPlayers();
            s.LoadGames();
            s.CalculateInitalGameSkills();
            bool hasNextGame = s.GetNextGameResults();
            while (hasNextGame)
            {
                hasNextGame = s.GetNextGameResults();
            }

            m_textBox.Text += "info mean stddev" + Environment.NewLine;
            foreach (var kvp in s.CurrentRatings)
            {
                m_textBox.Text += string.Format("{0} {1} {2}", kvp.Key.Id, kvp.Value.Mean, kvp.Value.StandardDeviation) + Environment.NewLine;
            }
        }
    }
}
