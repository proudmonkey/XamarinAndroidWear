using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Content;

namespace WearDemo
{
    [Activity(Label = "MainAppDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        TextView _txtMsg;

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our TextBox from the layout resource,
            _txtMsg = FindViewById<TextView>(Resource.Id.txtMessage);


            IntentFilter filter = new IntentFilter(Intent.ActionSend);
            MessageReciever receiver = new MessageReciever(this);
            LocalBroadcastManager.GetInstance(this).RegisterReceiver(receiver, filter);
        }

        public void ProcessMessage(Intent intent) {
            _txtMsg.Text = intent.GetStringExtra("WearMessage");
        }

        internal class MessageReciever : BroadcastReceiver
        {
            MainActivity _main;
            public MessageReciever(MainActivity owner) { this._main = owner; }
            public override void OnReceive(Context context, Intent intent) {
                _main.ProcessMessage(intent);
            }
        }
    }
}

