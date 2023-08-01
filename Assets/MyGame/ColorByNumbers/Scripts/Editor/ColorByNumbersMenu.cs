using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace BizzyBeeGames.ColorByNumbers
{
	public class ColorByNumbersMenu
	{
		[MenuItem("Tools/Royal Pixel/Clear Save Data")]
		private static void DeleteSaveData()
		{
			// Delete the save file if it exists
			System.IO.File.Delete(Application.persistentDataPath + "/save.json");
			PlayerPrefs.DeleteAll();
		}
		[MenuItem("Tools/Royal Pixel/Open App Data Directory")]
		public static void OpenAppDataDirectory()
		{
			EditorUtility.RevealInFinder(Application.persistentDataPath);
		}
		[MenuItem("Tools/Royal Pixel/Delete ALL")]
		public static void DeletePersistentDataInDevice()
		{
			var path = Application.persistentDataPath;
			if (Directory.Exists(path)) { Directory.Delete(path, true); }
			DeleteSaveData();
		}
		[MenuItem("Tools/Royal Pixel/Clear Save Data", true)] 
		private static bool DeleteSaveDataValidation()
		{
			// Don't allow deleting save data while the application is running
			return !Application.isPlaying;
		}

		[MenuItem("Tools/Royal Pixel/Complete Active Level")]
		private static void CompleteActiveLevel()
		{
			//GameController.Instance.CompleteActiveLevel();
		}
	}
}
