using static Modding.IMenuMod;

namespace Crystalblobbles;

public sealed partial class Crystalblobbles : IMenuMod {
	bool IMenuMod.ToggleButtonInsideMenu => true;

	List<MenuEntry> IMenuMod.GetMenuData(MenuEntry? toggleButtonEntry) => new() {
		toggleButtonEntry!.Value,
		new(
			"Option/ModifyPantheons".Localize(),
			new string[] {
				Lang.Get("MOH_OFF", "MainMenu"),
				Lang.Get("MOH_ON", "MainMenu")
			},
			"",
			i => GlobalSettings.modifyPantheons = i != 0,
			() => GlobalSettings.modifyPantheons ? 1 : 0
		)
	};
}
