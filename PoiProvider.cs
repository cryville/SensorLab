using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using Android.Net;
using Android.Runtime;
using Java.Lang;

namespace SensorLab {
	[Register("world.cryville.sensorlab.PoiProvider")]
	public class PoiProvider : ContentProvider {
		static readonly Uri _uri = Uri.Parse("content://world.cryville.sensorlab/pois");
		static readonly UriMatcher _uriMatcher = new UriMatcher(UriMatcher.NoMatch);
		static PoiProvider() {
			_uriMatcher.AddURI("world.cryville.sensorlab", "pois", 1);
		}

		SQLiteDatabase _db;

		class OpenHelper : SQLiteOpenHelper {
			public OpenHelper(Context context) : base(context, "poi", null, 1) { }

			public override void OnCreate(SQLiteDatabase db) {
				db.ExecSQL("CREATE TABLE Pois (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, color INTEGER, latitude REAL, longitude REAL)");
			}

			public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) { }
		}

		public override bool OnCreate() {
			var helper = new OpenHelper(Context);
			try {
				_db = helper.WritableDatabase;
			}
			catch (SQLiteException) {
				return false;
			}
			return true;
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) _db.Close();
		}

		public override string GetType(Uri uri) => _uriMatcher.Match(uri) switch {
			1 => "vnd.android.cursor.dir/vnd.world.cryville.sensorlab.pois",
			_ => throw new IllegalArgumentException("Unknown URI"),
		};

		public override ICursor Query(Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder) {
			SQLiteQueryBuilder qb = new SQLiteQueryBuilder {
				Tables = "Pois"
			};
			 switch(_uriMatcher.Match(uri)) {
				case 1:
					var c = qb.Query(_db, projection, selection, selectionArgs, null, null, sortOrder);
					c.SetNotificationUri(Context.ContentResolver, uri);
					return c;
				default: throw new IllegalArgumentException("Unknown URI");
			};
		}

		public override Uri Insert(Uri uri, ContentValues values) {
			var rowId = _db.Insert("Pois", null, values);
			if (rowId >= 0) {
				Uri itemUri = ContentUris.WithAppendedId(_uri, rowId);
				Context.ContentResolver.NotifyChange(itemUri, null);
				return itemUri;
			}
			throw new SQLException("Failed to add record");
		}

		public override int Update(Uri uri, ContentValues values, string selection, string[] selectionArgs) {
			int count = _uriMatcher.Match(uri) switch {
				1 => _db.Update("Pois", values, selection, selectionArgs),
				_ => throw new IllegalArgumentException("Unknown URI"),
			};
			Context.ContentResolver.NotifyChange(uri, null);
			return count;
		}

		public override int Delete(Uri uri, string selection, string[] selectionArgs) {
			int count = _uriMatcher.Match(uri) switch {
				1 => _db.Delete("Pois", selection, selectionArgs),
				_ => throw new IllegalArgumentException("Unknown URI"),
			};
			Context.ContentResolver.NotifyChange(uri, null);
			return count;
		}
	}
}