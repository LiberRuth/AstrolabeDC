using AstrolabeDC.ViewModels;

namespace AstrolabeDC.Views;

public partial class DetailPage : ContentPage
{
	public DetailPage(string URL)
	{
		InitializeComponent();
        BindingContext = new DetailViewModel(stackLayout, URL);
    }
}