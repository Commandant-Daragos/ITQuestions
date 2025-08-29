namespace ITQuestions.Enum
{
    public enum SyncStatus
    {
        /// <summary>
        /// Unchanged status.
        /// </summary>
        None,

        /// <summary>
        /// New question added.
        /// </summary>
        Add,

        /// <summary>
        /// Modified question.
        /// </summary>
        Update,

        /// <summary>
        /// Flag for soft-delete question.
        /// </summary>
        Delete
    }
}
