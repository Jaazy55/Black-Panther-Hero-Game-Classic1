using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class SceneChanger : MonoBehaviour
{
    static string SplashScreen = "Assets/Scenes/SplashScreen.unity";
    static string Menu = "Assets/Scenes/Menu.unity";
    static string Game = "Assets/Scenes/Game.unity";
    static string PerformanceDetecting = "Assets/Scenes/PerformanceDetecting.unity";
    static string ShopRoom = "Assets/Scenes/ShopRoom.unity";
    static string Arena = "Assets/Scenes/Arena.unity";

#if UNITY_EDITOR
    [MenuItem("Scene_Changer/SplashScreen")]
    private static void SplashScene()
    {
        if (!EditorApplication.isPlaying)
        {
            bool value = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (value)
            {
                EditorSceneManager.OpenScene(SplashScreen);
            }
        }
    }
    [MenuItem("Scene_Changer/Menu")]
    private static void MenuScene()
    {
        if (!EditorApplication.isPlaying)
        {
            bool value = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (value)
            {
                EditorSceneManager.OpenScene(Menu);
            }
        }
    }
    [MenuItem("Scene_Changer/Game")]
    private static void GameScene()
    {
        if (!EditorApplication.isPlaying)
        {
            bool value = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (value)
            {
                EditorSceneManager.OpenScene(Game);
            }
        }
    }
    [MenuItem("Scene_Changer/PerformanceDetecting")]
    private static void PerformanceDetectingScene()
    {
        if (!EditorApplication.isPlaying)
        {
            bool value = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (value)
            {
                EditorSceneManager.OpenScene(PerformanceDetecting);
            }
        }
    }
    [MenuItem("Scene_Changer/ShopRoom")]
    private static void ShopRoomScene()
    {
        if (!EditorApplication.isPlaying)
        {
            bool value = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (value)
            {
                EditorSceneManager.OpenScene(ShopRoom);
            }
        }
    }
    [MenuItem("Scene_Changer/Arena")]
    private static void ArenaScene()
    {
        if (!EditorApplication.isPlaying)
        {
            bool value = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (value)
            {
                EditorSceneManager.OpenScene(Arena);
            }
        }
    }



#endif
}
