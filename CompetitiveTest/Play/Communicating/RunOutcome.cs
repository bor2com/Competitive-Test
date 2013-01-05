namespace SSU.CompetitiveTest.Play.Communicating {

  using System;

  public sealed class RunOutcome {

    #region Fields

    private readonly String output;

    private readonly TimeSpan elapsed;

    private readonly DateTime timeStamp;

    #endregion

    #region Properties

    public String Output { get { return output; } }

    public TimeSpan Elapsed { get { return elapsed; } }

    public DateTime TimeStamp { get { return timeStamp; } }

    #endregion

    #region Methods

    public RunOutcome(String output, TimeSpan elapsed, DateTime timeStamp) {
      this.output = output;
      this.elapsed = elapsed;
      this.timeStamp = timeStamp;
    }

    #endregion

  }

}
