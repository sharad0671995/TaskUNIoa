namespace TaskManagementSystem.DTO.Task
{
    public class TaskUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }

        public int AssignedToId { get; set; }
    }

}
