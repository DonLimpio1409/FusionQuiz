using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class Localization : MonoBehaviour
{
    private bool spanish = false;
    [SerializeField] private TriviaQuestionSet Set;

    private async void Start()
    {
        await LocalizationSettings.InitializationOperation.Task;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spanish = !spanish;
            LocalizationSettings.SelectedLocale =LocalizationSettings.AvailableLocales.GetLocale(spanish ? "es" : "en");
            Set.UpdateQuestions();
        }
    }

    public void Spanish()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("es");
        Set.UpdateQuestions();
    }

    public void English()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
        Set.UpdateQuestions();
    }
}