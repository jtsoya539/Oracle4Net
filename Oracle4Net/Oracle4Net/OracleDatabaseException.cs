using System;
using Serilog;

namespace Oracle4Net
{

    [System.Serializable]
    public class OracleDatabaseException : System.Exception
    {

        public int Number { get; private set; }
        public OracleDatabaseErrorKind Kind { get; private set; }
        private ILogger logger;

        public OracleDatabaseException() : base("Oracle Database Exception") { }

        public OracleDatabaseException(string message) : base(message) { }

        public OracleDatabaseException(string message, System.Exception inner) : base(message, inner) { }

        public OracleDatabaseException(string message, int number) : base(message)
        {
            this.logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            this.Number = number;
            if (this.Number >= -20999 && this.Number <= -20000)
            {
                this.Kind = OracleDatabaseErrorKind.UserDefinedError;
                logger.Error(this.Message);
            }
            else
            {
                this.Kind = OracleDatabaseErrorKind.OraclePredefinedError;
                logger.Fatal(this.Message);
            }
        }

        protected OracleDatabaseException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
