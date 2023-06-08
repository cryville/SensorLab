using Android.Content;
using Android.Database;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;

namespace SensorLab.Fragments {
	public class Radar : Fragment {
		RecyclerView _poiList;

		public override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			var view = inflater.Inflate(Resource.Layout.fragment_radar, container, false);
			_poiList = view.FindViewById<RecyclerView>(Resource.Id.list_poi);
			_poiList.SetAdapter(new PoiRecyclerViewAdapter(Context.ContentResolver));
			return view;
		}
	}
	class PoiRecyclerViewAdapter : RecyclerView.Adapter {
		class ViewHolder : RecyclerView.ViewHolder {
			public TextView Caption { get; private set; }
			public ViewHolder(View view) : base(view) {
				Caption = view.FindViewById<TextView>(Resource.Id.text_poi_caption);
			}
		}

		static readonly Uri _uri = Uri.Parse("content://world.cryville.sensorlab/pois");
		readonly ContentResolver _resolver;
		readonly ICursor _cursor;
		readonly int _colName;
		public PoiRecyclerViewAdapter(ContentResolver resolver) {
			_resolver = resolver;
			_cursor = _resolver.Query(_uri, null, null, null, null);
			_colName = _cursor.GetColumnIndex("name");
			m_itemCount = _cursor.Count;
		}

		readonly int m_itemCount;
		public override int ItemCount => m_itemCount;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
			var h = (ViewHolder)holder;
			_cursor.MoveToPosition(position);
			h.Caption.Text = _cursor.GetString(_colName);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
			return new ViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_poi, parent, false));
		}
	}
}