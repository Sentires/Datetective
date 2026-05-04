namespace TheDates.Runtime.Quests
{
    public enum QuestState
    {
        Available, // Conditions met
        Unavailable, // Conditions failed
        Active, // In progress
        Achieved, // Completed successfully
        Concluded, // Already completed
    }
}
