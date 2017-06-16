using UnityEditor;

static class AvatarUnityIntegration {

	[MenuItem("Assets/Create/AvatarAsset")]
	public static void CreateYourScriptableObject() {
		ScriptableObjectUtility2.CreateAsset<AvatarAsset>();
	}

}
