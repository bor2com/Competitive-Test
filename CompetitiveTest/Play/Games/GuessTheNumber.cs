namespace SSU.CompetitiveTest.Play.Games {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public sealed class GuessTheNumber : Game {

        private struct PlayerInfo {
            public Int32 From, To, Place;
        }

        #region Fields

        private readonly Int32 playerCount = 2;

        #endregion

        #region Properties

        public override String Name {
            get {
                return "Вгадай число!";
            }
        }

        public override String Description {
            get {
                return @"Суддя загадує число, а гравець мусить його вгадати. На кожному кроці програма гравця отримує два числа - проміжок у якому знаходиться загадане число. Програма мусить виписати будь-яке число у цьому проміжку. Виграє той з гравців, хто перший вгадає загадане число.";
            }
        }

        public override Int32 PlayerCount {
            get {
                return playerCount;
            }
        }

        public override Int32 MaxSteps {
            get {
                return 100;
            }
        }

        public override TimeSpan TimeLimit {
            get {
                return TimeSpan.FromSeconds(5);
            }
        }

        #endregion

        #region Methods

        public GuessTheNumber() { }

        protected override String SpecificPlay() {
            Boolean noneLeft;
            ChatRecord sample;
            Int32 step, player, from, to, secret, response;
            PlayerInfo[] info = new PlayerInfo[PlayerCount];
            Communicator current;
            #region Subject to improve
            secret = rnd.Next(1000) + 1000;
            from = secret - rnd.Next(1000);
            to = secret + rnd.Next(1000);
            #endregion
            for (player = 0; player < PlayerCount; ++player) {
                info[player].From = from;
                info[player].To = to;
            }
            for (step = 0; step < MaxSteps; ++step) {
                noneLeft = true;
                for (player = 0; player < PlayerCount; ++player) {
                    if (info[player].Place == 0) {
                        current = Players[player];
                        noneLeft = false;
                        sample = current.Pull(String.Format("{0} {1}", info[player].From, info[player].To), TimeLimit);
                        if (sample.Resolution == RecordType.PlayerToJudge) {
                            if (Int32.TryParse(sample.Message, out response) && response >= info[player].From && response <= info[player].To) {
                                if (response == secret) {
                                    sample.Resolution = RecordType.PlayerCorrectAnswer;
                                    info[player].Place = 1;
                                } else {
                                    if (response > secret) {
                                        info[player].To = response - 1;
                                    } else {
                                        info[player].From = response + 1;
                                    }
                                }
                            } else {
                                sample.Resolution = RecordType.PlayerWrongOutputFormat;
                            }
                        }
                        if (sample.Resolution == RecordType.PlayerCrush
                            || sample.Resolution == RecordType.PlayerTimeOut
                            || sample.Resolution == RecordType.PlayerWrongOutputFormat) {
                            info[player].Place = -1;
                        }
                        current.Log.Add(sample);
                        if (info[player].Place == 1) {
                            return current.Name;
                        }
                    }
                }
                if (noneLeft) {
                    break;
                }
            }
            return null;
        }

        #endregion

    }

}
