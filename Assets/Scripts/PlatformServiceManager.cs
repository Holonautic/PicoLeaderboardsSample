using System.Text;
using Pico.Platform;
using Pico.Platform.Models;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformServiceManager : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;

    private const string leaderboardName = "TestLeaderboard";
    private readonly StringBuilder _stringBuilder = new();
    
    private void Awake()
    {
        CoreService.AsyncInitialize()
            .OnComplete(m =>
            {
                if (m.IsError)
                {
                    var errorText = $"Init error\nCode:{m.GetError().Code}\nMessage:{m.GetError().Message}";
                    _uiManager.SetText(errorText);
                    return;
                }

                if (m.Data != PlatformInitializeResult.Success && m.Data != PlatformInitializeResult.AlreadyInitialized)
                {
                    var errorText = $"Init error - already initialized\nCode:{m.GetError().Code}\nMessage:{m.GetError().Message}";
                    _uiManager.SetText(errorText);
                    return;
                }
                
                UserService.GetLoggedInUser()
                    .OnComplete(message =>
                    {
                        if (!message.IsError)
                        {         
                            var user = message.Data;
                            _uiManager.SetUsername(user.DisplayName);
                        }
                        else
                        {         
                            _uiManager.SetUsername("Error");         
                    
                            var error = message.GetError();         
                            var errorText = $"Error: {error.Code}\n{error.Message}";
                            _uiManager.SetText(errorText);
                        } 
                    });
            });
    }

    public void AddEntry()
    {
        var randomScore = (long)Random.Range(100, 1000);
        
        LeaderboardService.WriteEntry(leaderboardName, randomScore)
            .OnComplete(m =>
            {
                _uiManager.SetText(!m.IsError
                    ? $"Entry added! Randomly generated score: {randomScore}"
                    : $"Entry NOT added! Error: {m.GetError().Message}");
            });
    }

    public void LoadFriends()
    {
        _uiManager.SetTitle("Friends");
        
        LeaderboardService.GetEntries(leaderboardName, 10, 0, LeaderboardFilterType.Friends,
                LeaderboardStartAt.CenteredOnViewerOrTop)
            .OnComplete(OnEntriesReceived);
    }

    public void LoadGlobal()
    {
        _uiManager.SetTitle("Global");
        
        LeaderboardService.GetEntries(leaderboardName, 10, 0, LeaderboardFilterType.None,
                LeaderboardStartAt.CenteredOnViewerOrTop)
            .OnComplete(OnEntriesReceived);
    }

    public void LoadTop10()
    {
        _uiManager.SetTitle("Top 10");
        
        LeaderboardService.GetEntries(leaderboardName, 10, 0, LeaderboardFilterType.None, LeaderboardStartAt.Top)
            .OnComplete(OnEntriesReceived);
    }

    private void OnEntriesReceived(Message<LeaderboardEntryList> message)
    {
        _stringBuilder.Clear();
        
        if (!message.IsError)
        {
            foreach (var element in message.Data)
            {
                _stringBuilder.Append($"{element.Rank} | {element.User.DisplayName} | {element.Score}\n");
            }
            
            _uiManager.SetText(_stringBuilder.ToString());
        }
        else
        {
            var errorText = $"<color=red>Error receiving leaderboard entries:</color> {message.GetError().Message}";
            _uiManager.SetText(errorText);
        }
    }
}