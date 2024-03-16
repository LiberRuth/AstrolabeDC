using AstrolabeDC.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AstrolabeDC.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private string? _enteredText;
        public string? EnteredText
        {
            get { return _enteredText; }
            set { SetProperty(ref _enteredText, value); }
        }

        //public Command<string> NavigateCommand { get; }

        public MainViewModel()
        {
            //NavigateCommand = new Command<string>(NavigateButton);
        }

        [RelayCommand]
        private async Task NavigateButton(string text)
        {
            await Application.Current!.MainPage!.Navigation.PushAsync(new SearchPage(text));
        }
    }
}
