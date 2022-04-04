using System.IO;

using HutongGames.PlayMaker.Actions;

namespace Crystalblobbles;

public sealed partial class Crystalblobbles {
	private static AudioClip? shotClip = null;
	private static GameObject? crystalShot = null;

	private static bool didGlobalChange = false;

	private static tk2dSprite? oblobbleSprite = null;
	private static Texture2D? oblobbleTex = null;
	private static readonly Lazy<Texture2D> crystalTex = new(() => {
		Stream stream = typeof(Crystalblobbles).Assembly
			.GetManifestResourceStream("Crystalblobbles.Resources.Crystalblobble.png");
		MemoryStream ms = new((int) stream.Length);

		stream.CopyTo(ms);
		stream.Close();

		byte[] bytes = ms.ToArray();
		ms.Close();

		Texture2D texture2D = new(2, 2);
		texture2D.LoadImage(bytes, true);

		return texture2D;
	});

	public override List<(string, string)> GetPreloadNames() => new() {
		("Mines_07", "Crystal Flyer")
	};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void SavePreloads(Dictionary<string, Dictionary<string, GameObject>> preloads) {
		if (preloads == null) {
			return;
		}

		GameObject crystalHunter = preloads["Mines_07"]["Crystal Flyer"];
		shotClip = crystalHunter.LocateMyFSM("Crystal Flyer")
			.GetAction<AudioPlaySimple>("Fire", 0)
			.oneShotClip.Value as AudioClip;

		crystalShot = crystalHunter
			.GetComponent<PersonalObjectPool>()
			.startupPool[0].prefab;
		crystalShot.AddComponent<CrystalMarker>();
	}
}
