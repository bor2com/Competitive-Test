namespace SSU.CompetitiveTest.Play {

    using System;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using SSU.CompetitiveTest.Play.Logging;
    using SSU.CompetitiveTest.Play.Communicating;

    public sealed class Player : INotifyPropertyChanged {

        #region Fields

        private String status = "Select program";

        private readonly ObservableCollection<LogRecord> log = new ObservableCollection<LogRecord>();

        private Communicator communicator = null;

        #endregion

        #region Properties

        public String Name { get { return communicator.Name; } }

        public String Status {
            get {
                return status;
            }
            private set {
                status = value;
                RaisePropertyChanged("Status");
            }
        }
        
        public Boolean IsReady { get; private set; }

        public String Path {
            set {
                try {
                    communicator = new Communicator(value);
                    IsReady = true;
                    Status = "Ready";
                    RaisePropertyChanged("Name");
                } catch (Exception ex) {
                    communicator = null;
                    IsReady = false;
                    Status = "Select program";
                }
            }
        }

        public ObservableCollection<LogRecord> Log { get { return log; } }

        #endregion

        #region Methods

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
                record = LogRecord.Error(String.Format("RE {0}", ex.ErrorCode));
            } catch (TimeLimitException) {
                record = LogRecord.Error("TL");
            } catch (Exception) {
                record = LogRecord.Error("Communicator has failed");
                Path = null;
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
