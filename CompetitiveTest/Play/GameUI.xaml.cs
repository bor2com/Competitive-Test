namespace SSU.CompetitiveTest.Play {

  using System;
  using System.Threading;
  using System.Globalization;
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;
  using Microsoft.Win32;
  using System.Windows.Controls.Primitives;

  public partial class GameUI : UserControl, INotifyPropertyChanged {

    #region Fields

    private Game currentGame;

    private Player[] players;

    private GameState state = GameState.SelectPlayers;

    private Boolean toggleButtonPressed = false;

    #endregion

    #region Properties

    public String TimeLimitInput { get; set; }

    public String MaxStepsInput { get; set; }

    public Game CurrentGame { get { return currentGame; } }

    public Player[] Players { get { return players; } }

    public GameState State {
      get {
        return state;
      }
      set {
        if (state != value) {
          switch (state = value) {
            case GameState.Running:
              ToggleButtonPressed = true;
              break;
            default :
              ToggleButtonPressed = false;
              break;
          }
          RaisePropertyChanged("State");
        }
      }
    }

    public Boolean ToggleButtonPressed {
      get {
        return toggleButtonPressed;
      }
      set {
        if (toggleButtonPressed != value) {
          toggleButtonPressed = value;
          RaisePropertyChanged("ToggleButtonPressed");
        }
      }
    }

    #endregion

    #region Methods

    public GameUI() {
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
      InitializeComponent();
      TimeLimitInput = "2.0";
      MaxStepsInput = "500";
      currentGame = new Games.GuessTheNumber();
      currentGame.GameComplete += gameCompleteHandler;
      players = new Player[currentGame.Players];
      for (Int32 i = 0; i < currentGame.Players; ++i) {
        players[i] = new Player(Dispatcher);
      }
      DataContext = this;
    }

    void gameCompleteHandler(Object sender, RunWorkerCompletedEventArgs e) {
      State = GameState.Ready;
    }

    private void checkPlayers() {
      foreach (Player p in players) {
        if (p == null || !p.IsReady) {
          State = GameState.SelectPlayers;
          return;
        }
      }
      State = GameState.Ready;
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
      checkPlayers();
    }

    private void playHandler(Object sender, RoutedEventArgs e) {
      ToggleButton button = sender as ToggleButton;
      if (button.IsChecked.Value) {
        Int32 maxSteps;
        Double timeLimitSeconds;
        if (!Int32.TryParse(MaxStepsInput, out maxSteps)) {
          MessageBox.Show(String.Format("Wrong MaxSteps format: \"{0}\"", MaxStepsInput), "Can\'t run the game", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        } else if (!Double.TryParse(TimeLimitInput, out timeLimitSeconds)) {
          MessageBox.Show(String.Format("Wrong Time Limit format: \"{0}\"", TimeLimitInput), "Can\'t run the game", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        } else {
          checkPlayers();
          if (state == GameState.Ready) {
            CurrentGame.BeginPlay(players, maxSteps, TimeSpan.FromSeconds(timeLimitSeconds));
            State = GameState.Running;
          }
        }
      } else {
        currentGame.CalcelPlay();
        State = GameState.Ready;
      }
    }

    private void RaisePropertyChanged(String pname) {
      if (PropertyChanged != null) {
        PropertyChanged(this, new PropertyChangedEventArgs(pname));
      }
    }

    #endregion

    #region Events
  
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

  }

}
