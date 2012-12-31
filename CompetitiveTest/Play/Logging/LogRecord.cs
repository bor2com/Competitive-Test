namespace SSU.CompetitiveTest.Play.Logging {

    using System;

    public sealed class LogRecord {

        #region Fields

        private readonly String message;

        private readonly RecordClass recordClass;

        private readonly TimeSpan? runningTime;

        private readonly DateTime timeStamp;

        #endregion

        #region Properties

        public String Message { get { return message; } }

        public RecordClass RecordClass { get { return recordClass; } }

        public TimeSpan? RunningTime { get { return runningTime; } }

        public DateTime TimeStamp { get { return timeStamp; } }

        #endregion

        #region Methods

        public LogRecord(String message, RecordClass recordClass, TimeSpan? runningTime, DateTime timeStamp) {
            this.message = message;
            this.recordClass = recordClass;
            this.runningTime = runningTime;
            this.timeStamp = timeStamp;
        }

        #endregion

    }

}
