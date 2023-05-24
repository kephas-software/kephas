namespace ToDo.Domain
{
    using System;

    public class ToDoItem
    {
        public DateTimeOffset CreateDate { get; set; }

        public User CreatedBy { get; set; }

        public string Description { get; set; }

        public DateTimeOffset? ScheduledOn { get; set; }

        public User AssignedTo { get; set; }

        public DateTimeOffset CompletedOn { get; set; }
    }
}