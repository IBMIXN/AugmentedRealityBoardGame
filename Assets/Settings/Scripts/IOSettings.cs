using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class IOSettings
{
    // Used to load the users settings set
    public static void SaveSettings(SettingsManager setMan)
    {
        BinaryFormatter form = new BinaryFormatter();

        string path = Application.persistentDataPath + "/settings.bin";

        FileStream str = new FileStream(path, FileMode.Create);

        SettingsData data = new SettingsData(setMan);

        form.Serialize(str, data);

        str.Close();
    }

    // Used to save the users settings set
    public static SettingsData LoadSettings()
    {
        string path = Application.persistentDataPath + "/settings.bin";

        // Check if file couldn't be found
        if (!File.Exists(path))
        {
            Debug.LogError("Save file could not be found, attempting to create one.");

            // Attempt to create a file
            try {
                File.Create(path);

                // Sanity check
                if (!File.Exists(path))
                    throw new Exception("Could not find file.");
            }
            catch (Exception e)
            {
                Debug.LogError("Save file could not be created (" + e.ToString() + ")");
                return null;
            }
        }

        BinaryFormatter form = new BinaryFormatter();

        FileStream str = new FileStream(path, FileMode.Open);

        SettingsData data = form.Deserialize(str) as SettingsData;

        str.Close();

        return data;
    }
}
