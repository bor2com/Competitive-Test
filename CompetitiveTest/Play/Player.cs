namespace SSU.CompetitiveTest.Play {

    using System;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using SSU.CompetitiveTest.Play.Logging;
    using SSU.CompetitiveTest.Play.Communicating;
    using System.Windows.Threading;
    using System.Threading;

    public sealed class Player : INotifyPropertyChanged {

        #region Fields

        private String status = "Select program";

        private readonly ObservableCollection<LogRecord> log = new ObservableCollection<LogRecord>();

        private readonly Dispatcher dispatcher;

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

        public void AddAsyncRecord(LogRecord record) {
            dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)(() => log.Add(record)));
        }

        public Player(Dispatcher dispatcher) {
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// Makes a single turn utilizing the communicator and recording results
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="timeLimit">Time limit</param>
        /// <returns>Log record with the execution outcome</returns>
        public LogRecord Turn(String input, TimeSpan timeLimit) {
            AddAsyncRecord(LogRecord.Make(input, RecordClass.JudgeToPlayer));
            LogRecord record;
            try {
                RunOutcome ro = communicator.Run(input, timeLimit);
                record = new LogRecord(ro.Output.Trim(), RecordClass.PlayerToJudge, ro.Elapsed, ro.TimeStamp);
                Status = "OK";
            } catch (RuntimeErrorException ex) {
                record = LogRecord.Make(String.Format("Runtime Error #{0}", ex.ErrorCode), RecordClass.Error);
                Status = "RE";
            } catch (TimeLimitException) {
                record = LogRecord.Make("Time Limit has been exceeded", RecordClass.Error);
                Status = "TL";
            } catch (Exception) {
                record = LogRecord.Make("Communication has failed", RecordClass.Error);
                Path = null;
            }
            AddAsyncRecord(record);
            return record;
        }

        private void RaisePropertyChanged(String pname) {
            dispatcher.BeginInvoke(DispatcherPriority.DataBind, (ThreadStart)delegate() {
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs(pname));
                }
            });
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }

}
