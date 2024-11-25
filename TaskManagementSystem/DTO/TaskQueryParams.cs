namespace TaskManagementSystem.DTO
{
    public class TaskQueryParams
    {

        public int Page { get; set; } = 1;        // Default to page 1
        public int PageSize { get; set; } = 10;   
        public string? SearchTerm { get; set; }
    }
}
