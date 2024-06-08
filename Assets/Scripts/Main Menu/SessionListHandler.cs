using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionListHandler : MonoBehaviour
{
    [SerializeField] NetworkRunnerHandler _runnerHandler;

    [SerializeField] TMP_Text _StatusText;

    [SerializeField] SessionInfoItem _sessionItemPrefab;

    [SerializeField] VerticalLayoutGroup _verticalLayoutGroup;

    private void OnEnable()
    {
        _runnerHandler.OnSessionListUpdate += ReceiveSessionList;
    }
    private void OnDisable()
    {
        _runnerHandler.OnSessionListUpdate -= ReceiveSessionList;

    }

    void ClearBrowser()
    {
        foreach (Transform child in _verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
        _StatusText.gameObject.SetActive(false);
    }

    void ReceiveSessionList(List<SessionInfo> sessions)
    {
        ClearBrowser();

        if (sessions.Count == 0)
        {
            NoSessionFound();
            return;
        }
        foreach(var session in sessions)
        {
            AddToSessionBrowser(session);
        }
    }

    void NoSessionFound()
    {
        _StatusText.text = "No session found";
        _StatusText.gameObject.SetActive(true);
    }
    void AddToSessionBrowser(SessionInfo sessionInfo)
    {
        var newItem = Instantiate(_sessionItemPrefab, _verticalLayoutGroup.transform);
        newItem.SetSessionInfo(sessionInfo);
        newItem.OnJoinSession += JoinSelectedSession;
    }
    void JoinSelectedSession(SessionInfo session)
    {
        _runnerHandler.JoinGame(session);
    }
}
