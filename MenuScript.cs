using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject playerPrefab;

    public void HostGame()
    {
        menuPanel.SetActive(false);
        Instantiate(playerPrefab);
    }

    public void JoinGame()
    {
        menuPanel.SetActive(false);
    }
}
