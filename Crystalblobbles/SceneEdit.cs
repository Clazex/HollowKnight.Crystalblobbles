using HutongGames.PlayMaker.Actions;

namespace Crystalblobbles;

public sealed partial class Crystalblobbles {
	private static bool running = false;

	private static void EditScene(Scene prev, Scene next) {
		if (prev.name == "GG_Oblobbles") {
			UObject.FindObjectsOfType<CrystalMarker>()
				.ForEach(go => UObject.Destroy(go.gameObject));
		}

		if (Instance == null) {
			goto Inactive;
		}

		if (next.name != "GG_Oblobbles") {
			goto Inactive;
		}

		if (BossSequenceController.IsInSequence && !GlobalSettings.modifyPantheons) {
			goto Inactive;
		}

		running = true;

		if (!didGlobalChange) {
			oblobbleSprite ??= next.GetRootGameObjects()
				.First(go => go.name == "Mega Fat Bee")
				.GetComponent<tk2dSprite>();
			oblobbleTex ??= oblobbleSprite.CurrentSprite.material.mainTexture as Texture2D;

			oblobbleSprite!.CurrentSprite.material.mainTexture = crystalTex.Value;
			didGlobalChange = true;
		}

		GameObject[] oblobbles = next.GetRootGameObjects()
			.Filter(go => go.name == "Mega Fat Bee" || go.name == "Mega Fat Bee (1)")
			.ToArray();

		PlayMakerFSM[] atkFsms = oblobbles
			.Map(go => go.LocateMyFSM("Fatty Fly Attack"))
			.ToArray();

		FsmStateAction[] atkActions = atkFsms
			.FlatMap(fsm => new[] { fsm.GetState("Attack"), fsm.GetState("Attack 2") })
			.FlatMap(state => state.Actions)
			.ToArray();

		atkActions.OfType<AudioPlaySimple>()
			.ForEach(action => action.oneShotClip = shotClip);

		atkActions.OfType<FlingObjectsFromGlobalPool>()
			.ForEach(action => action.gameObject.Value = crystalShot);

		if (BossSceneController.Instance.BossLevel == 0) {
			oblobbles.Map(go => go.manageHealth(900));

			oblobbles.Map(go => go.LocateMyFSM("Set Rage").FsmVariables)
				.ForEach(vars => {
					vars.FindFsmInt("HP Add").Value = 200;
					vars.FindFsmInt("HP Max").Value = 1000;
				});
		} else {
			oblobbles.Map(go => go.manageHealth(1100));

			oblobbles.Map(go => go.LocateMyFSM("Set Rage").FsmVariables)
				.ForEach(vars => {
					vars.FindFsmInt("HP Add").Value = 350;
					vars.FindFsmInt("HP Max").Value = 1200;
				});

			atkFsms.Map(fsm => fsm.FsmVariables)
				.ForEach(vars => {
					vars.FindFsmFloat("Shot Speed").Value = 18f;
					vars.FindFsmFloat("Shot Pause").Value = 0.35f;
				});

			atkFsms.ForEach(fsm => {
				fsm.GetAction<SetFloatValue>("Rage Wait", 0).floatValue.Value = 25f;
				fsm.GetAction<SetFloatValue>("Rage Wait", 1).floatValue.Value = 0.15f;
			});
		}

		return;

	Inactive:
		running = false;

		if (didGlobalChange) {
			oblobbleSprite!.CurrentSprite.material.mainTexture = oblobbleTex;
			didGlobalChange = false;
		}

		return;
	}
}
