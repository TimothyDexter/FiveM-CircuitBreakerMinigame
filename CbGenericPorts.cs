//  CbGenericPorts.cs
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
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Common;
using Roleplay.SharedClasses;

namespace Roleplay.Client.Classes.Crime.MiniGames.CircuitBreaker
{
	public class CbGenericPorts
	{
		public CbGenericPorts( int levelNumber ) {
			SetPositions( levelNumber );
		}

		public Vector2 StartPortPos { get; private set; }
		public Vector2 FinishPortPos { get; private set; }
		public float StartPortHeading { get; private set; }
		public float FinishPortHeading { get; private set; }
		public CbPortLights StartPortLights { get; private set; }
		public CbPortLights FinishPortLights { get; private set; }
		public Vector2[] StartPortBounds { get; private set; }
		public Vector2[] FinishPortBounds { get; private set; }
		public Vector2[] WinBounds { get; private set; }

		/// <summary>
		/// Sets the positions.
		/// </summary>
		/// <param name="levelNumber">The level number.</param>
		public void SetPositions( int levelNumber ) {
			StartPortPos = GetStartPortPosition( levelNumber );
			FinishPortPos = GetFinishPortPosition( levelNumber, StartPortPos );

			StartPortHeading = GetPortHeading( StartPortPos );
			FinishPortHeading = GetPortHeading( FinishPortPos );

			StartPortLights = new CbPortLights( StartPortPos, StartPortHeading, CbPortPositionTypeEnum.Start );
			FinishPortLights = new CbPortLights( FinishPortPos, FinishPortHeading, CbPortPositionTypeEnum.Finish );

			StartPortBounds = GetPortCollisionBounds( StartPortPos, StartPortHeading, true );
			FinishPortBounds = GetPortCollisionBounds( FinishPortPos, FinishPortHeading, false );
			WinBounds = GetWinBounds();
		}

		/// <summary>
		/// Draws the ports.
		/// </summary>
		public void DrawPorts() {
			if( StartPortPos == Vector2.Zero || FinishPortPos == Vector2.Zero || StartPortHeading == -1 ||
			    FinishPortHeading == -1 ) {
				Log.Error(
					$"GenericPorts error setting position and heading.{StartPortPos},{StartPortHeading},{FinishPortPos},{FinishPortHeading}" );
				return;
			}

			DrawPortSprite( StartPortPos, StartPortHeading );
			DrawPortSprite( FinishPortPos, FinishPortHeading );

			StartPortLights.DrawLights();
			FinishPortLights.DrawLights();
		}

		/// <summary>
		/// Determines whether the specified cursor position is colliding with port] 
		/// </summary>
		/// <param name="cursorPosition">The cursor position.</param>
		/// <returns>
		///   <c>true</c> if [is collision with port] [the specified cursor position]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsCollisionWithPort( Vector2 cursorPosition ) {
			return CbHelper.IsInPoly( StartPortBounds, cursorPosition ) ||
			       CbHelper.IsInPoly( FinishPortBounds, cursorPosition ) &&
			       !IsCursorInGameWinningPosition( cursorPosition );
		}

		/// <summary>
		/// Determines whether [is cursor in game winning position] [the specified cursor position].
		/// </summary>
		/// <param name="cursorPosition">The cursor position.</param>
		/// <returns>
		///   <c>true</c> if [is cursor in game winning position] [the specified cursor position]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsCursorInGameWinningPosition( Vector2 cursorPosition ) {
			return CbHelper.IsInPoly( WinBounds, cursorPosition );
		}

		/// <summary>
		/// Draws the port sprite.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="heading">The heading.</param>
		private void DrawPortSprite( Vector2 position, float heading ) {
			float portHeight;
			float portWidth;

			if( heading == 0 || heading == 180 ) {
				portWidth = 0.02f;
				portHeight = 0.055f;
			}
			else {
				portWidth = 0.0325f;
				portHeight = 0.03f;
			}

			API.DrawSprite( "MPCircuitHack", "genericport", position.X, position.Y, portWidth, portHeight, heading, 255,
				255, 255, 255 );
		}

		/// <summary>
		/// Gets the port collision bounds.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="heading">The heading.</param>
		/// <param name="isStartPort">if set to <c>true</c> [is start port].</param>
		/// <returns></returns>
		private Vector2[] GetPortCollisionBounds( Vector2 position, float heading, bool isStartPort ) {
			float magnitude;
			int mult = -1;
			float[] angles;
			if( heading == 0 || heading == 180 ) {
				magnitude = isStartPort ? 0.0279f : 0.0266f;
				angles = isStartPort
					? new[] {289.75f, 250.75f, 109.75f, 70f}
					: new[] {277.75f, 259.25f, 100.75f, 82.5f};
				mult = 1;
			}
			else {
				magnitude = isStartPort ? 0.0211f : 0.0173f;
				angles = isStartPort ? new[] {313.25f, 227.75f, 132.25f, 48.5f} : new[] {111f, 66.5f, 293.25f, 249.25f};
			}

			var portBounds = new Vector2[4];
			int i = 0;
			foreach( float angle in angles ) {
				portBounds[i] = CbHelper.GetOffsetPosition( position, magnitude, (heading + angle) % 360, mult );
				i = i + 1;
			}

			return portBounds;
		}

		/// <summary>
		/// Gets the win bounds.
		/// </summary>
		/// <returns></returns>
		private Vector2[] GetWinBounds() {
			int mult;
			Tuple<float, float>[] magnitudeAngleOffsetPairs;

			if( FinishPortHeading == 0 || FinishPortHeading == 180 ) {
				//magnitude, angle offset
				magnitudeAngleOffsetPairs = new[] {
					new Tuple<float, float>( 0.0278f, 70.25f ), new Tuple<float, float>( 0.02807f, 289.5f ),
					new Tuple<float, float>( 0.02708f, 282f ), new Tuple<float, float>( 0.02665f, 77.75f )
				};
				mult = 1;
			}
			else {
				magnitudeAngleOffsetPairs = new[] {
					new Tuple<float, float>( 0.02088f, 228.5f ), new Tuple<float, float>( 0.01827f, 238.75f ),
					new Tuple<float, float>( 0.01806f, 121.75f ), new Tuple<float, float>( 0.02061f, 131.75f )
				};
				mult = -1;
			}

			var portBounds = new Vector2[4];
			int i = 0;
			foreach( var pair in magnitudeAngleOffsetPairs ) {
				portBounds[i] = CbHelper.GetOffsetPosition( FinishPortPos, pair.Item1,
					(FinishPortHeading + pair.Item2) % 360, mult );
				i = i + 1;
			}

			return portBounds;
		}

		/// <summary>
		/// Gets the start port position.
		/// </summary>
		/// <param name="levelNumber">The level number.</param>
		/// <returns></returns>
		private Vector2 GetStartPortPosition( int levelNumber ) {
			var potentialPortBounds = GetPortPositionBounds( levelNumber );
			if( !potentialPortBounds.Any() ) return Vector2.Zero;

			var startPortBounds = potentialPortBounds[Rand.GetRange( 0, potentialPortBounds.Count )];
			var startPos = Vector2.Zero;
			int attempts = 20;
			while( startPos == Vector2.Zero && attempts > 0 ) {
				startPos = GetRandomPortPosition( startPortBounds );
				attempts--;
			}

			return startPos;
		}

		/// <summary>
		/// Gets the finish port position.
		/// </summary>
		/// <param name="levelNumber">The level number.</param>
		/// <param name="startPortPosition">The start port position.</param>
		/// <returns></returns>
		private Vector2 GetFinishPortPosition( int levelNumber, Vector2 startPortPosition ) {
			var potentialPortBounds = GetPortPositionBounds( levelNumber );

			float maxDist = 0f;
			var endPos = Vector2.Zero;
			foreach( var bounds in potentialPortBounds ) {
				var potentialPos = Vector2.Zero;
				while( potentialPos == Vector2.Zero ) potentialPos = GetRandomPortPosition( bounds );

				float startEndDistance = Vector2.Distance( startPortPosition, potentialPos );
				if( startEndDistance > maxDist ) {
					maxDist = startEndDistance;
					endPos = potentialPos;
				}
			}

			return endPos;
		}

		/// <summary>
		/// Gets the port heading.
		/// </summary>
		/// <param name="portPosition">The port position.</param>
		/// <returns></returns>
		private float GetPortHeading( Vector2 portPosition ) {
			float minX = 0.159f;
			float maxX = 0.841f;

			float minY = 0.153f;
			float maxY = 0.848f;

			var xBounds = new List<float> {minX, maxX};
			var yBounds = new List<float> {minY, maxY};

			float closestX = xBounds.OrderBy( x => Math.Abs( portPosition.X - x ) ).FirstOrDefault();
			float closestY = yBounds.OrderBy( y => Math.Abs( portPosition.Y - y ) ).FirstOrDefault();

			if( Math.Abs( portPosition.X - closestX ) < Math.Abs( portPosition.Y - closestY ) ) {
				//X boundary
				if( Math.Abs( closestX - minX ) < Math.Abs( closestX - maxX ) )
					return 0;
				return 180f;
			}

			//Y boundary
			if( Math.Abs( closestY - minY ) < Math.Abs( closestY - maxY ) )
				return 90f;
			return 270f;
		}

		/// <summary>
		/// Gets the random port position.
		/// </summary>
		/// <param name="portBounds">The port bounds.</param>
		/// <returns></returns>
		private Vector2 GetRandomPortPosition( List<Vector2> portBounds ) {
			if( portBounds == null || portBounds.Count < 2 ) {
				Log.Error( $"CircuitBreaker: portBounds not formatted, count={portBounds?.Count}" );
				return Vector2.Zero;
			}

			float portX = Rand.GetRange( (int)(portBounds[0].X * 1000), (int)(portBounds[1].X * 1000) ) / 1000f;
			float portY =
				Rand.GetRange( (int)(portBounds[0].Y * 1000), (int)(portBounds[1].Y * 1000) ) / 1000f;

			return new Vector2( portX, portY );
		}

		/// <summary>
		/// Gets the port position bounds.
		/// </summary>
		/// <param name="mapNumber">The map number.</param>
		/// <returns></returns>
		private List<List<Vector2>> GetPortPositionBounds( int mapNumber ) {
			var portPositions = new List<List<Vector2>>();

			switch( mapNumber ) {
			case 1:
				portPositions = new List<List<Vector2>> {
					new List<Vector2> {
						new Vector2( 0.169f, 0.613f ),
						new Vector2( 0.169f, 0.816f )
					},
					new List<Vector2> {
						new Vector2( 0.179f, 0.837f ),
						new Vector2( 0.284f, 0.837f )
					},
					new List<Vector2> {
						new Vector2( 0.833f, 0.181f ),
						new Vector2( 0.833f, 0.277f )
					},
					new List<Vector2> {
						new Vector2( 0.751f, 0.163f ),
						new Vector2( 0.823f, 0.163f )
					}
				};
				break;
			case 2:
				portPositions = new List<List<Vector2>> {
					new List<Vector2> {
						new Vector2( 0.169f, 0.673f ),
						new Vector2( 0.169f, 0.818f )
					},
					new List<Vector2> {
						new Vector2( 0.18f, 0.838f ),
						new Vector2( 0.297f, 0.838f )
					},
					new List<Vector2> {
						new Vector2( 0.832f, 0.181f ),
						new Vector2( 0.832f, 0.324f )
					},
					new List<Vector2> {
						new Vector2( 0.778f, 0.16f ),
						new Vector2( 0.821f, 0.16f )
					}
				};
				break;
			case 3:
				portPositions = new List<List<Vector2>> {
					new List<Vector2> {
						new Vector2( 0.166f, 0.182f ),
						new Vector2( 0.166f, 0.263f )
					},
					new List<Vector2> {
						new Vector2( 0.166f, 0.745f ),
						new Vector2( 0.166f, 0.816f )
					},
					new List<Vector2> {
						new Vector2( 0.18f, 0.837f ),
						new Vector2( 0.31f, 0.837f )
					},
					new List<Vector2> {
						new Vector2( 0.184f, 0.164f ),
						new Vector2( 0.277f, 0.164f )
					}
				};
				break;
			case 4:
				portPositions = new List<List<Vector2>> {
					new List<Vector2> {
						new Vector2( 0.169f, 0.628f ),
						new Vector2( 0.169f, 0.817f )
					},
					new List<Vector2> {
						new Vector2( 0.183f, 0.838f ),
						new Vector2( 0.259f, 0.838f )
					},
					new List<Vector2> {
						new Vector2( 0.833f, 0.186f ),
						new Vector2( 0.833f, 0.359f )
					},
					new List<Vector2> {
						new Vector2( 0.797f, 0.161f ),
						new Vector2( 0.819f, 0.161f )
					}
				};
				break;
			case 5:
				portPositions = new List<List<Vector2>> {
					new List<Vector2> {
						new Vector2( 0.832f, 0.742f ),
						new Vector2( 0.832f, 0.811f )
					},
					new List<Vector2> {
						new Vector2( 0.761f, 0.839f ),
						new Vector2( 0.821f, 0.839f )
					},
					new List<Vector2> {
						new Vector2( 0.169f, 0.184f ),
						new Vector2( 0.169f, 0.383f )
					},
					new List<Vector2> {
						new Vector2( 0.184f, 0.162f ),
						new Vector2( 0.234f, 0.162f )
					}
				};
				break;
			case 6:
				portPositions = new List<List<Vector2>> {
					new List<Vector2> {
						new Vector2( 0.167f, 0.183f ),
						new Vector2( 0.167f, 0.3f )
					},
					new List<Vector2> {
						new Vector2( 0.18f, 0.162f ),
						new Vector2( 0.214f, 0.162f ),
					},
					new List<Vector2> {
						new Vector2( 0.833f, 0.186f ),
						new Vector2( 0.833f, 0.282f )
					},
					new List<Vector2> {
						new Vector2( 0.768f, 0.161f ),
						new Vector2( 0.82f, 0.161f )
					}
				};
				break;
			}

			return portPositions;
		}
	}
}