using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using SolidUtilities;

using GameFramework.System;

public class GameplayTagsWindow : EditorWindow
{
    static List<ValueTuple<string, string>> tags = new List<ValueTuple<string, string>>();
    bool wasChanged = false;

    ScrollView scrollView;
    TextField newTagText;

    [MenuItem("Tools/GameFramework/Gameplay Tags")]
    static void Init()
    {
        var window = CreateInstance<GameplayTagsWindow>();
        window.titleContent = new GUIContent("Gameplay Tags");
        window.Show();
    }

    private void OnEnable()
    {
        wasChanged = false;

        string[] tagsInFile = File.ReadAllLines("Assets/Resources/" + ProjectStatics.GameplayTagsAssetPath + ".txt");
        tags.Clear();

        for (int i = 0; i < tagsInFile.Length; i++)
        {
            string[] TagAndDescription = tagsInFile[i].Split(',');
            tags.Add(new(TagAndDescription[0], TagAndDescription.Length > 1 ? TagAndDescription[1] : ""));
        }
    }

    private void OnDisable()
    {
        if (wasChanged) SaveTags();
    }

    private void OnLostFocus()
    {
        if (wasChanged) SaveTags();
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        VisualElement newTagVis = new VisualElement();
        newTagVis.style.flexDirection = FlexDirection.Row;
        newTagVis.style.alignItems = Align.Center;
        newTagVis.style.flexBasis = 20f;
        newTagVis.style.minHeight = 20f;
        root.Add(newTagVis);

        Label newTagTextLabel = new Label("Tag name:");
        newTagVis.Add(newTagTextLabel);

        newTagText = new TextField();
        newTagText.style.flexGrow = 1;
        newTagVis.Add(newTagText);

        VisualElement newTagDescriptionVis = new VisualElement();
        newTagDescriptionVis.style.flexDirection = FlexDirection.Row;
        newTagDescriptionVis.style.alignItems = Align.Center;
        newTagDescriptionVis.style.flexBasis = 20f;
        newTagDescriptionVis.style.minHeight = 20f;
        root.Add(newTagDescriptionVis);

        Label newTagDescriptionLabel = new Label("Tag description:");
        newTagDescriptionVis.Add(newTagDescriptionLabel);

        TextField newTagDescription = new TextField();
        newTagDescription.style.flexGrow = 1;
        newTagDescriptionVis.Add(newTagDescription);

        Button button = new Button();
        button.text = "Add new tag";
        button.clicked += () => AddTag(newTagText.text, newTagDescription.text);
        button.style.flexBasis = 17f;
        button.style.minHeight = 17f;
        root.Add(button);

        Button discardButton = new Button();
        discardButton.text = "Discard changes";
        discardButton.clicked += () => { OnEnable(); FillScrollViev(); };
        discardButton.style.flexBasis = 17f;
        discardButton.style.minHeight = 17f;
        root.Add(discardButton);

        scrollView = new ScrollView(ScrollViewMode.Vertical);
        root.Add(scrollView);

        FillScrollViev();
    }

    private void FillScrollViev()
    {
        scrollView.Clear();

        foreach (var tag in tags)
        {
            VisualElement vis = new VisualElement();
            vis.style.flexDirection = FlexDirection.Row;
            vis.style.alignItems = Align.Center;
            vis.style.justifyContent = Justify.SpaceBetween;
            vis.style.borderBottomWidth = 0.1f;
            vis.style.borderBottomColor = Color.gray;
            scrollView.Add(vis);

            int lastIndexOf = tag.Item1.LastIndexOf('.');
            int charCount = tag.Item1.CountChars('.');

            //string labelText = "";
            //for(int i = 0; i < charCount; i++)
            //{
            //    labelText += " \u2011\u2011";
            //}
            string labelText = lastIndexOf != -1 ? tag.Item1.Substring(lastIndexOf + 1, tag.Item1.Length - 1 - lastIndexOf) : tag.Item1;

            Label label = new Label(labelText);
            label.style.left = charCount * 17f;
            label.tooltip = tag.Item2;
            vis.Add(label);

            VisualElement btns = new VisualElement();
            btns.style.flexDirection = FlexDirection.Row;
            vis.Add(btns);

            Button aBtn = new Button();
            aBtn.text = "+";
            aBtn.style.flexBasis = StyleKeyword.Auto;
            aBtn.style.height = 12;
            aBtn.clicked += () => { newTagText.value = tag.Item1 + "."; newTagText.Focus(); };
            btns.Add(aBtn);

            Button rBtn = new Button();
            rBtn.text = "-";
            rBtn.style.flexBasis = StyleKeyword.Auto;
            rBtn.style.height = 12;
            rBtn.clicked += () => { RemoveTag(tag.Item1); };
            btns.Add(rBtn);

            vis.Add(btns);
        }
    }

    private void AddTag(string tagName, string tagDescription)
    {
        if(tagName.Contains(" "))
        {
            EditorUtility.DisplayDialog("Gameplay Tags", "Wrong format", "Ok");
            return;
        }

        if(tagName.LastIndexOf(".") == tagName.Length - 1)
        {
            EditorUtility.DisplayDialog("Gameplay Tags", "GameplayTag is empty", "Ok");
            return;
        }

        int insertIndex = -1;

        for(int i = 0; i < tags.Count; i++)
        {
            if (tagName == tags[i].Item1)
            {
                EditorUtility.DisplayDialog("Gameplay Tags", "GameplayTag already exists. Tag description updated", "Ok");

                if (tagDescription != "")
                {
                    tags[i] = (tags[i].Item1, tagDescription);
                    SaveTags();
                }
                return;
            }
        }

        string parentTagName = tagName.LastIndexOf('.') != -1 ? tagName.Substring(0, tagName.LastIndexOf('.')) : tagName;
        int entryCharCount = parentTagName.CountChars('.');

        for (int i = 0; i < tags.Count; i++)
        {
            int charCount = tags[i].Item1.CountChars('.');

            if(charCount == entryCharCount)
            {
                if(tags[i].Item1 == parentTagName)
                {
                    insertIndex = i;
                    break;
                }
            }
        }

        if (insertIndex == -1)
        {
            string[] subTagNames = tagName.Split('.');

            for (int i = 1; i < subTagNames.Length - 1; i++)
            {
                subTagNames[i] = subTagNames[i - 1] + "." + subTagNames[i];
            }

            for (int i = 0; i < subTagNames.Length - 1; i++)
            {
                tags.Add(new(subTagNames[i], ""));
            }

            tags.Add(new(tagName, tagDescription));
        }
        else
        {
            tags.Insert(insertIndex + 1, new(tagName, tagDescription));
        }

        wasChanged = true;
        FillScrollViev();
    }

    private void RemoveTag(string tagName)
    {
        int removeIndex = -1;

        for (int i = 0; i < tags.Count; i++)
        {
            if (tagName == tags[i].Item1)
            {
                removeIndex = i;
                break;
            }
        }

        if( removeIndex == -1) { return; }

        List<string> tagsToRemove = new List<string>();
        tagsToRemove.Add(tagName);

        string[] entrySubTagNames = tagName.Split('.');

        for (int i = removeIndex + 1; i < tags.Count; i++)
        {
            string[] subTagNames = tags[i].Item1.Split('.');

            if (entrySubTagNames.Length > subTagNames.Length || entrySubTagNames.Length == subTagNames.Length) break;

            bool isSubTagOf = true;

            for (int j = 0; j < entrySubTagNames.Length; j++)
            {
                if (entrySubTagNames[j] == subTagNames[j])
                {
                    continue;
                }

                isSubTagOf = false;
                break;
            }

            if (!isSubTagOf) break;
            
            tagsToRemove.Add(tags[i].Item1);
        }

        for(int i = 0; i < tagsToRemove.Count; i++)
        {
            for(int j = 0; j < tags.Count; j++)
            {
                if (tags[j].Item1 == tagsToRemove[i])
                {
                    tags.RemoveAt(j);
                    break;
                }
            }
        }

        wasChanged = true;
        FillScrollViev();
    }

    private void SaveTags()
    {
        tags.Sort();
        string[] tagsToWrite = new string[tags.Count];

        for (int i = 0; i < tags.Count; i++)
        {
            tagsToWrite[i] = tags[i].Item1 + "," + tags[i].Item2;
        }

        File.WriteAllLines("Assets/Resources/" + ProjectStatics.GameplayTagsAssetPath + ".txt", tagsToWrite);
        AssetDatabase.Refresh();
    }
}
