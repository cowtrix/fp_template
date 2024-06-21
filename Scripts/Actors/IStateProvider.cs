namespace FPTemplate.Actors
{
    public interface IStateProvider
	{
		string GUID { get; }
		string GetSaveData();
		void LoadSaveData(string data);
	}
}