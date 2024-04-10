using AstrolabeDC.dcSharp;
using AstrolabeDC.Models;
using AstrolabeDC.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AstrolabeDC.ViewModels
{
    public partial class ListViewModel : ObservableObject
    {
        private int currentPage = 1;
        private int maxPage = 1;
        private string? exceptionMode;
        private string? currentURL;

        private GallList gallList = new GallList();
        private List<IDictionary<string, string>> gallListData = new List<IDictionary<string, string>>();

        [ObservableProperty]
        private ObservableCollection<ListModel>? _collectioItems;

        [ObservableProperty]
        private ObservableCollection<TabBoxModel>? _pickerItems;

        [ObservableProperty]
        private bool _isActivityIndicatorRunning;

        [ObservableProperty]
        private bool _isEnabledPreviousPage;

        [ObservableProperty]
        private bool _isEnabledNextPage;

        private ListModel? _collectioSelectedItem;
        public ListModel? CollectioSelectedItem
        {
            get { return _collectioSelectedItem; }
            set
            {
                SetProperty(ref _collectioSelectedItem, value);
                CollectioShowSelectedValue();
            }
        }

        private TabBoxModel? _pickerselectedItem;
        public TabBoxModel? PickerSelectedItem
        {
            get { return _pickerselectedItem!; }
            set
            {
                SetProperty(ref _pickerselectedItem, value);
                PickerShowSelectedValue();
            }
        }

        public ListViewModel(string url)
        {
            Initialize(url);
            currentURL = url;
        }

        private void PickerShowSelectedValue()
        {
            if (PickerSelectedItem != null)
            {
                exceptionMode = PickerSelectedItem.Value!;
                GetGallListData(currentURL + exceptionMode);
            }
        }

        private async void CollectioShowSelectedValue()
        {
            if (CollectioSelectedItem != null)
            {
                await Application.Current!.MainPage!.Navigation.PushAsync(new DetailPage($"https://gall.dcinside.com{CollectioSelectedItem.DataUrl}"));
            }
        }

        [RelayCommand]
        private void OnCollectioItemSelected(ListModel item)
        {
            CollectioSelectedItem = item;
        }

        [RelayCommand]
        private void OnPickeItemSelected(TabBoxModel item) 
        {
            PickerSelectedItem = item;
        }

        private void Initialize(string url)
        {
            GetGallListData(url);
            PickePages();
        }

        private async void GetGallListData(string url) 
        {
            CollectioItems = new ObservableCollection<ListModel>();
            CollectioItems.Clear();

            IsActivityIndicatorRunning = true;

#if ANDROID
            await Task.Delay(500);
#endif
            await gallList.GetGallList(url);
            gallListData = gallList.Gall_list();

            if (gallListData == null && gallList.errorMessage != null)
            {
                IsActivityIndicatorRunning = false;
                await Application.Current!.MainPage!.DisplayAlert("Error", gallList.errorMessage, "OK");
                return;
            }

            maxPage = gallList.MaxPaging();
            IsEnabledNextPage = (currentPage + 1 <= maxPage);
            IsEnabledPreviousPage = (currentPage > 1);

            if (gallListData != null)
            {
                foreach (var gallListItem in gallListData)
                {
                    CollectioItems.Add(new ListModel()
                    {
                        Id = gallListItem["Num"],
                        Title = gallListItem["Title"],
                        User = gallListItem["User"],
                        Views = gallListItem["Count"],
                        Reply = gallListItem["Reply"],
                        Recommend = gallListItem["Recommend"],
                        Time = gallListItem["Date"],
                        Subject = gallListItem["Subject"],
                        DataUrl = gallListItem["GallURL"],
                    });
                }
            }

            IsActivityIndicatorRunning = false;
        }

        [RelayCommand]
        private void PreviousButton()
        {
            currentPage--;
            GetGallListData($"{currentURL}&page={currentPage}{exceptionMode}");
        }

        [RelayCommand]
        private void NextButton()
        {
            currentPage++;
            GetGallListData($"{currentURL}&page={currentPage}{exceptionMode}");
        }

        private void PickePages()
        {
            //PickerItems.Add(new TabBoxModel { Number = i });
            PickerItems = new ObservableCollection<TabBoxModel> 
            {
               new TabBoxModel { Title = "일반", Value = "&exception_mode=all" },
               new TabBoxModel { Title = "개념글", Value = "&exception_mode=recommend" },
               new TabBoxModel { Title = "공지", Value = "&exception_mode=notice" },
            };
        }
    }
}
