namespace GymMembershipSystemA3.Models
{
    public class GymOccupancyViewModel
    {
        public int Capacity { get; set; }

        public int InsideCount { get; set; }

        public bool HasActiveSubscription { get; set; }

        public bool IsCheckedIn { get; set; }

        public List<GymVisit> VisitorsInside { get; set; } = new();

        public int AvailablePlaces
        {
            get
            {
                return Math.Max(0, Capacity - InsideCount);
            }
        }

        public int OccupancyPercent
        {
            get
            {
                if (Capacity <= 0)
                {
                    return 0;
                }

                return (int)Math.Round(InsideCount * 100.0 / Capacity);
            }
        }

        public string CrowdLevel
        {
            get
            {
                if (Capacity <= 0)
                {
                    return "Unavailable";
                }

                if (InsideCount >= Capacity)
                {
                    return "Full";
                }

                double percentage = InsideCount * 100.0 / Capacity;

                if (percentage >= 75)
                {
                    return "Busy";
                }

                if (percentage >= 40)
                {
                    return "Moderate";
                }

                return "Low";
            }
        }
    }
}