using System;
using Android.Runtime;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Views;
using Java.Interop;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;
using System.Linq;

namespace WearDemo
{
    [Activity(Label = "WearDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity,IDataApiDataListener, IGoogleApiClientConnectionCallbacks, IGoogleApiClientOnConnectionFailedListener
    {

        private IGoogleApiClient _client;
        const string _syncPath = "/WearDemo/Data";
        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            _client = new GoogleApiClientBuilder(this, this, this)
                             .AddApi(WearableClass.Api)
                             .Build();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            var v = FindViewById<WatchViewStub>(Resource.Id.watch_view_stub);
            v.LayoutInflated += delegate {

                // Get our button from the layout resource,
                // and attach an event to it
                Button button = FindViewById<Button>(Resource.Id.myButton);

                button.Click += delegate {
                    SendData();
                };
            };
        }

        public void SendData() {
            try {
                var request = PutDataMapRequest.Create(_syncPath);
                var map = request.DataMap;
                map.PutString("Message", "Vinz says Hello from Wearable!");
                map.PutLong("UpdatedAt", DateTime.UtcNow.Ticks);
                WearableClass.DataApi.PutDataItem(_client, request.AsPutDataRequest());
            }
            finally {
                _client.Disconnect();
            }

        }
        protected override void OnStart() {
            base.OnStart();
            _client.Connect();
        }
        public void OnConnected(Bundle p0) {
            WearableClass.DataApi.AddListener(_client, this);
        }

        public void OnConnectionSuspended(int reason) {
            Android.Util.Log.Error("GMS", "Connection suspended " + reason);
            WearableClass.DataApi.RemoveListener(_client, this);
        }

        public void OnConnectionFailed(Android.Gms.Common.ConnectionResult result) {
            Android.Util.Log.Error("GMS", "Connection failed " + result.ErrorCode);
        }

        protected override void OnStop() {
            base.OnStop();
            _client.Disconnect();
        }

        public void OnDataChanged(DataEventBuffer dataEvents) {
            var dataEvent = Enumerable.Range(0, dataEvents.Count)
                                      .Select(i => dataEvents.Get(i).JavaCast<IDataEvent>())
                                      .FirstOrDefault(x => x.Type == DataEvent.TypeChanged && x.DataItem.Uri.Path.Equals(_syncPath));
            if (dataEvent == null)
                return;

            //do stuffs here
        }
    }
}


