namespace SSU.CompetitiveTest.Play.Games {

  using System;
  using System.Diagnostics;
  using System.ComponentModel;
  using SSU.CompetitiveTest.Play.Logging;

  public sealed class GuessTheNumber : Game {

    #region Structs

    private class PlayerInfo {
      public Int32 From, To;
      public Boolean Done = false, InGame = true;
      public readonly Player Player;
      public PlayerInfo(Int32 from, Int32 to, Player player) {
        From = from;
        To = to;
        Player = player;
      }
    }

    #endregion

    #region Properties

    public override string Name {
      get { return "Guess the number"; }
    }

    public override string Description {
      get { return "Players receive a range of positive integers and try to guess a single number among them"; }
    }

    public override int Players {
      get { return 2; }
    }

    #endregion

    #region Methods

    protected override Player ActualPlay(Player[] players, Int32 maxSteps, TimeSpan timeLimit) {
      Int32 from, to, secret, i;
      #region Subject to improve
      secret = rnd.Next(1000) + 1000;
      from = secret - rnd.Next(1000);
      to = secret + rnd.Next(1000);
      #endregion
      PlayerInfo[] pinfo = new PlayerInfo[Players];
      for (i = 0; i < Players; ++i) {
        pinfo[i] = new PlayerInfo(from, to, players[i]);
        players[i].AddRecord(LogRecord.Make("Beginning of the game", RecordClass.Notification));
      }
      Int32 answer, done = 0, activePlayers = Players;
      for (i = 0; i < maxSteps && activePlayers != 0 && done == 0; ++i) {
        if (CancellationPending) {
          break;
        }
        activePlayers = 0;
        foreach (PlayerInfo p in pinfo) {
          if (p.InGame) {
            ++activePlayers;
            String passed = String.Format("{0} {1}", p.From, p.To);
            LogRecord response = p.Player.Turn(passed, timeLimit);
            if (response.RecordClass == RecordClass.PlayerToJudge) {
              if (Int32.TryParse(response.Message, out answer) && answer >= p.From && answer <= p.To) {
                if (answer == secret) {
                  ++done;
                  p.Done = true;
                } else {
                  if (secret > answer) {
                    p.From = answer + 1;
                  } else {
                    p.To = answer - 1;
                  }
                }
              } else {
                p.InGame = false;
                p.Player.AddRecord(LogRecord.Make("Wrong output format", RecordClass.Notification));
              }
            } else {
              p.InGame = false;
              p.Player.AddRecord(LogRecord.Make("Out of the game", RecordClass.Notification));
            }
          }
        }
      }
      if (done != 0) {
        foreach (PlayerInfo p in pinfo) {
          if (p.Done) {
            if (done == 1) {
              p.Player.AddRecord(LogRecord.Make("Winner!", RecordClass.Congratulation));
            } else {
              p.Player.AddRecord(LogRecord.Make("Draw", RecordClass.Notification));
            }
          } else if (p.InGame) {
            p.Player.AddRecord(LogRecord.Make("The game has been ended", RecordClass.Notification));
          }
        }
        if (done == 1) {
          foreach (PlayerInfo p in pinfo) {
            if (p.Done) {
              return p.Player;
            }
          }
        }
      } else {
        foreach (PlayerInfo p in pinfo) {
          if (p.InGame) {
            if (CancellationPending) {
              p.Player.AddRecord(LogRecord.Make("Game has been cancelled", RecordClass.Notification));
            } else {
              p.Player.AddRecord(LogRecord.Make("Steps limit has been reached", RecordClass.Notification));
            }
          }
        }
      }
      return null;
    }

    #endregion

  }

}
