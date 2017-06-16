using UnityEditor;

static class MonsterUnityIntegration {

	[MenuItem("Assets/Create/MonsterAsset")]
	public static void CreateYourScriptableObject() {
		ScriptableObjectUtility2.CreateAsset<MonsterAsset>();
	}

}
