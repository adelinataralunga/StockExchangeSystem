namespace StockExchangeSystem.Domain
{
    public class InvalidTradeDataException : Exception
    {
        public InvalidTradeDataException() : base() { }
        public InvalidTradeDataException(string message) : base(message) { }
    }

    public class StockNotFoundException : Exception
    {
        public StockNotFoundException() : base() { }
        public StockNotFoundException(string message) : base(message) { }
    }

    public class TradeNotFoundException : Exception
    {
        public TradeNotFoundException() : base() { }
        public TradeNotFoundException(string message) : base(message) { }
    }

    public class BrokerNotFoundException : Exception
    {
        public BrokerNotFoundException() : base() { }
        public BrokerNotFoundException(string message) : base(message) { }
    }

    // Represents an error when a user attempts to access a resource they are not permitted to.
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException() : base() { }
        public AccessDeniedException(string message) : base(message) { }
    }

    // Represents an error when a user exceeds a specified resource limit, e.g., API call limits.
    public class ResourceLimitExceededException : Exception
    {
        public ResourceLimitExceededException() : base() { }
        public ResourceLimitExceededException(string message) : base(message) { }
    }
}
