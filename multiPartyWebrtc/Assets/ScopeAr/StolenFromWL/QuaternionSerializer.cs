using UnityEngine;
using System.IO;

namespace Scope.RemoteAR
{

	[System.Serializable]
	public class QuaternionSerializer: StructSerializer<Quaternion>
	{
		public float x;
		public float y;
		public float z;
		public float w;
		
		public QuaternionSerializer (Quaternion q) {
			Fill (q);
		}
		
		public void Fill(Quaternion q)
		{
			x = q.x;
			y = q.y;
			z = q.z;
			w = q.w;
		}
		
		public Quaternion Q { get { return new Quaternion(x, y, z, w); } set { Fill(value); } }

	    #region implemented abstract members of StructSerializer
	    
	    public override Quaternion ReadFromStream (BinaryReader reader)
	    {
	        var x = reader.ReadSingle ();
	        var y = reader.ReadSingle ();
	        var z = reader.ReadSingle ();
	        var w = reader.ReadSingle ();
	        return new Quaternion (x,y,z,w);
	    }
	    
	    public override void WriteToStream (BinaryWriter writer)
	    {
	        writer.Write (x);
	        writer.Write (y);
	        writer.Write (z);
	        writer.Write (w);
	    }
	    
	    #endregion
		
	}
}