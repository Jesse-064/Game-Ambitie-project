using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HighScoreControl : MonoBehaviour
{
    public string addScoreURL = "http://localhost/php/addscore.php?";
    public string highscoreURL = "http://localhost/php/display.php";
    public Text nameTextInput;
    public Text timeTextInput;
    public Text lapTimeTextInput;
    public Text levelTextInput;
    public Text nameResultText;
    public Text scoreResultText;

    public void GetScoreBtn()
    {
        nameResultText.text = "Player: \n\n";
        scoreResultText.text = "Score: \n\n";
        StartCoroutine(GetScores());
    }

    public void SendScoreBtn()
    {
        StartCoroutine(PostScores(
            nameTextInput.text,
            timeTextInput.text,
            lapTimeTextInput.text,
            int.Parse(levelTextInput.text)
            ));
        nameTextInput.gameObject.transform.parent.GetComponent<InputField>().text = "";
    }

    IEnumerator GetScores()
    {
        UnityWebRequest hs_get = UnityWebRequest.Get(highscoreURL);
        yield return hs_get.SendWebRequest();

        if (hs_get.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("There was an error getting the high score: " + hs_get.error);
        }
        else
        {
            string dataText = hs_get.downloadHandler.text;
            MatchCollection mc = Regex.Matches(dataText, @"_");
            if (mc.Count > 0)
            {
                string[] splitData = Regex.Split(dataText, @"_");
                for (int i = 0; i < mc.Count; i++)
                {
                    if (i % 2 == 0)
                        nameResultText.text += splitData[i];
                    else
                        scoreResultText.text += splitData[i];
                }
            }
        }
    }

    IEnumerator PostScores(string name, string time, string lapTime, int level)
    {
        string post_url = addScoreURL + "player_name=" + UnityWebRequest.EscapeURL(name) + "&level=" + level + "&time=" + UnityWebRequest.EscapeURL(time) + "&best_lap_time=" + UnityWebRequest.EscapeURL(lapTime);

        UnityWebRequest hs_post = UnityWebRequest.PostWwwForm(post_url, "");
        yield return hs_post.SendWebRequest();

        if (hs_post.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("There was an error posting the high score: " + hs_post.error);
        }
    }
}
