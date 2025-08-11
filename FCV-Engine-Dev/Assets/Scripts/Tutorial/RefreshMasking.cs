using System.Threading.Tasks;
using Unity.Tutorials.Core.Editor;
using UnityEngine;


[CreateAssetMenu(fileName = "RefreshMasking", menuName = "Tutorials/Create RefreshMasking")]
public class RefreshMasking : ScriptableObject
{
    private bool subscribed = false;
    public void StartConstantRefreshing(Tutorial tutorial, TutorialPage page, int id) => StartConstantRefreshing();
    public void StopConstantRefreshing(Tutorial tutorial, TutorialPage page) => StopConstantRefreshing();
    public void StopConstantRefreshing(Tutorial tutorial) => StopConstantRefreshing();
    
    
    
    public void StartConstantRefreshing()
    {
        if (subscribed) return;
        
        subscribed = true;
        EditorSelection.OnEditorInteracted += UpdateMasking;
    }

    public void StopConstantRefreshing()
    {
        subscribed = false;
        EditorSelection.OnEditorInteracted -= UpdateMasking;
    }

    private async void UpdateMasking()
    {
        await Task.Yield();
        if (!TutorialWindow.Instance.CurrentTutorial)
        {
            return;
        }
        Debug.Log("RefreshMasking");
        TutorialWindow.Instance.CurrentTutorial.CurrentPage.RaiseMaskingSettingsChanged();
    }
}
