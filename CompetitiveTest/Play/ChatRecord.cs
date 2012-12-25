namespace SSU.CompetitiveTest.Play {

    using System;

    public sealed class ChatRecord {

        #region Fields

        public String Message { get; set; }

        public RecordType Resolution { get; set; }

        public TimeSpan WorkingTime { get; private set; }

        #endregion

        #region Methods

        public override string ToString() {
            String comment;
             switch (Resolution) {
                case RecordType.PlayerToJudge:
                case RecordType.JudgeToPlayer:
                    return Message;
                case RecordType.PlayerCrush:
                    comment = "Програма завершилась з помилкою";
                    break;
                case RecordType.PlayerTimeOut:
                    comment = "Програма перевищила ліміт часу";
                    break;
                case RecordType.PlayerWrongOutputFormat:
                    comment = "Формат відповіді не правильний";
                    break;
                case RecordType.PlayerCorrectAnswer:
                    comment = "Програма дала правильну відповідь";
                    break;
                default:
                    throw new ArgumentException();
            }
            comment = (Message == null ? String.Empty : Message.Trim()) + Environment.NewLine + comment;
            return comment;
        }

        public ChatRecord(String message, RecordType resolution) {
            this.Message = message;
            this.Resolution = resolution;
        }

        public ChatRecord(String message, RecordType resolution, TimeSpan workingTime) : this(message, resolution){
            this.WorkingTime = workingTime;
        }

        #endregion

    }

}
