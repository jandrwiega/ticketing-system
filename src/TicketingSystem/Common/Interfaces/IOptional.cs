namespace TicketingSystem.Common.Interfaces
{
    public interface IOptional
    {
        bool IsPresent { get; }
        object? Value { get; }
    }
}
