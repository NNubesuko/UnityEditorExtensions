using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    private void Awake() {
        Foo.x++;
        Foo.x.EditorLog();
    }

}

public class Foo {

    public static int x;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() {
        x = 10;
    }

}