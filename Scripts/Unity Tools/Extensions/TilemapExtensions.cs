#if UNITY_TILEMAP //Defined in Elanetic.Tools assembly definition file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace Elanetic.Tools.Unity
{

	#region Tilemap

	static public class TilemapExtensions
	{
		private delegate UnityEngine.Object[] GetTilesBlockFastDelegate(Vector3Int position, Vector3Int blockDimensions);
		static private Dictionary<Tilemap, GetTilesBlockFastDelegate> m_SavedDelegates = new Dictionary<Tilemap, GetTilesBlockFastDelegate>();
		static private MethodInfo m_SavedMethodInfo = null;
		static private int m_CountCheck = 50;
		static public UnityEngine.Object[] GetTilesBlockFast(this Tilemap tilemap, BoundsInt bounds)
		{
			if(!m_SavedDelegates.TryGetValue(tilemap, out GetTilesBlockFastDelegate del))
			{
				if(m_SavedMethodInfo == null)
				{
					m_SavedMethodInfo = typeof(Tilemap).GetMethod("GetTileAssetsBlock", BindingFlags.NonPublic | BindingFlags.Instance);
				}
				del = (GetTilesBlockFastDelegate)Delegate.CreateDelegate(typeof(GetTilesBlockFastDelegate), tilemap, m_SavedMethodInfo);
				m_SavedDelegates.Add(tilemap, del);

				//This checks to automatically clean out any null(destroyed) Tilemaps to help keep dictionary lookups performance up to par
				if(m_SavedDelegates.Count > m_CountCheck)
				{
					Tilemap[] ts = m_SavedDelegates.Keys.ToArray();

					for(int i = 0; i < ts.Length; i++)
					{
						if(ts[i] == null)
						{
							m_SavedDelegates.Remove(ts[i]);
						}
					}
					m_CountCheck = m_SavedDelegates.Count + 50;
				}
			}
			return del.Invoke(bounds.min, bounds.size);
		}
	}

	#endregion Tilemap

	#region ITilemap

	static public class ITilemapExtension
	{
		static private Func<ITilemap, Tilemap> GetTilemapDelegate;
		static private bool retrieved = false;

		static public Tilemap GetTilemap(this ITilemap iTilemap)
		{
			if(!retrieved)
			{
				GetTilemapDelegate = CreateGetTilemapDelegate();
				retrieved = true;
			}
			return GetTilemapDelegate(iTilemap);
		}

		static private Func<ITilemap, Tilemap> CreateGetTilemapDelegate()
		{
			ParameterExpression sourceParameter = Expression.Parameter(typeof(ITilemap), "source");

			MemberExpression fieldExpression = Expression.Field(sourceParameter, typeof(ITilemap).GetField("m_Tilemap", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));

			LambdaExpression lambda = Expression.Lambda(typeof(Func<ITilemap, Tilemap>), fieldExpression, sourceParameter);

			return (Func<ITilemap, Tilemap>)lambda.Compile();
		}
	}

	#endregion ITilemap

}
#endif