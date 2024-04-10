using AstrolabeDC.dcSharp;
using AstrolabeDC.Models;
using AstrolabeDC.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Web;

namespace AstrolabeDC.ViewModels
{
    public partial class SearchViewModel : ObservableObject
    {
#if WINDOWS || MACCATALYST
        private int pageSize = 50;
#elif ANDROID || IOS
        private int pageSize = 30;
#endif
        private int pageMax = 1;

        private Search search = new Search();
        private List<IDictionary<string, string>> searchData = new List<IDictionary<string, string>>();

        [ObservableProperty]
        private ObservableCollection<SearchModel>? _collectioItems;

        [ObservableProperty]
        private ObservableCollection<PagePickerModel>? _pickerItems;

        [ObservableProperty]
        private bool _isActivityIndicatorRunning;

        private SearchModel? _collectioSelectedItem;
        public SearchModel? CollectioSelectedItem
        {
            get { return _collectioSelectedItem; }
            set
            {
                SetProperty(ref _collectioSelectedItem, value);
                CollectioShowSelectedValue();
            }
        }

        private PagePickerModel? _pickerselectedItem;
        public PagePickerModel? PickerSelectedItem
        {
            get { return _pickerselectedItem!; }
            set
            {
                SetProperty(ref _pickerselectedItem, value);
                PickerShowSelectedValue();
            }
        }

        public SearchViewModel(string keyword)
        {
            Initialize($"https://search.dcinside.com/gallery/q/{UrlConversion(keyword)}");
        }

        private void PickerShowSelectedValue()
        {
            if (PickerSelectedItem != null) 
            {
                var searchPageData = GetPage(searchData, PickerSelectedItem!.Number, pageSize);

                CollectioItems = new ObservableCollection<SearchModel>();
                CollectioItems.Clear();

                foreach (var searchItem in searchPageData)
                {
                    CollectioItems.Add(new SearchModel()
                    {
                        Title = searchItem["Title"],
                        Text = searchItem["Text"],
                        DataUrl = searchItem["Url"],
                    });
                }
            }
        }

        private async void CollectioShowSelectedValue() 
        {
            if (CollectioSelectedItem != null)
            {
                await Application.Current!.MainPage!.Navigation.PushAsync(new ListPage(CollectioSelectedItem.DataUrl!));
            }
        }

        [RelayCommand]
        private void OnCollectioItemSelected(SearchModel item)
        {
            CollectioSelectedItem = item;
        }

        [RelayCommand]
        private void OnPickeItemSelected(PagePickerModel item)
        {
            PickerSelectedItem = item;
        }

        private async void Initialize(string url)
        {
            await GetSearchPageData(url);
            PickePages();
        }

        private async Task GetSearchPageData(string url)
        {
            CollectioItems = new ObservableCollection<SearchModel>();
            CollectioItems.Clear();

            IsActivityIndicatorRunning = true;

#if ANDROID
            await Task.Delay(500);
#endif
            //await Task.Delay(500);
            await search.GetSearch(url);
            searchData = search.Gallery();

            if (searchData == null)
            {
                IsActivityIndicatorRunning = false;
                await Application.Current!.MainPage!.DisplayAlert("Error", search.errorMessage, "OK");
                return;
            }

            int total = searchData.Count();

            while (true) { pageMax++; total = total - pageSize; if (total <= 0) { break; } }

            /* var searchPageData = GetPage(searchData, 1, pageSize);

            foreach (var searchItem in searchPageData)
            {
                CollectioItems.Add(new SearchModel()
                {
                    Title = searchItem["Title"],
                    Text = searchItem["Text"],
                    DataUrl = searchItem["Url"],
                });
            } */

            IsActivityIndicatorRunning = false;
        }

        private void PickePages() 
        {
            PickerItems = new ObservableCollection<PagePickerModel>();
            for (int i = 1; i < pageMax; i++)
            {
                PickerItems.Add(new PagePickerModel{ Number = i});
            }

            PickerSelectedItem = PickerItems.FirstOrDefault();
        }

        private static List<IDictionary<string, string>> GetPage(List<IDictionary<string, string>> source, int pageNumber, int pageSize)
        {
            int skipCount = (pageNumber - 1) * pageSize;
            return source.Skip(skipCount).Take(pageSize).ToList();
        }

        private string UrlConversion(string originalText)
        {
            string encodedText = HttpUtility.UrlEncode(originalText);
            encodedText = encodedText.Replace("%", ".");
            encodedText = encodedText.Replace("+", ".20");
            return encodedText;
        }
    }
}
