using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _username;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _leaderboardTitle;

    public void SetUsername(string username) => _username.text = username;
    public void SetText(string text) => _text.text = text;
    public void SetTitle(string title) => _leaderboardTitle.text = title;
}
