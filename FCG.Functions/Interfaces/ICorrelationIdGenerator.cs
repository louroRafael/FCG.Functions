namespace FCG.Functions.Interfaces;

public interface ICorrelationIdGenerator
{
    string Get();
    void Set(string correlationId);
}
