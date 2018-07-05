using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using SharpCompress.Archives.Zip;
using SharpCompress.Archives.Rar;
using SharpCompress.Readers;
using SharpCompress.Common;
using XFSampleApp.ViewModels;

namespace XFSampleApp
{
	public partial class MainPage : ContentPage
	{
        private MainPageViewModel _mainPageViewModel;
        private string _archiveTypeStr;
        public MainPage()
		{
			InitializeComponent();
            BindingContext = _mainPageViewModel = new MainPageViewModel();
		}

        private async void MyButton_Clicked(object sender, EventArgs e)
        {
            var myButton = (sender as Button);
            myButton.IsEnabled = false;

            _archiveTypeStr = myButton.Text.Substring(0, 3);
            var unarchiveResult = await UnarchiveByFilePath($"Test{_archiveTypeStr}File.{_archiveTypeStr.ToLower()}");

            var message = unarchiveResult.Item1 ? "解壓縮完畢" : string.IsNullOrEmpty(unarchiveResult.Item2) ? "解壓縮失敗" : unarchiveResult.Item2;
            await DisplayAlert("通知",message, "Ok");

            myButton.IsEnabled = true;

        }
        private void MyButtonWithPwd_Clicked(object sender, EventArgs e)
        {
            var myButton = (sender as Button);
            myButton.IsEnabled = false;

            _mainPageViewModel.IsNeedPassword = true;
            _archiveTypeStr = myButton.Text.Substring(0, 3);

            myButton.IsEnabled = true;

        }

        private async void PasswordButtonDone_Clicked(object sender, EventArgs e)
        {
            var myButton = (sender as Button);
            myButton.IsEnabled = false;

            var unarchiveResult = await UnarchiveByFilePath($"Test{_archiveTypeStr}File_withPWD.{_archiveTypeStr.ToLower()}", PasswordEntry.Text);

            var message = unarchiveResult.Item1 ? "解壓縮完畢" : string.IsNullOrEmpty(unarchiveResult.Item2) ? "解壓縮失敗" : unarchiveResult.Item2;

            _mainPageViewModel.IsNeedPassword = false;

            await DisplayAlert("通知", message, "Ok");

            myButton.IsEnabled = true;


        }

        private void PasswordButtonCancel_Clicked(object sender, EventArgs e)
        {

            _mainPageViewModel.IsNeedPassword = false;

        }

        private static async Task<Tuple<bool, string>> UnarchiveByFilePath(string fileFullPath, string password = "")
        {
            using (var stream = await FileSystem.OpenAppPackageFileAsync(fileFullPath))
            {
                ReaderOptions readerOptions = null;

                if (!string.IsNullOrEmpty(password))
                {
                   
                    readerOptions = new ReaderOptions() { Password = password };

                }
                try
                {
                    using (var reader = ReaderFactory.Open(stream, readerOptions))
                    {
                        while (reader.MoveToNextEntry())
                        {
                            if (!reader.Entry.IsDirectory)
                            {
                                Console.WriteLine(reader.Entry.Key);
                                reader.WriteEntryToDirectory(FileSystem.AppDataDirectory, new ExtractionOptions()
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                            }
                        }
                        return new Tuple< bool,string>(true, string.Empty);
                    }
                }
                catch (CryptographicException cryptographicException)
                {
                    System.Diagnostics.Debug.WriteLine(cryptographicException.Message);
                    return new Tuple<bool, string>(false, "解壓縮密碼錯誤");
                }
            }
        }
    }
}
