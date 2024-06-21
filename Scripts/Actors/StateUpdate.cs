namespace FPTemplate.Actors
{
    public struct StateUpdate<T>
    {
        public string StateKey;
        public T Delta;
        public T Value;
        public string Description;
        public bool Success;

        public StateUpdate(string key, string desc, T val, T delta, bool success)
        {
            StateKey = key;
            Value = val;
            Delta = delta;
            Description = desc;
            Success = success;
        }
    }
}