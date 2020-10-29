//  MapBoundaries.cs
//  Author: Timothy Dexter
//  Release: 0.0.1
//  Date: 04/13/2019
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
//  Revision 0.0.1 2019/04/14 10:51 AM EDT TimothyDexter 
//  - Initial release
//   

using System.Collections.Generic;
using CitizenFX.Core;

namespace Roleplay.Client.Classes.Crime.MiniGames.CircuitBreaker
{
	/// <summary>
	///		Out of bounds areas for each map
	/// </summary>
	public static class CbMapBoundaries
	{
		public static List<List<Vector2>> GetBoxBounds( int mapNumber ) {
			var polyBounds = new List<List<Vector2>>();

			switch( mapNumber ) {
			case 1: {
				polyBounds = new List<List<Vector2>> {
					new List<Vector2> {
						new Vector2( 0.18f, 0.155f ),
						new Vector2( 0.18f, 0.583f ),
						new Vector2( 0.307f, 0.583f ),
						new Vector2( 0.307f, 0.154f )
					},
					new List<Vector2> {
						new Vector2( 0.321f, 0.154f ),
						new Vector2( 0.321f, 0.477f ),
						new Vector2( 0.382f, 0.477f ),
						new Vector2( 0.382f, 0.154f )
					},
					new List<Vector2> {
						new Vector2( 0.396f, 0.154f ),
						new Vector2( 0.396f, 0.379f ),
						new Vector2( 0.429f, 0.379f ),
						new Vector2( 0.429f, 0.155f )
					},
					new List<Vector2> {
						new Vector2( 0.443f, 0.155f ),
						new Vector2( 0.443f, 0.378f ),
						new Vector2( 0.477f, 0.378f ),
						new Vector2( 0.477f, 0.154f )
					},
					new List<Vector2> {
						new Vector2( 0.491f, 0.154f ),
						new Vector2( 0.491f, 0.379f ),
						new Vector2( 0.525f, 0.379f ),
						new Vector2( 0.525f, 0.155f )
					},
					new List<Vector2> {
						new Vector2( 0.538f, 0.155f ),
						new Vector2( 0.538f, 0.308f ),
						new Vector2( 0.585f, 0.308f ),
						new Vector2( 0.585f, 0.155f )
					},
					new List<Vector2> {
						new Vector2( 0.597f, 0.155f ),
						new Vector2( 0.597f, 0.308f ),
						new Vector2( 0.645f, 0.308f ),
						new Vector2( 0.645f, 0.155f )
					},
					new List<Vector2> {
						new Vector2( 0.66f, 0.155f ),
						new Vector2( 0.66f, 0.255f ),
						new Vector2( 0.73f, 0.255f ),
						new Vector2( 0.73f, 0.154f )
					},
					new List<Vector2> {
						new Vector2( 0.692f, 0.311f ),
						new Vector2( 0.692f, 0.373f ),
						new Vector2( 0.584f, 0.376f ),
						new Vector2( 0.584f, 0.452f ),
						new Vector2( 0.838f, 0.452f ),
						new Vector2( 0.838f, 0.31f )
					},
					new List<Vector2> {
						new Vector2( 0.343f, 0.544f ),
						new Vector2( 0.343f, 0.639f ),
						new Vector2( 0.398f, 0.639f ),
						new Vector2( 0.398f, 0.544f )
					},
					new List<Vector2> {
						new Vector2( 0.302f, 0.7f ),
						new Vector2( 0.302f, 0.846f ),
						new Vector2( 0.434f, 0.846f ),
						new Vector2( 0.434f, 0.7f )
					},
					new List<Vector2> {
						new Vector2( 0.451f, 0.435f ),
						new Vector2( 0.451f, 0.847f ),
						new Vector2( 0.569f, 0.847f ),
						new Vector2( 0.569f, 0.436f )
					},
					new List<Vector2> {
						new Vector2( 0.587f, 0.477f ),
						new Vector2( 0.587f, 0.846f ),
						new Vector2( 0.705f, 0.846f ),
						new Vector2( 0.705f, 0.477f )
					},
					new List<Vector2> {
						new Vector2( 0.721f, 0.477f ),
						new Vector2( 0.721f, 0.846f ),
						new Vector2( 0.838f, 0.846f ),
						new Vector2( 0.838f, 0.475f )
					}
				};
			}
				break;
			case 2:
				polyBounds = new List<List<Vector2>> {
					new List<Vector2> {
						new Vector2( 0.162f, 0.152f ),
						new Vector2( 0.163f, 0.645f ),
						new Vector2( 0.249f, 0.643f ),
						new Vector2( 0.252f, 0.275f ),
						new Vector2( 0.375f, 0.275f ),
						new Vector2( 0.375f, 0.35f ),
						new Vector2( 0.416f, 0.35f ),
						new Vector2( 0.416f, 0.157f )
					},
					new List<Vector2> {
						new Vector2( 0.313f, 0.36f ),
						new Vector2( 0.313f, 0.844f ),
						new Vector2( 0.442f, 0.844f ),
						new Vector2( 0.442f, 0.419f ),
						new Vector2( 0.349f, 0.415f ),
						new Vector2( 0.348f, 0.36f )
					},
					new List<Vector2> {
						new Vector2( 0.458f, 0.238f ),
						new Vector2( 0.458f, 0.844f ),
						new Vector2( 0.515f, 0.844f ),
						new Vector2( 0.515f, 0.238f )
					},
					new List<Vector2> {
						new Vector2( 0.555f, 0.156f ),
						new Vector2( 0.555f, 0.454f ),
						new Vector2( 0.541f, 0.458f ),
						new Vector2( 0.538f, 0.551f ),
						new Vector2( 0.685f, 0.551f ),
						new Vector2( 0.688f, 0.46f ),
						new Vector2( 0.725f, 0.456f ),
						new Vector2( 0.728f, 0.309f ),
						new Vector2( 0.757f, 0.303f ),
						new Vector2( 0.759f, 0.157f )
					},
					new List<Vector2> {
						new Vector2( 0.552f, 0.635f ),
						new Vector2( 0.552f, 0.786f ),
						new Vector2( 0.695f, 0.787f ),
						new Vector2( 0.695f, 0.633f )
					},
					new List<Vector2> {
						new Vector2( 0.776f, 0.36f ),
						new Vector2( 0.776f, 0.455f ),
						new Vector2( 0.839f, 0.455f ),
						new Vector2( 0.839f, 0.358f )
					},
					new List<Vector2> {
						new Vector2( 0.739f, 0.517f ),
						new Vector2( 0.739f, 0.679f ),
						new Vector2( 0.801f, 0.681f ),
						new Vector2( 0.801f, 0.514f )
					},
					new List<Vector2> {
						new Vector2( 0.739f, 0.749f ),
						new Vector2( 0.739f, 0.846f ),
						new Vector2( 0.839f, 0.846f ),
						new Vector2( 0.838f, 0.747f )
					}
				};
				break;
			case 3:
				polyBounds = new List<List<Vector2>> {
					new List<Vector2> {
						new Vector2( 0.299f, 0.153f ),
						new Vector2( 0.299f, 0.245f ),
						new Vector2( 0.372f, 0.249f ),
						new Vector2( 0.375f, 0.343f ),
						new Vector2( 0.465f, 0.344f ),
						new Vector2( 0.465f, 0.247f ),
						new Vector2( 0.448f, 0.242f ),
						new Vector2( 0.446f, 0.154f )
					},
					new List<Vector2> {
						new Vector2( 0.163f, 0.298f ),
						new Vector2( 0.163f, 0.715f ),
						new Vector2( 0.328f, 0.715f ),
						new Vector2( 0.331f, 0.578f ),
						new Vector2( 0.499f, 0.578f ),
						new Vector2( 0.502f, 0.771f ),
						new Vector2( 0.567f, 0.771f ),
						new Vector2( 0.568f, 0.564f ),
						new Vector2( 0.649f, 0.564f ),
						new Vector2( 0.649f, 0.473f ),
						new Vector2( 0.574f, 0.468f ),
						new Vector2( 0.572f, 0.247f ),
						new Vector2( 0.501f, 0.247f ),
						new Vector2( 0.501f, 0.403f ),
						new Vector2( 0.329f, 0.403f ),
						new Vector2( 0.328f, 0.299f )
					},
					new List<Vector2> {
						new Vector2( 0.365f, 0.674f ),
						new Vector2( 0.365f, 0.846f ),
						new Vector2( 0.436f, 0.846f ),
						new Vector2( 0.436f, 0.674f )
					},
					new List<Vector2> {
						new Vector2( 0.615f, 0.154f ),
						new Vector2( 0.615f, 0.383f ),
						new Vector2( 0.839f, 0.383f ),
						new Vector2( 0.839f, 0.155f )
					},
					new List<Vector2> {
						new Vector2( 0.698f, 0.429f ),
						new Vector2( 0.698f, 0.561f ),
						new Vector2( 0.839f, 0.561f ),
						new Vector2( 0.839f, 0.43f )
					},
					new List<Vector2> {
						new Vector2( 0.613f, 0.649f ),
						new Vector2( 0.613f, 0.845f ),
						new Vector2( 0.839f, 0.845f ),
						new Vector2( 0.839f, 0.649f )
					}
				};
				break;
			case 4:
				polyBounds = new List<List<Vector2>> {
					new List<Vector2> {
						new Vector2( 0.162f, 0.154f ),
						new Vector2( 0.162f, 0.593f ),
						new Vector2( 0.305f, 0.595f ),
						new Vector2( 0.307f, 0.654f ),
						new Vector2( 0.419f, 0.658f ),
						new Vector2( 0.421f, 0.78f ),
						new Vector2( 0.54f, 0.78f ),
						new Vector2( 0.542f, 0.658f ),
						new Vector2( 0.69f, 0.653f ),
						new Vector2( 0.69f, 0.559f ),
						new Vector2( 0.542f, 0.552f ),
						new Vector2( 0.54f, 0.489f ),
						new Vector2( 0.324f, 0.484f ),
						new Vector2( 0.322f, 0.154f )
					},
					new List<Vector2> {
						new Vector2( 0.276f, 0.728f ),
						new Vector2( 0.276f, 0.846f ),
						new Vector2( 0.381f, 0.846f ),
						new Vector2( 0.381f, 0.73f )
					},
					new List<Vector2> {
						new Vector2( 0.352f, 0.22f ),
						new Vector2( 0.352f, 0.298f ),
						new Vector2( 0.368f, 0.302f ),
						new Vector2( 0.369f, 0.434f ),
						new Vector2( 0.421f, 0.434f ),
						new Vector2( 0.422f, 0.41f ),
						new Vector2( 0.576f, 0.41f ),
						new Vector2( 0.576f, 0.478f ),
						new Vector2( 0.735f, 0.48f ),
						new Vector2( 0.736f, 0.715f ),
						new Vector2( 0.578f, 0.718f ),
						new Vector2( 0.578f, 0.847f ),
						new Vector2( 0.837f, 0.847f ),
						new Vector2( 0.837f, 0.397f ),
						new Vector2( 0.78f, 0.397f ),
						new Vector2( 0.779f, 0.427f ),
						new Vector2( 0.763f, 0.427f ),
						new Vector2( 0.761f, 0.374f ),
						new Vector2( 0.687f, 0.369f ),
						new Vector2( 0.687f, 0.23f ),
						new Vector2( 0.643f, 0.23f ),
						new Vector2( 0.643f, 0.371f ),
						new Vector2( 0.624f, 0.371f ),
						new Vector2( 0.623f, 0.315f ),
						new Vector2( 0.422f, 0.313f ),
						new Vector2( 0.421f, 0.22f )
					},
					new List<Vector2> {
						new Vector2( 0.46f, 0.154f ),
						new Vector2( 0.46f, 0.263f ),
						new Vector2( 0.596f, 0.261f ),
						new Vector2( 0.597f, 0.154f )
					},
					new List<Vector2> {
						new Vector2( 0.723f, 0.154f ),
						new Vector2( 0.723f, 0.262f ),
						new Vector2( 0.778f, 0.262f ),
						new Vector2( 0.778f, 0.155f )
					}
				};
				break;
			case 5:
				polyBounds = new List<List<Vector2>> {
					new List<Vector2> {
						new Vector2( 0.254f, 0.156f ),
						new Vector2( 0.253f, 0.436f ),
						new Vector2( 0.195f, 0.439f ),
						new Vector2( 0.195f, 0.514f ),
						new Vector2( 0.253f, 0.515f ),
						new Vector2( 0.255f, 0.701f ),
						new Vector2( 0.337f, 0.704f ),
						new Vector2( 0.339f, 0.788f ),
						new Vector2( 0.372f, 0.787f ),
						new Vector2( 0.372f, 0.636f ),
						new Vector2( 0.401f, 0.636f ),
						new Vector2( 0.401f, 0.673f ),
						new Vector2( 0.471f, 0.672f ),
						new Vector2( 0.471f, 0.637f ),
						new Vector2( 0.606f, 0.637f ),
						new Vector2( 0.606f, 0.682f ),
						new Vector2( 0.652f, 0.682f ),
						new Vector2( 0.652f, 0.483f ),
						new Vector2( 0.497f, 0.483f ),
						new Vector2( 0.496f, 0.53f ),
						new Vector2( 0.328f, 0.53f ),
						new Vector2( 0.328f, 0.261f ),
						new Vector2( 0.409f, 0.261f ),
						new Vector2( 0.41f, 0.359f ),
						new Vector2( 0.441f, 0.359f ),
						new Vector2( 0.441f, 0.244f ),
						new Vector2( 0.531f, 0.244f ),
						new Vector2( 0.532f, 0.305f ),
						new Vector2( 0.577f, 0.305f ),
						new Vector2( 0.577f, 0.255f ),
						new Vector2( 0.605f, 0.253f ),
						new Vector2( 0.605f, 0.154f )
					},
					new List<Vector2> {
						new Vector2( 0.163f, 0.58f ),
						new Vector2( 0.163f, 0.635f ),
						new Vector2( 0.219f, 0.635f ),
						new Vector2( 0.219f, 0.581f )
					},
					new List<Vector2> {
						new Vector2( 0.232f, 0.761f ),
						new Vector2( 0.232f, 0.844f ),
						new Vector2( 0.305f, 0.846f ),
						new Vector2( 0.305f, 0.761f )
					},
					new List<Vector2> {
						new Vector2( 0.383f, 0.413f ),
						new Vector2( 0.383f, 0.493f ),
						new Vector2( 0.461f, 0.493f ),
						new Vector2( 0.461f, 0.414f )
					},
					new List<Vector2> {
						new Vector2( 0.417f, 0.744f ),
						new Vector2( 0.417f, 0.846f ),
						new Vector2( 0.654f, 0.846f ),
						new Vector2( 0.654f, 0.744f ),
						new Vector2( 0.552f, 0.743f ),
						new Vector2( 0.55f, 0.704f ),
						new Vector2( 0.497f, 0.704f ),
						new Vector2( 0.495f, 0.742f ),
						new Vector2( 0.417f, 0.745f )
					},
					new List<Vector2> {
						new Vector2( 0.482f, 0.301f ),
						new Vector2( 0.482f, 0.431f ),
						new Vector2( 0.561f, 0.431f ),
						new Vector2( 0.561f, 0.368f ),
						new Vector2( 0.511f, 0.364f ),
						new Vector2( 0.509f, 0.302f )
					},
					new List<Vector2> {
						new Vector2( 0.658f, 0.199f ),
						new Vector2( 0.657f, 0.366f ),
						new Vector2( 0.578f, 0.368f ),
						new Vector2( 0.578f, 0.432f ),
						new Vector2( 0.75f, 0.434f ),
						new Vector2( 0.75f, 0.495f ),
						new Vector2( 0.694f, 0.496f ),
						new Vector2( 0.694f, 0.845f ),
						new Vector2( 0.742f, 0.845f ),
						new Vector2( 0.743f, 0.646f ),
						new Vector2( 0.763f, 0.644f ),
						new Vector2( 0.764f, 0.555f ),
						new Vector2( 0.805f, 0.554f ),
						new Vector2( 0.805f, 0.435f ),
						new Vector2( 0.788f, 0.432f ),
						new Vector2( 0.787f, 0.368f ),
						new Vector2( 0.707f, 0.367f ),
						new Vector2( 0.706f, 0.199f )
					},
					new List<Vector2> {
						new Vector2( 0.754f, 0.155f ),
						new Vector2( 0.753f, 0.22f ),
						new Vector2( 0.775f, 0.22f ),
						new Vector2( 0.775f, 0.155f )
					},
					new List<Vector2> {
						new Vector2( 0.818f, 0.259f ),
						new Vector2( 0.818f, 0.327f ),
						new Vector2( 0.838f, 0.325f ),
						new Vector2( 0.838f, 0.258f )
					},
					new List<Vector2> {
						new Vector2( 0.808f, 0.616f ),
						new Vector2( 0.809f, 0.707f ),
						new Vector2( 0.838f, 0.706f ),
						new Vector2( 0.838f, 0.616f )
					}
				};
				break;
			case 6:
				polyBounds = new List<List<Vector2>> {
					new List<Vector2> {
						new Vector2( 0.232f, 0.155f ),
						new Vector2( 0.232f, 0.218f ),
						new Vector2( 0.254f, 0.218f ),
						new Vector2( 0.254f, 0.154f )
					},
					new List<Vector2> {
						new Vector2( 0.225f, 0.281f ),
						new Vector2( 0.224f, 0.328f ),
						new Vector2( 0.162f, 0.331f ),
						new Vector2( 0.162f, 0.515f ),
						new Vector2( 0.214f, 0.515f ),
						new Vector2( 0.214f, 0.425f ),
						new Vector2( 0.247f, 0.422f ),
						new Vector2( 0.247f, 0.281f )
					},
					new List<Vector2> {
						new Vector2( 0.163f, 0.572f ),
						new Vector2( 0.163f, 0.847f ),
						new Vector2( 0.273f, 0.847f ),
						new Vector2( 0.273f, 0.758f ),
						new Vector2( 0.205f, 0.757f ),
						new Vector2( 0.205f, 0.622f ),
						new Vector2( 0.216f, 0.621f ),
						new Vector2( 0.216f, 0.572f )
					},
					new List<Vector2> {
						new Vector2( 0.24f, 0.648f ),
						new Vector2( 0.24f, 0.715f ),
						new Vector2( 0.261f, 0.715f ),
						new Vector2( 0.261f, 0.649f )
					},
					new List<Vector2> {
						new Vector2( 0.301f, 0.154f ),
						new Vector2( 0.3f, 0.249f ),
						new Vector2( 0.284f, 0.251f ),
						new Vector2( 0.284f, 0.327f ),
						new Vector2( 0.3f, 0.331f ),
						new Vector2( 0.3f, 0.47f ),
						new Vector2( 0.251f, 0.472f ),
						new Vector2( 0.251f, 0.563f ),
						new Vector2( 0.299f, 0.563f ),
						new Vector2( 0.3f, 0.537f ),
						new Vector2( 0.324f, 0.539f ),
						new Vector2( 0.324f, 0.603f ),
						new Vector2( 0.298f, 0.605f ),
						new Vector2( 0.298f, 0.697f ),
						new Vector2( 0.324f, 0.7f ),
						new Vector2( 0.325f, 0.806f ),
						new Vector2( 0.499f, 0.806f ),
						new Vector2( 0.499f, 0.758f ),
						new Vector2( 0.377f, 0.755f ),
						new Vector2( 0.377f, 0.598f ),
						new Vector2( 0.425f, 0.596f ),
						new Vector2( 0.425f, 0.543f ),
						new Vector2( 0.377f, 0.541f ),
						new Vector2( 0.375f, 0.458f ),
						new Vector2( 0.354f, 0.455f ),
						new Vector2( 0.354f, 0.253f ),
						new Vector2( 0.392f, 0.25f ),
						new Vector2( 0.392f, 0.155f )
					},
					new List<Vector2> {
						new Vector2( 0.375f, 0.339f ),
						new Vector2( 0.375f, 0.407f ),
						new Vector2( 0.396f, 0.407f ),
						new Vector2( 0.396f, 0.339f )
					},
					new List<Vector2> {
						new Vector2( 0.453f, 0.154f ),
						new Vector2( 0.453f, 0.225f ),
						new Vector2( 0.474f, 0.223f ),
						new Vector2( 0.474f, 0.155f )
					},
					new List<Vector2> {
						new Vector2( 0.454f, 0.282f ),
						new Vector2( 0.452f, 0.341f ),
						new Vector2( 0.425f, 0.344f ),
						new Vector2( 0.425f, 0.423f ),
						new Vector2( 0.599f, 0.426f ),
						new Vector2( 0.599f, 0.511f ),
						new Vector2( 0.525f, 0.514f ),
						new Vector2( 0.524f, 0.65f ),
						new Vector2( 0.422f, 0.653f ),
						new Vector2( 0.422f, 0.71f ),
						new Vector2( 0.536f, 0.713f ),
						new Vector2( 0.537f, 0.846f ),
						new Vector2( 0.838f, 0.846f ),
						new Vector2( 0.838f, 0.747f ),
						new Vector2( 0.755f, 0.746f ),
						new Vector2( 0.754f, 0.696f ),
						new Vector2( 0.647f, 0.695f ),
						new Vector2( 0.646f, 0.745f ),
						new Vector2( 0.591f, 0.745f ),
						new Vector2( 0.59f, 0.653f ),
						new Vector2( 0.57f, 0.65f ),
						new Vector2( 0.57f, 0.598f ),
						new Vector2( 0.651f, 0.596f ),
						new Vector2( 0.653f, 0.342f ),
						new Vector2( 0.666f, 0.34f ),
						new Vector2( 0.665f, 0.216f ),
						new Vector2( 0.629f, 0.216f ),
						new Vector2( 0.628f, 0.342f ),
						new Vector2( 0.478f, 0.342f ),
						new Vector2( 0.477f, 0.282f )
					},
					new List<Vector2> {
						new Vector2( 0.464f, 0.477f ),
						new Vector2( 0.464f, 0.616f ),
						new Vector2( 0.485f, 0.615f ),
						new Vector2( 0.485f, 0.477f )
					},
					new List<Vector2> {
						new Vector2( 0.51f, 0.164f ),
						new Vector2( 0.51f, 0.286f ),
						new Vector2( 0.589f, 0.286f ),
						new Vector2( 0.589f, 0.165f )
					},
					new List<Vector2> {
						new Vector2( 0.698f, 0.155f ),
						new Vector2( 0.697f, 0.577f ),
						new Vector2( 0.681f, 0.58f ),
						new Vector2( 0.681f, 0.629f ),
						new Vector2( 0.747f, 0.627f ),
						new Vector2( 0.749f, 0.559f ),
						new Vector2( 0.796f, 0.556f ),
						new Vector2( 0.797f, 0.458f ),
						new Vector2( 0.749f, 0.456f ),
						new Vector2( 0.749f, 0.154f )
					},
					new List<Vector2> {
						new Vector2( 0.779f, 0.319f ),
						new Vector2( 0.779f, 0.402f ),
						new Vector2( 0.838f, 0.401f ),
						new Vector2( 0.838f, 0.319f )
					},
					new List<Vector2> {
						new Vector2( 0.784f, 0.615f ),
						new Vector2( 0.784f, 0.696f ),
						new Vector2( 0.837f, 0.695f ),
						new Vector2( 0.837f, 0.615f )
					}
				};
				break;
			}

			return polyBounds;
		}
	}
}