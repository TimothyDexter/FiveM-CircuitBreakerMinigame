//  CbCursor.cs
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
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Roleplay.Client.Classes.Crime.MiniGames.CircuitBreaker
{
	public class CbCursor
	{
		private const string TextureDict = "MPCircuitHack";
		public readonly float CursorHeadSize = 0.0125f;
		private readonly List<Vector2> History = new List<Vector2>();

		private int Alpha = 255;
		public bool IsAlive = true;
		public bool IsVisible = true;

		public CbDirectionsEnum LastDirection;
		public Vector2 Position;

		public CbCursor( CbGenericPorts gamePorts ) {
			InitializeCursor( gamePorts );
		}

		/// <summary>
		/// Initializes the cursor.
		/// </summary>
		/// <param name="gamePorts">The game ports.</param>
		private void InitializeCursor( CbGenericPorts gamePorts ) {
			SetCursorStartPosition( gamePorts );
			History.Clear();
			History.Add( gamePorts.StartPortPos );
			SetStartDirection( gamePorts.StartPortHeading );
		}

		/// <summary>
		/// Draws the cursor.
		/// </summary>
		public void DrawCursor() {
			if( !IsAlive )
				API.DrawSprite( "MPCircuitHack", "spark", Position.X, Position.Y, 0.0125f, 0.0125f,
					0, 255,
					255, 255, Alpha );

			if( CircuitBreaker.HasCircuitFailed )
				API.DrawSprite( TextureDict, "head", Position.X, Position.Y, CursorHeadSize,
					CursorHeadSize, 0, CbColors.RedColor.R, CbColors.RedColor.G, CbColors.RedColor.B, Alpha );
			else
				API.DrawSprite( TextureDict, "head", Position.X, Position.Y, CursorHeadSize,
					CursorHeadSize, 0, CbColors.GreenColor.R, CbColors.GreenColor.G, CbColors.GreenColor.B, Alpha );
		}

		/// <summary>
		/// Draws the tail history.
		/// </summary>
		public void DrawTailHistory() {
			for( int i = 0; i < History.Count; i++ ) {
				float distance;
				float xDelta;
				float yDelta;
				Vector2 centerPoint;

				if( i + 1 == History.Count ) {
					//Tail to current cursor
					distance = Vector2.Distance( Position, History[i] );
					xDelta = Position.X - History[i].X;
					yDelta = Position.Y - History[i].Y;

					if( Math.Abs( xDelta ) > Math.Abs( yDelta ) )
						centerPoint = xDelta < 0
							? new Vector2( Position.X + distance / 2, Position.Y )
							: new Vector2( Position.X - distance / 2, Position.Y );
					else
						centerPoint = yDelta < 0
							? new Vector2( Position.X, Position.Y + distance / 2 )
							: new Vector2( Position.X, Position.Y - distance / 2 );
				}
				else {
					distance = Vector2.Distance( History[i], History[i + 1] );
					xDelta = History[i + 1].X - History[i].X;
					yDelta = History[i + 1].Y - History[i].Y;

					if( Math.Abs( xDelta ) > Math.Abs( yDelta ) )
						centerPoint = xDelta < 0
							? new Vector2( History[i + 1].X + distance / 2, History[i + 1].Y )
							: new Vector2( History[i + 1].X - distance / 2, History[i + 1].Y );
					else
						centerPoint = yDelta < 0
							? new Vector2( History[i + 1].X, History[i + 1].Y + distance / 2 )
							: new Vector2( History[i + 1].X, History[i + 1].Y - distance / 2 );
				}

				if( Math.Abs( xDelta ) > Math.Abs( yDelta ) ) {
					if( CircuitBreaker.HasCircuitFailed )
						API.DrawSprite( TextureDict, "tail", centerPoint.X, centerPoint.Y, distance + 0.0018f, 0.003f,
							0, CbColors.RedColor.R, CbColors.RedColor.G, CbColors.RedColor.B, Alpha );
					else
						API.DrawSprite( TextureDict, "tail", centerPoint.X, centerPoint.Y, distance + 0.0018f, 0.003f,
							0, CbColors.GreenColor.R, CbColors.GreenColor.G, CbColors.GreenColor.B, Alpha );
				}
				else {
					if( CircuitBreaker.HasCircuitFailed )
						API.DrawSprite( TextureDict, "tail", centerPoint.X, centerPoint.Y, 0.0018f, distance + 0.001f,
							0, CbColors.RedColor.R, CbColors.RedColor.G, CbColors.RedColor.B, Alpha );
					else
						API.DrawSprite( TextureDict, "tail", centerPoint.X, centerPoint.Y, 0.0018f, distance + 0.001f,
							0, CbColors.GreenColor.R, CbColors.GreenColor.G, CbColors.GreenColor.B, Alpha );
				}
			}
		}

		/// <summary>
		/// Moves the cursor.
		/// </summary>
		/// <param name="cursorSpeed">The cursor speed.</param>
		public void MoveCursor( float cursorSpeed ) {
			SetNewCursorPosition( cursorSpeed );
		}

		/// <summary>
		/// Adds to tail history.
		/// </summary>
		/// <param name="directionChangePoint">The direction change point.</param>
		public void AddToTailHistory( Vector2 directionChangePoint ) {
			History.Add( directionChangePoint );
		}

		/// <summary>
		/// Sets the start direction.
		/// </summary>
		/// <param name="startHeading">The start heading.</param>
		public void SetStartDirection( float startHeading ) {
			if( startHeading == 0 )
				LastDirection = CbDirectionsEnum.Right;
			else if( startHeading == 90 )
				LastDirection = CbDirectionsEnum.Down;
			else if( startHeading == 180 )
				LastDirection = CbDirectionsEnum.Left;
			else
				LastDirection = CbDirectionsEnum.Up;
		}

		/// <summary>
		/// Checks the tail collision.
		/// </summary>
		/// <returns></returns>
		public bool CheckTailCollision() {
			for( int i = 0; i < History.Count; i++ ) {
				float distance;
				float xDelta;
				float yDelta;
				Vector2 centerPoint;

				if( i + 1 == History.Count ) {
					//Tail to current cursor
					distance = Vector2.Distance( Position, History[i] );
					xDelta = Position.X - History[i].X;
					yDelta = Position.Y - History[i].Y;

					if( Math.Abs( xDelta ) > Math.Abs( yDelta ) )
						centerPoint = xDelta < 0
							? new Vector2( Position.X + distance / 2, Position.Y )
							: new Vector2( Position.X - distance / 2, Position.Y );
					else
						centerPoint = yDelta < 0
							? new Vector2( Position.X, Position.Y + distance / 2 )
							: new Vector2( Position.X, Position.Y - distance / 2 );
				}
				else {
					distance = Vector2.Distance( History[i], History[i + 1] );
					xDelta = History[i + 1].X - History[i].X;
					yDelta = History[i + 1].Y - History[i].Y;

					if( Math.Abs( xDelta ) > Math.Abs( yDelta ) )
						centerPoint = xDelta < 0
							? new Vector2( History[i + 1].X + distance / 2, History[i + 1].Y )
							: new Vector2( History[i + 1].X - distance / 2, History[i + 1].Y );
					else
						centerPoint = yDelta < 0
							? new Vector2( History[i + 1].X, History[i + 1].Y + distance / 2 )
							: new Vector2( History[i + 1].X, History[i + 1].Y - distance / 2 );
				}

				if( Math.Abs( xDelta ) > Math.Abs( yDelta ) ) {
					if( Math.Round( Position.X, 4 ) > Math.Round( centerPoint.X - distance / 2, 4 ) &&
					    Math.Round( Position.X, 4 ) < Math.Round( centerPoint.X + distance / 2, 4 ) &&
					    Math.Abs( Position.Y - centerPoint.Y ) <= 0.006f )
						return true;
				}
				else {
					if( Math.Round( Position.Y, 4 ) > Math.Round( centerPoint.Y - distance / 2, 4 ) &&
					    Math.Round( Position.Y, 4 ) < Math.Round( centerPoint.Y + distance / 2, 4 ) &&
					    Math.Abs( Position.X - centerPoint.X ) <= 0.006f )
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the cursor input from player.
		/// </summary>
		public void GetCursorInputFromPlayer() {
			var newDirection = LastDirection;
			var lastPos = Position;

			if( ControlHelper.IsDisabledControlPressed( Control.MoveLeftOnly ) )
				newDirection = CbDirectionsEnum.Left;
			else if( ControlHelper.IsDisabledControlPressed( Control.MoveRightOnly ) )
				newDirection = CbDirectionsEnum.Right;
			else if( ControlHelper.IsDisabledControlPressed( Control.MoveUpOnly ) )
				newDirection = CbDirectionsEnum.Up;
			else if( ControlHelper.IsDisabledControlPressed( Control.MoveDownOnly ) )
				newDirection = CbDirectionsEnum.Down;

			if( newDirection != LastDirection ) {
				LastDirection = newDirection;
				AddToTailHistory( lastPos );
				API.PlaySoundFrontend( -1, "Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true );
			}
		}

		/// <summary>
		/// Starts the cursor death animation.
		/// </summary>
		public async void StartCursorDeathAnimation() {
			while( Alpha > 0 ) {
				UpdateAlpha();
				await BaseScript.Delay( 0 );
			}
		}

		/// <summary>
		/// Updates the alpha.
		/// </summary>
		private void UpdateAlpha() {
			if( !IsAlive ) {
				Alpha = MathUtil.Clamp( Alpha - 5, 0, 255 );
				if( Alpha <= 0 ) IsVisible = false;
			}
		}

		/// <summary>
		/// Sets the cursor start position.
		/// </summary>
		/// <param name="gamePorts">The game ports.</param>
		private void SetCursorStartPosition( CbGenericPorts gamePorts ) {
			float magnitude = 0.0210f;
			if( gamePorts.StartPortHeading == 0 || gamePorts.StartPortHeading == 180 ) magnitude = 0.0144f;

			Position = CbHelper.GetOffsetPosition( gamePorts.StartPortPos, magnitude, gamePorts.StartPortHeading, 1 );
		}

		/// <summary>
		/// Sets the new cursor position.
		/// </summary>
		/// <param name="cursorSpeed">The cursor speed.</param>
		private void SetNewCursorPosition( float cursorSpeed ) {
			SetPosition( LastDirection, cursorSpeed );
		}

		public void DebugCursorPosition( CbDirectionsEnum direction, float cursorSpeed ) {
			SetPosition( direction, cursorSpeed );
		}

		private void SetPosition( CbDirectionsEnum direction, float cursorSpeed ) {
			switch( direction ) {
			case CbDirectionsEnum.Up:
				Position.Y = Position.Y - cursorSpeed;
				break;
			case CbDirectionsEnum.Down:
				Position.Y = Position.Y + cursorSpeed;
				break;
			case CbDirectionsEnum.Left:
				Position.X = Position.X - cursorSpeed;
				break;
			case CbDirectionsEnum.Right:
				Position.X = Position.X + cursorSpeed;
				break;
			}

			Position.X = MathUtil.Clamp( Position.X, 0, 1f );
			Position.Y = MathUtil.Clamp( Position.Y, 0, 1f );
		}
	

	}
}