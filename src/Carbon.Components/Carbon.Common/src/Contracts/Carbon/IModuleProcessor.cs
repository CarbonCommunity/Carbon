using Carbon.Base.Interfaces;

namespace Carbon.Contracts;

public interface IModuleProcessor : IDisposable
{
	void Init();
	void OnServerInit();
	void OnServerSave();
	void Setup(BaseHookable hookable);
	void Build(params Type[] types);
	void Uninstall(IModule module);
	List<BaseHookable> Modules { get; }
}
