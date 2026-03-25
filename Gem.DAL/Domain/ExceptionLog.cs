using System.ComponentModel.DataAnnotations;

namespace Gem.DAL.Domain
{
    public class ExceptionLog
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? ActivatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
