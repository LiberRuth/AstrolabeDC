using AstrolabeDC.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AstrolabeDC.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? _enteredText;

        [RelayCommand]
        private async Task NavigateButton(string text)
        {
            if (string.IsNullOrEmpty(text)) return; 
            await Application.Current!.MainPage!.Navigation.PushAsync(new SearchPage(text));
        }
    }
}
