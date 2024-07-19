using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace DevPenguin.Utilities
{
	public class SaveController : MonoBehaviour
	{
		#region Declarations

		private const string TAG = "SaveController";
		
		[Header("Debug")]
		[SerializeField] private bool areLogsEnabled;
		
		[Header("Save Controller Properties")]
		[SerializeField] private bool isSaveEnabled = true;

		#endregion
		
		#region Helper Methods
		
		/// <summary>
		/// Save data into a file on the system and crypto it.
		/// Path on Windows C:/Users/{UserName}/AppData/LocalLow/{CompanyName}/{GameName}/
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="saveData"></param>
		public void SaveBinaryData(string fileName, object saveData)
		{
			try
			{
				if (isSaveEnabled)
				{
					// Get file in the system storage.
					string _filePath = $"{Application.persistentDataPath}/{fileName}";
					BinaryFormatter _binaryFormatter = new BinaryFormatter();
					FileStream _fileStream = File.Open(_filePath, FileMode.OpenOrCreate);

					// Construct the data to be saved.
					object _data = saveData;

					// Save it to the file.
					_binaryFormatter.Serialize(_fileStream, _data);

					// Close the file.
					_fileStream.Close();

					if (areLogsEnabled)
						Debug.Log($"{TAG}: Successfully saved {fileName} to {_filePath}");
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Could not save data: " + e);
			}
		}

		/// <summary>
		/// Load data from a file on the system and decry it.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public object LoadBinaryData(string fileName)
		{
			try
			{
				if (isSaveEnabled)
				{
					string _filePath = $"{Application.persistentDataPath}/{fileName}";

					if (!File.Exists(_filePath))
					{
						if (areLogsEnabled)
							Debug.Log($"{TAG}: The file {fileName} was not created yet. Creating one.");
					}
					else
					{
						// Get file in the system storage
						BinaryFormatter _binaryFormatter = new BinaryFormatter();
						FileStream _fileStream = File.Open(_filePath, FileMode.Open);

						// Extract the data on the file to a new data class
						object _data = _binaryFormatter.Deserialize(_fileStream);

						// Close the file
						_fileStream.Close();

						if (areLogsEnabled)
							Debug.Log($"{TAG}: Successfully loaded {fileName} in {_filePath}");

                        return _data;
					}

					return null;
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Could not LoadBinaryData: " + e);
			}

			return null;
		}
		
		#endregion
		
	}
}
