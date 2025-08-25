namespace ITQuestions.Enum
{
    public enum SyncStatus
    {
        None,   // unchanged
        Add,    // new, needs to be pushed
        Update, // modified, needs update
        Delete  // soft-deleted, needs to propagate
    }
}
