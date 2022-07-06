using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.DaramCommonLib
{
	[Serializable]
	public class UndoManager<T> where T : class
	{
		[NonSerialized]
		BinaryFormatter bf = new BinaryFormatter ();
		Stack<byte[]> undoStack = new Stack<byte[]> ();
		Stack<byte[]> redoStack = new Stack<byte[]> ();

		public event EventHandler UpdateUndo, UpdateRedo;

		public bool IsUndoStackEmpty { get { return undoStack.Count == 0; } }
		public bool IsRedoStackEmpty { get { return redoStack.Count == 0; } }

		public void SaveToUndoStack (T fileInfoCollection, bool clearRedoStack = true, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
		{
			using (MemoryStream memStream = new MemoryStream ()) {
#pragma warning disable SYSLIB0011 // 类型或成员已过时
				bf.Serialize (memStream, fileInfoCollection ?? throw new ArgumentNullException ());
#pragma warning restore SYSLIB0011 // 类型或成员已过时
				undoStack.Push (memStream.ToArray ());
			}

			if (clearRedoStack)
				ClearRedoStack ();

			UpdateUndo?.Invoke (this, EventArgs.Empty);
		}

		public void SaveToRedoStack (T fileInfoCollection, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
		{
			using (MemoryStream memStream = new MemoryStream ()) {
#pragma warning disable SYSLIB0011 // 类型或成员已过时
				bf.Serialize (memStream, fileInfoCollection ?? throw new ArgumentNullException ());
#pragma warning restore SYSLIB0011 // 类型或成员已过时
				redoStack.Push (memStream.ToArray ());
			}

			UpdateRedo?.Invoke (this, EventArgs.Empty);
		}

		public T LoadFromUndoStack ()
		{
			if (IsUndoStackEmpty) return null;
			using (MemoryStream memStream = new MemoryStream (undoStack.Pop ())) {
#pragma warning disable SYSLIB0011 // 类型或成员已过时
				var ret = bf.Deserialize (memStream) as T ?? throw new InvalidCastException ();
#pragma warning restore SYSLIB0011 // 类型或成员已过时
				UpdateUndo?.Invoke (this, EventArgs.Empty);
				return ret;
			}
		}

		public T LoadFromRedoStack ()
		{
			if (IsRedoStackEmpty) return null;
			using (MemoryStream memStream = new MemoryStream (redoStack.Pop ())) {
#pragma warning disable SYSLIB0011 // 类型或成员已过时
				var ret = bf.Deserialize (memStream) as T ?? throw new InvalidCastException ();
#pragma warning restore SYSLIB0011 // 类型或成员已过时
				UpdateRedo?.Invoke (this, EventArgs.Empty);
				return ret;
			}
		}

		/*public void Backup ()
		{
			using ( Stream backupFile = new FileStream ( "crashed_backup.dat", FileMode.Create, FileAccess.Write ) )
			{
				bf.Serialize ( backupFile, this );
			}
		}

		public static UndoManager<T> Restore ()
		{
			if ( !File.Exists ( "crashed_backup.dat" ) )
				return null;

			BinaryFormatter bf = new BinaryFormatter ();
			UndoManager<T> ret;
			try
			{
				using ( Stream backupFile = new FileStream ( "crashed_backup.dat", FileMode.Open, FileAccess.Read ) )
				{
					ret = bf.Deserialize ( backupFile ) as UndoManager<T>;
				}
				ret.bf = new BinaryFormatter ();
			}
			catch { ret = null; }
			File.Delete ( "crashed_backup.dat" );
			return ret;
		}*/

		public void ClearUndoStack () { undoStack.Clear (); }
		public void ClearRedoStack () { redoStack.Clear (); }
		public void ClearAll () { ClearUndoStack (); ClearRedoStack (); }
	}
}
