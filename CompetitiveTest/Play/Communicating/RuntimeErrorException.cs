namespace SSU.CompetitiveTest.Play.Communicating {

    using System;

    [global::System.Serializable]
    public sealed class RuntimeErrorException : Exception {

        #region Fields

        private readonly Int32 errorCode;

        #endregion

        #region Properties

        public Int32 ErrorCode { get { return errorCode; } }

        #endregion

        #region Methods

        public RuntimeErrorException(Int32 errorCode) { this.errorCode = errorCode; }
        public RuntimeErrorException(Int32 errorCode, String message) : this(errorCode, message, null) { }
        public RuntimeErrorException(Int32 errorCode, String message, Exception inner) : base(message, inner) { this.errorCode = errorCode; }

        #endregion

    }

}
