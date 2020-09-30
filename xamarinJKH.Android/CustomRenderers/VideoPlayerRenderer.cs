using Java.Lang;
using Java.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Views = Android.Views;
using Android.Widget;
using Andr = Android.Content;

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
using Android.Graphics.Drawables;
using Com.Google.Android.Exoplayer2.Video;
using Com.Google.Android.Exoplayer2.Analytics;
using Java.Interop;

[assembly: ExportRenderer(typeof(VideoPlayerExo), typeof(VideoPlayerRenderer))]
namespace xamarinJKH.Droid.CustomRenderers
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayerExo, SimpleExoPlayerView>, IAdaptiveMediaSourceEventListener
    {

        Context Context;
        public VideoPlayerRenderer(Context context) : base(context)
        {
            Context = context;
            MessagingCenter.Subscribe<object, bool>(this, "FullScreen", (sender, rotated) =>
            {
                var activity = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity;

                if (rotated)
                {
                    activity.RequestedOrientation = Andr.PM.ScreenOrientation.Portrait;
                }
                else
                {
                    activity.RequestedOrientation = Andr.PM.ScreenOrientation.Landscape;
                    activity.SetActionBar(null);
                    //activity.SetContentView(_view);
                }
            });
        }
        SimpleExoPlayer _player;
        SimpleExoPlayerView _view;
        string url;

        int viewWidth;
        int viewHeight;

        View view;

        IVideoListener listener;


        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayerExo> e)
        {
            base.OnElementChanged(e);

            if (_player != null)
            {
                _player.Release();
            }
            if (_player == null)
            {
                _player = ExoPlayerFactory.NewSimpleInstance(Context);
                _player.PlayWhenReady = true;
                _view = new SimpleExoPlayerView(Context);
                _view.Background = new GradientDrawable(GradientDrawable.Orientation.BottomTop, new int[] { Color.Transparent.ToAndroid(), Color.Transparent.ToAndroid() });
                _view.ControllerAutoShow = false;

                _view.UseController = false;
                _player.RenderedFirstFrame += _player_RenderedFirstFrame;
                _player.VideoSizeChanged += _player_VideoSizeChanged;

                _view.Player = _player;

                url = e.NewElement.Source;
                SetNativeControl(_view);
            }

            Play();
        }

        private void _player_VideoSizeChanged(object sender, VideoSizeChangedEventArgs e)
        {
            var player = sender as SimpleExoPlayer;
            var format = player.VideoFormat;
            var ratio = (float)format.Height / (float)format.Width;
            MessagingCenter.Send<object, float>(sender, "SetRatio", ratio);

            var activity = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity;
            viewHeight = _view.Height;
            viewWidth = _view.Width;


            //activity.RequestedOrientation = Andr.PM.ScreenOrientation.Landscape;
        }

        private void _player_RenderedFirstFrame(object sender, System.EventArgs e)
        {
            MessagingCenter.Send<object>(sender, "StopLoadingPlayer");
        }

        private void Play()
        {
            if (!string.IsNullOrEmpty(url))
            {
                Uri uri = Uri.Parse(url);
                var agent = Com.Google.Android.Exoplayer2.Util.Util.GetUserAgent(Context, Context.ApplicationInfo.Name);
                DefaultHttpDataSourceFactory httpDataSourceFactory = new DefaultHttpDataSourceFactory(agent);
                DefaultSsChunkSource.Factory ssChunkFactory = new DefaultSsChunkSource.Factory(httpDataSourceFactory);
                Handler emptyHandler = new Handler();

                //SsMediaSource ssMediaSource = new SsMediaSource(uri, httpDataSourceFactory, ssChunkFactory, emptyHandler);
                var mediaSource = new HlsMediaSource.Factory(httpDataSourceFactory).CreateMediaSource(uri);
                _player.Prepare(mediaSource);
                var playing = _player.IsPlaying;
            }

        }

        public void OnDownstreamFormatChanged(int p0, Format p1, int p2, Java.Lang.Object p3, long p4)
        {
        }

        public void OnLoadCanceled(DataSpec p0, int p1, int p2, Format p3, int p4, Java.Lang.Object p5, long p6, long p7, long p8, long p9, long p10)
        {
        }

        public void OnLoadCompleted(DataSpec p0, int p1, int p2, Format p3, int p4, Java.Lang.Object p5, long p6, long p7, long p8, long p9, long p10)
        {

        }

        public void OnLoadError(DataSpec p0, int p1, int p2, Format p3, int p4, Java.Lang.Object p5, long p6, long p7, long p8, long p9, long p10, IOException p11, bool p12)
        {
        }

        public void OnLoadStarted(DataSpec p0, int p1, int p2, Format p3, int p4, Java.Lang.Object p5, long p6, long p7, long p8)
        {
        }

        public void OnUpstreamDiscarded(int p0, long p1, long p2)
        {
        }
    }
}