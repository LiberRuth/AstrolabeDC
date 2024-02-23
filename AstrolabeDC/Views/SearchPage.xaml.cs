using AstrolabeDC.ViewModels;

namespace AstrolabeDC.Views;

public partial class SearchPage : ContentPage
{
    public SearchPage(string keyword)
	{
		InitializeComponent();
        BindingContext = new SearchViewModel(keyword);
    }
}
