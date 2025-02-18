using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using System;
using TMPro;
using VRC.SDK3.Data;

namespace CytraX
{
    public class PatreonList : UdonSharpBehaviour
    {
        [SerializeField]TextMeshProUGUI m_text;
        public VRCUrl PatreonGHURL;
        public float ReloadDelay = 60;
        public string PatreonNameList;
        public string[] PatreonNames;
        public string[] RolesToDisplay;

        private void Start()
        {
            _DownloadList();
        }
        public void _DownloadList()
        {
            VRCStringDownloader.LoadUrl(PatreonGHURL, (IUdonEventReceiver)this);
            SendCustomEventDelayedSeconds(nameof(_DownloadList), ReloadDelay);
        }
        public override void OnStringLoadSuccess(IVRCStringDownload data)
        {
            if (VRCJson.TryDeserializeFromJson(data.Result, out DataToken result))
            {
                if (result.TokenType == TokenType.DataDictionary)
                {
                    PatreonNameList = null;

                    foreach (var key in RolesToDisplay)
                    {
                        if (result.DataDictionary.TryGetValue(key, out var contributorsToken))
                        {
                            if (contributorsToken.TokenType == TokenType.DataList)
                            {
                                PatreonNameList += $"<color=red>{key}</color><br><br>";

                                string[] PatreonNames = new string[contributorsToken.DataList.Count];
                                for (int i = 0; i < contributorsToken.DataList.Count; i++)
                                {
                                    PatreonNames[i] = contributorsToken.DataList[i].ToString();
                                }

                                PatreonNameList += string.Join(", ", PatreonNames) + "<br><br>";
                            }
                        }
                    }

                    if (PatreonNameList.EndsWith(", "))
                    {
                        PatreonNameList = PatreonNameList.Substring(0, PatreonNameList.Length - 2);
                    }

                    m_text.text = PatreonNameList;
                }
            }
        }
        public override void OnStringLoadError(IVRCStringDownload result)
        {
            Debug.Log(result.Error);
        }
    }
}
