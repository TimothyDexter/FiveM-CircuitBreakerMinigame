//  CBPortLights.cs
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
//  Revision 0.0.1 2019/04/12 5:18 PM EDT TimothyDexter 
//  - Initial release
//   

using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Roleplay.Client.Classes.Crime.MiniGames.CircuitBreaker
{
	public class CbPortLights
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CbPortLights"/> class.
		/// </summary>
		/// <param name="portPos">The port position.</param>
		/// <param name="portHeading">The port heading.</param>
		/// <param name="type">The type.</param>
		public CbPortLights( Vector2 portPos, float portHeading, CbPortPositionTypeEnum type ) {
			Light0Position = GetLightPosition( portPos, portHeading, 0 );
			Light1Position = GetLightPosition( portPos, portHeading, 1 );
			PortType = type;
			Alpha = 255;
		}

		public Vector2 Light0Position { get; }
		public Vector2 Light1Position { get; }
		public CbPortPositionTypeEnum PortType { get; }
		private DateTime LastBlink { get; set; }
		private int Alpha { get; set; }

		/// <summary>
		/// Draws the lights.
		/// </summary>
		public void DrawLights() {
			if( PortType == CbPortPositionTypeEnum.Start ) {
				DrawLightSprite( Light0Position, 45, 203, 134 );
				DrawLightSprite( Light1Position, 45, 203, 134 );
			}
			else {
				if( DateTime.Now.CompareTo( LastBlink.AddMilliseconds( 500 ) ) >= 0 ) {
					Alpha = Alpha == 255 ? 0 : 255;
					LastBlink = DateTime.Now;
				}

				DrawLightSprite( Light0Position, 188, 49, 43, Alpha );
				DrawLightSprite( Light1Position, 188, 49, 43, Alpha );
			}
		}

		/// <summary>
		/// Draws the light sprite.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="red">The red.</param>
		/// <param name="green">The green.</param>
		/// <param name="blue">The blue.</param>
		/// <param name="alpha">The alpha.</param>
		public void DrawLightSprite( Vector2 position, int red, int green, int blue, int alpha = 255 ) {
			API.DrawSprite( "MPCircuitHack", "light", position.X, position.Y, 0.00775f, 0.00775f, 0, red,
				green, blue, alpha );
		}

		/// <summary>
		/// Gets the light position.
		/// </summary>
		/// <param name="portPos">The port position.</param>
		/// <param name="portHeading">The port heading.</param>
		/// <param name="lightNum">The light number.</param>
		/// <returns></returns>
		private Vector2 GetLightPosition( Vector2 portPos, float portHeading, int lightNum ) {
			float magnitude;
			float angleOffset;
			int multiplier = 1;
			if( portHeading == 90 || portHeading == 270 ) {
				angleOffset = lightNum > 0 ? 128.75f : 232f;
				magnitude = 0.0164f;
				multiplier = -1;
			}
			else {
				angleOffset = lightNum > 0 ? 73f : 287.25f;
				magnitude = 0.0228f;
			}

			return CbHelper.GetOffsetPosition( portPos, magnitude, (angleOffset + portHeading) % 360, multiplier );
		}
	}
}