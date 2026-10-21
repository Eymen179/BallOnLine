#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelGenerator generator = (LevelGenerator)target;

        GUILayout.Space(15);
        GUI.backgroundColor = new Color(0.2f, 1f, 0.4f);
        if (GUILayout.Button("BÖLÜM İSKELETİNİ OLUŞTUR", GUILayout.Height(40)))
        {
            generator.GenerateScaffolding();
            // Yapılan değişikliği sahneye kaydet
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(generator.gameObject.scene);
        }
    }
}
#endif