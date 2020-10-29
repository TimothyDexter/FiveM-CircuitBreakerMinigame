//  Helper.cs
//  Author: Timothy Dexter
//  Release: 0.0.1
//  Date: 04/12/2019
//  
//   
//  Known Issues
//   
//   
//  Please send any edits/improvements/bugs to this script back to the author. 
//   
//  Usage 
//   
//   
//  History:
//  Revision 0.0.1 2019/04/14 10:49 AM EDT TimothyDexter 
//  - Initial release
//   

using System;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Roleplay.Client.Classes.Crime.MiniGames.CircuitBreaker
{
	public static class CbHelper
	{
		/// <summary>
		/// Determines whether the point is inside the given polygon
		/// </summary>
		/// <param name="poly">The poly.</param>
		/// <param name="point">The point.</param>
		/// <returns>
		///   <c>true</c> if [is in poly] [the specified poly]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsInPoly( Vector2[] poly, Vector2 point ) {
			double MinX = poly.Min( a => a.X );
			double MinY = poly.Min( a => a.Y );
			double MaxX = poly.Max( a => a.X );
			double MaxY = poly.Max( a => a.Y );

			if( point.X < MinX || point.X > MaxX || point.Y < MinY || point.Y > MaxY )
				return false;

			int I = 0;
			int J = poly.Count() - 1;
			bool IsMatch = false;

			for( ; I < poly.Count(); J = I++ ) {
				//When the position is right on a point, count it as a match.
				if( poly[I].X == point.X && poly[I].Y == point.Y )
					return true;
				if( poly[J].X == point.X && poly[J].Y == point.Y )
					return true;

				//When the position is on a horizontal or vertical line, count it as a match.
				if( poly[I].X == poly[J].X && point.X == poly[I].X && point.Y >= Math.Min( poly[I].Y, poly[J].Y ) &&
				    point.Y <= Math.Max( poly[I].Y, poly[J].Y ) )
					return true;
				if( poly[I].Y == poly[J].Y && point.Y == poly[I].Y && point.X >= Math.Min( poly[I].X, poly[J].X ) &&
				    point.X <= Math.Max( poly[I].X, poly[J].X ) )
					return true;

				if( poly[I].Y > point.Y != poly[J].Y > point.Y && point.X <
				    (poly[J].X - poly[I].X) * (point.Y - poly[I].Y) / (poly[J].Y - poly[I].Y) + poly[I].X )
					IsMatch = !IsMatch;
			}

			return IsMatch;
		}

		/// <summary>
		/// Gets the 2d offset position relative to the start position
		/// </summary>
		/// <param name="startPosition">The start position.</param>
		/// <param name="magnitude">The magnitude.</param>
		/// <param name="heading">The heading.</param>
		/// <param name="multiplier">The multiplier.</param>
		/// <returns></returns>
		public static Vector2 GetOffsetPosition( Vector2 startPosition, float magnitude, float heading,
			int multiplier ) {
			double cosx = multiplier * Math.Cos( heading * (Math.PI / 180f) );
			double siny = multiplier * Math.Sin( heading * (Math.PI / 180f) );

			float x = startPosition.X;
			float y = startPosition.Y;

			float newX = (float)(x + magnitude * cosx);
			float newY = (float)(y + magnitude * siny);

			return new Vector2( newX, newY );
		}

		/// <summary>
		/// Debugs the positions.
		/// </summary>
		public static void DebugPositions() {
			//TODO: Cleanup whenever a custom map needs to be added
			//Log.ToChat( $"start {_genericPorts.StartPortPos.X},{_genericPorts.StartPortPos.Y}" );
			//Log.ToChat( $"head {_cursor.Position.X},{_cursor.Position.Y}" );
			//Log.ToChat( $"Mag={Math.Round( Math.Sqrt( Math.Pow( (_genericPorts.StartPortPos.X - _cursor.Position.X), 2 ) + Math.Pow( (_genericPorts.StartPortPos.Y - _cursor.Position.Y), 2 ) ), 5 ) }" );

			//var mult = -1;
			//var mag = Math.Round(
			//		Math.Sqrt( Math.Pow( (_genericPorts.StartPortPos.X - _cursor.Position.X), 2 ) +
			//		           Math.Pow( (_genericPorts.StartPortPos.Y - _cursor.Position.Y), 2 ) ), 5 );
			//if( _genericPorts.StartPortHeading == 0 || _genericPorts.StartPortHeading == 180 ) {
			//	mult = 1;
			//}

			//var lowestDist = 100f;
			//float ang = -1;
			//float xx = 0f;
			//float yy = 0f;
			//for( float i = 0; i < 361; i += 0.25f ) {
			//	var cosx = mult * Math.Cos( ((i + _genericPorts.StartPortHeading) % 360) * (Math.PI / 180f) );
			//	var siny = mult * Math.Sin( ((i + _genericPorts.StartPortHeading) % 360) * (Math.PI / 180f) );

			//	var x = _genericPorts.StartPortPos.X;
			//	var y = _genericPorts.StartPortPos.Y;
			//	var newX = (float)(x + mag * cosx);
			//	var newY = (float)(y + mag * siny);


			//	if( Vector2.Distance( new Vector2( _cursor.Position.X, _cursor.Position.Y ), new Vector2( newX, newY ) ) < lowestDist ) {
			//		lowestDist = Vector2.Distance( new Vector2( _cursor.Position.X, _cursor.Position.Y ), new Vector2( newX, newY ) );
			//		ang = i;
			//		xx = newX;
			//		yy = newY;
			//	}
			//}
			//Log.Info( $"Light={Math.Round( _cursor.Position.X, 3 )},{Math.Round( _cursor.Position.Y, 3 )}" );
			//Log.Info( $"ang={ang}, x={xx},y={yy}" );
		}

		public static int GetDebugCursorDirectionInput() {
			CbDirectionsEnum newDirection = CbDirectionsEnum.Left;
			bool suppliedInput = false;
			if( ControlHelper.IsDisabledControlPressed( Control.MoveLeftOnly ) ) {
				newDirection = CbDirectionsEnum.Left;
				suppliedInput = true;
			}
			else if( ControlHelper.IsDisabledControlPressed( Control.MoveRightOnly ) ) {
				newDirection = CbDirectionsEnum.Right;
				suppliedInput = true;
			}
			else if( ControlHelper.IsDisabledControlPressed( Control.MoveUpOnly ) ) {
				newDirection = CbDirectionsEnum.Up;
				suppliedInput = true;
			}
			else if( ControlHelper.IsDisabledControlPressed( Control.MoveDownOnly ) ) {
				newDirection = CbDirectionsEnum.Down;
				suppliedInput = true;
			}

			return suppliedInput ? (int)newDirection : -1;
		}

		public static float GetDebugCursorSpeedInput() {
			var debugCursorSpeed = 0f;

			if( ControlHelper.IsControlPressed( Control.VehicleSubPitchUpOnly ) ) {
				debugCursorSpeed = 0.00025f;
			}
			else if( ControlHelper.IsControlPressed( Control.VehicleSubPitchUpDown ) ) {
				debugCursorSpeed = -0.00025f;
			}

			return debugCursorSpeed;
		}

		public static void DrawDebugSprite( string spriteName, Vector2 position, float heading, float width, float height) {
			API.DrawSprite( "MPCircuitHack", spriteName, position.X, position.Y, width, height,
				heading, 255,
				255, 255, 255 );
		}

		public static void DebugPortSprite(Vector2 position,float portHeading) {
			float spriteWidth;
			float spriteHeight;
			if( portHeading == 0 || portHeading == 180 ) {
				spriteWidth = 0.02f;
				spriteHeight = 0.055f;
			}
			else {
				spriteWidth = 0.0325f;
				spriteHeight = 0.03f;
			}

			DrawDebugSprite( "genericport", position, portHeading, spriteWidth, spriteHeight);
		}

		public static float GetPortDebugHeading( float currenthHeading ) {
			if( ControlHelper.IsControlPressed( Control.VehicleSubTurnLeftOnly ) ) {
				currenthHeading = (int)(currenthHeading - 90);
			}
			else if( ControlHelper.IsControlPressed( Control.VehicleSubTurnRightOnly ) )
			{
				currenthHeading = (int)(currenthHeading + 90);
			}

			return currenthHeading;
		}

		/// <summary>
		/// Draws the debug box.
		/// </summary>
		private static void DrawDebugBox() {
			//var points = GetCursorMaxPoints( new Vector2( _cursor.Position.X, _cursor.Position.Y ),
			//	_cursor.CursorHeadSize + -0.375f * _cursor.CursorHeadSize );

			//foreach( var pt in points ) API.DrawRect( pt.X, pt.Y, 0.001f, 0.0025f, 220, 30, 5, 255 );
		}
	}
}