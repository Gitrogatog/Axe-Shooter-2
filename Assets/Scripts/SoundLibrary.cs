using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] soundGroups;
    Dictionary<string, AudioClip[]> groupDict = new Dictionary<string, AudioClip[]>();

    void Awake(){
        foreach(SoundGroup sGroup in soundGroups){
            groupDict.Add(sGroup.groupID, sGroup.group);
        }
    }
    public AudioClip GetClipFromName(string soundName){
        if(groupDict.ContainsKey(soundName)){
            AudioClip[] sounds = groupDict[soundName];
            return sounds[Random.Range(0, sounds.Length)];
        }
        return null;
    }

    [System.Serializable]
    public class SoundGroup{
        public string groupID;
        public AudioClip[] group;
    }
}
