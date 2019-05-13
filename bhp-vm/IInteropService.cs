namespace Bhp.VM
{
    public interface IInteropService
    {
        bool Invoke(uint method, ExecutionEngine engine);
    }
}
