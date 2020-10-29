//  GameStatusEnum.cs
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

namespace Roleplay.Client.Classes.Crime.MiniGames.CircuitBreaker
{
	public enum CbGameStatusEnum
	{
		Error = -5,
		FailedToStart = -4,
		MissingHackingKit = -3,
		TakingDamage = -2,
		Failure = -1,
		PlayerQuit = 0,
		Success = 1
	}
}