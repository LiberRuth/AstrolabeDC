using AstrolabeDC.ViewModels;

namespace AstrolabeDC.Views;

public partial class ListPage : ContentPage
{
	public ListPage(string URL)
	{
		InitializeComponent();
        BindingContext = new ListViewModel(URL);
    }
}