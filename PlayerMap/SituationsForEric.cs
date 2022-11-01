using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PlayerMap
{
    public class SituationsForEric
    {
        [Fact]
        public async Task GetSituationsArray()
        {
            var result = new List<string>();
            foreach (var playId in ExtensionMethods.GetEnumList<FiltrationPlayid>())
            {
                var attr = playId.GetAttributeOfType<DisplayAttribute>();
                result.Add(attr.Name);
            }

            await File.WriteAllTextAsync(@"C:\temp\Sportradar\SituationsForEric.txt", string.Join(",", result));
        }
        
        public enum FiltrationPlayid : byte
        {
            [Display(Name = "0 Pts")] _0Pts, //0
            [Display(Name = "1 Pts")] _1Pts, //1
            [Display(Name = "2 Pts")] _2Pts, //2
            [Display(Name = "3 Pts")] _3Pts, //3
            [Display(Name = "8 Sec Violation")] _8SecViolation, //4
            [Display(Name = "At Basket")] AtBasket, //5
            [Display(Name = "Ball Delivered")] BallDelivered, //6
            [Display(Name = "Ballhandler")] Ballhandler, //7
            [Display(Name = "Baseline")] Baseline, //8
            [Display(Name = "Basket")] Basket, //9
            [Display(Name = "Curl")] Curl, //10
            [Display(Name = "Cut")] Cut, //11
            [Display(Name = "Defense Commits")] DefenseCommits, //12

            [Display(Name = "Defensive 3 Sec Violation")]
            Defensive3SecViolation,
            [Display(Name = "Defunct")] Defunct,
            [Display(Name = "Dribble Jumper")] DribbleJumper, //15
            [Display(Name = "Dribble Move")] DribbleMove, //16
            [Display(Name = "Dribble Off Pick")] DribbleOffPick, //17
            [Display(Name = "Drive Baseline")] DriveBaseline,
            [Display(Name = "Drive Middle")] DriveMiddle,
            [Display(Name = "Drives Left")] DrivesLeft, //20
            [Display(Name = "Drives Right")] DrivesRight, //21
            [Display(Name = "Drives Straight")] DrivesStraight, //22
            [Display(Name = "Early Jumper")] EarlyJumper, //23
            [Display(Name = "Face-up")] Face_up, //24
            [Display(Name = "Fade")] Fade, //25
            [Display(Name = "First Middle")] FirstMiddle, //26
            [Display(Name = "Flare")] Flare, //27
            [Display(Name = "Flash")] Flash, //28
            [Display(Name = "Flash Middle")] FlashMiddle, //29
            [Display(Name = "Foul")] Foul, //30
            [Display(Name = "Free Throw")] FreeThrow, //31
            [Display(Name = "From Dribble")] FromDribble, //32
            [Display(Name = "From Stationary")] FromStationary, //33
            [Display(Name = "Go Away from Pick")] GoAwayfromPick, //34
            [Display(Name = "Guarded")] Guarded, //35
            [Display(Name = "Hand Off")] HandOff, //36
            [Display(Name = "High P&R")] HighPandR, //37
            [Display(Name = "Isolation")] ISO, //38
            [Display(Name = "Jumper")] Jumper, //39
            [Display(Name = "Kicked Ball")] KickedBall, //40
            [Display(Name = "Leak Outs")] LeakOuts, //41
            [Display(Name = "Left")] Left, //42
            [Display(Name = "Left Block")] LeftBlock, //43
            [Display(Name = "Left P&R")] LeftPandR, //44
            [Display(Name = "Left Shoulder")] LeftShoulder, //45
            [Display(Name = "Left Wing")] LeftWing, //46
            [Display(Name = "Lineup Clip")] LineupClip, //47
            [Display(Name = "Long")] Long, //48
            [Display(Name = "Long/3pt")] Long3pt, //49
            [Display(Name = "Make 2 Pts")] Make2Pts, //50
            [Display(Name = "Make 2 Pts Foul")] Make2PtsFoul, //51
            [Display(Name = "Make 3 Pts")] Make3Pts, //52
            [Display(Name = "Make 3 Pts Foul")] Make3PtsFoul, //53
            [Display(Name = "Management")] Management, //54
            [Display(Name = "Medium/17' to 3pt")] Medium17to3pt, //55
            [Display(Name = "Miss 2 Pts")] Miss2Pts, //56
            [Display(Name = "Miss 3 Pts")] Miss3Pts, //57
            [Display(Name = "No Dribble Jumper")] NoDribbleJumper, //58
            [Display(Name = "No Dribble Move")] NoDribbleMove,
            [Display(Name = "No Play Type")] NoPlayType, //60
            [Display(Name = "No Violation")] NoViolation, //61
            [Display(Name = "Non Possession")] NonPossession, //62
            [Display(Name = "Non Shooting Foul")] NonShootingFoul, //63
            [Display(Name = "Off Screen")] OffScreen, //64
            [Display(Name = "Offensive Rebound")] OffensiveRebound, //65
            [Display(Name = "Open")] Open, //66

            [Display(Name = "Out of Bound 5 Sec Violation")]
            OutofBound5SecViolation,

            [Display(Name = "Out of Bound Timeout")]
            OutofBoundTimeout,
            [Display(Name = "P&R Ball Handler")] PandRBallHandler, //69
            [Display(Name = "P&R Roll Man")] PandRRollMan, //70
            [Display(Name = "Pick and Pops")] PickandPops, //71
            [Display(Name = "Post Pin")] PostPin, //72
            [Display(Name = "Post-Up")] Post_Up, //73
            [Display(Name = "Right")] Right, //74
            [Display(Name = "Right Block")] RightBlock, //75
            [Display(Name = "Right P&R")] RightPandR, //76
            [Display(Name = "Right Shoulder")] RightShoulder, //77
            [Display(Name = "Right Wing")] RightWing, //78
            [Display(Name = "Rolls to Basket")] RollstoBasket, //79
            [Display(Name = "Run Offense")] RunOffense, //80
            [Display(Name = "Scoring Attempt")] ScoringAttempt, //81
            [Display(Name = "Screen")] Screen, //82
            [Display(Name = "Select Player")] SelectPlayer, //83
            [Display(Name = "Short")] Short, //84
            [Display(Name = "Short to < 17'")] Shortto17, //85

            [Display(Name = "Shot Clock Violation")]
            ShotClockViolation,
            [Display(Name = "Side")] Side, //87
            [Display(Name = "Slips the Pick")] SlipsthePick, //88
            [Display(Name = "Split")] Split, //89
            [Display(Name = "Spot-Up")] Spot_Up, //90
            [Display(Name = "Straight")] Straight, //91

            [Display(Name = "Takes Early Jump Shot")]
            TakesEarlyJumpShot, //92
            [Display(Name = "To Basket")] ToBasket, //93
            [Display(Name = "To Drop Step")] ToDropStep, //94
            [Display(Name = "To Hook")] ToHook, //95
            [Display(Name = "To Jumper")] ToJumper, //96
            [Display(Name = "To Shooter's Left")] ToShootersLeft, //97
            [Display(Name = "To Shooter's Right")] ToShootersRight, //98
            [Display(Name = "To Up and Under")] ToUpandUnder, //99
            [Display(Name = "Top")] Top, //100
            [Display(Name = "Trailer")] Trailer, //101
            [Display(Name = "Transition")] Transition, //102
            [Display(Name = "Trash Clip")] TrashClip,
            [Display(Name = "Turnover")] Turnover, //104
            [Display(Name = "Unknown")] Unknown,
            [Display(Name = "Zone")] Zone, //106

            [Display(Name = "Offensive Goaltending")]
            OffensiveGoaltending, //456
            [Display(Name = "Personal Matter")] PersonalMatter, //482
            [Display(Name = "Time Mark")] TimeMark, //380
            [Display(Name = "Free Throw 2 of 3")] FreeThrow2Of3, //436

            [Display(Name = "Flagrant Free Throw 1 of 1")]
            FlagrantFreeThrow1Of1, //441

            [Display(Name = "Select Defensive Player")]
            SelectDefensivePlayer, //338
            [Display(Name = "Out of Bounds")] OutOfBounds, //445
            [Display(Name = "Start Period")] StartPeriod, //373
            [Display(Name = "2nd Quarter")] SecondQuarter, //382
            [Display(Name = "Game Roster")] GameRoster, //472
            [Display(Name = "Double")] Double, //364

            [Display(Name = "Specify Turnover Type")]
            SpecifyTurnoverType, //327
            [Display(Name = "Home Team Player")] HomeTeamPlayer, //415
            [Display(Name = "Select Home Player")] SelectHomePlayer, //490
            [Display(Name = "Make 2 Pts No Foul")] Make2PtsNoFoul, //397
            [Display(Name = "Violation")] Violation, //323
            [Display(Name = "3 Point Attempt")] ThreePointAttempt, //394
            [Display(Name = "Tip Shot")] TipShot, //423
            [Display(Name = "Hanging On Rim")] HangingOnRim, //365
            [Display(Name = "Hook Shot")] HookShot, //406
            [Display(Name = "1st Quarter")] FirstQuarter, //381
        }
    }
}