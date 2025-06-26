namespace MetricClock
{
    public static class TimeHelpers
    {
        public static MetricTime GetMetricTime(DateTime dateTime)
        {
            var totalSeconds = dateTime.Hour * 3600 + dateTime.Minute * 60 + dateTime.Second + dateTime.Millisecond / 1000.0;
            var totalMetricSeconds = totalSeconds * 100000.0 / 86400.0;

            var metricHours = (int)(totalMetricSeconds / 10000);
            var metricMinutes = (int)((totalMetricSeconds % 10000) / 100);
            var metricSeconds = (int)(totalMetricSeconds % 100);
            var metricMilliseconds = (int)((totalMetricSeconds % 1) * 1000);

            return new MetricTime
            {
                Hours = metricHours,
                Minutes = metricMinutes,
                Seconds = metricSeconds,
                Milliseconds = metricMilliseconds
            };
        }
    }

    public struct MetricTime
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public int Milliseconds { get; set; }

        public override string ToString()
        {
            return $"{Hours:D1}:{Minutes:D2}:{Seconds:D2}";
        }

        public string ToStringNoSeconds()
        {
            return $"{Hours:D1}:{Minutes:D2}";
        }

        public string ToStringWithMilliseconds()
        {
            return $"{Hours:D1}:{Minutes:D2}:{Seconds:D2}.{Milliseconds:D3}";
        }
    }
}