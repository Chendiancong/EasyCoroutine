using UnityEngine;

namespace AsyncWork.Core
{
    public struct TimeIntervalInfo
    {
        public bool isRealTime;
        public float startTime;
        public float interval;

        public float EndTime => startTime + interval;

        public void SetInterval(float interval)
        {
            this.interval = interval;
            startTime = isRealTime ?
                Time.realtimeSinceStartup : Time.time;
        }

        public bool CheckTime()
        {
            return isRealTime ?
                EndTime <= Time.realtimeSinceStartup :
                EndTime <= Time.time;
        }
    }
}