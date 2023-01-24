using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;
using System.Linq;

public class FBXPrefabCreator  {

    [MenuItem("Assets/Create/Prefab From Selected FBX")]
    private static void CreatePrefabFromFBX() {
        var targetFBX = Selection.activeGameObject;
        CreatePrefabFromFBX(targetFBX);
    }

    private static void CreatePrefabFromFBX(GameObject sourceFBX) {
        var sourcePath = AssetDatabase.GetAssetPath(sourceFBX); // FBXファイルのパスを取得
        var savePath = Path.GetDirectoryName(sourcePath);       // FBXファイルのディレクトリを取得
        var extension = "prefab";                               // 保存する際の拡張子
        
        // FBXじゃなかったら処理しない
        if (!Path.HasExtension(sourcePath) || Path.GetExtension(sourcePath) != ".fbx") {
            throw new System.Exception("This is not fbx.");
        }

        var defaultFileName = Path.GetFileNameWithoutExtension(sourcePath); // 元のファイル名を取得
        // 保存用のパネルを表示
        savePath = EditorUtility.SaveFilePanelInProject(
            "Save Asset",
            defaultFileName,
            extension,
            "",
            savePath
        );

        // 保存パスが存在しない場合は、処理しない
        if (string.IsNullOrEmpty(savePath)) throw new System.Exception("Path to save is invalid.");

        // Prefabのインスタンスを作成
        var tempInstance = GameObject.Instantiate(sourceFBX);

        // インスタンスを編集したければここで行う
        {
        }

        var targetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(savePath);
        if (targetPrefab != null) {
            // Prefabが存在する場合は、古いPrefabからコンポーネントを継承する
            ComponentInheritance(tempInstance, targetPrefab);
        }

        // Prefabを新規作成or上書き保存
        PrefabUtility.SaveAsPrefabAssetAndConnect(
            tempInstance,
            savePath,
            InteractionMode.AutomatedAction
        );

        // 一時的に配置されたインスタンスの削除
        GameObject.DestroyImmediate(tempInstance);
    }

    private static void ComponentInheritance(GameObject tempPrefab, GameObject targetPrefab) {
        foreach (var oldPrefab in targetPrefab.GetComponentsInChildren<Transform>(true)) {
            // 同じ名前のPrefabを同一のものとみなす
            var newPrefab = tempPrefab.transform.Find(oldPrefab.name);
            // ルートオブジェクトの場合は、強制的に同一のものとみなす
            if (oldPrefab == targetPrefab.transform.root) {
                newPrefab = tempPrefab.transform.root;
            }

            // 新しいPrefabが存在しない場合は、抜ける
            if (newPrefab == null) continue;

            var components = oldPrefab.GetComponents<Component>();
            var componentTypes = components.Select(x => x.GetType()).Distinct();

            // コンポーネントを重複しないようにコピペする
            CopyAndPasteComponents(components, componentTypes, newPrefab);
        }
    }

    private static void CopyAndPasteComponents(
        Component[] components,
        IEnumerable<System.Type> componentTypes,
        Transform attachPrefabTransform
    ) {
        foreach (var componentType in componentTypes) {
            if (attachPrefabTransform.GetComponents(componentType).Length != 0) continue;

            foreach (var component in components.Where(x => x.GetType() == componentType)) {
                ComponentUtility.CopyComponent(component);
                ComponentUtility.PasteComponentAsNew(attachPrefabTransform.gameObject);
            }
        }
    }

}