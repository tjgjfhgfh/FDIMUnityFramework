#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FDIMEmbedGuard
{
    static FDIMEmbedGuard()
    {
        string embeddedPath = "Packages/com.fdim.framework";
        if (Directory.Exists(embeddedPath))
        {
            Debug.LogError("[FDIM] ��⵽���� Embedded����д��������� Git URL ���룬���̶� tag/commit��");
            EditorUtility.DisplayDialog("FDIM ����",
                "��⵽���� Embedded����д����\n���Ϊʹ�� Git URL�����̶� tag/commit��",
                "��֪����");
        }
    }
}
#endif
