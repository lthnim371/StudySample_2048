using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.IO;

public class XmlManager : Singleton<XmlManager> {

    //리소스폴더에서 찾을 파일명
    private readonly string ACHIEVEMENTLIST = "AchievementList";
    private readonly string GAMESYSTEMSETTING = "GameSystemSetting";

    struct sAchievementGroup //잠금업적과 누적업적 이름 모음
    {
        public string UnlockName { get; set; }
        public string IncrementalName { get; set; }

        public sAchievementGroup(string unlockName, string incrementalName)
        {
            this.UnlockName = unlockName;
            this.IncrementalName = incrementalName;
        }
    }

    private List<sAchievementGroup> achievementGroupList; //잠금업적,누적업적 모음 리스트
    private Dictionary<string, float> gameSystemSettingMap; //게임시스템셋팅값 맵

    protected override void Awake()
    {
        this.gameObject.name = "_XmlManager"; //본체의 이름 변경

        //초기화 및 xml 읽어오기
        this.achievementGroupList = new List<sAchievementGroup>();
        this.LoadXml_achievementList();
        this.gameSystemSettingMap = new Dictionary<string, float>();
        this.LoadXml_GameSystemSetting();
    }

    /// <summary>
    /// xml파일명으로 reader준비하기
    /// </summary>
    /// <param name="xmlReader">초기화돼서 반환됨</param>
    /// <param name="findXmlName">찾는 xml파일명</param>
    /// <returns></returns>
    private bool SetXmlReader(out XmlReader xmlReader, string findXmlName)
    {
        TextAsset xmlTextAsset =
            Resources.Load(string.Format("XMLs/{0}", findXmlName)) as TextAsset;
        string xmlStr = xmlTextAsset.ToString();
        StringReader strReader = new StringReader(xmlStr);
        try
        {
            xmlReader = XmlReader.Create(strReader);
        }
        catch (System.Exception e)
        {
            print(e.Message);
            print(string.Format("{0}_XML를 찾을수가없다.", findXmlName));
            xmlReader = null;
            return false;
        }

        return true;
    }

    /// <summary>
    /// 잠금업적, 누적업적의 이름 xml파일 읽어오기
    /// </summary>
    private void LoadXml_achievementList()
    {
        //xmlreader준비 및 못 찾으면 패스
        XmlReader xmlReader = null;
        if (!this.SetXmlReader(out xmlReader, this.ACHIEVEMENTLIST))
            return;

        while (xmlReader.Read())
        {
            if (xmlReader.Name.CompareTo("Achievement") == 0 &&
                xmlReader.NodeType == XmlNodeType.Element)
            {
                sAchievementGroup newStruct = new sAchievementGroup(
                    xmlReader.GetAttribute("UnlockName")
                    , xmlReader.GetAttribute("IncrementalName"));

                this.achievementGroupList.Add(newStruct);
            }
        }//while

        xmlReader.Close(); //사용 후에는 닫아준다.

#if SHOW_DEBUG_MESSAGES
        print(string.Format(
            "{0}_XML 읽어옴. 읽어온 데이터 총 {1}개",
            this.ACHIEVEMENTLIST, this.achievementGroupList.Count.ToString()));
#endif
    }

    /// <summary>
    /// 게임시스템셋팅값 xml 읽어오기
    /// </summary>
    private void LoadXml_GameSystemSetting()
    {
        XmlReader xmlReader = null;
        if (!this.SetXmlReader(out xmlReader, this.GAMESYSTEMSETTING))
            return;

        while (xmlReader.Read())
        {
            if (xmlReader.Name.CompareTo("System") == 0 &&
                xmlReader.NodeType == XmlNodeType.Element)
            {
                this.gameSystemSettingMap.Add(
                    xmlReader.GetAttribute("Name"),
                    float.Parse(xmlReader.GetAttribute("Value"))
                    );
            }
        }

        xmlReader.Close();

#if SHOW_DEBUG_MESSAGES
        print(string.Format(
            "{0}_XML 읽어옴. 읽어온 데이터 총 {1}개",
            this.GAMESYSTEMSETTING, this.gameSystemSettingMap.Count.ToString()));
#endif
    }

    /// <summary>
    /// 게임시스템셋팅값 찾기
    /// </summary>
    /// <param name="findName">찾는 시스템 이름</param>
    /// <param name="originalValue">못 찾을경우 반환받을 기본값</param>
    /// <returns></returns>
    public float FindGameSystemSettingValue(string findName, float originalValue = 0f)
    {
        //찾는 이름의 데이터가 존재한다면
        if (this.gameSystemSettingMap.ContainsKey(findName))
            return this.gameSystemSettingMap[findName];
        else //없으면 일단 0으로 반환하자
#if SHOW_DEBUG_MESSAGES
            print(string.Format("{0}의 딕셔너리 데이터가 없다", findName));
#endif
        return originalValue;
    }
}
