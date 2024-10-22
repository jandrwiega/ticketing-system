namespace TicketingSystem.Core.Interfaces
{
    public interface IOptional
    {
        bool IsPresent { get; }
        object? Value { get; }
    }
}
