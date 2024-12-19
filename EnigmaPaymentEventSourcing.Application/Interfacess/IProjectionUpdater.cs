namespace EnigmaPaymentEventSourcing.Application.Interfacess
{
    public interface IProjectionUpdater
    {
        void UpdateProjection(IEnumerable<object> events);
    }
}
