using AstrolabeDC.dcSharp;
using AstrolabeDC.Models;
using AstrolabeDC.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AstrolabeDC.ViewModels
{
    public class ListViewModel : ObservableObject
    {
        private int currentPage = 1;
        private int maxPage = 1;
        private string? exceptionMode;
        private string? currentURL;

        private GallList gallList = new GallList();
        private List<IDictionary<string, string>> gallListData = new List<IDictionary<string, string>>();

        private ObservableCollection<ListModel>? _collectioItems;
        public ObservableCollection<ListModel>? CollectioItems
        {
            get { return _collectioItems; }
            set { SetProperty(ref _collectioItems, value); }
        }

        private ObservableCollection<TabBoxModel>? _pickerItems;
        public ObservableCollection<TabBoxModel>? PickerItems
        {
            get { return _pickerItems; }
            set { SetProperty(ref _pickerItems, value); }
        }

        private bool _isActivityIndicatorRunning;
        public bool IsActivityIndicatorRunning
        {
            get => _isActivityIndicatorRunning;
            set => SetProperty(ref _isActivityIndicatorRunning, value);
        }

        private bool _isEnabledPreviousPage;
        public bool IsEnabledPreviousPage
        {
            get => _isEnabledPreviousPage;
            set => SetProperty(ref _isEnabledPreviousPage, value);
        }

        private bool _isEnabledNextPage;
        public bool IsEnabledNextPage
        {
            get => _isEnabledNextPage;
            set => SetProperty(ref _isEnabledNextPage, value);
        }

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

        public ICommand CollectioItemSelectedCommand { get; }
        public ICommand PickeItems { get; }
        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }

        public ListViewModel(string url)
        {
            Initialize(url);
            currentURL = url;
            CollectioItemSelectedCommand = new Command<ListModel>(OnCollectioItemSelected);
            PickeItems = new Command<TabBoxModel>(OnPickeItemSelected);
            PreviousCommand = new Command(PreviousPageCommand);
            NextCommand = new Command(NextPageCommand);
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

        private void OnCollectioItemSelected(ListModel item)
        {
            CollectioSelectedItem = item;
        }

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

            //await Task.Delay(500);
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

        private void PreviousPageCommand()
        {
            currentPage--;
            GetGallListData($"{currentURL}&page={currentPage}");
        }

        private void NextPageCommand()
        {
            currentPage++;
            GetGallListData($"{currentURL}&page={currentPage}");
        }

        private void PickePages()
        {
            //PickerItems.Add(new TabBoxModel { Number = i });
            PickerItems = new ObservableCollection<TabBoxModel> 
            {
               new TabBoxModel { Title = "일반", Value = "&exception_mode=all" },
               new TabBoxModel { Title = "개념글", Value = "&exception_mod=erecommend" },
               new TabBoxModel { Title = "공지", Value = "&exception_mode=notice" },
            };
        }
    }
}
