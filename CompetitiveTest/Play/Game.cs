﻿namespace SSU.CompetitiveTest.Play {

  using System;
  using System.ComponentModel;

  public abstract class Game {

    #region Class

    private class GameInputData {
      public readonly Player[] Players;
      public readonly Int32 MaxSteps;
      public readonly TimeSpan TimeLimit;
      public GameInputData(Player[] players, Int32 maxSteps, TimeSpan timeLimit) {
        Players = players;
        MaxSteps = maxSteps;
        TimeLimit = timeLimit;
      }
    }

    #endregion

    #region Fields

    protected static Random rnd = new Random();

    private BackgroundWorker worker = new BackgroundWorker() {
      WorkerSupportsCancellation = true
    };

    private Boolean cancellationPending;

    #endregion

    #region Properties

    public abstract String Name { get; }

    public abstract String Description { get; }

    /// <summary>
    /// Required number of players
    /// </summary>
    public abstract Int32 Players { get; }

    protected Boolean CancellationPending {
      get {
        lock (this) {
          return cancellationPending;
        }
      }
      private set {
        lock (this) {
          cancellationPending = value;
        }
      }
    }

    #endregion

    #region Methods

    public Game() {
      worker.DoWork += backgroundWork;
    }

    /// <summary>
    /// Run the game asynchronously with specified players and settings. Afterwards writes either pointer to winner or null in case of a draw to the Result.
    /// </summary>
    /// <param name="players">Array of players</param>
    /// <param name="maxSteps">Game steps limit</param>
    /// <param name="timeLimit">Execution time limit for a single game step</param>
    /// <exception cref="ArgumentNullException">Neither <c>players</c> nor it's element can be null</exception>
    /// <exception cref="ArgumentException">The <c>players</c> element count should match <c>Players</c> and <c>maxSteps</c> must be a positive integer</exception>
    /// <exception cref="InvalidOperationException">When the game already has been started</exception>
    public void BeginPlay(Player[] players, Int32 maxSteps, TimeSpan timeLimit) {
      if (players == null || Array.IndexOf(players, null) >= 0) {
        throw new ArgumentNullException("players", "The array and any element can\'t be null");
      }
      if (Players != players.Length) {
        throw new ArgumentException(String.Format("Invalid number of players. Expected {0}, passed {1}", Players, players.Length), "players");
      }
      if (maxSteps <= 0) {
        throw new ArgumentException(String.Format("The argument must be a positive integer, passed {0}", maxSteps), "maxSteps");
      }
      if (worker.IsBusy) {
        throw new InvalidOperationException("The game is still in progress");
      }
      worker.RunWorkerAsync(new GameInputData(players, maxSteps, timeLimit));
      worker.CancelAsync();
    }

    public void CalcelPlay() {
      if (worker.WorkerSupportsCancellation && worker.IsBusy) {
        CancellationPending = true;
      }
    }

    private void backgroundWork(Object sender, DoWorkEventArgs e) {
      cancellationPending = false;
      GameInputData data = e.Argument as GameInputData;
      e.Result = ActualPlay(data.Players, data.MaxSteps, data.TimeLimit);
    }

    protected abstract Player ActualPlay(Player[] players, Int32 maxSteps, TimeSpan timeLimit);

    #endregion

    #region Events

    public event RunWorkerCompletedEventHandler GameComplete {
      add {
        worker.RunWorkerCompleted += value;
      }
      remove {
        worker.RunWorkerCompleted -= value;
      }
    }

    #endregion

  }

}
