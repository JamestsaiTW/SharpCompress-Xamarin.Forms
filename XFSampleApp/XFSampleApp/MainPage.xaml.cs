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

namespace XFSampleApp
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

        private async void MyButton_Clicked(object sender, EventArgs e)
        {
            var myButton = (sender as Button);
            myButton.IsEnabled = false;

            var archiveTypeStr = myButton.Text.Substring(0, 3);
            var isOk = await UnarchiveByFilePath($"localdemofiles/Test{archiveTypeStr}File.{archiveTypeStr.ToLower()}");

            var message = isOk ? "解壓縮完畢" : "解壓縮失敗";
            await DisplayAlert("通知",message, "Ok");

            myButton.IsEnabled = true;

        }
        private async void MyButtonWithPwd_Clicked(object sender, EventArgs e)
        {
            var myButton = (sender as Button);
            myButton.IsEnabled = false;

            var archiveTypeStr = myButton.Text.Substring(0, 3);
            var isOk = await UnarchiveByFilePath($"localdemofiles/Test{archiveTypeStr}File_withPWD.{archiveTypeStr.ToLower()}", "223456");

            var message = isOk ? "解壓縮完畢" : "解壓縮失敗";
            await DisplayAlert("通知", message, "Ok");

            myButton.IsEnabled = true;

        }
        private static async Task<bool> UnarchiveByFilePath(string fileFullPath, string password = "")
        {
            using (var stream = await FileSystem.OpenAppPackageFileAsync(fileFullPath))
            {
                ReaderOptions readerOptions = null;

                if (!string.IsNullOrEmpty(password))
                    readerOptions = new ReaderOptions() { Password = password };

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
                    return true;
                }
            }    
        }
    }
}
