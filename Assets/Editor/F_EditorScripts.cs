using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace F_Editor_Utility {

//*********************************
// EditorScripts by The5_1
//*********************************
// USE AT YOUR OWN RISK!
// This code was written for the Windows version, might not work with other ones.
//*********************************
// USAGE:
// Create a "Editor" Folder in your Assets Folder.
// Paste this script in there.
// The script adds a menu to the toolbar called "F_Scripts" by default.
// You can change the menu's names in the code below.
//*********************************
// Backup Script
//*********************************
// Coppies all the content of the .../ProjectName/Assets/Code Folder
// and pastes it to .../ProjectName/backup/Code Folder
// You can change the targetFolderToCopy and the backupFolderName to your needs.
// Or copy the whole MenuItem to make additional menus.


//EditorScript!!!
    [InitializeOnLoad]
    public class F_EditorScripts
    {
        private const string _BackupsFolderName = "Backups";


        // Change that string here for to use your own Menu names
        [MenuItem("F_Scripts/Backup: Code and Data")]
        static void BackupCodeAndData()
        {
            string dateString = string.Format("_{0:yyyy-MM-dd_HH-mm-ss}",DateTime.Now);
            BackupFolder("Code", "backup" + dateString);
            BackupFolder("Data", "backup" + dateString);
        }

        [MenuItem("F_Scripts/Open Backup Directory")]
        static void OpenBackupDirectory()
        {
            int index = Application.dataPath.LastIndexOf("/");
            string path = Application.dataPath.Substring(0, index) + "/" + _BackupsFolderName + "/";
            EditorUtility.OpenWithDefaultApp(path);
        }

        /*
        [MenuItem("F_Scripts/Test Outside Dir")]
        static void Test()
        {
            int index = Application.dataPath.LastIndexOf("/");
            string path = "../";
            EditorUtility.OpenWithDefaultApp(path);
        }
        */

        static void BackupFolder(string targetFolderToCopy, string backupFolderName)
        {
            //Get ../Project/Assets Path
            string assetsPath = Application.dataPath;

            //check if the Folder we want to copy even exists
            if (!Directory.Exists(assetsPath + "/" + targetFolderToCopy))
            {
                Debug.Log("F_Script: Folder: \"" + (assetsPath + "/" + targetFolderToCopy) + "\" does not exist!");
                return;
            }

            //Build a Path pointing to .../Project/backupFolderName
            int index = assetsPath.LastIndexOf("/");
            string backupPath = assetsPath.Substring(0, index) +"/" + _BackupsFolderName + "/";

            //if the target backup Folder does not exist, make one
            if (!Directory.Exists(backupPath + "/" + backupFolderName))
            {
                Directory.CreateDirectory(backupPath + "/" + backupFolderName);
                //Debug.Log("F_Script: Folder \"" + backupPath + "/" + backupFolderName +"\" created.");
            }

            //copy the Folder into the Backup folder, overwrite existing
            FileUtil.ReplaceDirectory(assetsPath + "/" + targetFolderToCopy, backupPath + backupFolderName + "/" + targetFolderToCopy);

            Debug.Log("F_Script: Backup Complete!\n--> From: \"" + assetsPath + "/" + targetFolderToCopy + "\"\n--> To: \"" + backupPath + backupFolderName + "/" + targetFolderToCopy +"\"");
        }

        }

}

/*

            //Get Project/Assets Path and remove the /Assets portion
        string assetsPath = Application.dataPath;
        string backupPath;
        string targetFolderToCopy = "Code";
        string backupFolderName = "backup";
        int index = assetsPath.LastIndexOf("/");
        backupPath = assetsPath.Substring(0, index+1); // index + 1 to keep the slash
        if (!Directory.Exists(backupPath))
        { Directory.CreateDirectory(backupPath); }

        FileUtil.ReplaceDirectory(assetsPath + "/" + targetFolderToCopy, backupPath + backupFolderName + "/" + targetFolderToCopy);
        Debug.Log("F_Script: Backup Complete!\nFrom: " + assetsPath + "/" + targetFolderToCopy + "\nTo: " + backupPath + backupFolderName + "/" + targetFolderToCopy);
*/


//=========================================
// INFO
//=========================================

/******* Start EditorScript on launch
[InitializeOnLoad]
public class Startup
{
    static Startup()
    {
        Debug.Log("Up and running");
    }
}
*/


/******* Path to Project Folder
Application.dataPath returns <path to project folder>/Assets
*/
/******* Copy Files or Folders and overwrite them
FileUtil.ReplaceDirectory("path/To/Your/Folder", "path/To/Your/ReplacedFolder");
*/
/******* Copy Files or Folders WITHOUT OVERWRITING
FileUtil.CopyFileOrDirectory("path/YourFileOrFolder", "copy/path/YourFileOrFolder");
*/
/******* Custom Menu must be STATIC METHOD!!!
@MenuItem("Menu Name/Do Stuff")
static function DoStuff()
{
    // Code Here
}
*/


/******** Custom Windows inherit from EditorWindow
using UnityEditor;
using UnityEngine;

public class MyWindow : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
    
    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/My Window")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(MyWindow));
    }
    
    void OnGUI()
    {
        GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField ("Text Field", myString);
        
        groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle ("Toggle", myBool);
            myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup ();
    }
}
*/


//=========================================
// Dowloaded Scripts
//=========================================


//Scripts to save the scene when hitting play mode.
//dont need that, i want backups of my scripts

//http://answers.unity3d.com/questions/158498/auto-save-every-five-minutes.html
/*
 using UnityEngine;
 using UnityEditor;
 
 [InitializeOnLoad]
 public class AutosaveOnRun
 {
     static AutosaveOnRun()
     {
         EditorApplication.playmodeStateChanged = () =>
         {
             if(EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
             {
                 Debug.Log("Auto-Saving scene before entering Play mode: " + EditorApplication.currentScene);
                 
                 EditorApplication.SaveScene();
                 EditorApplication.SaveAssets();
             }
         };
     }
 }
*/

//http://www.daikonforge.com/dfgui/save-on-run/
/*
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class OnUnityLoad
{

    static OnUnityLoad()
    {

        EditorApplication.playmodeStateChanged = () =>
        {

            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {

                Debug.Log("Auto-Saving scene before entering Play mode: " + EditorApplication.currentScene);

                EditorApplication.SaveScene();
                EditorApplication.SaveAssets();
            }

        };

    }

}
*/
