using Content.Client.Stylesheets;
using Content.Shared.Traits;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Lobby.UI.Roles;

[GenerateTypedNameReferences]
public sealed partial class TraitPreferenceSelector : Control
{
    public int Cost;
    private bool _preference;

    public bool Preference
    {
        get => _preference;
        set
        {
            _preference = value;
            UpdateButtonState();
        }
    }

    public event Action<bool>? PreferenceChanged;

    public TraitPreferenceSelector(TraitPrototype trait)
    {
        RobustXamlLoader.Load(this);

        Cost = trait.Cost;

        // Set the trait name
        NameLabel.Text = Loc.GetString(trait.Name);

        // Set the cost text and color
        string costText = "";
        if (trait.Cost > 0)
        {
            costText = $"+{trait.Cost}";
            CostLabel.FontColorOverride = Color.Red;
        }
        else if (trait.Cost < 0)
        {
            costText = $"{trait.Cost}";
            CostLabel.FontColorOverride = Color.Green;
        }
        else
        {
            costText = $"{trait.Cost}";
            CostLabel.FontColorOverride = Color.Gray;
        }

        CostLabel.Text = costText;

        // Set up button event
        TraitButton.OnPressed += OnTraitButtonPressed;

        if (trait.Description is { } desc)
        {
            TraitButton.ToolTip = Loc.GetString(desc);
        }

        UpdateButtonState();
    }

    private void OnTraitButtonPressed(BaseButton.ButtonEventArgs args)
    {
        // Clicking the trait button toggles selection
        Preference = !Preference;
        PreferenceChanged?.Invoke(Preference);
    }

    private void UpdateButtonState()
    {
        if (Preference)
        {
            // Add a visual indicator that it's selected
            TraitButton.StyleClasses.Remove("OpenBottom");
            TraitButton.AddStyleClass("OpenBottomSelected");

            TraitButton.ModulateSelfOverride = Color.FromHex("#505050");
        }
        else
        {
            // Remove the selected visual indicator
            TraitButton.StyleClasses.Remove("OpenBottomSelected");
            TraitButton.StyleClasses.Add("OpenBottom");

            // Reset the background color
            TraitButton.ModulateSelfOverride = null;
        }
    }

    /// <summary>
    /// Sets the trait as unavailable for selection by coloring the name text red
    /// instead of coloring the entire button.
    /// </summary>
    /// <param name="unavailable">Whether the trait is unavailable for selection</param>
    public void SetUnavailable(bool unavailable)
    {
        if (unavailable)
        {
            NameLabel.FontColorOverride = Color.Red;
            TraitButton.Disabled = true;

            // Preserve the darker gray background if this trait is selected
            if (Preference)
            {
                TraitButton.ModulateSelfOverride = Color.FromHex("#454545");
            }
        }
        else
        {
            NameLabel.FontColorOverride = null;
            TraitButton.Disabled = false;

            // Restore the appropriate background color based on selection state
            if (Preference)
            {
                TraitButton.ModulateSelfOverride = Color.FromHex("#505050");
            }
            else
            {
                TraitButton.ModulateSelfOverride = null;
            }
        }
    }
}
