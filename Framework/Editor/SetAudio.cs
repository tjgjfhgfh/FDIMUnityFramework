using System.IO;
using UnityEditor;
using UnityEngine;

public class AudioClipSetting : EditorWindow
{
    [MenuItem("Assets/音频设置", false, 20)]
    public static void ProcessAudioFiles()
    {
        // ��ȡ��ǰѡ��Ķ���
        if (Selection.activeObject == null || !(Selection.activeObject is DefaultAsset))
        {
            Debug.LogWarning("Please select a valid audio folder in the Assets directory.");
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning("Selected object is not a valid folder.");
            return;
        }

        var audioFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);
        foreach (var file in audioFiles)
        {
            if (!file.ToLower().EndsWith(".wav") && !file.ToLower().EndsWith(".mp3") &&
                !file.ToLower().EndsWith(".ogg"))
                continue;
            var assetPath = file.Replace(Application.dataPath, "Assets");
            var importer = AssetImporter.GetAtPath(assetPath) as AudioImporter;
            if (importer == null) continue;
            importer.forceToMono = true;
            // ������Ƶ������
            var defaultImportSet = importer.defaultSampleSettings;
            defaultImportSet.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
            defaultImportSet.sampleRateOverride = 22050;
            // ���� Load Type ������Ƶ����
            AudioClip audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
            if (audioClip == null) return;
            float lengthInSeconds = audioClip.length;
            if (lengthInSeconds < 5f)
            {
                // ����Ƶ
                defaultImportSet.compressionFormat = AudioCompressionFormat.ADPCM;
                defaultImportSet.loadType = AudioClipLoadType.DecompressOnLoad;
#if UNITY_2022
                defaultImportSet.preloadAudioData = true;
#else
                importer.preloadAudioData = true;
#endif
            }
            else if (lengthInSeconds < 30f)
            {
                // �еȳ�����Ƶ
                defaultImportSet.compressionFormat = AudioCompressionFormat.Vorbis;
                defaultImportSet.loadType = AudioClipLoadType.CompressedInMemory;
            }
            else
            {
                // ����Ƶ
                defaultImportSet.compressionFormat = AudioCompressionFormat.PCM;
                defaultImportSet.loadType = AudioClipLoadType.Streaming;
            }

            importer.defaultSampleSettings = defaultImportSet;
            importer.SaveAndReimport();
            Debug.Log($"设置音频: {assetPath}");
        }

        Debug.Log("全部设置完整");
    }
}