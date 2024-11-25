namespace TaskManagementSystem.DTO.User
{
    public class TaskSummaryDto
    {

        public int Id { get; set; }  // Task TaskId
        public string? Title { get; set; }  // Task Title
        public bool IsCompleted { get; set; }
    }
}
