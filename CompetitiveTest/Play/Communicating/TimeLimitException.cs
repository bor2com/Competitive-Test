namespace SSU.CompetitiveTest.Play.Communicating {

  using System;

  [global::System.Serializable]
  public sealed class TimeLimitException : Exception {
    public TimeLimitException() { }
    public TimeLimitException(String message) : base(message) { }
    public TimeLimitException(String message, Exception inner) : base(message, inner) { }
  }

}
