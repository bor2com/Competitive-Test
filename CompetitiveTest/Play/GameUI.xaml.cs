namespace SSU.CompetitiveTest.Play {

  using System;
  using System.Threading;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Controls;
  using Microsoft.Win32;

  public partial class GameUI : UserControl {

    #region Fields

    private Game currentGame;

    private Player[] players;

    #endregion

    #region Properties

    public String TimeLimitInput { get; set; }

    public String MaxStepsInput { get; set; }

    public Game CurrentGame { get { return currentGame; } }

    public Player[] Players { get { return players; } }

    #endregion

    #region Methods

    public GameUI() {
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
      InitializeComponent();
      TimeLimitInput = "2.0";
      MaxStepsInput = "500";
      currentGame = new Games.GuessTheNumber();
      players = new Player[currentGame.Players];
      for (Int32 i = 0; i < currentGame.Players; ++i) {
        players[i] = new Player(Dispatcher);
      }
      DataContext = this;
    }

    private Boolean CheckPlayers() {
      foreach (Player p in players) {
        if (p == null || !p.IsReady) {
          playButton.IsEnabled = false;
          return false;
        }
      }
      return true;
    }

    private void selectPlayerHandler(Object sender, RoutedEventArgs e) {
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.Multiselect = true;
      dlg.Filter = "Executable files (.exe)|*.exe";
      if (dlg.ShowDialog() == true) {
        Int32 selected = Math.Max(playerList.SelectedIndex, 0), cnt, n = currentGame.Players, i;
        if (n > 0) {
          for (cnt = i = 0; cnt < n && i < dlg.FileNames.Length; ++cnt, ++i) {
            players[(selected + i) % n].Path = dlg.FileNames[i];
          }
        }
      }
      playButton.IsEnabled = CheckPlayers();
    }

    private void playHandler(Object sender, RoutedEventArgs e) {
      Int32 maxSteps;
      Double timeLimitSeconds;
      if (!Int32.TryParse(MaxStepsInput, out maxSteps)) {
        MessageBox.Show("Wrong Max Steps format");
      } else if (!Double.TryParse(TimeLimitInput, out timeLimitSeconds)) {
        MessageBox.Show("Wrong Time Limit format " + TimeLimitInput);
      } else if (CheckPlayers()) {
        CurrentGame.BeginPlay(players, maxSteps, TimeSpan.FromSeconds(timeLimitSeconds));
      } else {
        playButton.IsEnabled = false;
      }
    }

    #endregion

  }

}
