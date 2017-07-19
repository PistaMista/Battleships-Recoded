using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

using System.Linq;
using System.IO;

public class BattleLoader : MonoBehaviour
{

    static BattleSaver.MainBattleData candidateData;

    public static int LoadData ()
    {
        if (File.Exists( BattleSaver.saveFilePath ))
        {
            FileStream stream = new FileStream( BattleSaver.saveFilePath, FileMode.Open );
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                candidateData = (BattleSaver.MainBattleData)formatter.Deserialize( stream );
            }
            catch
            {
                Debug.LogError( "Battle save file is corrupted - deserialization failed." );
                return 2;
            }

            return 0;
        }
        else
        {
            return 1;
        }
    }

    public static void ReconstructBattle ()
    {
        Debug.Log( candidateData.combatants[0].label );
    }
}
