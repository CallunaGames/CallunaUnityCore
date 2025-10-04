namespace Calluna.Process
{
    public interface ControllableProcess : Process
    {
        public void Start();
        public void Tick();
        public void Abort();
    }
}
