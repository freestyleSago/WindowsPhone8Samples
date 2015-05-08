using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace DelayLoadDemo
{
    public class Data : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public Page Page { get; set; }

        private Uri imageUri;
        public Uri ImageUri
        {
            get
            {
                return imageUri;
            }
            set
            {
                if (imageUri == value)
                {
                    return;
                }
                imageUri = value;
                bitmapImage = null;
            }
        }
        WeakReference bitmapImage;

        public ImageSource ImageSource
        {

            get
            {
                if (bitmapImage != null)
                {
                    if (bitmapImage.IsAlive)
                        return (ImageSource)bitmapImage.Target;
                    else
                        Debug.WriteLine("数据已经被回收");
                }
                if (imageUri != null)
                {
                    Task.Factory.StartNew(() => { DownloadImage(imageUri); });
                }
                return null;
            }
        }

        void DownloadImage(object state)
        {
            HttpWebRequest request = WebRequest.CreateHttp(state as Uri);
            request.BeginGetResponse(DownloadImageComplete, request);
        }

        async void DownloadImageComplete(IAsyncResult result)
        {
            try
            {
                HttpWebRequest request = result.AsyncState as HttpWebRequest;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
                Stream stream = response.GetResponseStream();
                int length = int.Parse(response.Headers["Content-Length"]);
                Stream streamForUI = new MemoryStream(length);
                byte[] buffer = new byte[length];
                int read = 0;
                do
                {
                    read = stream.Read(buffer, 0, length);
                    streamForUI.Write(buffer, 0, read);
                } while (read == length);
                streamForUI.Seek(0, SeekOrigin.Begin);
                if (Page == null) return;
                await Page.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    BitmapImage bm = new BitmapImage();
                    bm.SetSource(streamForUI.AsRandomAccessStream());

                    if (bitmapImage == null)
                        bitmapImage = new WeakReference(bm);
                    else
                        bitmapImage.Target = bm;
                    //触发UI的改变
                    OnPropertyChanged("ImageSource");
                }
                );

            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        async void OnPropertyChanged(string property)
        {
            var hander = PropertyChanged;
            if (hander != null)
                if (Page == null) return;
            await Page.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                hander(this, new PropertyChangedEventArgs(property));
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
