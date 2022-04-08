namespace Crystalblobbles;

public sealed partial class Crystalblobbles : Mod, ITogglableMod {
	public static Crystalblobbles? Instance { get; private set; }

	private static readonly Lazy<string> Version = new(() => Assembly
		.GetExecutingAssembly()
		.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
		.InformationalVersion
#if DEBUG
		+ "-dev"
#endif
	);

	public override string GetVersion() => Version.Value;

	public Crystalblobbles() =>
		USceneManager.activeSceneChanged += EditScene;

	public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloads) {
		if (Instance != null) {
			LogWarn("Attempting to initialize multiple times, operation rejected");
			return;
		}

		Instance = this;
		SavePreloads(preloads);

		ModHooks.LanguageGetHook += Localize;
	}

	public void Unload() {
		ModHooks.LanguageGetHook -= Localize;

		Instance = null;
	}

	private static string Localize(string key, string sheet, string orig) =>
		running || GameManager.instance.sceneName == "GG_Workshop"
		? sheet switch {
			"Journal" when key is "NAME_OBLOBBLE" => "Name".Localize(),
			"CP3" when key is "GG_S_BIGBEES" => "Desc".Localize(),
			"Titles" => key switch {
				"OBLOBBLES_SUPER" => "Title/Super".Localize(),
				"OBLOBBLES_MAIN" => "Title/Main".Localize(),
				"OBLOBBLES_SUB" => "Title/Sub".Localize(),
				_ => orig,
			},
			_ => orig,
		} : orig;
}
