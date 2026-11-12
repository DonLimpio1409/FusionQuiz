using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

/// <summary>
/// Scriptable object that defines the trivia questions.  Can be defined by a CSV file.
/// </summary>
[CreateAssetMenu()]
public class TriviaQuestionSet : ScriptableObject
{
	[SerializeField] private TextAsset _questionCSVEnglish;
	[SerializeField] private TextAsset _questionCSVSpanish;

	[SerializeField, Tooltip("List of the trivia questions.")]
	private List<TriviaQuestion> _questions;

	/// <summary>
	/// Simple getter for the Trivia Questions
	/// </summary>
	public ReadOnlyCollection<TriviaQuestion> Questions => _questions.AsReadOnly();
	
	[System.Serializable()]
	public class TriviaQuestion
	{
		[Tooltip("The question being asked.")]
		public string question;

		[Tooltip("The correct answer to the question.")]
		public string correctAnswer;
		public string[] decoyAnswers;
	}

	public void UpdateQuestions()
	{
		CreateQuestions();
	}

	[ContextMenu("Create Questions")]
	public void CreateQuestions()
	{
		_questions = new List<TriviaQuestion>();

		if (_questionCSVEnglish == null || _questionCSVSpanish == null)
		{
			Debug.LogWarning("CSVs not assigned.");
			return;
		}

		string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
		string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
		char[] TRIM_CHARS = { '\"' };

		List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

		bool spanish = LocalizationSettings.SelectedLocale.Identifier.Code == "es";

		TextAsset data = spanish
			? _questionCSVSpanish
			: _questionCSVEnglish;

		var lines = Regex.Split(data.text, LINE_SPLIT_RE);
		Debug.Log("Líneas encontradas: " + lines.Length);

		if (lines.Length > 1)
		{
			var header = Regex.Split(lines[0], SPLIT_RE);
			for (var i = 1; i < lines.Length; i++)
			{

				var values = Regex.Split(lines[i], SPLIT_RE);
				if (values.Length == 0 || values[0] == "") continue;

				var entry = new Dictionary<string, object>();
				for (var j = 0; j < header.Length && j < values.Length; j++)
				{
					string value = values[j];

					value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
					value = value.Replace("\"\"", "\"");

					object finalvalue = value;

					entry[header[j]] = finalvalue;
				}
				list.Add(entry);
			}
		}

		// Goes through each line and sets up the questions
		for (int i = 0; i < list.Count; i++)
        {
			TriviaQuestion question = new TriviaQuestion();
			List<string> keys = new List<string>(list[i].Keys);

				if (keys.Count < 5)
				{
					Debug.LogError("Fila inválida: " + i);
					continue;
				}
				question.question      = list[i][keys[0]] as string;
				question.correctAnswer = list[i][keys[1]] as string;

				question.decoyAnswers = new string[3];
				question.decoyAnswers[0] = list[i][keys[2]] as string;
				question.decoyAnswers[1] = list[i][keys[3]] as string;
				question.decoyAnswers[2] = list[i][keys[4]] as string;

			_questions.Add(question);
        }
	}
}
