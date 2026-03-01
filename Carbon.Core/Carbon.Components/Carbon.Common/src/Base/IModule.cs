namespace Carbon.Base.Interfaces;

public interface IModule : IDisposable
{
	string Name { get; }

	void Init();
	bool InitEnd();
	void Save();
	void Load();
	void Reload();
	void OnUnload();
	void Shutdown();

	void OnServerInit(bool initial);
	void OnPostServerInit(bool initial);
	void OnServerSaved();
	void SetEnabled(bool enabled);
	bool IsEnabled();
	void OnEnableStatus();

	Dictionary<string, Dictionary<string, string>> GetDefaultPhrases();

	void OnEnabled(bool initialized);
	void OnDisabled(bool initialized);
}
