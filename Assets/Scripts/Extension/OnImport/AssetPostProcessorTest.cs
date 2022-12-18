using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

public class AssetPostProcessorTest : AssetPostprocessor {

    private static void OnPostprocessAllAssets(
        string[] import,
        string[] delete,
        string[] move,
        string[] moveFrom
    ) {
        List<string> paths = new List<string>();

        // 日本語が含まれているファイル名を列挙し、リストに追加
        foreach (string path in import) {
            string[] splitString = path.Split(new char[] {'\\'});
            // ファイル名を取得し、日本語か判定
            if (splitString[splitString.Length - 1].IsJapanese()) {
                // 絶対パスを追加
                paths.Add(path.AssetsToAbsolutePath());
            }
        }

        RenameAssetWindow.Open(paths);
    }

}

public class RenameAssetWindow : EditorWindow {

    private static List<string> jpNamePaths;
    private static int pathIndex = 0;

    private string newName = "";

    public static void Open(List<string> paths) {
        if (paths.Count == 0) return;

        RenameAssetWindow renameAssetWindow = GetWindow<RenameAssetWindow>();
        renameAssetWindow.titleContent = new GUIContent("ファイル名変更");
        renameAssetWindow.minSize = new Vector2(500f, 300f);

        jpNamePaths = paths;
        pathIndex = 0;
    }

    public void OnGUI() {
        // パスの変換が終了した場合は、ウィンドウを閉じて終了する
        if (pathIndex >= jpNamePaths.Count) {
            AssetDatabase.Refresh();
            this.Close();
            return;
        }

        string[] splitPath = SplitWithLastPath(jpNamePaths[pathIndex]);
        string extension = splitPath[splitPath.Length - 1];
        int splitPathLength = splitPath.Length - 2;

        GUILayout.Label("ファイル名に日本語が含まれていました");
        GUILayout.Label($"現在のファイル名：{splitPath[splitPathLength]}");
        newName = GUILayout.TextField(newName);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("変更する")) {
            splitPath[splitPathLength] = newName;
            splitPath = splitPath.Where(e => e != extension).ToArray();

            string joinPath = string.Join('\\', splitPath);
            string[] newSplitPath = new string[] {joinPath, extension};
            string newPath = string.Join(".", newSplitPath);

            File.Move(jpNamePaths[pathIndex], newPath);

            NextPath();
        }

        if (GUILayout.Button("変更しない")) {
            NextPath();
        }
        GUILayout.EndHorizontal();
    }

    /*
     * \と.で分割するメソッド
     * 配列の最後が拡張子 [Length - 1]
     * 配列の最後から１つ戻った場所がファイル名 [Length - 2]
     */
    private string[] SplitWithLastPath(string path) {
        string[] splitPath = path.Split(new char[] {'\\', '.'});
        return splitPath;
    }

    private void NextPath() {
        newName = "";
        pathIndex++;
    }

}