using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using System;

namespace Scope.RemoteAR
{

	public abstract class StructSerializer<T> where T : new()
	{
	    public static byte [] StructureToByteArray(object obj)
	    {
	        int len = Marshal.SizeOf(obj);
	        
	        byte [] arr = new byte[len];
	        
	        IntPtr ptr = Marshal.AllocHGlobal(len);
	        
	        Marshal.StructureToPtr(obj, ptr, true);
	        
	        Marshal.Copy(ptr, arr, 0, len);
	        
	        Marshal.FreeHGlobal(ptr);
	        
	        return arr;
	    }
	    
	    public static void ByteArrayToStructure(byte [] bytearray, ref object obj)
	    {
	        int len = Marshal.SizeOf(obj);
	        
	        IntPtr i = Marshal.AllocHGlobal(len);
	        
	        Marshal.Copy(bytearray,0, i,len);
	        
	        obj = Marshal.PtrToStructure(i, obj.GetType());
	        
	        Marshal.FreeHGlobal(i);
	    }

	    public static int Size(object obj)
	    {
	        return Marshal.SizeOf (obj);
	    }

	    public T ReadFromStream(Stream from)
	    {
	        BinaryReader reader = new BinaryReader(from);
	        return ReadFromStream(reader);
	    }
	    
	    public abstract T ReadFromStream (BinaryReader reader);
	    
	    public void WriteToStream (Stream where)
	    {
	        BinaryWriter writer = new BinaryWriter(where);

	        WriteToStream(writer);
	    }
	    
	    public abstract void WriteToStream(BinaryWriter writer);

	}


	[System.Serializable]
	public class Vector3Serializer: StructSerializer<Vector3> 
	{
		public float x;
		public float y;
		public float z;

		public Vector3Serializer (Vector3 v) {
			Fill (v);
		}
		
		public void Fill(Vector3 v3)
		{
			x = v3.x;
			y = v3.y;
			z = v3.z;
		}
		
		public Vector3 V3 { get { return new Vector3(x, y, z); } set { Fill(value); } }	

	    #region implemented abstract members of StructSerializer

	    public override Vector3 ReadFromStream (BinaryReader reader)
	    {
	        var x = reader.ReadSingle ();
	        var y = reader.ReadSingle ();
	        var z = reader.ReadSingle ();
	        return new Vector3 (x,y,z);
	    }

	    public override void WriteToStream (BinaryWriter writer)
	    {
	        writer.Write (x);
	        writer.Write (y);
	        writer.Write (z);
	    }

	    #endregion
	}


	[System.Serializable]
	public class Matrix4Serializer: StructSerializer<Matrix4x4> 
	{
	    public Matrix4x4 m;

	    public Matrix4Serializer (Matrix4x4 v) {
	        Fill (v);
	    }
	    
	    public void Fill(Matrix4x4 v3)
	    {
	        m = v3;
	    }
	    
	    #region implemented abstract members of StructSerializer
	    
	    public override Matrix4x4 ReadFromStream (BinaryReader reader)
	    {
	        Matrix4x4 m = new Matrix4x4 ();
	        for (int i=0; i<4; i++)
	            for (int j=0; j<4; j++)
	                m [i, j] = reader.ReadSingle ();

	        return m;
	    }
	    
	    public override void WriteToStream (BinaryWriter writer)
	    {
	        for(int i=0; i<4; i++)
	            for (int j=0; j<4; j++)
	                writer.Write (m[i,j]);        
	    }
	    
	    #endregion
	}


	[System.Serializable]
	public class Vector2Serializer: StructSerializer<Vector2>
	{
	    public float x;
	    public float y;
	    
	    public Vector2Serializer (Vector2 v) {
	        Fill (v);
	    }
	    
	    public void Fill(Vector2 v3)
	    {
	        x = v3.x;
	        y = v3.y;
	    }
	    
	    public Vector3 V2 { get { return new Vector2(x, y); } set { Fill(value); } } 

	    #region implemented abstract members of StructSerializer
	    
	    public override Vector2 ReadFromStream (BinaryReader reader)
	    {
	        var x = reader.ReadSingle ();
	        var y = reader.ReadSingle ();
	        return new Vector2 (x,y);
	    }
	    
	    public override void WriteToStream (BinaryWriter writer)
	    {
	        writer.Write (x);
	        writer.Write (y);
	    }
	    
	    #endregion
	}
}


