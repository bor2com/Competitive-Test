namespace SSU.CompetitiveTest.Play {

    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Diagnostics;

    public sealed class Communicator : INotifyPropertyChanged {

        #region Fields

        private String path = null;

        private String name;

        private ObservableCollection<ChatRecord> log = new ObservableCollection<ChatRecord>();

        private ProcessStartInfo startInfo = new ProcessStartInfo();

        private Boolean isReady;

        private String condition;

        #endregion

        #region Properties

        public String Path {
            get {
                return path;
            }
            set {
                if (path != value) {
                    path = value;
                    RaisePropertyChanged("Path");
                    IsReady = File.Exists(path) && path.ToLower().EndsWith(".exe");
                    if (isReady) {
                        Int32 slash = value.LastIndexOf('\\') + 1;
                        Name = value.Substring(slash, value.LastIndexOf('.') - slash);
                        startInfo.FileName = value;
                    }
                }
            }
        }

        public String Name {
            get {
                return name;
            }
            set {
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        public ObservableCollection<ChatRecord> Log {
            get {
                return log;
            }
        }

        public Boolean IsReady {
            get {
                return isReady;
            }
            private set {
                isReady = value;
                RaisePropertyChanged("IsReady");
                if (!isReady) {
                    Condition = "Виберіть програму";
                }
            }
        }

        public String Condition {
            get {
                return condition;
            }
            set {
                condition = value;
                RaisePropertyChanged("Condition");
            }
        }

        #endregion

        #region Methods

        public Communicator(String name) {
            this.name = name;
            IsReady = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.ErrorDialog = false;
        }

        public ChatRecord Pull(String incoming, TimeSpan timeLimit) {
            DateTime begin, end;
            ChatRecord response = null;
            try {
                using (var p = new Process() { StartInfo = startInfo }) {
                    if (p.Start()) {
                        p.StandardInput.WriteLine(incoming);
                        p.StandardInput.Flush();
                        begin = DateTime.Now;
                        p.WaitForExit((Int32)timeLimit.TotalMilliseconds);
                        end = DateTime.Now;
                        log.Add(new ChatRecord(incoming, RecordType.JudgeToPlayer));
                        TimeSpan elapsed = end - begin;
                        if (p.HasExited) {
                            response = p.ExitCode == 0
                                ? new ChatRecord(p.StandardOutput.ReadToEnd(), RecordType.PlayerToJudge, elapsed)
                                : new ChatRecord(null, RecordType.PlayerCrush, elapsed);
                        } else {
                            p.Kill();
                            response = new ChatRecord(null, RecordType.PlayerTimeOut, elapsed);
                        }
                    } else {
                        throw new InvalidOperationException();
                    }
                }
            } catch (Exception ) {
                IsReady = false;
            }
            return response;
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
