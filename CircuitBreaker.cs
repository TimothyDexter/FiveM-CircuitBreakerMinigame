//  CircuitBreaker.cs
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
//  - To run a completely custom game, await RunMiniGame. Anything other than a
//	  positive result indicates they were not successful.  Look at DemoCircuitBreaker
//	  method for example.  Add RunMiniGame overload if you see fit.
//	- If you want to make sure they /use the hacking kit near your position.  Poll
//	  IsPositionSetToExecuteHack when they're nearby the position you determine for
//	  the hack to occur.  When true, await RunMiniGame.
//  - Hacking Kit Version V1, V2, V3
//		- V1 allows player to play minigame
//		- V2 reduces the disconnect chance, increases reconnect time
//		- V3 reduces disconnect check, reduces disconnect check rate, reduces cursor speed,
//		  and does not change cursor speed on reconnect
//		- /Use a hacking kit nearby a computer shows a readme for that kit version
//		- /Use a hacking kit elsewhere sets them ready to execute the hack until they move again
//   
//  History:
//  Revision 0.0.1 2019/04/13 8:46 PM EDT TimothyDexter 
//  - Initial release
//   

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Common;
using Roleplay.Client.Classes.Inventory;
using Roleplay.Client.Classes.Player;
using Roleplay.Client.Helpers;
using Roleplay.Server.Enums;
using Roleplay.SharedClasses;
using Roleplay.SharedModels;
using Newtonsoft.Json;

namespace Roleplay.Client.Classes.Crime.MiniGames.CircuitBreaker
{
	internal static class CircuitBreaker
	{
		public static bool IsPositionSetToExecuteHack;

		public static bool HasCircuitFailed;
		public static bool HasCircuitCompleted;

		private const float HackingKitResetExePosThreshold = 2f;

		private const int DefaultDelayStartTimeMs = 1000;
		private const int MinDelayEndGameTimeMs = 5000;
		private const int MaxDelayEndGameTimeMs = 15000;

		private const int MinReconnectTimeMs = 3000;
		private const int MaxReconnectTimeMs = 30000;
		private const float MaxDisconnectChance = 0.90f;
		private const int MinDisconnectCheckRateMs = 500;

		private const float MinCursorSpeed = 0.00085f;
		private const float MaxCursorSpeed = 0.01f;

		private static bool IsInstanceInitialized;

		private static readonly List<Vector2> GameBounds = new List<Vector2> {
			new Vector2( 0.159f, 0.153f ), //TL 
			new Vector2( 0.159f, 0.848f ), //BL
			new Vector2( 0.841f, 0.848f ), //BR
			new Vector2( 0.841f, 0.153f ) //TR
		};
		private static readonly string[] TextureDictionaries = { "MPCircuitHack", "MPCircuitHack2", "MPCircuitHack3" };

		private static DateTime _gameEndTime;
		private static DateTime _gameStartTime;

		private static float _initCursorSpeed;
		private static float _cursorSpeed;

		private static List<List<Vector2>> _illegalAreas;

		private static CbGenericPorts _genericPorts;
		private static CbCursor _cursor;

		private static Scaleform _scaleform;

		private static CbDifficultyLevelEnum _currentDifficulty;
		private static int _currentLevelNumber;

		private static bool _isEndScreenActive;

		private static int _hackingKitVersionNumber;
		private static bool _isHackingKitDisconnected;

		private static bool _isDisconnectedScreenActive;
		private static DateTime _lastDisconnectCheckTime;
		private static DateTime _reconnectTime;

		private static int _backgroundSoundId;

		private static bool _isShowingHackingKitReadme;

		private static int _startingHealth;

		private static bool _debuggingMapPositions = false;
		private static float _debugPortHeading = 0f;

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public static void Init() {
			try {
#if DEBUG
				Client.ActiveInstance.ClientCommands.Register( "/democb", DemoCircuitBreaker );
				Client.ActiveInstance.ClientCommands.Register( "/debugcb", DebugCircuitBreaker );
#endif
			}
			catch( Exception ex ) {
				Log.Error( ex );
			}
		}

		private static async void DebugCircuitBreaker( Command command ) {
			await RunDefaultMiniGameFromDifficulty( command.Args.GetInt32( 0 ), CbDifficultyLevelEnum.Easy );
		}

		/// <summary>
		/// Demos the circuit breaker.
		/// </summary>
		/// <param name="command">The command.</param>
		private static async void DemoCircuitBreaker( Command command ) {
			try {
				if( !DoesPlayerHaveHackingKit() )
					if( PlayerInventory.AllItems.TryGetValue( "dext_sw_v1", out var sw ) ) {
						string data = JsonConvert.SerializeObject( sw );
						BaseScript.TriggerServerEvent( "Shops.AddItemToInventory", data, 1 );
					}

				int levelNumber = 1;
				if( command.Args.Count > 0 && int.TryParse( command.Args.Get( 0 ), out levelNumber ) )
					levelNumber = MathUtil.Clamp( levelNumber, 1,
						6 );

				int numberOfLives = 3;
				if( command.Args.Count > 1 && int.TryParse( command.Args.Get( 1 ), out numberOfLives ) )
					numberOfLives = MathUtil.Clamp( numberOfLives, 1, 6 );

				int difficultyResult = 1;
				if( command.Args.Count > 2 && int.TryParse( command.Args.Get( 2 ), out difficultyResult ) )
					difficultyResult = MathUtil.Clamp( difficultyResult, 0,
						Enum.GetValues( typeof(CbDifficultyLevelEnum) ).Length - 1 );

				while( numberOfLives > 0 && levelNumber <= 6 ) {
					int gameResult =
						await RunDefaultMiniGameFromDifficulty( levelNumber, (CbDifficultyLevelEnum)difficultyResult );
					if( (CbGameStatusEnum)gameResult == CbGameStatusEnum.Success )
						levelNumber = levelNumber + 1;
					else
						numberOfLives = numberOfLives - 1;
				}
			}
			catch( Exception ex ) {
				Log.Error( ex );
			}
		}

		/// <summary>
		/// Handles the use hacking software.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		public static async Task HandleUseHackingSoftware(InventoryItemsModel model) {
			if( _isShowingHackingKitReadme || IsPositionSetToExecuteHack ) return;

			if( IsNearComputerProp() ) {
				await ShowHackingKitReadme( model.ItemKey );
			}
			else {
				await SetPositionToExecuteHack(model.ItemKey);
			}
		}
		
		/// <summary>
		/// Runs the default mini game from difficulty.
		/// </summary>
		/// <param name="levelNumber">The level number.</param>
		/// <param name="difficultyLevel">The difficulty level.</param>
		/// <returns></returns>
		public static async Task<int> RunDefaultMiniGameFromDifficulty( int levelNumber,
			CbDifficultyLevelEnum difficultyLevel ) {
			return await RunMiniGame( levelNumber, difficultyLevel,
				GetCursorSpeedFromDifficulty( difficultyLevel ), DefaultDelayStartTimeMs,
				MinDelayEndGameTimeMs, MaxDelayEndGameTimeMs, GetDisconnectChanceFromDifficulty( difficultyLevel ),
				GetDisconnectCheckRateMsFromDifficulty( difficultyLevel ), MinReconnectTimeMs, MinReconnectTimeMs );
		}

		/// <summary>
		/// Runs the mini game.
		/// </summary>
		/// <param name="levelNumber">The level number.</param>
		/// <param name="difficultyLevel">The difficulty level.</param>
		/// <param name="cursorSpeed">The cursor speed.</param>
		/// <param name="delayStartMs">The delay start ms.</param>
		/// <param name="minFailureDelayTimeMs">The minimum failure delay time ms.</param>
		/// <param name="maxFailureDelayTimeMs">The maximum failure delay time ms.</param>
		/// <param name="disconnectChance">The disconnect chance.</param>
		/// <param name="disconnectCheckRateMs">The disconnect check rate ms.</param>
		/// <param name="minReconnectTimeMs">The minimum reconnect time ms.</param>
		/// <param name="maxReconnectTimeMs">The maximum reconnect time ms.</param>
		/// <returns></returns>
		public static async Task<int> RunMiniGame( int levelNumber, CbDifficultyLevelEnum difficultyLevel,
			float cursorSpeed, int delayStartMs, int minFailureDelayTimeMs, int maxFailureDelayTimeMs,
			float disconnectChance, int disconnectCheckRateMs, int minReconnectTimeMs, int maxReconnectTimeMs ) {
			cursorSpeed = MathUtil.Clamp( cursorSpeed, MinCursorSpeed, MaxCursorSpeed );

			delayStartMs = MathUtil.Clamp( delayStartMs, 1000, 60000 );
			minFailureDelayTimeMs =
				MathUtil.Clamp( minFailureDelayTimeMs, MinDelayEndGameTimeMs, maxFailureDelayTimeMs );
			maxFailureDelayTimeMs = MathUtil.Clamp( maxFailureDelayTimeMs, MinDelayEndGameTimeMs,
				maxFailureDelayTimeMs > minFailureDelayTimeMs ? maxFailureDelayTimeMs : minFailureDelayTimeMs + 1 );

			disconnectChance = MathUtil.Clamp( disconnectChance, 0, MaxDisconnectChance );
			disconnectCheckRateMs =
				MathUtil.Clamp( disconnectCheckRateMs, MinDisconnectCheckRateMs, disconnectCheckRateMs );
			minReconnectTimeMs = MathUtil.Clamp( minReconnectTimeMs, MinReconnectTimeMs, maxReconnectTimeMs );
			maxReconnectTimeMs = MathUtil.Clamp( maxReconnectTimeMs, minReconnectTimeMs + 1, MaxReconnectTimeMs );

			CurrentPlayer.EnableWeaponWheel( false );

			int gameResult = await RunMiniGameTask( levelNumber, difficultyLevel, cursorSpeed, delayStartMs,
				maxFailureDelayTimeMs,
				maxFailureDelayTimeMs, disconnectChance, disconnectCheckRateMs, minReconnectTimeMs,
				maxReconnectTimeMs );

			return gameResult;
		}

		/// <summary>
		/// Runs the mini game task.
		/// </summary>
		/// <param name="levelNumber">The level number.</param>
		/// <param name="difficultyLevel">The difficulty level.</param>
		/// <param name="cursorSpeed">The cursor speed.</param>
		/// <param name="delayStartMs">The delay start ms.</param>
		/// <param name="minFailureDelayTimeMs">The minimum failure delay time ms.</param>
		/// <param name="maxFailureDelayTimeMs">The maximum failure delay time ms.</param>
		/// <param name="disconnectChance">The disconnect chance.</param>
		/// <param name="disconnectCheckRateMs">The disconnect check rate ms.</param>
		/// <param name="minReconnectTimeMs">The minimum reconnect time ms.</param>
		/// <param name="maxReconnectTimeMs">The maximum reconnect time ms.</param>
		/// <returns></returns>
		private static async Task<int> RunMiniGameTask( int levelNumber, CbDifficultyLevelEnum difficultyLevel,
			float cursorSpeed, int delayStartMs, int minFailureDelayTimeMs, int maxFailureDelayTimeMs,
			float disconnectChance, int disconnectCheckRateMs, int minReconnectTimeMs, int maxReconnectTimeMs ) {
			try {
				if( !Session.HasJoinedRP ) {
					await BaseScript.Delay( 1000 );
					EndGame();
					return (int)CbGameStatusEnum.FailedToStart;
				}

				if( !DoesPlayerHaveHackingKit() ) return (int)CbGameStatusEnum.MissingHackingKit;

				await InitializeResources();

				API.PlaySoundFrontend( _backgroundSoundId, "Background", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true );
				PlayStartSound( delayStartMs );

				InitializeLevelVariables( levelNumber, difficultyLevel, cursorSpeed, delayStartMs );

				var debugCursorSpeed = 0.0015f;
				while( true ) {
					if( IsPlayerTakingDamage() ) {
						return (int)CbGameStatusEnum.TakingDamage;
					}

					if( ControlHelper.IsControlPressed( Control.Cover ) && !HasCircuitFailed ) {
						EndGame();
						return (int)CbGameStatusEnum.PlayerQuit;
					}

					//Sprite order matters
					DrawMapSprite( _currentLevelNumber );
					DisableControls();

					if( _debuggingMapPositions ) {
						var newDirection = CbHelper.GetDebugCursorDirectionInput();
						debugCursorSpeed = debugCursorSpeed + CbHelper.GetDebugCursorSpeedInput();
						if( newDirection >= 0 ) {
							_cursor.DebugCursorPosition( (CbDirectionsEnum)newDirection, debugCursorSpeed );
						}

						_debugPortHeading = CbHelper.GetPortDebugHeading( _debugPortHeading );
						CbHelper.DebugPortSprite( _cursor.Position, _debugPortHeading );

						if( ControlHelper.IsControlJustPressed( Control.Pickup ) ) {
							Log.Info($"new Vector2({_cursor.Position.X}, {_cursor.Position.Y}),");
						}

						await BaseScript.Delay( 0 );
						continue;
					}

					DrawCursorAndPortSprites();
					_scaleform?.Render2D();

					if( !_isEndScreenActive && _genericPorts.IsCursorInGameWinningPosition( _cursor.Position ) ) {
						HasCircuitCompleted = true;
						_gameEndTime = DateTime.Now.AddMilliseconds( MinDelayEndGameTimeMs );

						ShowSuccessScreenAndPlaySound();
						_isEndScreenActive = true;

					}
					else if( !_isEndScreenActive && IsCursorOutOfBounds( _illegalAreas, GameBounds ) ||
					         _genericPorts.IsCollisionWithPort( _cursor.Position ) || _cursor.CheckTailCollision() ) {

						HasCircuitFailed = true;

						if( _cursor.IsAlive ) {
							_cursor.IsAlive = false;
							_cursor.StartCursorDeathAnimation();
						}

						if( !_isEndScreenActive && !_cursor.IsVisible ) {
							ShowFailureScreenAndPlaySound();

							var failureTimeDelay = Rand.GetRange( minFailureDelayTimeMs, maxFailureDelayTimeMs );
							_gameEndTime = DateTime.Now.AddMilliseconds( failureTimeDelay );

							_isEndScreenActive = true;
						}
					}
					else if( !_isEndScreenActive && _isHackingKitDisconnected ) {
						if( !_isDisconnectedScreenActive ) {
							StartReconnection( minReconnectTimeMs, maxReconnectTimeMs );
							_isDisconnectedScreenActive = true;
						}

						if( _isDisconnectedScreenActive && DateTime.Now.CompareTo( _reconnectTime ) >= 0 ) {
							FinishReconnection();
							_isDisconnectedScreenActive = false;
						}
					}

					if( !_isHackingKitDisconnected && disconnectChance > 0 )
						CheckIfHackingKitDisconnected( disconnectChance, disconnectCheckRateMs );

					if( DateTime.Now.CompareTo( _gameStartTime.AddMilliseconds( delayStartMs ) ) >= 0 &&
					    !_isEndScreenActive && _cursor.IsAlive && !_isHackingKitDisconnected ) {
						_cursor.GetCursorInputFromPlayer();
						_cursor.MoveCursor( _cursorSpeed );
					}

					if(_isEndScreenActive && ( HasCircuitFailed || HasCircuitCompleted ))
						if( DateTime.Now.CompareTo( _gameEndTime ) >= 0 ) {

							API.StopSound( _backgroundSoundId );

							if( HasCircuitCompleted )
								API.PlaySoundFrontend( -1, "Success", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true );

							EndGame();
							return HasCircuitCompleted ? (int)CbGameStatusEnum.Success : (int)CbGameStatusEnum.Failure;
						}

					await BaseScript.Delay( 0 );
				}
			}
			catch( Exception ex ) {
				Log.Error( ex );
				EndGame();
				return (int)CbGameStatusEnum.Error;
			}
		}

		/// <summary>
		/// Is the player currently taking damage.
		/// </summary>
		/// <returns>true if player taking damage</returns>
		private static bool IsPlayerTakingDamage() {
			if( Cache.PlayerHealth < _startingHealth ) {
				return true;
			}

			if( Cache.PlayerHealth > _startingHealth ) {
				_startingHealth = Cache.PlayerHealth;
			}
			return false;
		}

		/// <summary>
		/// Does the player have hacking kit.
		/// </summary>
		/// <returns>true if player has kit</returns>
		private static bool DoesPlayerHaveHackingKit() {
			bool hasHackingKit =
				PlayerInventory.CurrentPlayerInventory.Items.Exists( i =>
					i.MetaData.ItemKey.Contains( "dext_sw_v".ToLower() ) );
			return hasHackingKit;
		}

		/// <summary>
		/// Gets the hacking kit version number.
		/// </summary>
		/// <returns></returns>
		private static int GetHackingKitVersionNumber() {
			var items = PlayerInventory.CurrentPlayerInventory.Items.Where( i =>
				i.MetaData.ItemKey.Contains( "dext_sw_v".ToLower() ) );

			int versionNumber = 1;
			foreach( var item in items ) {
				string versionString = new string( item.ItemKey.Where( char.IsDigit ).ToArray() );
				if( !string.IsNullOrEmpty( versionString ) ) {
					var version = int.Parse( versionString );
					if( version > versionNumber ) {
						versionNumber = version;
					}
				}
			}

			return versionNumber;
		}

		/// <summary>
		/// Initializes the level variables.
		/// </summary>
		/// <param name="levelNumber">The level number.</param>
		/// <param name="difficultyLevel">The difficulty level.</param>
		/// <param name="cursorSpeed">The cursor speed.</param>
		/// <param name="delayStartMs">The delay start ms.</param>
		private static void InitializeLevelVariables( int levelNumber, CbDifficultyLevelEnum difficultyLevel,
			float cursorSpeed, int delayStartMs ) {
			HasCircuitFailed = false;
			HasCircuitCompleted = false;
			_isEndScreenActive = false;
			_isHackingKitDisconnected = false;
			_hackingKitVersionNumber = GetHackingKitVersionNumber();

			_currentLevelNumber = levelNumber;
			_illegalAreas = CbMapBoundaries.GetBoxBounds( _currentLevelNumber );

			_genericPorts = new CbGenericPorts( _currentLevelNumber );

			_cursor = new CbCursor( _genericPorts );
			InitializeCursorSpeed( cursorSpeed );

			_currentDifficulty = difficultyLevel;

			_gameStartTime = DateTime.Now;
			_lastDisconnectCheckTime = DateTime.Now.AddMilliseconds( delayStartMs );
		}

		/// <summary>
		/// Initializes the cursor speed.
		/// </summary>
		/// <param name="cursorSpeed">The cursor speed.</param>
		private static void InitializeCursorSpeed( float cursorSpeed ) {
			var bonus = ApplyHackingKitBonusToCursorSpeed();
			cursorSpeed = MathUtil.Clamp( cursorSpeed - bonus, 0.001f, cursorSpeed );

			_initCursorSpeed = cursorSpeed;
			_cursorSpeed = _initCursorSpeed;
		}

		/// <summary>
		/// Shows the success screen and play sound.
		/// </summary>
		private static void ShowSuccessScreenAndPlaySound() {
			API.PlaySoundFrontend( -1, "Goal", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true );

			ShowDisplayScaleform( "CIRCUIT COMPLETE",
				"Decryption Execution x86 Tunneling", CbColors.GreenColor.R,
				CbColors.GreenColor.G, CbColors.GreenColor.B, true );
		}

		/// <summary>
		/// Shows the failure screen and play sound.
		/// </summary>
		private static void ShowFailureScreenAndPlaySound() {
			API.PlaySoundFrontend( -1, "Crash", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true );

			ShowDisplayScaleform( "CIRCUIT FAILED", "Security Tunnel Detected", CbColors.RedColor.R,
				CbColors.RedColor.G, CbColors.RedColor.B, false );
		}

		/// <summary>
		/// Checks if hacking kit disconnected.
		/// </summary>
		/// <param name="disconnectChance">The disconnect chance.</param>
		/// <param name="disconnectCheckRateMs">The disconnect check rate ms.</param>
		private static void CheckIfHackingKitDisconnected( float disconnectChance, float disconnectCheckRateMs ) {
			int hackingKitBonus = ApplyHackingKitBonusToDisconnectCheckRate();
			if( DateTime.Now.CompareTo(
				    _lastDisconnectCheckTime.AddMilliseconds( disconnectCheckRateMs + hackingKitBonus ) ) <
			    0 ) return;
			
			var bonus = ApplyHackingKitBonusToDisconnectChance();
			disconnectChance = disconnectChance + bonus;
			_isHackingKitDisconnected = Rand.GetScalar() < disconnectChance;

			_lastDisconnectCheckTime = DateTime.Now;
		}

		/// <summary>
		/// Applies the hacking kit bonus to disconnect check rate.
		/// </summary>
		/// <returns></returns>
		private static int ApplyHackingKitBonusToDisconnectCheckRate( ) {
			int bonusTimeMs = 0;
			if( _hackingKitVersionNumber == 3 ) bonusTimeMs = 3000;
			return bonusTimeMs;
		}

		/// <summary>
		/// Applies the hacking kit bonus to disconnect chance.
		/// </summary>
		/// <returns></returns>
		private static float ApplyHackingKitBonusToDisconnectChance() {
			if( _hackingKitVersionNumber == 2 )
				return -0.15f;
			if( _hackingKitVersionNumber == 3 ) return -0.2f;

			return 0f;
		}

		/// <summary>
		/// Applies the hacking kit bonus to reconnect time.
		/// </summary>
		/// <returns></returns>
		private static int ApplyHackingKitBonusToReconnectTime() {
			var bonus = 0;
			if( _hackingKitVersionNumber == 2 ) {
				bonus = -2000;
			}

			return bonus;
		}

		/// <summary>
		/// Applies the hacking kit bonus to cursor speed.
		/// </summary>
		/// <returns></returns>
		private static float ApplyHackingKitBonusToCursorSpeed() {
			var bonus = 0f;
			if( _hackingKitVersionNumber == 3 ) {
				bonus = Rand.GetRange( 0, 2000 ) / 100000f;
			}
			return bonus;
		}

		/// <summary>
		/// Starts the reconnection.
		/// </summary>
		/// <param name="minReconnectTimeMs">The minimum reconnect time ms.</param>
		/// <param name="maxReconnectTimeMs">The maximum reconnect time ms.</param>
		private static void StartReconnection( int minReconnectTimeMs, int maxReconnectTimeMs ) {

			API.PlaySoundFrontend( -1, "Power_Down", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true );

			_scaleform?.CallFunction( "SET_DISPLAY", 0, "CONNECTION LOST", "Reconnecting...",
				(int)CbColors.RedColor.R, (int)CbColors.RedColor.G, (int)CbColors.RedColor.B, 0 );

			ShowDisplayScaleform( "CONNECTION LOST", "Reconnecting...", CbColors.RedColor.R, CbColors.RedColor.G,
				CbColors.RedColor.B, false );

			var reconnectTime = Rand.GetRange( minReconnectTimeMs, maxReconnectTimeMs ) +
			                    ApplyHackingKitBonusToReconnectTime();
			_reconnectTime =
				DateTime.Now.AddMilliseconds(MathUtil.Clamp( reconnectTime , 0, reconnectTime) );
		}

		/// <summary>
		/// Finishes the reconnection.
		/// </summary>
		private static void FinishReconnection() {
			API.PlaySoundFrontend( -1, "Start", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true );
			SetScaleform();
			_isHackingKitDisconnected = false;
			_lastDisconnectCheckTime = DateTime.Now;

			_cursorSpeed = GetCursorSpeedOnReconnect( _currentDifficulty );
		}

		/// <summary>
		/// Gets the cursor speed on reconnect.
		/// </summary>
		/// <param name="currentDifficulty">The current difficulty.</param>
		/// <returns></returns>
		private static float GetCursorSpeedOnReconnect( CbDifficultyLevelEnum currentDifficulty ) {
			if( _hackingKitVersionNumber == 3 ) return _cursorSpeed;

			int maxSpeed = (int)(GetMaxSpeedIncreaseOnReconnect( currentDifficulty ) * 100000);
			float speedDelta = Rand.GetRange( 0, (int)(maxSpeed * 0.25f) ) / 100000f;
			if( Rand.GetScalar() > 0.75 ) speedDelta = speedDelta * -1;
			return MathUtil.Clamp( _cursorSpeed + speedDelta, _initCursorSpeed, maxSpeed );
		}

		/// <summary>
		/// Gets the maximum speed increase on reconnect.
		/// </summary>
		/// <param name="currentDifficulty">The current difficulty.</param>
		/// <returns></returns>
		private static float GetMaxSpeedIncreaseOnReconnect( CbDifficultyLevelEnum currentDifficulty ) {
			switch( currentDifficulty ) {
			default:
			case CbDifficultyLevelEnum.Beginner:
				return 0.002f;
			case CbDifficultyLevelEnum.Easy:
				return 0.004f;
			case CbDifficultyLevelEnum.Medium:
				return 0.006f;
			case CbDifficultyLevelEnum.Hard:
				return 0.01f;
			}
		}

		/// <summary>
		/// Gets the cursor speed from difficulty.
		/// </summary>
		/// <param name="currentDifficulty">The current difficulty.</param>
		/// <returns></returns>
		private static float GetCursorSpeedFromDifficulty( CbDifficultyLevelEnum currentDifficulty ) {
			switch( currentDifficulty ) {
			default:
			case CbDifficultyLevelEnum.Beginner:
				return 0.00085f;
			case CbDifficultyLevelEnum.Easy:
				return 0.002f;
			case CbDifficultyLevelEnum.Medium:
				return 0.004f;
			case CbDifficultyLevelEnum.Hard:
				return 0.006f;
			}
		}

		/// <summary>
		/// Gets the disconnect chance from difficulty.
		/// </summary>
		/// <param name="currentDifficulty">The current difficulty.</param>
		/// <returns></returns>
		private static float GetDisconnectChanceFromDifficulty( CbDifficultyLevelEnum currentDifficulty ) {
			switch( currentDifficulty ) {
			default:
			case CbDifficultyLevelEnum.Beginner:
				return 0f;
			case CbDifficultyLevelEnum.Easy:
				return 0.15f;
			case CbDifficultyLevelEnum.Medium:
				return 0.30f;
			case CbDifficultyLevelEnum.Hard:
				return 0.45f;
			}
		}

		/// <summary>
		/// Gets the disconnect check rate ms from difficulty.
		/// </summary>
		/// <param name="currentDifficulty">The current difficulty.</param>
		/// <returns></returns>
		private static int GetDisconnectCheckRateMsFromDifficulty( CbDifficultyLevelEnum currentDifficulty ) {
			//Average level at medium speed = 8s
			switch( currentDifficulty ) {
			default:
			case CbDifficultyLevelEnum.Beginner:
				return 10000;
			case CbDifficultyLevelEnum.Easy:
				return 2000;
			case CbDifficultyLevelEnum.Medium:
				return 1000;
			case CbDifficultyLevelEnum.Hard:
				return 500;
			}
		}

		/// <summary>
		/// Draws the sprites.
		/// </summary>
		/// <param name="currentMap">The current map.</param>
		private static void DrawMapSprite( int currentMap ) {
			string levelTextureDict = currentMap > 3 ? "MPCircuitHack3" : "MPCircuitHack2";

			API.DrawSprite( levelTextureDict, $"cblevel{_currentLevelNumber}", 0.5f, 0.5f, 1,
				1, 0,
				255, 255, 255, 255 );
		}

		private static void DrawCursorAndPortSprites() {
			_cursor.DrawCursor();
			_cursor.DrawTailHistory();

			_genericPorts.DrawPorts();
		}

		/// <summary>
		/// Shows the hacking kit readme.
		/// </summary>
		/// <param name="itemKey">The item key.</param>
		/// <returns></returns>
		public static async Task ShowHackingKitReadme(string itemKey) {
			_isShowingHackingKitReadme = true;

			var versionNumber = new string( itemKey.Where( char.IsDigit ).ToArray() );
			if( string.IsNullOrEmpty( versionNumber ) ) return;

			var dict = "pcdextsw";
			API.RequestStreamedTextureDict( dict, false );
			var timeout = DateTime.Now.AddSeconds( 1 );
			while( DateTime.Now.CompareTo( timeout ) < 0 ) {
				await BaseScript.Delay( 50 );
				if( API.HasStreamedTextureDictLoaded( dict ) ) {
					break;
				}
			}

			var initHealth = Cache.PlayerHealth;
			while( true ) {
				API.DrawSprite( "pcdextsw", $"readmev" + versionNumber, 0.5f, 0.5f, 1,
					1, 0,
					255, 255, 255, 255 );

				var currentHealth = Cache.PlayerHealth;
				if( currentHealth < initHealth || DidPlayerInputCancel() ) {
					break;
				}
				else if( currentHealth > initHealth ) {
					initHealth = currentHealth;
				}

				await BaseScript.Delay( 0 );
			}

			API.SetStreamedTextureDictAsNoLongerNeeded( dict );
			_isShowingHackingKitReadme = false;
		}

		/// <summary>
		/// Determines whether [is near computer property].
		/// </summary>
		/// <returns>
		///   <c>true</c> if [is near computer property]; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsNearComputerProp() {
			try {
				var nearbyProps = Props.FindProps( 4f );
				foreach( var nearbyProp in nearbyProps ) {
					var entity = Entity.FromHandle( nearbyProp );
					if( entity == null || !entity.Exists() ) continue;
					var objName = Enum.GetName( typeof( ObjectHash ), entity.Model.Hash );
					if( string.IsNullOrEmpty( objName ) ) continue;
					if( objName.Contains( "laptop" ) ||
					    objName.Contains( "_pc" ) ||
					    objName.Contains( "_monitor" ) ) {
						return true;
					}
				}
			}
			catch( Exception ex ) {
				Log.Error( ex );
			}
			return false;
		}

		/// <summary>
		/// Sets the position to execute hack.
		/// </summary>
		/// <param name="itemKey">The item key.</param>
		/// <returns></returns>
		private static async Task SetPositionToExecuteHack( string itemKey ) {
			Log.Info( $"Attempting to use {itemKey} circuitbreaker exploit." );
			var initPos = Cache.PlayerPos;
			while( true ) {
				if( Cache.PlayerPos.DistanceToSquared2D( initPos ) > HackingKitResetExePosThreshold ) break;
				IsPositionSetToExecuteHack = true;
				await BaseScript.Delay( 100 );
			}
			IsPositionSetToExecuteHack = false;
		}

		/// <summary>
		/// Did the player input cancel.
		/// </summary>
		/// <returns></returns>
		private static bool DidPlayerInputCancel() {
			return ControlHelper.IsControlPressed( Control.MoveLeftOnly ) ||
			       ControlHelper.IsControlPressed( Control.MoveRightOnly ) ||
			       ControlHelper.IsControlPressed( Control.MoveUpOnly ) ||
			       ControlHelper.IsControlPressed( Control.MoveDownOnly ) ||
			       ControlHelper.IsControlPressed( Control.Context ) ||
			       ControlHelper.IsControlPressed( Control.Jump ) ||
			       ControlHelper.IsControlPressed( Control.FrontendCancel );
		}
		private static async void PlayStartSound( int delayMs ) {
			await BaseScript.Delay( delayMs );
			API.PlaySoundFrontend( -1, "Start", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true );
		}

		/// <summary>
		/// Disables the controls.
		/// </summary>
		private static void DisableControls() {
			API.DisableControlAction( 2, (int)Control.MoveLeftOnly, true );
			API.DisableControlAction( 2, (int)Control.MoveRightOnly, true );
			API.DisableControlAction( 2, (int)Control.MoveUpOnly, true );
			API.DisableControlAction( 2, (int)Control.MoveDownOnly, true );
			API.DisableControlAction( 2, 27, true ); //Phone

			CurrentPlayer.DisableWeaponSelectControls();
			CurrentPlayer.DisableAttackControls();
		}

		/// <summary>
		/// Initializes the resources.
		/// </summary>
		/// <returns></returns>
		private static async Task InitializeResources() {
			foreach( string dict in TextureDictionaries ) API.RequestStreamedTextureDict( dict, false );

			var timeout = DateTime.Now.AddSeconds( 5 );
			while( DateTime.Now.CompareTo( timeout ) < 0 ) {
				bool allLoaded = true;
				foreach( string dict in TextureDictionaries )
					if( !API.HasStreamedTextureDictLoaded( dict ) )
						allLoaded = false;

				if( allLoaded ) break;
				await BaseScript.Delay( 100 );
			}

			foreach( string dict in TextureDictionaries )
				if( !API.HasStreamedTextureDictLoaded( dict ) )
					Log.Error( $"Failed to load {dict}." );

			SetScaleform();
			_backgroundSoundId = API.GetSoundId();
		}

		/// <summary>
		/// Sets the scaleform.
		/// </summary>
		private static async void SetScaleform() {
			await DisposeScaleform();
			_scaleform = new Scaleform( "HACKING_MESSAGE" );

			int loadAttempt = 0;
			while( !_scaleform.IsLoaded ) {
				await BaseScript.Delay( 5 );
				if( loadAttempt++ > 50 ) break;
			}
		}

		/// <summary>
		/// Shows the display scaleform.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <param name="msg">The MSG.</param>
		/// <param name="r">The r.</param>
		/// <param name="g">The g.</param>
		/// <param name="b">The b.</param>
		/// <param name="stagePassed">if set to <c>true</c> [stage passed].</param>
		private static void ShowDisplayScaleform( string title, string msg, int r, int g, int b, bool stagePassed ) {
			_scaleform?.CallFunction( "SET_DISPLAY", 0, title, msg,
				r, g, b, stagePassed );
		}

		/// <summary>
		/// Ends the game.
		/// </summary>
		private static void EndGame() {
			CurrentPlayer.EnableWeaponWheel( true );
			Dispose();
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		private static void Dispose() {
			DisposeSounds();
			DisposeTextureDictionaries();
			DisposeScaleform();
		}

		/// <summary>
		/// Disposes the sounds.
		/// </summary>
		private static void DisposeSounds() {
			API.StopSound( _backgroundSoundId );
			if( _backgroundSoundId > 0 ) API.ReleaseSoundId( _backgroundSoundId );
		}

		/// <summary>
		/// Disposes the texture dictionaries.
		/// </summary>
		private static void DisposeTextureDictionaries() {
			foreach( string dict in TextureDictionaries ) API.SetStreamedTextureDictAsNoLongerNeeded( dict );
		}

		/// <summary>
		/// Disposes the scaleform.
		/// </summary>
		/// <returns></returns>
		private static async Task DisposeScaleform() {
			_scaleform?.Dispose();
			_scaleform = null;

			await BaseScript.Delay( 50 );
		}

		/// <summary>
		/// Determines whether [is cursor out of bounds] [the specified poly bounds].
		/// </summary>
		/// <param name="polyBounds">The poly bounds.</param>
		/// <param name="mapBounds">The map bounds.</param>
		/// <returns>
		///   <c>true</c> if [is cursor out of bounds] [the specified poly bounds]; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsCursorOutOfBounds( IEnumerable<IEnumerable<Vector2>> polyBounds,
			IEnumerable<Vector2> mapBounds ) {
			var coord = new Vector2( _cursor.Position.X, _cursor.Position.Y );

			var headPts = GetCursorMaxPoints( coord, _cursor.CursorHeadSize + -0.375f * _cursor.CursorHeadSize );

			var polyList = polyBounds.ToList();
			var mapList = mapBounds.ToArray();
			foreach( var pt in headPts ) {
				foreach( var bounds in polyList )
					if( CbHelper.IsInPoly( bounds.ToArray(), pt ) )
						return true;

				if( !CbHelper.IsInPoly( mapList, pt ) ) return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the cursor maximum points.
		/// </summary>
		/// <param name="cursorCoord">The cursor coord.</param>
		/// <param name="cursorHeadSize">Size of the cursor head.</param>
		/// <returns></returns>
		private static IEnumerable<Vector2> GetCursorMaxPoints( Vector2 cursorCoord, float cursorHeadSize ) {
			float headHeight = cursorHeadSize;
			float headWidth = cursorHeadSize;

			var headPt1 = new Vector2( cursorCoord.X - headWidth / 2, cursorCoord.Y );
			var headPt2 = new Vector2( cursorCoord.X + headWidth / 2, cursorCoord.Y );
			var headPt3 = new Vector2( cursorCoord.X, cursorCoord.Y - headHeight / 2 );
			var headPt4 = new Vector2( cursorCoord.X, cursorCoord.Y + headHeight / 2 );

			return new[] {headPt1, headPt2, headPt3, headPt4, cursorCoord};
		}
	}
}