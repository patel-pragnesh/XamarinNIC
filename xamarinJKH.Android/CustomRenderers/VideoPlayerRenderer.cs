using Java.Lang;
using Java.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Source.Dash;
using Com.Google.Android.Exoplayer2.Source.Dash.Manifest;
using Com.Google.Android.Exoplayer2.Source.Smoothstreaming;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using xamarinJKH.Droid.CustomRenderers;
using xamarinJKH.VideoStreaming;
using Xamarin.Forms.Platform.Android;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using Android.Net;
using Com.Google.Android.Exoplayer2.Source.Hls;
using Util = Com.Google.Android.Exoplayer2.Util;

[assembly: ExportRenderer(typeof(VideoPlayerExo), typeof(VideoPlayerRenderer))]
namespace xamarinJKH.Droid.CustomRenderers
{
    [System.Obsolete]
    public class VideoPlayerRenderer: ViewRenderer<VideoPlayerExo, SimpleExoPlayerView>, IAdaptiveMediaSourceEventListener
    {

        Context Context;
        public VideoPlayerRenderer(Context context) : base(context) { Context = context; }
        SimpleExoPlayer _player;
        SimpleExoPlayerView _view;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayerExo> e)
        {
            base.OnElementChanged(e);
            if (_player == null)
            {
                _player = ExoPlayerFactory.NewSimpleInstance(Context);
                _player.PlayWhenReady = true;
                _view = new SimpleExoPlayerView(Context);
                _view.Player = _player;
                SetNativeControl(_view);
            }

            Play();
        }

        private void Play()
        {
            Uri uri = Uri.Parse("https://vs.domru.ru/translation?id=331972335&guid=dc3b372c7e421f19192a&mode=hls");
            var agent = Com.Google.Android.Exoplayer2.Util.Util.GetUserAgent(Context, Context.ApplicationInfo.Name);
            DefaultHttpDataSourceFactory httpDataSourceFactory = new DefaultHttpDataSourceFactory(agent);
            DefaultSsChunkSource.Factory ssChunkFactory = new DefaultSsChunkSource.Factory(httpDataSourceFactory);
            Handler emptyHandler = new Handler();

            //SsMediaSource ssMediaSource = new SsMediaSource(uri, httpDataSourceFactory, ssChunkFactory, emptyHandler);
            var mediaSource = new HlsMediaSource.Factory(httpDataSourceFactory).CreateMediaSource(uri);
            _player.Prepare(mediaSource);
            var playing = _player.IsPlaying;
        }

        public void OnDownstreamFormatChanged(int p0, Format p1, int p2, Object p3, long p4)
        {
        }

        public void OnLoadCanceled(DataSpec p0, int p1, int p2, Format p3, int p4, Object p5, long p6, long p7, long p8, long p9, long p10)
        {
        }

        public void OnLoadCompleted(DataSpec p0, int p1, int p2, Format p3, int p4, Object p5, long p6, long p7, long p8, long p9, long p10)
        {
        }

        public void OnLoadError(DataSpec p0, int p1, int p2, Format p3, int p4, Object p5, long p6, long p7, long p8, long p9, long p10, IOException p11, bool p12)
        {
        }

        public void OnLoadStarted(DataSpec p0, int p1, int p2, Format p3, int p4, Object p5, long p6, long p7, long p8)
        {
        }

        public void OnUpstreamDiscarded(int p0, long p1, long p2)
        {
        }
    }
}