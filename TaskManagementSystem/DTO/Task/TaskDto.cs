namespace TaskManagementSystem.DTO.Task
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? CreatedByName { get; set; }
        public string? AssignedToName { get; set; }
       // public TaskStatus Status { get; set; }  // 
    }

}
