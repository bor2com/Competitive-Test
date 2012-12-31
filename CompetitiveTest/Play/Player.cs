namespace SSU.CompetitiveTest.Play {

    using System;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using SSU.CompetitiveTest.Play.Logging;
    using SSU.CompetitiveTest.Play.Communicating;

    public sealed class Player : INotifyPropertyChanged {

        #region Fields

        private String status;

        private readonly ObservableCollection<LogRecord> log = new ObservableCollection<LogRecord>();

        private readonly Communicator communicator;

        #endregion

        #region Properties

        public String Name { get { return communicator.Name; } }

        public String Status {
            get {
                return status;
            }
            set {
                status = value;
                RaisePropertyChanged("Status");
            }
        }

        #endregion

        #region Methods

        public Player(String executablePath) {
            communicator = new Communicator(executablePath);
            status = "Ready";
        }

        /// <summary>
        /// Runs the report of 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="timeLimit"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public LogRecord Turn(String input, TimeSpan timeLimit) {
            LogRecord record;
            try {
                RunOutcome ro = communicator.Run(input, timeLimit);
                record = new LogRecord(ro.Output, RecordClass.Description, ro.Elapsed, ro.TimeStamp);
            } catch (RuntimeErrorException ex) {
                record = new LogRecord(String.Format("RE {0}", ex.ErrorCode), RecordClass.Error, null, DateTime.Now);
            } catch (TimeLimitException) {
                record = new LogRecord("TL", RecordClass.Error, timeLimit, DateTime.Now);
            }
            log.Add(record);
            return record;
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
