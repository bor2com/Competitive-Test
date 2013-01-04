namespace SSU.CompetitiveTest.Play.Games {

    using System;
    using System.Diagnostics;
    using SSU.CompetitiveTest.Play.Logging;

    public sealed class GuessTheNumber : Game {

        #region Structs

        private struct PlayerInfo {
            public Int32 From, To;
            public Boolean Done, OutOfGame;
        }

        #endregion

        #region Properties

        public override string Name {
            get { return "Guess the number"; }
        }

        public override string Description {
            get { return "Players receive a range of positive integers and try to guess a single number among them"; }
        }

        public override int Players {
            get { return 2; }
        }

        #endregion

        #region Methods

        protected override Player ActualPlay(Player[] players, Int32 maxSteps, TimeSpan timeLimit) {
            Int32 from, to, secret, i;
            #region Subject to improve
            secret = rnd.Next(1000) + 1000;
            from = secret - rnd.Next(1000);
            to = secret + rnd.Next(1000);
            #endregion
            PlayerInfo[] pinfo = new PlayerInfo[Players];
            for (i = 0; i < Players; ++i) {
                pinfo[i].From = from;
                pinfo[i].To = to;
                players[i].Log.Add(LogRecord.Notification("Beginning of the game"));
            }
            Int32 answer, done = 0, active = Players;
            Boolean draw;
            for (Int32 step = 0; step < maxSteps && active != 0 && done == 0; ++step) {
                for (active = i = 0; i < Players; ++i) {
                    if (!pinfo[i].OutOfGame) {
                        ++active;
                        Debug.Assert(pinfo[i].From <= pinfo[i].To);
                        LogRecord one = players[i].Turn(String.Format("{0} {1}", pinfo[i].From, pinfo[i].To), timeLimit);
                        if (one.RecordClass == RecordClass.Description) {
                            if (Int32.TryParse(one.Message, out answer) && answer >= pinfo[i].From && answer <= pinfo[i].To) {
                                if (answer == secret) {
                                    ++done;
                                    pinfo[i].Done = true;
                                } else {
                                    if (secret > answer) {
                                        pinfo[i].From = answer + 1;
                                    } else {
                                        pinfo[i].To = answer - 1;
                                    }
                                }
                            } else {
                                pinfo[i].OutOfGame = true;
                                players[i].Log.Add(LogRecord.Notification("Wrong output format"));
                            }
                        } else {
                            pinfo[i].OutOfGame = true;
                            players[i].Log.Add(LogRecord.Notification("Out of the game"));
                        }
                    }
                }
            }
            if (done != 0) {
                draw = done != 1;
                Int32 last = -1;
                for (i = 0; i < Players; ++i) {
                    if (pinfo[i].Done) {
                        last = i;
                        players[i].Log.Add(LogRecord.Notification(draw ? "Draw" : "Winner!"));
                    } else if (!pinfo[i].OutOfGame) {
                        players[i].Log.Add(LogRecord.Notification("The game has been ended"));
                    }
                }
                if (!draw) {
                    return players[last];
                }
            }
            return null;
        }

        #endregion

    }

}
