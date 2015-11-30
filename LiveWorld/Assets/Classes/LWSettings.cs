using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace LiveWorld
{
    public class LWSettings : MonoBehaviour
    {
        public static float sfx_vol;
        public static float ui_vol;
        public static float music_vol;
        public static float vo_vol;

        //Generate default settings if
        public static void SetDefaultSettings()
        {
            PlayerPrefs.SetInt("firstrun", 1);
            PlayerPrefs.SetFloat("sfx_vol", .5f);
            PlayerPrefs.SetFloat("ui_vol", .5f);
            PlayerPrefs.SetFloat("music_vol", .5f);
            PlayerPrefs.SetFloat("vo_vol", .5f);
        }

        public static void GetSettings()
        {
            if (PlayerPrefs.HasKey("firstrun"))
            {
                sfx_vol = PlayerPrefs.GetFloat("sfx_vol");
                ui_vol = PlayerPrefs.GetFloat("ui_vol");
                music_vol = PlayerPrefs.GetFloat("music_vol");
                vo_vol = PlayerPrefs.GetFloat("vo_vol");
            }
            else
            {
                print("Default settings not set.");
            }
        }

        public static void SetVolumeSettings(float sfx, float ui, float music, float vo)
        {
            PlayerPrefs.SetFloat("sfx_vol", sfx);
            PlayerPrefs.SetFloat("ui_vol", ui);
            PlayerPrefs.SetFloat("music_vol", music);
            PlayerPrefs.SetFloat("vo_vol", vo);
        }
    }
}
