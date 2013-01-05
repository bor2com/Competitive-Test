namespace SSU.CompetitiveTest.Play.Logging {

    using System;

    public enum RecordClass : byte {

        Error = 1,

        PlayerToJudge,

        JudgeToPlayer,

        Notification,

        Congratulation

    }

}
