using AstrolabeDC.dcSharp;
using AstrolabeDC.MediaRendering;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using LiteHtmlMaui.Controls;
using System.Collections.ObjectModel;

namespace AstrolabeDC.ViewModels
{
    public class DetailViewModel : ObservableObject
    {
        private readonly StackLayout _stackLayout;
        private GallDetail gallDetail = new GallDetail();
        ImageRendering imageRequest = new ImageRendering();

        private string? titleHeader;
        public string? TitleHeader
        {
            get { return titleHeader; }
            set { SetProperty(ref titleHeader, value); }
        }

        private string? userHeade;
        public string? UserHeade
        {
            get { return userHeade; }
            set { SetProperty(ref userHeade, value); }
        }


        private string? countHeade;
        public string? CountHeade
        {
            get { return countHeade; }
            set { SetProperty(ref countHeade, value); }
        }


        private string? commenHeade;
        public string? CommenHeade
        {
            get { return commenHeade; }
            set { SetProperty(ref commenHeade, value); }
        }

        private string? replynumHeade;
        public string? ReplynumHeade
        {
            get { return replynumHeade; }
            set { SetProperty(ref replynumHeade, value); }
        }

        private string? dateHeade;
        public string? DateHeade
        {
            get { return dateHeade; }
            set { SetProperty(ref dateHeade, value); }
        }

        private string? upBox;
        public string? UpBox
        {
            get { return upBox; }
            set { SetProperty(ref upBox, value); }
        }

        private string? downBox;
        public string? DownBox
        {
            get { return downBox; }
            set { SetProperty(ref downBox, value); }
        }

        public DetailViewModel(StackLayout stackLayout, string url) 
        {
            _stackLayout = stackLayout;
            GetGallDetailData(url);
        }

        private void HeaderData(IDictionary<string, string> heade_data) 
        {
            TitleHeader = heade_data["Title"];
            UserHeade = heade_data["User"];
            CountHeade = heade_data["Count"];
            CommenHeade = heade_data["Comment"];
            ReplynumHeade = heade_data["Replynum"];
            DateHeade = heade_data["Date"];
        }

        private void RecommendBox(IDictionary<string, string> recommend_data) 
        {
            UpBox =  $"추천 {recommend_data["Up"]}";
            DownBox = $"비추천 {recommend_data["Down"]}";
        }

        private async void GetGallDetailData(string url) 
        {

#if ANDROID
            await Task.Delay(500);
#endif
            await gallDetail.GetGallDetail(url);
            var detail_data = await gallDetail.DetailData();
            HeaderData(gallDetail.GallUserData());
            RecommendBox(gallDetail.RecommendBox());

            foreach (var detail_item in detail_data)
            {
                if (detail_item.ContainsKey("Image"))
                {
                    var imageView = new Image
                    {
                        Source = await imageRequest.LoadImage(detail_item["Image"]),
                        IsAnimationPlaying = true,
                    };
                    imageView.HeightRequest = imageRequest.LoadImageHeight();

                    _stackLayout.Children.Add(imageView);
                }
                else if (detail_item.ContainsKey("GIF"))
                {
                    var gifView = new Image
                    {
                        Source = await imageRequest.LoadImage(detail_item["GIF"]),
                        IsAnimationPlaying = true,
                    };
                    gifView.HeightRequest = imageRequest.LoadImageHeight();

                    _stackLayout.Children.Add(gifView);
                }
                else if (detail_item.ContainsKey("Embed")) 
                {
                    var labelView = new Label
                    {
                        Text = detail_item["Embed"],
                        HorizontalOptions = LayoutOptions.Center,
                    };

                    _stackLayout.Children.Add(labelView);
                }
                else if (detail_item.ContainsKey("Audio"))
                {
                    var audioPaly = new MediaElement
                    {
                        Source = detail_item["Audio"],
                    };
#if ANDROID
                    audioPaly.HeightRequest = 300;
                    audioPaly.WidthRequest = 400;
#elif WINDOWS || IOS || MACCATALYST || TIZEN
                    audioPaly.WidthRequest = 400;
#endif
                    _stackLayout.Children.Add(audioPaly);
                }
                else if (detail_item.ContainsKey("Html"))
                {
                    var htmlView = new LiteHtml
                    {
                        Html = detail_item["Html"],
                        HorizontalOptions = LayoutOptions.Center,
                    };

                    _stackLayout.Children.Add(htmlView);
                }
                await Task.Delay(100);
            }
        }
    }
}
