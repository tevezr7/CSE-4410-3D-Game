using UnityEngine;
using UnityEditor;

public class FixAnimationPaths : EditorWindow
{
    private AnimationClip clip;
    private string oldRoot = "";
    private string newRoot = "Arms";

    [MenuItem("Tools/Fix Animation Paths")]
    public static void ShowWindow()
    {
        GetWindow<FixAnimationPaths>("Fix Animation Paths");
    }

    void OnGUI()
    {
        GUILayout.Label("Fix Missing Bone Paths", EditorStyles.boldLabel);
        GUILayout.Space(10);

        clip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", clip, typeof(AnimationClip), false);
        oldRoot = EditorGUILayout.TextField("Old Root (leave blank)", oldRoot);
        newRoot = EditorGUILayout.TextField("New Root", newRoot);

        GUILayout.Space(10);

        if (GUILayout.Button("Fix Paths"))
        {
            if (clip == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign an Animation Clip.", "OK");
                return;
            }
            FixPaths();
        }
    }

    void FixPaths()
    {
        var bindings = AnimationUtility.GetCurveBindings(clip);
        int fixed_count = 0;

        foreach (var binding in bindings)
        {
            var curve = AnimationUtility.GetEditorCurve(clip, binding);
            var newBinding = binding;

            if (string.IsNullOrEmpty(oldRoot))
            {
                // prepend newRoot to every path
                newBinding.path = string.IsNullOrEmpty(binding.path)
                    ? newRoot
                    : newRoot + "/" + binding.path;
            }
            else
            {
                // replace oldRoot with newRoot
                if (binding.path.StartsWith(oldRoot))
                    newBinding.path = binding.path.Replace(oldRoot, newRoot);
                else
                    continue;
            }

            AnimationUtility.SetEditorCurve(clip, binding, null);       // remove old
            AnimationUtility.SetEditorCurve(clip, newBinding, curve);   // add new
            fixed_count++;
        }

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Done", $"Fixed {fixed_count} bone paths.", "OK");
    }
}