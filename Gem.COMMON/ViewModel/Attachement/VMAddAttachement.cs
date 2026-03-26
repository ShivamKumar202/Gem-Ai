using Microsoft.AspNetCore.Http;
namespace Gem.COMMON.ViewModel.Attachement
{
    public class VMAddAttachment
    {
        public string MessageId { get; set; }        
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public IFormFile File { get; set; }           
        public byte[] Data { get; set; }            
        public float[] Embedding { get; set; }
    }
}
