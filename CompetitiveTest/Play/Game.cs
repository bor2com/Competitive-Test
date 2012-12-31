namespace SSU.CompetitiveTest.Play {

    using System;

    public abstract class Game {

        #region Properties

        public abstract String Name { get; }

        public abstract String Description { get; }

        /// <summary>
        /// Required number of players
        /// </summary>
        public abstract Int32 Players { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Run the game with specified players and settings.
        /// </summary>
        /// <param name="players">Array of players</param>
        /// <param name="maxSteps">Game steps limit</param>
        /// <param name="timeLimit">Execution time limit for a single game step</param>
        /// <returns>Player which wins the game or null in case of a draw</returns>
        /// <exception cref="ArgumentNullException">Neither <c>players</c> nor it's element can be null</exception>
        /// <exception cref="ArgumentException">The <c>players</c> element count should match <c>Players</c> and <c>maxSteps</c> must be a positive integer</exception>
        public Player Play(Player[] players, Int32 maxSteps, TimeSpan timeLimit) {
            if (players == null || Array.IndexOf(players, null) >= 0) {
                throw new ArgumentNullException("players", "The array and any element can\'t be null");
            }
            if (Players != players.Length) {
                throw new ArgumentException(String.Format("Invalid number of players. Expected {0}, passed {1}", Players, players.Length), "players");
            }
            if (maxSteps <= 0) {
                throw new ArgumentException(String.Format("The argument must be a positive integer, passed {0}", maxSteps), "maxSteps");
            }
            return BeginPlay(players, maxSteps, timeLimit);
        }

        protected abstract Player BeginPlay(Player[] players, Int32 maxSteps, TimeSpan timeLimit);

        #endregion

    }

}
