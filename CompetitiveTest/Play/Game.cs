namespace SSU.CompetitiveTest.Play {

    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;

    public abstract class Game : INotifyPropertyChanged {

        #region Fields

        private Boolean isReady;

        static protected Random rnd = new Random();

        #endregion

        #region Properties

        public abstract String Name { get; }

        public abstract String Description { get; }

        public abstract Int32 PlayerCount { get; }

        public abstract Int32 MaxSteps { get; }

        public abstract TimeSpan TimeLimit { get; }

        public ObservableCollection<Communicator> Players { get; private set; }

        public Boolean IsReady {
            get {
                return isReady;
            }
            set {
                if (isReady != value) {
                    isReady = value;
                    RaisePropertyChanged("IsReady");
                }
            }
        }

        #endregion

        #region Methods

        protected Game() {
            Players = new ObservableCollection<Communicator>();
            for (Int32 i = 1; i <= PlayerCount; ++i) {
                var com = new Communicator("Програма " + i);
                com.PropertyChanged += playerPropertyChanged;
                Players.Add(com);
            }
        }

        private void playerPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "IsReady") {
                Boolean newIsReady = true;
                foreach (var com in Players) {
                    if (!com.IsReady) {
                        newIsReady = false;
                        break;
                    }
                }
                IsReady = newIsReady;
            }
        }

        protected void RaisePropertyChanged(String pname) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(pname));
            }
        }

        /// <summary>
        /// Guides a single game between choosen players.
        /// </summary>
        /// <returns>Winning program's name.</returns>
        /// <exception cref="InvalidOperationException">
        /// In case there are no necessary amount of programs.
        /// </exception>
        public String Play() {
            if (!IsReady) {
                throw new InvalidOperationException("There are not necessary amount of players");
            }
            /* Similar aliases check */
            Boolean same = false;
            for (Int32 j, i = 0, n = PlayerCount; i < n; ++i) {

                for (j = 0; j < i; ++j) {
                    if (Players[i].Name == Players[j].Name) {
                        same = true;
                        i = n;
                    }
                }
            }
            if (same) {
                switch (MessageBox.Show("Серед гравців є однакові імена програм. Продовжити?", "Знайдено однакові імена", MessageBoxButton.YesNo, MessageBoxImage.Question)) {
                    case MessageBoxResult.OK:
                    case MessageBoxResult.Yes:
                        break;
                    default:
                        return null;
                }
            }
            return SpecificPlay();
        }

        protected abstract String SpecificPlay();

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }

}
