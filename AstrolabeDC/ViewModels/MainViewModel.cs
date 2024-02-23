using AstrolabeDC.Views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AstrolabeDC.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private string? _enteredText;
        public string? EnteredText
        {
            get { return _enteredText; }
            set { SetProperty(ref _enteredText, value); }
        }

        public Command<string> NavigateCommand { get; }

        public MainViewModel()
        {
            NavigateCommand = new Command<string>(NavigateToNewPage);
        }

        private async void NavigateToNewPage(string text)
        {
            await Application.Current!.MainPage!.Navigation.PushAsync(new SearchPage(text));
        }
    }
}
