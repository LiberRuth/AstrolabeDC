﻿using AstrolabeDC.ViewModels;

namespace AstrolabeDC.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }
}