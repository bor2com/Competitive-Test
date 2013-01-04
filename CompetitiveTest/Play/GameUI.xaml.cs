namespace SSU.CompetitiveTest.Play {

    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Win32;
    using System.Collections.ObjectModel;

    public partial class GameUI : UserControl {

        #region Fields

        private Game currentGame;

        #endregion

        #region Properties

        public String TimeLimitInput { get; set; }

        public String MaxStepsInput { get; set; }

        public Game CurrentGame { get { return currentGame; } }

        public ObservableCollection<Player> Players { get; private set; }

        #endregion

        #region Methods

        public GameUI() {
            InitializeComponent();
            TimeLimitInput = "2.0";
            MaxStepsInput = "500";
            currentGame = new Games.GuessTheNumber();
            Players = new ObservableCollection<Player>();
            Players.Add(new Player());
            Players.Add(new Player());
            DataContext = this;
        }

        private void selectPlayerHandler(Object sender, RoutedEventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "Executable files (.exe)|*.exe";
            if (dlg.ShowDialog() == true) {
                Int32 selected = Math.Max(playerList.SelectedIndex, 0), cnt, n = currentGame.Players, i;
                if (n > 0) {
                    for (cnt = i = 0; cnt < n && i < dlg.FileNames.Length; ++cnt, ++i) {
                        Players[(selected + i) % n].Path = dlg.FileNames[i];
                    }
                }
            }
        }

        private void playHandler(Object sender, RoutedEventArgs e) {

        }

        #endregion

    }

}
