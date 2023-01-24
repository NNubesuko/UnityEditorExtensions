using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugExtension {

    public static void EditorLog(this object self) {
        #if UNITY_EDITOR
            Debug.Log(self);
        #endif
    }

}
