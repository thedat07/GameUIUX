using UniRx;

namespace Core.Events
{
    public static class GameEventSub
    {
        // Thắng / Thua
        public static readonly Subject<Unit> OnLevelWin = new Subject<Unit>();
        public static readonly Subject<Unit> OnLevelLose = new Subject<Unit>();

        // Time Change
        public static readonly Subject<(float a, float b, float c)> OnTimeInGameChange = new Subject<(float, float, float)>();
        public static readonly Subject<(float a, float b, float c)> OnFreezeTimeChange = new Subject<(float, float, float)>();

        // Booster Events
        public static readonly Subject<Unit> OnUseMagnetComplete = new Subject<Unit>();
        public static readonly Subject<Unit> CheckWinCondition = new Subject<Unit>();
        public static readonly Subject<Unit> OnUseMagnet = new Subject<Unit>();
        public static readonly Subject<Unit> OnUsePropeller = new Subject<Unit>();
        public static readonly Subject<Unit> OnCancelBoosterMagnet = new Subject<Unit>();
        public static readonly Subject<Unit> OnCancelBoosterPropeller = new Subject<Unit>();

        static GameEventSub()
        {

        }
    }
}
