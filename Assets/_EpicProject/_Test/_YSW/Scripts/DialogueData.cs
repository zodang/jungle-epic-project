// DialogueData.cs
using System;
using System.Collections.Generic;

// JSON 파일의 "lines" 배열 내 각 요소에 해당
[Serializable]
public class DialogueLine
{
    public string speaker; // 화자 (예: "Player", "NPC_Guard")
    public string text;    // 대사 내용
    // 필요하다면 여기에 표정(string portraitKey), 음성 파일명(string voiceClipName) 등 추가 가능
}

// JSON 파일의 "choices" 배열 내 각 요소에 해당
[Serializable]
public class DialogueChoice
{
    public string text;           // 선택지에 표시될 텍스트
    public string nextDialogueId; // 이 선택지를 골랐을 때 이어질 다음 대화의 ID
    // 필요하다면 여기에 특정 조건(string condition), 결과 이벤트(string outcomeEvent) 등 추가 가능
}

// JSON 파일의 "dialogues" 배열 내 각 대화 묶음에 해당
[Serializable]
public class DialogueEntry
{
    public string id;                      // 각 대화 묶음을 식별하는 고유 ID
    public List<DialogueLine> lines;       // 해당 대화의 대사들
    public List<DialogueChoice> choices;   // 해당 대화 후 나올 선택지들 (없을 수도 있음)
    // 필요하다면 여기에 대화 시작 시 실행할 이벤트(string startEvent), 종료 시 이벤트(string endEvent) 등 추가 가능
}

// JSON 파일 전체 구조에 해당 (최상위 객체)
[Serializable]
public class DialogueCollection
{
    public List<DialogueEntry> dialogues; // 모든 대화 묶음의 리스트
}