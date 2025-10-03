namespace Calluna.Process
{
    public interface MutableProcess : Process
    {
        public void Start();
        public void Tick();
        public void Abort();
    }
}
